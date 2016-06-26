using MongoDB.Bson;
using System;
using System.Collections.Concurrent;

namespace Artefacts
{
    interface IArtefactCache
    {
        ConcurrentDictionary<ObjectId, Artefact> FromId { get; }
        ConcurrentDictionary<object, Artefact> FromRef { get; }

    }
}
