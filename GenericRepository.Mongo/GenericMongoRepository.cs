using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using MongoDB.Driver;

namespace GenericRepository.Mongo
{
	internal class GenericMongoRepository<T, TKey> : IGenericRepository<T, TKey>
		where TKey : IEquatable<TKey>
	{
		private readonly Func<T, TKey> _keySelector;
		private readonly Expression<Func<T, TKey>> _keySelectorExpression;
		private readonly IMongoCollection<T> _collection;

		public GenericMongoRepository(Expression<Func<T, TKey>> keySelectorExpression, IMongoCollection<T> collection)
		{
			_keySelectorExpression = keySelectorExpression;
			_keySelector = _keySelectorExpression.Compile();
			_collection = collection;
		}

		public async Task<T> Get(TKey key)
		{
			return (await _collection.FindAsync(GetFilter(key)))
				.SingleOrDefault();
		}

		public async Task Save(T item)
		{
			var existingItem = await Get(_keySelector(item));
			if (existingItem == null)
			{
				await _collection.InsertOneAsync(item);
				return;
			}

			await _collection.ReplaceOneAsync(GetFilter(_keySelector(item)), item);
		}

		public async Task Delete(TKey key)
		{
			await _collection.DeleteOneAsync(GetFilter(key));
		}

		private FilterDefinition<T> GetFilter(TKey key)
			=> Builders<T>.Filter.Eq(_keySelectorExpression, key);
	}
}