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
			var collection = sp.GetRequiredService<IMongoCollection<Document1>>();
			var id = Guid.NewGuid();
			await collection.InsertOneAsync(new Document1(id));

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

		private IServiceProvider CreateServicesAndReturnServiceProvider<TEntity, TKey, TDocument>(IGenericMongoRepositoryArgs<TEntity, TKey, TDocument> args)
			where TKey : IEquatable<TKey>
		{
			var sc = new ServiceCollection();
			CreateServiceCreator(sc)
				.CreateServices(new GenericMongoRepositoryArgsType(args.GetType()));

			return sc.BuildServiceProvider();
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

		private class Args1 : IGenericMongoRepositoryArgs<Entity1, Guid, Document1>
		{
			public Expression<Func<Document1, Guid>> KeySelector { get; } = x => x.Id;
			public Expression<Func<Document1, Entity1>> MapFromDocument { get; } = x => new Entity1(x.Id);
			public Func<Entity1, Document1> MapToDocument { get; } = x => new Document1(x.Id);
		}

		private class Entity1
		{
			public Guid Id { get; }

			public Entity1(Guid id)
			{
				Id = id;
			}
		}

		private class Document1
		{
			public Guid Id { get; }

			public Document1(Guid id)
			{
				Id = id;
			}
		}

		private class IdenticalArgs1 : IGenericMongoRepositoryArgs<Entity1, Guid, Document1>
		{
			public Expression<Func<Document1, Guid>> KeySelector { get; } = x => x.Id;
			public Expression<Func<Document1, Entity1>> MapFromDocument { get; } = x => new Entity1(x.Id);
			public Func<Entity1, Document1> MapToDocument { get; } = x => new Document1(x.Id);
		}

		private class Args2 : IGenericMongoRepositoryArgs<Entity2, Guid, Document2>
		{
			public Expression<Func<Document2, Guid>> KeySelector { get; } = x => x.Id;
			public Expression<Func<Document2, Entity2>> MapFromDocument { get; } = x => new Entity2(x.Id);
			public Func<Entity2, Document2> MapToDocument { get; } = x => new Document2(x.Id);
		}

		private class Entity2
		{
			public Guid Id { get; }

			public Entity2(Guid id)
			{
				Id = id;
			}
		}

		private class Document2
		{
			public Guid Id { get; }

			public Document2(Guid id)
			{
				Id = id;
			}
		}

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