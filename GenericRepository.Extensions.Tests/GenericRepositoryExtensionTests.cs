using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Callinho;
using CollectionUtilities;
using NUnit.Framework;

namespace GenericRepository.Extensions.Tests
{
	public partial class GenericRepositoryExtensionTests
	{
		private readonly RepoItemRepository _repo = new();

		[Test]
		public async Task Save_DoesSaveMutlple()
		{
			var id1 = Guid.NewGuid();
			var id2 = Guid.NewGuid();
			await _repo.Save(new List<RepoItem> { Create(id1), Create(id2) });

			Assert.IsNotNull(_repo.Get(id1));
			Assert.IsNotNull(_repo.Get(id2));
		}

		[Test]
		public async Task Get_DoesGetMultipleItemsOrNull()
		{
			var id1 = Guid.NewGuid();
			var id2 = Guid.NewGuid();
			var id3 = Guid.NewGuid();
			await _repo.Save(Create(id1));
			await _repo.Save(Create(id2));

			var items = await _repo.Get(new List<Guid> { id1, id2, id3 }).ToList();
			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(id1, items[0].Id);
			Assert.AreEqual(id2, items[1].Id);
			Assert.Null(items[2]);
		}

		[Test]
		public async Task Delete_DoesDeleteMultiple()
		{
			var id1 = Guid.NewGuid();
			var id2 = Guid.NewGuid();
			await _repo.Save(Create(id1));
			await _repo.Save(Create(id2));
			await _repo.Delete(new List<Guid> { id1, id2 });

			Assert.Null(await _repo.Get(id1));
			Assert.Null(await _repo.Get(id2));
		}

		private RepoItem Create(Guid id) => new(id);
	}
}