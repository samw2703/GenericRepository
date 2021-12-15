using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericRepository.Abstractions;

namespace GenericRepository.TestContext
{
	public class GenericRepositoryAssertions<T, TKey> where TKey : IEquatable<TKey>
	{
		private readonly IGenericRepository<T, TKey> _genericRepository;
		private readonly IAssertionRules _assertionRules;

		internal GenericRepositoryAssertions(IGenericRepository<T, TKey> genericRepository, IAssertionRules assertionRules)
		{
			_genericRepository = genericRepository;
			_assertionRules = assertionRules;
		}

		public async Task<EntityAssertions<T>> Single(TKey key)
		{
			var entity = await _genericRepository.Get(key);
			_assertionRules.NotNull(entity, $"Entity of type {typeof(T).FullName} with key {key} could not be found");

			return new EntityAssertions<T>(entity, _assertionRules);
		}

		public async Task HasSingleMatch(Expression<Func<T, bool>> predicate)
		{
			var entities = await _genericRepository.GetWhere(predicate);
			EnsureSingle(entities);
		}

		public async Task<EntityAssertions<T>> Single()
			=> EnsureSingle(await _genericRepository.GetWhere(_ => true));

		private EntityAssertions<T> EnsureSingle(List<T> entities)
		{
			if (entities.Count == 0)
				_assertionRules.Fail($"Could not find a {typeof(T).Name} that matched your predicate");

			if (entities.Count > 1)
				_assertionRules.Fail($"Found multiple entities of type {typeof(T).Name} that matched your predicate");

			return new EntityAssertions<T>(entities.Single(), _assertionRules);
		}

		public async Task DoesNotContain(TKey key) 
			=> _assertionRules.Null(await _genericRepository.Get(key), $"The {typeof(T).Name} does in fact exist");
	}
}