using System;
using System.Threading.Tasks;
using Callinho;
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
			await _repo.Save(new RepoItem(_firstItemId, _firstItemValue));
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
				.UseAndReturn(async id => await _repo.Save(new RepoItem(id, "hey")));

			Assert.IsNotNull(await _repo.Get(itemId));
		}

		[Test]
		public async Task Save_IfItemWithKeyExists_UpdatesItem()
		{
			const string updateText = "I have been updated";
			var itemId = Guid.NewGuid()
				.UseAndReturn(async id => await _repo.Save(new RepoItem(id, "hey")))
				.UseAndReturn(async id => await _repo.Save(new RepoItem(id, updateText)));

			Assert.AreEqual(updateText, await _repo.Get(itemId).Call(x => x.Value));
		}

		[Test]
		public async Task Delete_IfItemExists_DeletesIt()
		{
			var itemId = Guid.NewGuid()
				.UseAndReturn(async id => await _repo.Save(new RepoItem(id, "hey")))
				.UseAndReturn(async id => await _repo.Delete(id));

			Assert.Null(await _repo.Get(itemId));
		}

		[Test]
		public async Task Delete_IfItemDoesNotExist_DoesNotThrow()
		{
			Assert.DoesNotThrowAsync(async () => await _repo.Delete(Guid.NewGuid()));
		}
	}
}