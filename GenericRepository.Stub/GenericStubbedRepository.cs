using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using StubbedRepository;

namespace GenericRepository.Stub
{
	public abstract class GenericStubbedRepository<T, TKey> : IGenericRepository<T, TKey>
		where TKey : IEquatable<TKey>
	{
		private readonly List<T> _items = new List<T>();

		public Task<T> Get(TKey key)
			=> _items
				.SingleOrDefault(x => key.Equals(GetKey(x)))
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

		protected abstract TKey GetKey(T item);
	}
}