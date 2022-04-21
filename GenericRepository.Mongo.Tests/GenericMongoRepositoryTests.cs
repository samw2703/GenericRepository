using System;
using System.Linq;
using System.Threading.Tasks;
using Callinho;
using GenericRepository.Abstractions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GenericRepository.Mongo.Tests
{
	public class GenericMongoRepositoryTests : MongoTestsBase
	{
		private IGenericRepository<Item, Guid> _repo;

		[SetUp]
		public void Setup()
		{
			_repo = CreateRepo();
		}

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
			await _repo.Save(new Item(id, value: "original"));
			await _repo.Save(new Item(id, value: updatedValue));

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

		[Test]
		public async Task GetWhere_GetsMultipleItems()
		{
			var otherId = 1;
			var item1 = Guid.NewGuid()
				.Call(x => new Item(x, otherId))
				.UseAndReturn(async x => await _repo.Save(x));
			var item2 = Guid.NewGuid()
				.Call(x => new Item(x, otherId))
				.UseAndReturn(async x => await _repo.Save(x));
			await _repo.Save(new Item(Guid.NewGuid()));

			var items = await _repo.GetWhere(x => x.OtherId == otherId);

			Assert.AreEqual(2, items.Count);
			Assert.True(items.Any(x => JsonConvert.SerializeObject(x) == JsonConvert.SerializeObject(item1)));
			Assert.True(items.Any(x => JsonConvert.SerializeObject(x) == JsonConvert.SerializeObject(item2)));
		}

		[Test]
		public async Task DeleteWhere_DeletesMultipleItems()
		{
			var otherId = 1;
			var id1 = Guid.NewGuid();
			var id2 = Guid.NewGuid();
			var id3 = Guid.NewGuid();
			await _repo.Save(new Item(id1, otherId));
			await _repo.Save(new Item(id2, otherId));
			await _repo.Save(new Item(id3));
			await _repo.DeleteWhere(x => x.OtherId == otherId);

			Assert.Null(await _repo.Get(id1));
			Assert.Null(await _repo.Get(id2));
			Assert.NotNull(await _repo.Get(id3));
		}

		[Test]
		public async Task UpdateWhere_DeletesMultipleItems()
		{
			const string updateText = " with an update";
			const string item1InitialValue = "Item1 initial value";
			const string item2InitialValue = "Item2 initial value";
			const string item3InitialValue = "Item3 initial value";
			var otherId = 1;
			var id1 = Guid.NewGuid();
			var id2 = Guid.NewGuid();
			var id3 = Guid.NewGuid();
			await _repo.Save(new Item(id1, otherId, item1InitialValue));
			await _repo.Save(new Item(id2, otherId, item2InitialValue));
			await _repo.Save(new Item(id3, value: item3InitialValue));

			await _repo.UpdateWhere(x => x.SetValue(x.Value + " with an update"), x => x.OtherId == otherId);

			Assert.AreEqual($"{item1InitialValue}{updateText}", (await _repo.Get(id1)).Value);
			Assert.AreEqual($"{item2InitialValue}{updateText}", (await _repo.Get(id2)).Value);
			Assert.AreEqual(item3InitialValue, (await _repo.Get(id3)).Value);
		}

        private IGenericRepository<Item, Guid> CreateRepo()
            => new GenericMongoRepository<Item, Guid>(x => x.Id, GetCollection<Item>());

		protected class Item
		{
			public Guid Id { get; }
			public int OtherId { get; }
			public string Value { get; private set; }

			public void SetValue(string newValue)
				=> Value = newValue;

			public Item(Guid id, int otherId = 0, string value = "")
			{
				Id = id;
				OtherId = otherId;
				Value = value;
			}
		}
	}
}