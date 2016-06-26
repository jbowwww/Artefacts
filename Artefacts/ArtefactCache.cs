using MongoDB.Bson;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

namespace Artefacts
{
    internal class ArtefactCache : IDictionary<object, Artefact>
    {
        //class Item
        //{
        //    object Instance;
        //    ObjectId Id;
        //    Artefact Artefact;
        //}

        //const int InitialCacheCapacity = 32;

        //Array artefacts = Array.CreateInstance(typeof(Item), InitialCacheCapacity);
        
        Dictionary<ObjectId, Artefact> FromId { get; } = new Dictionary<ObjectId, Artefact>();
        Dictionary<object, Artefact> FromRef { get; } = new Dictionary<object, Artefact>();
        
        public ICollection<Object> Keys
        {
            get
            {
                return ((IDictionary<Object, Artefact>)FromRef).Keys;
            }
        }
        public ICollection<Artefact> Values {  get { return FromRef.Values; } }

        public int Count
        {
            get
            {
                return ((IDictionary<object, Artefact>)FromRef).Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;// return ((IDictionary<object, Artefact>)FromRef).IsReadOnly;
            }
        }

        public Artefact this[object key]
        {
            get
            {
                return ((IDictionary<object, Artefact>)FromRef)[key];
            }

            set
            {
                ((IDictionary<object, Artefact>)FromRef)[key] = value;
            }
        }

        public ArtefactCache()
        {
        }

        public bool ContainsKey(object key)
        {
            return FromRef.ContainsKey(key);
        }
        public bool Contains(KeyValuePair<object, Artefact> item)
        {
            return FromRef.Contains(item);
        }

        public void Add(object key, Artefact value)
        {
            FromRef.Add(key, value);
            FromId.Add(value.Id, value);
        }
        public void Add(KeyValuePair<object, Artefact> item)
        {
            FromRef.Add(item.Key, item.Value);
            FromId.Add(item.Value.Id, item.Value);
        }

        public void Clear()
        {
            FromRef.Clear();
            FromId.Clear();
        }
        public bool Remove(object key)
        {
            if (FromRef.ContainsKey(key))
            {
                Artefact artefact = FromRef[key];
                FromRef.Remove(key);
                FromId.Remove(artefact.Id);
                return true;
            }
            return false;
        }
        public bool Remove(KeyValuePair<object, Artefact> item)
        {
            if (FromRef.ContainsKey(item.Key))
            {
                Artefact artefact = FromRef[item.Key];
                FromRef.Remove(item.Key);
                FromId.Remove(artefact.Id);
                return true;
            }
            return false;
        }

        public bool TryGetValue(object key, out Artefact value)
        {
            return FromRef.TryGetValue(key, out value);
        }      
        public void CopyTo(KeyValuePair<object, Artefact>[] array, int arrayIndex)
        {
            ((IDictionary<object, Artefact>)FromRef).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<object, Artefact>> GetEnumerator()
        {
            return ((IDictionary<object, Artefact>)FromRef).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Artefact GetOrCreate<T>(object instance)
        {
            Artefact artefact;
            if (ContainsKey(instance))
            {
                artefact = FromRef[instance];
            }
            else
            {
                artefact = new Artefact(instance);
                FromRef.Add(instance, artefact);
                FromId.Add(artefact.Id, artefact);
            }
            artefact.StoreInstance(instance, typeof(T));
            return artefact;
        }
    }
}