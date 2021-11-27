using System;
using System.Linq.Expressions;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using SharpCompress.Common.Tar;

namespace GenericRepository.Mongo
{
	public class GenericMongoRepositoryBuilder
	{
		private readonly IServiceCollection _serviceCollection;

		internal GenericMongoRepositoryBuilder(IServiceCollection serviceCollection)
		{
			_serviceCollection = serviceCollection;
		}

		public GenericMongoRepositoryBuilder Add<TEntity, TKey>(Expression<Func<TEntity, TKey>> keySelector)
			where TKey : IEquatable<TKey>
		{
			var repo = new SimpleGenericMongoRepository<TEntity, TKey>(keySelector);
			if (_serviceCollection.ContainsGenericRepository<TEntity, TKey>())
				throw new ArgumentException($"A repository for {typeof(TEntity).FullName} with key {typeof(TKey).FullName} has already been registered");

			_serviceCollection.AddSingleton<IGenericRepository<TEntity, TKey>>(repo);
			return this;
		}
	}
}