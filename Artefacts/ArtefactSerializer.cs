using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Conventions;

namespace Artefacts
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Interfaces/classes not currently used but maybe in future, keep in mind:
    ///     <see cref="IBsonPolymorphicSerializer"/>
    ///     <see cref="IBsonArraySerializer"/>
    ///     <see cref="IDictionaryRepresentationConfigurable"/>
    ///     <see cref="MongoDB.Bson.Serialization.IChildSerializerConfigurable"/>
    ///     <see cref="MongoDB.Bson.Serialization.IRepresentationConfigurable"/> and others in namespace
    /// </remarks>
    public class ArtefactSerializer : SerializerBase<Artefact>, IBsonIdProvider, IBsonDocumentSerializer, IBsonDictionarySerializer
    {
        private static readonly IBsonSerializer _keySerializer = new StringSerializer(BsonType.String);
        private static readonly IBsonSerializer _valueSerializer = new ObjectSerializer(ObjectDiscriminatorConvention.Instance);

        private static ArtefactSerializer _instance = null;
        public static ArtefactSerializer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ArtefactSerializer();
                return _instance;
            }
        }

        public DictionaryRepresentation DictionaryRepresentation { get { return DictionaryRepresentation.Document; } }
        public IBsonSerializer KeySerializer { get { return _keySerializer; } }
        public IBsonSerializer ValueSerializer { get { return _valueSerializer; } }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Artefact value)
        {
            BsonDocumentSerializer.Instance.Serialize(context, args, value);
        }
        public override Artefact Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return new Artefact(BsonDocumentSerializer.Instance.Deserialize(context, args));
        }

        public bool GetDocumentId(object document, out object id, out Type idNominalType, out IIdGenerator idGenerator)
        {
            if (document == null)
                throw new ArgumentNullException();
            Artefact a = document as Artefact;
            if (a == null)  //document.GetType().IsSubclassOf(Artefact._T))
                throw new ArgumentOutOfRangeException(nameof(document), document, "Parameter type is not a subclass of " + nameof(Artefact));
            id = a.Id;
            idNominalType = typeof(ObjectId);
            idGenerator = MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance;
            return true;
        }
        public void SetDocumentId(object document, object id)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            Artefact a = document as Artefact;
            if (a == null)  //document.GetType().IsSubclassOf(Artefact._T))
                throw new ArgumentOutOfRangeException(nameof(document), document, "Parameter type is not a subclass of " + nameof(Artefact));
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (!id.GetType().IsSubclassOf(typeof(ObjectId)))   // == null)  //document.GetType().IsSubclassOf(Artefact._T))
                throw new ArgumentOutOfRangeException(nameof(id), id, "Parameter type is not a subclass of " + nameof(ObjectId));
            a.Id = (ObjectId)id;
        }

        public bool TryGetMemberSerializationInfo(string memberName, out BsonSerializationInfo serializationInfo)
        {
            switch (memberName)
            {
                case "Id":
                    serializationInfo = new BsonSerializationInfo("_id", BsonStringSerializer.Instance, typeof(ObjectId));
                    break;
                //case "":

                //    break;
                default:
                    serializationInfo = null;
                    return false;
            }
            return true;
        }
    }
}
