using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NUnit.Framework;

namespace GenericRepository.Mongo.Tests
{
	public class ServiceCreatorTests : MongoTestsBase
	{
        [Test]
		public async Task CreateSimpleServices_CanAddARepository()
		{
			var sp = CreateSimpleServicesAndReturnServiceProvider(new SimpleArgs1());
			var repo = sp.GetRequiredService<IGenericRepository<SimpleEntity1, Guid>>();
			var id = Guid.NewGuid();
			await repo.Save(new SimpleEntity1(id));

			Assert.AreEqual(id, (await repo.Get(id)).Id);
		}

		[Test]
		public async Task CreateSimpleServices_AddingARepositoryAlsoAddsMongoCollection()
		{
			var sp = CreateSimpleServicesAndReturnServiceProvider(new SimpleArgs1());
			var collection = sp.GetRequiredService<IMongoCollection<SimpleEntity1>>();
			var id = Guid.NewGuid();
			await collection.InsertOneAsync(new SimpleEntity1(id));

			Assert.AreEqual(id, (await collection.FindAsync(x => x.Id == id)).Single().Id);
		}

		[Test]
		public void CreateSimpleServices_CanAddTwoDifferentTypesOfRepository()
		{
			var serviceCreator = CreateServiceCreator(new ServiceCollection());
			serviceCreator.CreateSimpleServices(new SimpleGenericMongoRepositoryArgsType(typeof(SimpleArgs1)));
			Assert.DoesNotThrow(() => serviceCreator.CreateSimpleServices(new SimpleGenericMongoRepositoryArgsType(typeof(SimpleArgs2))));
		}

		[Test]
		public void CreateSimpleServices_CannotAddTwoOfTheSameRepository()
		{
			var serviceCreator = CreateServiceCreator(new ServiceCollection());
			serviceCreator.CreateSimpleServices(new SimpleGenericMongoRepositoryArgsType(typeof(SimpleArgs1)));
			var ex = Assert.Throws<ArgumentException>(() => serviceCreator.CreateSimpleServices(new SimpleGenericMongoRepositoryArgsType(typeof(IdenticalSimpleArgs1))));

			Assert.AreEqual("A repository for GenericRepository.Mongo.Tests.ServiceCreatorTests+SimpleEntity1 with key System.Guid has already been registered", ex.Message);
		}

        private IServiceProvider CreateSimpleServicesAndReturnServiceProvider<TEntity, TKey>(ISimpleGenericMongoRepositoryArgs<TEntity, TKey> args)
			where TKey : IEquatable<TKey>
		{
			var sc = new ServiceCollection();
			CreateServiceCreator(sc)
				.CreateSimpleServices(new SimpleGenericMongoRepositoryArgsType(args.GetType()));

			return sc.BuildServiceProvider();
		}

		private ServiceCreator CreateServiceCreator(ServiceCollection sc)
			=> new(sc, Config.ConnectionString, Config.DatabaseName);

        private class SimpleArgs1 : ISimpleGenericMongoRepositoryArgs<SimpleEntity1, Guid>
		{
			public Expression<Func<SimpleEntity1, Guid>> KeySelector { get; } = x => x.Id;
		}

		private class SimpleEntity1
		{
			public Guid Id { get; }

			public SimpleEntity1(Guid id)
			{
				Id = id;
			}
		}

		private class IdenticalSimpleArgs1 : ISimpleGenericMongoRepositoryArgs<SimpleEntity1, Guid>
		{
			public Expression<Func<SimpleEntity1, Guid>> KeySelector { get; } = x => x.Id;
		}

		private class SimpleArgs2 : ISimpleGenericMongoRepositoryArgs<SimpleEntity2, Guid>
		{
			public Expression<Func<SimpleEntity2, Guid>> KeySelector { get; } = x => x.Id;
		}

		private class SimpleEntity2
		{
			public Guid Id { get; }
		}
    }
}