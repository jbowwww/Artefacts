using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;

using Artefacts.Extensions;

namespace Artefacts
{
    public class Artefact : DynamicObject, IDictionary<string, object>, IConvertibleToBsonDocument
    {
        public static readonly Type _T = typeof(Artefact);

        public delegate IEnumerable<MemberInfo> GetInstanceMembersDelegate(object instance, Type type);
        internal static GetInstanceMembersDelegate DefaultGetInstanceMembers = new GetInstanceMembersDelegate(
            (object instance, Type type) =>
            {
                const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
                return type.GetProperties(bindingFlags);
            });

        private BsonDocument _bsonDocument;

        private readonly Dictionary<Type, object> _typedInstances = new Dictionary<Type, object>();

        /// <summary>
        /// Get/set the artefact's <see cref="ObjectId"/>
        /// </summary>
        /// <remarks>
        /// TODO: Maybe want to not store this value in the backing bsondocument?
        /// One effect e.g. would be <see cref="Artefact.Count"/> not including the artefact's inherent id
        /// Con: might make <see cref="Artefact"/> c'tor(/assignment from conv.?) harder to convert from bsondocument
        /// </remarks>
        public ObjectId Id {
            get { return (ObjectId)GetValue("_id"); } // { return _bsonDocument.GetValue("_id").AsObjectId; }
            internal set { SetValue("_id", value); } // { _bsonDocument.Set("_id", BsonTypeMapper.MapToBsonValue(value)); }
        }

        public object this[string propertyName]
        {
            get { return GetValue(propertyName); }
            set { SetValue(propertyName, value); }
        }

        public ICollection<string> Keys
        {
            get { return new ReadOnlyCollection<string>(_bsonDocument.Names.ToList()); }
        }
        public ICollection<object> Values
        {
            get
            {
                return new ReadOnlyCollection<object>(
                    _bsonDocument.Values.Select(
                        bsonValue => BsonTypeMapper.MapToDotNetValue(bsonValue))
                    .ToList());
            }
        }

        public int Count
        {
            get { return _bsonDocument.ElementCount; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        
        public Artefact()
        {
            _bsonDocument = new BsonDocument();
            Id = ObjectId.GenerateNewId();
        }
        /// <summary>
        /// Called from <see cref="ArtefactCache.GetOrCreate(object)"/>
        /// </summary>
        /// <param name="instance"></param>
        internal Artefact(object instance)
			: this()
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            Type T = instance.GetType();
            if (_typedInstances.ContainsKey(T))
                throw new ArgumentException("Instance type " + T.FullName + " already exists", nameof(instance));
            StoreInstance(instance, T);
            _typedInstances[T] = instance;
        }
        internal Artefact(BsonDocument bsonDocument)
        {
            _bsonDocument = bsonDocument;
        }

        internal void StoreInstance(object instance, Type type)
        {
            StoreInstance(instance, type, DefaultGetInstanceMembers);
        }
        internal void StoreInstance(object instance, Type type, GetInstanceMembersDelegate getInstanceMembers)
        {
			if (!_typedInstances.ContainsKey(type))
				_typedInstances.Add(type, instance);
            foreach (MemberInfo member in getInstanceMembers(instance, type))
            {
                SetValue(member.Name, member.GetValue(instance));
            }
        }
		internal object GetInstance(Type type)
		{
			return GetInstance(type, DefaultGetInstanceMembers);
		}
		internal object GetInstance(Type type, GetInstanceMembersDelegate getInstanceMembers)
		{
			object instance;
			if (_typedInstances.ContainsKey(type))
				instance = _typedInstances[type];
			else
			{
				instance = Activator.CreateInstance(type);
				_typedInstances.Add(type, instance);
			}
			foreach (MemberInfo member in getInstanceMembers(instance, type))
			{
				if (ContainsKey(member.Name))
					member.SetValue(instance, GetValue(member.Name));
			}
			return instance;
		}

        public T As<T>()
        {
			return (T)GetInstance(typeof(T));
        }

        public object GetValue(string propertyName)
        {
            return BsonTypeMapper.MapToDotNetValue(_bsonDocument.GetValue(propertyName));
        }
        public void SetValue(string propertyName, object value)
        {
            _bsonDocument.Set(propertyName, BsonTypeMapper.MapToBsonValue(value));
        }

        public bool ContainsKey(string key)
        {
            return _bsonDocument.Names.Contains(key);
        }
        public bool Contains(KeyValuePair<string, object> item)
        {
            return ContainsKey(item.Key);
        }
        public bool TryGetValue(string key, out object value)
        {
            value = null;
            if (!ContainsKey(key))
                return false;
            value = _bsonDocument[key];
            return true;
        }
        public void Add(string key, object value)
        {
            _bsonDocument.Add(key, BsonTypeMapper.MapToBsonValue(value));
        }
        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }
        public bool Remove(string key)
        {
            if (!ContainsKey(key))
                return false;
            _bsonDocument.Remove(key);
            return true;
        }
        public bool Remove(KeyValuePair<string, object> item)
        {
            return Remove(item.Key);
        }
		public void Clear()
        {
            _bsonDocument.Clear();
        }
		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            foreach (BsonElement element in _bsonDocument.Elements)
                array[arrayIndex++] = new KeyValuePair<string, object>(element.Name, element.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// TODO: Today 12/6/16 commented previous implementation and now just using _bsonDocument.GetEnumerator()
        ///     - See how that goes or otherwise fall back to below plan
        /// How does this behave(or not?) using dynamic alloc'd local memory for var dict?
        /// TODO: Probably needs an Enumerator class implementing IDisposable, containing the array,
        /// which on Finalise() does the class does delete [] dict
        /// </remarks>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<string, object>>)_bsonDocument.GetEnumerator();
            //KeyValuePair<string, object>[] dict = new KeyValuePair<string, object>[_bsonDocument.ElementCount];
            //CopyTo(dict, 0);
            //return (IEnumerator<KeyValuePair<string, object>>)dict.ToList().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            if (!ContainsKey(binder.Name))
                return false;
            result = GetValue(binder.Name);
            return true;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetValue(binder.Name, value);
            return true;
        }
 
        public BsonDocument ToBsonDocument()
        {
            return _bsonDocument;
        }
        public static implicit operator BsonDocument(Artefact artefact)
        {
            return artefact.ToBsonDocument();
        }
    }
}
