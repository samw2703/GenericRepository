using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using MongoDB.Driver;

namespace GenericRepository.Mongo
{
	internal class GenericMongoRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
		where TKey : IEquatable<TKey>
	{
		private readonly Expression<Func<TEntity, TKey>> _keySelectorExpression;
		private readonly IMongoCollection<TEntity> _collection;

		public GenericMongoRepository(Expression<Func<TEntity, TKey>> keySelectorExpression, IMongoCollection<TEntity> collection)
		{
			_keySelectorExpression = keySelectorExpression;
			_collection = collection;
		}

		public async Task<TEntity> Get(TKey key)
		{
			return (await _collection.FindAsync(GetFilter(key)))
				.SingleOrDefault();
		}

		public async Task<List<TEntity>> GetWhere(Expression<Func<TEntity, bool>> where)
			=> (await _collection.FindAsync(where)).ToList();
		

		public async Task Save(TEntity item)
		{
			var existingItem = await Get(GetKey(item));
			if (existingItem == null)
			{
				await _collection.InsertOneAsync(item);
				return;
			}

			await _collection.ReplaceOneAsync(GetFilter(GetKey(item)), item);
		}

		public async Task UpdateWhere(Expression<Action<TEntity>> update, Expression<Func<TEntity, bool>> where)
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

		public async Task DeleteWhere(Expression<Func<TEntity, bool>> where)
			=> await _collection.DeleteManyAsync(where);

		private TKey GetKey(TEntity item) => _keySelectorExpression.Compile()(item);

		private FilterDefinition<TEntity> GetFilter(TKey key)
			=> Builders<TEntity>.Filter.Eq(_keySelectorExpression, key);
	}
}