using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NUnit.Framework;

namespace GenericRepository.Mongo.Tests
{
	public class ServiceCreatorTests : MongoTestsBase
	{
        [SetUp]
        public void Setup()
        {
            BsonClassMapHelper.Clear();
		}

        [Test]
		public async Task CreateServices_CanAddARepository()
		{
			var sp = CreateServicesAndReturnServiceProvider(new Args1());
			var repo = sp.GetRequiredService<IGenericRepository<Entity1, Guid>>();
			var id = Guid.NewGuid();
			await repo.Save(new Entity1(id));

			Assert.AreEqual(id, (await repo.Get(id)).Id);
		}

		[Test]
		public async Task CreateServices_AddingARepositoryAlsoAddsMongoCollection()
		{
			var sp = CreateServicesAndReturnServiceProvider(new Args1());
			var collection = sp.GetRequiredService<IMongoCollection<Entity1>>();
			var id = Guid.NewGuid();
			await collection.InsertOneAsync(new Entity1(id));

			Assert.AreEqual(id, (await collection.FindAsync(x => x.Id == id)).Single().Id);
		}

		[Test]
		public void CreateServices_CanAddTwoDifferentTypesOfRepository()
		{
			var serviceCreator = CreateServiceCreator(new ServiceCollection());
			serviceCreator.CreateServices(new GenericMongoRepositoryArgsType(typeof(Args1)));
			Assert.DoesNotThrow(() => serviceCreator.CreateServices(new GenericMongoRepositoryArgsType(typeof(Args2))));
		}

		[Test]
		public void CreateServices_CannotAddTwoOfTheSameRepository()
		{
			var serviceCreator = CreateServiceCreator(new ServiceCollection());
			serviceCreator.CreateServices(new GenericMongoRepositoryArgsType(typeof(Args1)));
			var ex = Assert.Throws<ArgumentException>(() => serviceCreator.CreateServices(new GenericMongoRepositoryArgsType(typeof(IdenticalArgs1))));

			Assert.AreEqual("A repository for GenericRepository.Mongo.Tests.ServiceCreatorTests+Entity1 with key System.Guid has already been registered", ex.Message);
		}

        private IServiceProvider CreateServicesAndReturnServiceProvider<TEntity, TKey>(GenericMongoRepositoryArgs<TEntity, TKey> args)
			where TKey : IEquatable<TKey>
		{
			var sc = new ServiceCollection();
			CreateServiceCreator(sc)
				.CreateServices(new GenericMongoRepositoryArgsType(args.GetType()));

			return sc.BuildServiceProvider();
		}

		private ServiceCreator CreateServiceCreator(ServiceCollection sc)
			=> new(sc, Config.ConnectionString, Config.DatabaseName);

        private class Args1 : GenericMongoRepositoryArgs<Entity1, Guid>
		{
			public override Expression<Func<Entity1, Guid>> KeySelector { get; } = x => x.Id;
		}

		private class Entity1
		{
			public Guid Id { get; }

			public Entity1(Guid id)
			{
				Id = id;
			}
		}

		private class IdenticalArgs1 : GenericMongoRepositoryArgs<Entity1, Guid>
		{
			public override Expression<Func<Entity1, Guid>> KeySelector { get; } = x => x.Id;
		}

		private class Args2 : GenericMongoRepositoryArgs<Entity2, Guid>
		{
			public override Expression<Func<Entity2, Guid>> KeySelector { get; } = x => x.Id;
		}

		private class Entity2
		{
			public Guid Id { get; }
		}
    }
}