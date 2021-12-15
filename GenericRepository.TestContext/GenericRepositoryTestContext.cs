using System;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using GenericRepository.Stub;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.TestContext
{
	public class GenericRepositoryTestContext<T, TKey> where TKey : IEquatable<TKey>
	{
		private readonly object _repositoryFactoryLock = new object();
		private IGenericRepository<T, TKey> _repository;
		private readonly IServiceCollection _serviceCollection;
		private readonly Func<TKey, T> _createDefault;
		private readonly IAssertionRules _assertionRules;

		public GenericRepositoryTestContext(IServiceCollection serviceCollection, IAssertionRules assertionRules, Func<T, TKey> keySelector, Func<TKey, T> createDefault)
		{
			_assertionRules = assertionRules;
			serviceCollection.UseStubbedGenericRepositories().Add(keySelector);
			_createDefault = createDefault;
			_serviceCollection = serviceCollection;
		}

		public async Task CreateDefault(TKey key, Action<T> mutate = null)
		{
			var entity = _createDefault(key);
				if (mutate != null)
					mutate(entity);
				await Save(entity);
		}

		public async Task Save(T entity)
			=> await GetRepository().Save(entity);

		public GenericRepositoryAssertions<T, TKey> Assert()
			=> new GenericRepositoryAssertions<T, TKey>(GetRepository(), _assertionRules);

		private IGenericRepository<T, TKey> GetRepository()
		{
			lock (_repositoryFactoryLock)
			{
				if (_repository == null)
					_repository = _serviceCollection
						.BuildServiceProvider()
						.GetRequiredService<IGenericRepository<T, TKey>>();

				return _repository;
			}
		}
	}
}