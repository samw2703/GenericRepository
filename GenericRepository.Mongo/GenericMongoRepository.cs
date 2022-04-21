using System;
using System.Collections.Generic;
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
		private readonly Expression<Func<T, TKey>> _keySelectorExpression;
		private readonly IMongoCollection<T> _collection;

		public GenericMongoRepository(Expression<Func<T, TKey>> keySelectorExpression, IMongoCollection<T> collection)
		{
			_keySelectorExpression = keySelectorExpression;
			_collection = collection;
		}

		public async Task<T> Get(TKey key)
		{
			return (await _collection.FindAsync(GetFilter(key)))
				.SingleOrDefault();
		}

		public async Task<List<T>> GetWhere(Expression<Func<T, bool>> where)
			=> (await _collection.FindAsync(where)).ToList();
		

		public async Task Save(T item)
		{
			var existingItem = await Get(GetKey(item));
			if (existingItem == null)
			{
				await _collection.InsertOneAsync(item);
				return;
			}

			await _collection.ReplaceOneAsync(GetFilter(GetKey(item)), item);
		}

		public async Task UpdateWhere(Expression<Action<T>> update, Expression<Func<T, bool>> where)
		{
			var updateFunc = update.Compile();
			var updated = (await GetWhere(where)).Select(x =>
			{
				updateFunc(x);
				return x;
			});
			await updated
				.Select(Save)
				.WhenAll();
		}

		public async Task Delete(TKey key)
		{
			await _collection.DeleteOneAsync(GetFilter(key));
		}

		public async Task DeleteWhere(Expression<Func<T, bool>> where)
			=> await _collection.DeleteManyAsync(where);

		private TKey GetKey(T item) => _keySelectorExpression.Compile()(item);

		private FilterDefinition<T> GetFilter(TKey key)
			=> Builders<T>.Filter.Eq(_keySelectorExpression, key);
	}
}