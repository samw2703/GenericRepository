using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Callinho;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NUnit.Framework;

namespace GenericRepository.Mongo.Tests
{
	public class GenericMongoRepositoryBuilderTests : MongoTestsBase
	{
		private Expression<Func<RepoItemEntity, RepoItem>> _mapToDocument = x => new RepoItem(x.Id);
		private Expression<Func<RepoItem, RepoItemEntity>> _mapFromDocument = x => new RepoItemEntity(x.Id);

		[SetUp]
		public void Setup()
		{

		}

		[Test]
		public void SimpleAdd_WhenNoRepoForEntityKeyComboYetAdded_DoesAddRepository()
		{
			var serviceCollection = new ServiceCollection();
			new GenericMongoRepositoryBuilder(serviceCollection, Config.ConnectionString, Config.DatabaseName)
				.SimpleAdd<RepoItem, int>(x => x.Id);

			serviceCollection
				.SingleOrDefault(x => x.ServiceType == typeof(IGenericRepository<RepoItem, int>) && x.ImplementationInstance?.GetType() == typeof(SimpleGenericMongoRepository<RepoItem, int>))
				.Use(Assert.IsNotNull);
		}

		[Test]
		public void SimpleAdd_WhenNoRepoForEntityKeyComboYetAdded_DoesAddCollection()
		{
			var serviceCollection = new ServiceCollection();
			new GenericMongoRepositoryBuilder(serviceCollection, Config.ConnectionString, Config.DatabaseName)
				.SimpleAdd<RepoItem, int>(x => x.Id);

			serviceCollection
				.SingleOrDefault(x => x.ServiceType == typeof(IMongoCollection<RepoItem>))
				.Use(Assert.IsNotNull);
		}

		[Test]
		public void SimpleAdd_WhenRepoAlreadyAddedForEntityKeyCombo_Throws()
		{
			var serviceCollection = new ServiceCollection();
			var builder = new GenericMongoRepositoryBuilder(serviceCollection, Config.ConnectionString, Config.DatabaseName)
				.SimpleAdd<RepoItem, int>(x => x.Id);

			var ex = Assert.Throws<ArgumentException>(() => builder.SimpleAdd<RepoItem, int>(x => x.Id));
			Assert.AreEqual("A repository for GenericRepository.Mongo.Tests.GenericMongoRepositoryBuilderTests+RepoItem with key System.Int32 has already been registered", ex.Message);
		}

		[Test]
		public async Task SimpleAdd_RepoThatIsWiredUpIsValidMongoRepo()
		{
			var serviceCollection = new ServiceCollection();
			new GenericMongoRepositoryBuilder(serviceCollection, Config.ConnectionString, Config.DatabaseName)
				.SimpleAdd<RepoItem, int>(x => x.Id);

			var id = 1;
			var serviceProvider = serviceCollection.BuildServiceProvider();
			await serviceProvider.GetRequiredService<IGenericRepository<RepoItem, int>>().Save(new RepoItem(id));
			var item = (await serviceProvider.GetService<IMongoCollection<RepoItem>>().FindAsync(x => x.Id == id))
				.SingleOrDefault();

			Assert.AreEqual(id, item?.Id);
		}

		[Test]
		public void Add_WhenNoRepoForEntityKeyComboYetAdded_DoesAddRepository()
		{
			var serviceCollection = new ServiceCollection();
			new GenericMongoRepositoryBuilder(serviceCollection, Config.ConnectionString, Config.DatabaseName)
				.Add(x => x.Id, _mapFromDocument, _mapToDocument);

			serviceCollection
				.SingleOrDefault(x => x.ServiceType == typeof(IGenericRepository<RepoItemEntity, int>) && x.ImplementationInstance?.GetType() == typeof(GenericMongoRepository<RepoItemEntity, int, RepoItem>))
				.Use(Assert.IsNotNull);
		}

		[Test]
		public void Add_WhenNoRepoForEntityKeyComboYetAdded_DoesAddCollection()
		{
			var serviceCollection = new ServiceCollection();
			new GenericMongoRepositoryBuilder(serviceCollection, Config.ConnectionString, Config.DatabaseName)
				.Add(x => x.Id, _mapFromDocument, _mapToDocument);

			serviceCollection
				.SingleOrDefault(x => x.ServiceType == typeof(IMongoCollection<RepoItem>))
				.Use(Assert.IsNotNull);
		}

		[Test]
		public void Add_WhenRepoAlreadyAddedForEntityKeyCombo_Throws()
		{
			var serviceCollection = new ServiceCollection();
			var builder = new GenericMongoRepositoryBuilder(serviceCollection, Config.ConnectionString, Config.DatabaseName)
				.Add(x => x.Id, _mapFromDocument, _mapToDocument);

			var ex = Assert.Throws<ArgumentException>(() => builder.Add(x => x.Id, _mapFromDocument, _mapToDocument));
			Assert.AreEqual("A repository for GenericRepository.Mongo.Tests.GenericMongoRepositoryBuilderTests+RepoItemEntity with key System.Int32 has already been registered", ex.Message);
		}

		[Test]
		public async Task Add_RepoThatIsWiredUpIsValidMongoRepo()
		{
			var serviceCollection = new ServiceCollection();
			new GenericMongoRepositoryBuilder(serviceCollection, Config.ConnectionString, Config.DatabaseName)
				.Add(x => x.Id, _mapFromDocument, _mapToDocument);

			var id = 1;
			var serviceProvider = serviceCollection.BuildServiceProvider();
			await serviceProvider.GetRequiredService<IGenericRepository<RepoItemEntity, int>>().Save(new RepoItemEntity(id));
			var item = (await serviceProvider.GetService<IMongoCollection<RepoItem>>().FindAsync(x => x.Id == id))
				.SingleOrDefault();

			Assert.AreEqual(id, item?.Id);
		}

		private class RepoItem
		{
			public int Id { get; }

			public RepoItem(int id)
			{
				Id = id;
			}
		}

		private class RepoItemEntity
		{
			public int Id { get; }

			public RepoItemEntity(int id)
			{
				Id = id;
			}
		}
	}
}