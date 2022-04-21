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
			var sp = CreateSimpleServicesAndReturnServiceProvider(new Args1());
			var repo = sp.GetRequiredService<IGenericRepository<SimpleEntity1, Guid>>();
			var id = Guid.NewGuid();
			await repo.Save(new SimpleEntity1(id));

			Assert.AreEqual(id, (await repo.Get(id)).Id);
		}

		[Test]
		public async Task CreateSimpleServices_AddingARepositoryAlsoAddsMongoCollection()
		{
			var sp = CreateSimpleServicesAndReturnServiceProvider(new Args1());
			var collection = sp.GetRequiredService<IMongoCollection<SimpleEntity1>>();
			var id = Guid.NewGuid();
			await collection.InsertOneAsync(new SimpleEntity1(id));

			Assert.AreEqual(id, (await collection.FindAsync(x => x.Id == id)).Single().Id);
		}

		[Test]
		public void CreateSimpleServices_CanAddTwoDifferentTypesOfRepository()
		{
			var serviceCreator = CreateServiceCreator(new ServiceCollection());
			serviceCreator.CreateSimpleServices(new GenericMongoRepositoryArgsType(typeof(Args1)));
			Assert.DoesNotThrow(() => serviceCreator.CreateSimpleServices(new GenericMongoRepositoryArgsType(typeof(Args2))));
		}

		[Test]
		public void CreateSimpleServices_CannotAddTwoOfTheSameRepository()
		{
			var serviceCreator = CreateServiceCreator(new ServiceCollection());
			serviceCreator.CreateSimpleServices(new GenericMongoRepositoryArgsType(typeof(Args1)));
			var ex = Assert.Throws<ArgumentException>(() => serviceCreator.CreateSimpleServices(new GenericMongoRepositoryArgsType(typeof(IdenticalArgs1))));

			Assert.AreEqual("A repository for GenericRepository.Mongo.Tests.ServiceCreatorTests+SimpleEntity1 with key System.Guid has already been registered", ex.Message);
		}

        private IServiceProvider CreateSimpleServicesAndReturnServiceProvider<TEntity, TKey>(GenericMongoRepositoryArgs<TEntity, TKey> args)
			where TKey : IEquatable<TKey>
		{
			var sc = new ServiceCollection();
			CreateServiceCreator(sc)
				.CreateSimpleServices(new GenericMongoRepositoryArgsType(args.GetType()));

			return sc.BuildServiceProvider();
		}

		private ServiceCreator CreateServiceCreator(ServiceCollection sc)
			=> new(sc, Config.ConnectionString, Config.DatabaseName);

        private class Args1 : GenericMongoRepositoryArgs<SimpleEntity1, Guid>
		{
			public override Expression<Func<SimpleEntity1, Guid>> KeySelector { get; } = x => x.Id;
		}

		private class SimpleEntity1
		{
			public Guid Id { get; }

			public SimpleEntity1(Guid id)
			{
				Id = id;
			}
		}

		private class IdenticalArgs1 : GenericMongoRepositoryArgs<SimpleEntity1, Guid>
		{
			public override Expression<Func<SimpleEntity1, Guid>> KeySelector { get; } = x => x.Id;
		}

		private class Args2 : GenericMongoRepositoryArgs<SimpleEntity2, Guid>
		{
			public override Expression<Func<SimpleEntity2, Guid>> KeySelector { get; } = x => x.Id;
		}

		private class SimpleEntity2
		{
			public Guid Id { get; }
		}
    }
}