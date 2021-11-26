using System;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using MongoDB.Bson.Serialization.Attributes;
using NUnit.Framework;

namespace GenericRepository.Mongo.Tests
{
	public abstract class BaseMongoRepositoryTests : MongoTestsBase
	{
		private IGenericRepository<Item, Guid> _repo;

		[SetUp]
		public void Setup()
		{
			_repo = CreateRepo();
		}

		protected abstract IGenericRepository<Item, Guid> CreateRepo();

		[Test]
		public async Task CanSaveAndThenGetARecord()
		{
			var id = Guid.NewGuid();
			await _repo.Save(new Item(id));

			Assert.IsNotNull(await _repo.Get(id));
		}

		[Test]
		public async Task Get_WhenRecordDoesNotExist_ReturnsNull()
		{
			Assert.Null(await _repo.Get(Guid.NewGuid()));
		}

		[Test]
		public async Task Save_WhenRecordAlreadyExists_UpdatesRecord()
		{
			const string updatedValue = "I am updated";
			var id = Guid.NewGuid();
			await _repo.Save(new Item(id, "original"));
			await _repo.Save(new Item(id, updatedValue));

			Assert.AreEqual(updatedValue, (await _repo.Get(id)).Value);
		}

		[Test]
		public async Task Delete_WhenItemExists_DoesDelete()
		{
			var id = Guid.NewGuid();
			await _repo.Save(new Item(id));
			await _repo.Delete(id);

			Assert.IsNull(await _repo.Get(id));
		}

		[Test]
		public async Task Delete_WhenItemDoesNotExist_DoesNotThrow()
		{
			Assert.DoesNotThrowAsync(async () => await _repo.Delete(Guid.NewGuid()));
		}

		protected class Item
		{
			public Guid Id { get; }
			public string Value { get; }

			public Item(Guid id, string value = "")
			{
				Id = id;
				Value = value;
			}
		}
	}
}