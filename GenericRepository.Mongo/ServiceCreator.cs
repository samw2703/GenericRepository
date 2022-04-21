using System;
using System.Linq;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace GenericRepository.Mongo
{
	internal class ServiceCreator
	{
		private readonly IServiceCollection _services;
        private readonly Pluralizer.Pluralizer _pluralizer = new Pluralizer.Pluralizer();

		public ServiceCreator(IServiceCollection services, string connectionString, string databaseName)
		{
			_services = services;
            _services.AddSingleton(new MongoClient(connectionString).GetDatabase(databaseName));
		}

        public void CreateServices(GenericMongoRepositoryArgsType argsType)
		{
			ValidateGenericRepositoryNotAlreadyWired(argsType.EntityType, argsType.KeyType);
			AddMongoCollection(argsType.EntityType);
			AddRepository(argsType);
		}

        private void AddRepository(GenericMongoRepositoryArgsType argsType)
		{
			var serviceType = Helper.CreateIGenericRepositoryType(argsType.EntityType, argsType.KeyType);
			_services.AddSingleton(serviceType, sp =>
			{
				var impl = argsType.CreateGenericMongoRepositoryType();
				var constructor = impl.GetConstructors().Single();
				return constructor.Invoke(new[]
				{
					argsType.GetKeySelector(),
					sp.GetRequiredService(typeof(IMongoCollection<>).MakeGenericType(argsType.EntityType))
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