using System;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Stub
{
	public class GenericStubbedRepositoryBuilder
	{
		private readonly IServiceCollection _serviceCollection;

		internal GenericStubbedRepositoryBuilder(IServiceCollection serviceCollection)
		{
			_serviceCollection = serviceCollection;
		}

		public GenericStubbedRepositoryBuilder Add<T, TKey>(Func<T, TKey> keySelector)
			where TKey : IEquatable<TKey>
		{
			var repo = new GenericStubbedRepository<T, TKey>(keySelector);
			if (_serviceCollection.ContainsGenericRepository<T, TKey>())
				throw new ArgumentException($"A repository for {typeof(T).FullName} with key {typeof(TKey).FullName} has already been registered");
			
			_serviceCollection.AddSingleton<IGenericRepository<T, TKey>>(repo);
			return this;
		}
	}
}