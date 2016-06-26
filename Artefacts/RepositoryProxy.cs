using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Linq.Expressions;
using System.Collections;

namespace Artefacts
{
	public class RepositoryProxy : IRepository
	{
		internal ArtefactCache _cache = new ArtefactCache();
		private IClient _client;

		public RepositoryProxy(IClient client)
		{
			_client = client;
		}

		public void Save(Artefact artefact)
		{
			_client.Serialize<Artefact>(artefact);
		}

		public IEnumerable<Artefact> Get(Expression<Func<Artefact, bool>> predicate)
		{
			_client.Serialize(predicate);
			return _client.Deserialize<IEnumerable<Artefact>>();
		}

		IEnumerable<Artefact> IRepository.Get()
		{
			Expression<Func<Artefact, bool>> lambda = artefact => true;
			return Get(lambda);
		}

		Artefact IRepository.Get(ObjectId artefactId)
		{
			_client.Serialize<ObjectId>(artefactId);
			return _client.Deserialize<Artefact>();
		}
	}

	public class RepositoryProxy<T> : RepositoryProxy, IRepository<T>
	{
		public RepositoryProxy(IClient client)
			: base(client)
		{

		}

		public void Save(T instance)
		{
			Artefact artefact = _cache.GetOrCreate<T>(instance);
			Save(artefact);
		}

		public IEnumerable<T> Get(Expression<Func<T, bool>> predicate)
		{
			Expression newPredicate = new QueryTranslator<T>().Visit(predicate);
			IEnumerable<T> results = base.Get((Expression<Func<Artefact, bool>>)newPredicate).Select(artefact => artefact.As<T>());
			return results;
		}

		public IEnumerable<T> Get()
		{
			return (this as IRepository).Get().Select(artefact => artefact.As<T>());
		}

		public T Get(ObjectId artefactId)
		{
			return (this as IRepository).Get(artefactId).As<T>();
		}

		public IEnumerable Get(Expression select)
		{
			throw new NotImplementedException();
		}
	}
}
