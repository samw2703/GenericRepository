using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericRepository.Abstractions;

namespace GenericRepository.Extensions.Tests
{
	partial class GenericRepositoryExtensionTests
	{
		private class RepoItem
		{
			public Guid Id { get; set; }

			public RepoItem(Guid id)
			{
				Id = id;
			}
		}

		private class RepoItemRepository : IGenericRepository<RepoItem, Guid>
		{
			private readonly List<RepoItem> _items = new();

			public Task<RepoItem> Get(Guid id)
				=> _items
					.SingleOrDefault(x => x.Id == id)
					.ToTask();


			public async Task Save(RepoItem item)
			{
				var existingItem = await Get(item.Id);
				if (existingItem == null)
				{
					_items.Add(item);
					return;
				}


				_items.Replace(existingItem, item);
			}

			public async Task Delete(Guid id)
			{
				var item = await Get(id);
				if (item == null)
					return;

				_items.Remove(item);
			}
		}
	}
}
