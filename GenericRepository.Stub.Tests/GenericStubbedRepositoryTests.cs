using System;
using System.Linq;
using System.Threading.Tasks;
using Callinho;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GenericRepository.Stub.Tests
{
	public partial class GenericStubbedRepositoryTests
	{
		private readonly GenericStubbedRepository<RepoItem, Guid> _repo = new(x => x.Id);
		private readonly Guid _firstItemId = Guid.NewGuid();
		private readonly string _firstItemValue = "I am the first item";

		[SetUp]
		public async Task Setup()
		{
			await _repo.Save(new RepoItem(_firstItemId, value: _firstItemValue));
		}

		[Test]
		public async Task Get_IfItemExists_ReturnsIt()
		{
			var item = await _repo.Get(_firstItemId);
			
			Assert.AreEqual(_firstItemValue, item?.Value);
		}

		[Test]
		public async Task Get_IfItemDoesNotExist_ReturnsNull()
		{
			var item = await _repo.Get(Guid.Empty);

			Assert.Null(item);
		}

		[Test]
		public async Task Save_IfNoItemWithKeyExists_SavesNewItem()
		{
			var itemId = Guid.NewGuid()
				.UseAndReturn(async id => await _repo.Save(new RepoItem(id, value: "hey")));

			Assert.IsNotNull(await _repo.Get(itemId));
		}

		[Test]
		public async Task Save_IfItemWithKeyExists_UpdatesItem()
		{
			const string updateText = "I have been updated";
			var itemId = Guid.NewGuid()
				.UseAndReturn(async id => await _repo.Save(new RepoItem(id, value: "hey")))
				.UseAndReturn(async id => await _repo.Save(new RepoItem(id, value: updateText)));

			Assert.AreEqual(updateText, await _repo.Get(itemId).Call(x => x.Value));
		}

		[Test]
		public async Task Delete_IfItemExists_DeletesIt()
		{
			var itemId = Guid.NewGuid()
				.UseAndReturn(async id => await _repo.Save(new RepoItem(id, value: "hey")))
				.UseAndReturn(async id => await _repo.Delete(id));

			Assert.Null(await _repo.Get(itemId));
		}

		[Test]
		public async Task Delete_IfItemDoesNotExist_DoesNotThrow()
		{
			Assert.DoesNotThrowAsync(async () => await _repo.Delete(Guid.NewGuid()));
		}

		[Test]
		public async Task GetWhere_GetsMultipleItems()
		{
			var otherId = 1;
			var item1 = Guid.NewGuid()
				.Call(x => new RepoItem(x, otherId))
				.UseAndReturn(async x => await _repo.Save(x));
			var item2 = Guid.NewGuid()
				.Call(x => new RepoItem(x, otherId))
				.UseAndReturn(async x => await _repo.Save(x));
			await _repo.Save(new RepoItem(Guid.NewGuid()));

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
			await _repo.Save(new RepoItem(id1, otherId));
			await _repo.Save(new RepoItem(id2, otherId));
			await _repo.Save(new RepoItem(id3));
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
			await _repo.Save(new RepoItem(id1, otherId, item1InitialValue));
			await _repo.Save(new RepoItem(id2, otherId, item2InitialValue));
			await _repo.Save(new RepoItem(id3, value: item3InitialValue));

			await _repo.UpdateWhere(x => x.SetValue(x.Value + " with an update"), x => x.OtherId == otherId);

			Assert.AreEqual($"{item1InitialValue}{updateText}", (await _repo.Get(id1)).Value);
			Assert.AreEqual($"{item2InitialValue}{updateText}", (await _repo.Get(id2)).Value);
			Assert.AreEqual(item3InitialValue, (await _repo.Get(id3)).Value);
		}
	}
}