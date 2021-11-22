using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericRepository.Abstractions;

namespace StubbedRepository
{
	public abstract class StubbedRepository<T, TKey> : IRepository<T, TKey>
	{
		private readonly List<T> _items = new List<T>();

		public Task<T> Get(TKey key)
			=> _items
				.SingleOrDefault(x => KeyMatch(x, key))
				.ToTask();
		

		public async Task Save(T item)
		{
			var existingItem = await Get(GetKey(item));
			if (existingItem == null)
			{
				_items.Add(item);
				return;
			}
			

			_items.Replace(existingItem, item);
		}

		public async Task Delete(TKey key)
		{
			var item = await Get(key);
			if (item == null)
				return;

			_items.Remove(item);
		}

		public abstract bool KeyMatch(T item, TKey key);

		public abstract TKey GetKey(T item);
	}
}