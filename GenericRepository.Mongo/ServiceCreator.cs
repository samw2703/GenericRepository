﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;

namespace GenericRepository.Mongo
{
	internal class ServiceCreator
	{
		private readonly IServiceCollection _services;
		private readonly IMongoDatabase _database;
		private readonly Pluralizer.Pluralizer _pluralizer = new Pluralizer.Pluralizer();

		public ServiceCreator(IServiceCollection services, string connectionString, string databaseName)
		{
			_services = services;
			_database = new MongoClient(connectionString).GetDatabase(databaseName);
			_services.AddSingleton(_database);
		}

		public void CreateServices(GenericMongoRepositoryArgsType argsType)
		{
			ValidateGenericRepositoryNotAlreadyWired(argsType.GetEntityType(), argsType.GetKeyType());
			AddMongoCollection(argsType.GetDocumentType());
			AddRepository(argsType);
		}

		private void AddRepository(GenericMongoRepositoryArgsType argsType)
		{
			var serviceType = typeof(IGenericRepository<object, int>)
				.GetGenericTypeDefinition()
				.MakeGenericType(argsType.GetEntityType(), argsType.GetKeyType());
			_services.AddSingleton(serviceType, sp =>
			{
				var impl = argsType.CreateGenericMongoRepositoryType();
				var constructor = impl.GetConstructors().Single();
				return constructor.Invoke(new[]
				{
					argsType.GetKeySelector(),
					argsType.GetMapFromDocument(),
					argsType.GetMapToDocument(),
					sp.GetRequiredService(typeof(IMongoCollection<>).MakeGenericType(argsType.GetDocumentType()))
				});
			});
		}

		private void AddMongoCollection(Type documentType)
		{
			_services.AddSingleton(typeof(IMongoCollection<>).MakeGenericType(documentType), sp =>
			{
				var db = sp.GetRequiredService<IMongoDatabase>();
				return db.GetType()
					.GetMethod(nameof(db.GetCollection))
					.MakeGenericMethod(documentType)
					.Invoke(db, new object[] { _pluralizer.Pluralize(documentType.Name), null });
			});
		}

		private void ValidateGenericRepositoryNotAlreadyWired(Type entityType, Type keyType)
		{
			var genericRepositoryType = typeof(IGenericRepository<object, int>)
				.GetGenericTypeDefinition()
				.MakeGenericType(entityType, keyType);

			if (_services.HasService(genericRepositoryType))
				throw new ArgumentException($"A repository for {entityType.FullName} with key {keyType.FullName} has already been registered");
		}
	}
}