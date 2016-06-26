using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Artefacts
{
    abstract class Repository : IRepository
    {
        public ArtefactCache Cache { get; } = new ArtefactCache();

        public abstract void Save(Artefact artefact);
		public abstract IEnumerable<Artefact> Get();
        public abstract IEnumerable<Artefact> Get(Expression<Func<Artefact, bool>> match);
        public abstract Artefact Get(ObjectId artefactId);
    }
}
