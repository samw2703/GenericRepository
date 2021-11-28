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
		private static readonly Expression<Func<RepoItemEntity, RepoItem>> MapToDocument = x => new RepoItem(x.Id);
		private static readonly Expression<Func<RepoItem, RepoItemEntity>> MapFromDocument = x => new RepoItemEntity(x.Id);

		public delegate void AddMongoRepository(GenericMongoRepositoryBuilder builder);

		private static void AddGenericMongoRepository(GenericMongoRepositoryBuilder builder)
			=> builder.Add(x => x.Id, MapFromDocument, MapToDocument);

		private static void SimpleAddGenericMongoRepository(GenericMongoRepositoryBuilder builder)
			=> builder.SimpleAdd<RepoItemEntity, int>(x => x.Id);

		private static object[] _simpleAddThrowTestCases =
		{
			new AddMongoRepository[] { SimpleAddGenericMongoRepository },
			new AddMongoRepository[] { AddGenericMongoRepository }
		};

		private static object[] _singleAddTestCases =
		{
			new AddMongoRepository[] { AddGenericMongoRepository }
		};

		private static object[] _addThrowTestCases =
		{
			new AddMongoRepository[] { SimpleAddGenericMongoRepository, AddGenericMongoRepository },
			new AddMongoRepository[] { AddGenericMongoRepository, AddGenericMongoRepository }
		};

		[Test]
		public void SimpleAdd_PassedInKeySelectorIsNull_Throws()
		{
			var serviceCollection = new ServiceCollection();
			var ex = Assert.Throws<ArgumentException>(() => CreateBuilder(serviceCollection).SimpleAdd<RepoItemEntity, int>(null));

			Assert.AreEqual("The key selector for the repository defined for GenericRepository.Mongo.Tests.GenericMongoRepositoryBuilderTests+RepoItemEntity cannot be null", ex.Message);
		}
		
		[Test]
		public void SimpleAdd_WhenNoRepoForEntityKeyComboYetAdded_DoesAddRepository()
		{
			var serviceCollection = new ServiceCollection();
			CreateBuilder(serviceCollection).SimpleAdd<RepoItemEntity, int>(x => x.Id);

			serviceCollection
				.SingleOrDefault(x => x.ServiceType == typeof(IGenericRepository<RepoItemEntity, int>) && x.ImplementationInstance?.GetType() == typeof(SimpleGenericMongoRepository<RepoItemEntity, int>))
				.Use(Assert.IsNotNull);
		}

		[Test]
		public void SimpleAdd_WhenNoRepoForEntityKeyComboYetAdded_DoesAddCollection()
		{
			var serviceCollection = new ServiceCollection();
			CreateBuilder(serviceCollection).SimpleAdd<RepoItemEntity, int>(x => x.Id);

			serviceCollection
				.SingleOrDefault(x => x.ServiceType == typeof(IMongoCollection<RepoItemEntity>))
				.Use(Assert.IsNotNull);
		}

		[TestCaseSource(nameof(_simpleAddThrowTestCases))]
		public void SimpleAdd_WhenRepoAlreadyAddedForEntityKeyCombo_Throws(AddMongoRepository initialAdd)
		{
			var serviceCollection = new ServiceCollection();
			var builder = CreateBuilder(serviceCollection).UseAndReturn(x => initialAdd(x));

			var ex = Assert.Throws<ArgumentException>(() => builder.SimpleAdd<RepoItemEntity, int>(x => x.Id));
			Assert.AreEqual("A repository for GenericRepository.Mongo.Tests.GenericMongoRepositoryBuilderTests+RepoItemEntity with key System.Int32 has already been registered", ex.Message);
		}

		[Test]
		public async Task SimpleAdd_RepoThatIsWiredUpIsValidMongoRepo()
		{
			var serviceCollection = new ServiceCollection();
			CreateBuilder(serviceCollection).SimpleAdd<RepoItemEntity, int>(x => x.Id);

			var id = 1;
			var serviceProvider = serviceCollection.BuildServiceProvider();
			await serviceProvider.GetRequiredService<IGenericRepository<RepoItemEntity, int>>().Save(new RepoItemEntity(id));
			var item = (await serviceProvider.GetService<IMongoCollection<RepoItemEntity>>().FindAsync(x => x.Id == id))
				.SingleOrDefault();

			Assert.AreEqual(id, item?.Id);
		}

		[TestCaseSource(nameof(_singleAddTestCases))]
		public void Add_WhenNoRepoForEntityKeyComboYetAdded_DoesAddRepository(AddMongoRepository add)
		{
			var serviceCollection = new ServiceCollection();
			CreateBuilder(serviceCollection).Use(x => add(x));

			serviceCollection
				.SingleOrDefault(x => x.ServiceType == typeof(IGenericRepository<RepoItemEntity, int>) && x.ImplementationInstance?.GetType() == typeof(GenericMongoRepository<RepoItemEntity, int, RepoItem>))
				.Use(Assert.IsNotNull);
		}

		[TestCaseSource(nameof(_singleAddTestCases))]
		public void Add_WhenNoRepoForEntityKeyComboYetAdded_DoesAddCollection(AddMongoRepository add)
		{
			var serviceCollection = new ServiceCollection();
			CreateBuilder(serviceCollection).Use(x => add(x));

			serviceCollection
				.SingleOrDefault(x => x.ServiceType == typeof(IMongoCollection<RepoItem>))
				.Use(Assert.IsNotNull);
		}

		[TestCaseSource(nameof(_addThrowTestCases))]
		public void Add_WhenRepoAlreadyAddedForEntityKeyCombo_Throws(AddMongoRepository initialAdd, AddMongoRepository duplicateAdd)
		{
			var serviceCollection = new ServiceCollection();
			var builder = CreateBuilder(serviceCollection).UseAndReturn(x => initialAdd(x));

			var ex = Assert.Throws<ArgumentException>(() => builder.Use(x => duplicateAdd(x)));
			Assert.AreEqual("A repository for GenericRepository.Mongo.Tests.GenericMongoRepositoryBuilderTests+RepoItemEntity with key System.Int32 has already been registered", ex.Message);
		}

		[TestCaseSource(nameof(_singleAddTestCases))]
		public async Task Add_RepoThatIsWiredUpIsValidMongoRepo(AddMongoRepository add)
		{
			var serviceCollection = new ServiceCollection();
			CreateBuilder(serviceCollection).Use(x => add(x));

			var id = 1;
			var serviceProvider = serviceCollection.BuildServiceProvider();
			await serviceProvider.GetRequiredService<IGenericRepository<RepoItemEntity, int>>().Save(new RepoItemEntity(id));
			var item = (await serviceProvider.GetService<IMongoCollection<RepoItem>>().FindAsync(x => x.Id == id))
				.SingleOrDefault();

			Assert.AreEqual(id, item?.Id);
		}

		private GenericMongoRepositoryBuilder CreateBuilder(IServiceCollection serviceCollection)
			=> new(serviceCollection, Config.ConnectionString, Config.DatabaseName);

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