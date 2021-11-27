using System;
using System.Linq.Expressions;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace GenericRepository.Mongo
{
	public class GenericMongoRepositoryBuilder
	{
		private readonly IServiceCollection _serviceCollection;
		private readonly IMongoDatabase _database;
		private readonly Pluralizer.Pluralizer _pluralizer = new Pluralizer.Pluralizer();
		
		internal GenericMongoRepositoryBuilder(IServiceCollection serviceCollection, string connectionString, string databaseName)
		{
			_serviceCollection = serviceCollection;
			_database = new MongoClient(connectionString).GetDatabase(databaseName);
		}
		
		public GenericMongoRepositoryBuilder SimpleAdd<TEntity, TKey>(Expression<Func<TEntity, TKey>> keySelector)
			where TKey : IEquatable<TKey>
		{
			var collection = CreateAndWireUpConnectionCollection<TEntity>();
			if (_serviceCollection.ContainsGenericRepository<TEntity, TKey>())
				throw new ArgumentException($"A repository for {typeof(TEntity).FullName} with key {typeof(TKey).FullName} has already been registered");

			var repo = new SimpleGenericMongoRepository<TEntity, TKey>(keySelector, collection);
			_serviceCollection.AddSingleton<IGenericRepository<TEntity, TKey>>(repo);
			return this;
		}

		private IMongoCollection<TEntity> CreateAndWireUpConnectionCollection<TEntity>()
		{
			var collection = _database
				.GetCollection<TEntity>(_pluralizer.Pluralize(typeof(TEntity).Name));
			_serviceCollection.AddSingleton(collection);
			return collection;
		}
	}
}