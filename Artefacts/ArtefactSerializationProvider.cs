using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;

namespace Artefacts
{
    class ArtefactSerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (type == Artefact._T)
                return ArtefactSerializer.Instance;
            return null;
        }
    }
}
