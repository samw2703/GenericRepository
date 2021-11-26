using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using StubbedRepository;

namespace GenericRepository.Stub
{
	internal class GenericStubbedRepository<T, TKey> : IGenericRepository<T, TKey>
		where TKey : IEquatable<TKey>
	{
		private readonly List<T> _items = new List<T>();
		private readonly Func<T, TKey> _keySelector;

		public GenericStubbedRepository(Func<T, TKey> keySelector)
		{
			_keySelector = keySelector;
		}

		public Task<T> Get(TKey key)
			=> _items
				.SingleOrDefault(x => key.Equals(_keySelector(x)))
				.ToTask();
		

		public async Task Save(T item)
		{
			var existingItem = await Get(_keySelector(item));
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
	}
}