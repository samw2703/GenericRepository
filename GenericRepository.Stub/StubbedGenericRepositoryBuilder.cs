using System;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Stub
{
	public class StubbedGenericRepositoryBuilder
	{
		private readonly IServiceCollection _serviceCollection;

		internal StubbedGenericRepositoryBuilder(IServiceCollection serviceCollection)
		{
			_serviceCollection = serviceCollection;
		}

		public StubbedGenericRepositoryBuilder Add<T, TKey>(Func<T, TKey> keySelector)
			where TKey : IEquatable<TKey>
		{
			var repo = new GenericStubbedRepository<T, TKey>(keySelector);
			if (_serviceCollection.HasService<IGenericRepository<T, TKey>>())
				throw new ArgumentException($"A repository for {typeof(T).FullName} with key {typeof(TKey).FullName}");
			
			_serviceCollection.AddSingleton<IGenericRepository<T, TKey>>(repo);
			return this;
		}
	}
}