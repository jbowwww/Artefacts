using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Artefacts
{
    public interface IRepository
    {
		void Save(Artefact artefact);
        IEnumerable<Artefact> Get();
        IEnumerable<Artefact> Get(Expression<Func<Artefact, bool>> match);
		Artefact Get(ObjectId artefactId);
	}

	public interface IRepository<T> : IRepository
    {
        void Save(T instance);
		new IEnumerable<T> Get();
		IEnumerable<T> Get(Expression<Func<T, bool>> match);
		new T Get(ObjectId artefactId);
		IEnumerable Get(Expression select);
    }
}
