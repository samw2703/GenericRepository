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
			ValidateKeySelector<TEntity, TKey, TEntity>(keySelector);
			var collection = CreateAndWireUpConnectionCollection<TEntity>();
			if (_serviceCollection.ContainsGenericRepository<TEntity, TKey>())
				throw new ArgumentException($"A repository for {typeof(TEntity).FullName} with key {typeof(TKey).FullName} has already been registered");

			var repo = new SimpleGenericMongoRepository<TEntity, TKey>(keySelector, collection);
			_serviceCollection.AddSingleton<IGenericRepository<TEntity, TKey>>(repo);
			return this;
		}

		public GenericMongoRepositoryBuilder Add<TEntity, TKey, TDocument>(Expression<Func<TDocument, TKey>> keySelector,
			Expression<Func<TDocument, TEntity>> mapFromDocument,
			Expression<Func<TEntity, TDocument>> mapToDocument)
			where TKey : IEquatable<TKey>
		{
			ValidateArgs(keySelector, mapFromDocument, mapToDocument);
			var collection = CreateAndWireUpConnectionCollection<TDocument>();
			if (_serviceCollection.ContainsGenericRepository<TEntity, TKey>())
				throw new ArgumentException($"A repository for {typeof(TEntity).FullName} with key {typeof(TKey).FullName} has already been registered");

			var repo = new GenericMongoRepository<TEntity, TKey, TDocument>(keySelector, mapFromDocument, mapToDocument.Compile(), collection);
			_serviceCollection.AddSingleton<IGenericRepository<TEntity, TKey>>(repo);
			return this;
		}

		public GenericMongoRepositoryBuilder Add<TEntity, TKey, TDocument>(
			IGenericMongoRepositoryArgs<TEntity, TKey, TDocument> args)
			where TKey : IEquatable<TKey>
		{
			return Add(args.keySelector, args.MapFromDocument, args.MapToDocument);
		}

		private void ValidateArgs<TEntity, TKey, TDocument>(Expression<Func<TDocument, TKey>> keySelector,
			Expression<Func<TDocument, TEntity>> mapFromDocument,
			Expression<Func<TEntity, TDocument>> mapToDocument)
		{
			ValidateKeySelector<TDocument, TKey, TEntity>(keySelector);
			if (mapFromDocument == null)
				throw new ArgumentException($"The map from document args for the repository defined for {typeof(TEntity).FullName} cannot be null");

			if (mapToDocument == null)
				throw new ArgumentException($"The map to document args for the repository defined for {typeof(TEntity).FullName} cannot be null");
		}

		private void ValidateKeySelector<T, TKey, TEntity>(Expression<Func<T, TKey>> keySelector)
		{
			if (keySelector == null)
				throw new ArgumentException($"The key selector for the repository defined for {typeof(TEntity).FullName} cannot be null");
		}

		private IMongoCollection<TEntity> CreateAndWireUpConnectionCollection<TEntity>()
		{
			var collection = _database
				.GetCollection<TEntity>(_pluralizer.Pluralize(typeof(TEntity).Name));
			_serviceCollection.AddSingleton(collection);
			return collection;
		}
	}

	public interface IGenericMongoRepositoryArgs<TEntity, TKey, TDocument> where TKey : IEquatable<TKey>
	{
		Expression<Func<TDocument, TKey>> keySelector { get; }
		Expression<Func<TDocument, TEntity>> MapFromDocument { get; }
		Expression<Func<TEntity, TDocument>> MapToDocument { get; }
	}
}