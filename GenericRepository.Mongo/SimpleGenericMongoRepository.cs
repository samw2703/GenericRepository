using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using MongoDB.Driver;

namespace GenericRepository.Mongo
{
	internal class SimpleGenericMongoRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
		where TKey : IEquatable<TKey>
	{
		private readonly Expression<Func<TEntity, TKey>> _keySelectorExpression;
		private readonly IMongoCollection<TEntity> _collection;

		public SimpleGenericMongoRepository(Expression<Func<TEntity, TKey>> keySelectorExpression, IMongoCollection<TEntity> collection)
		{
			_keySelectorExpression = keySelectorExpression;
			_collection = collection;
		}

		public async Task<TEntity> Get(TKey key)
		{
			return (await _collection.FindAsync(GetFilter(key)))
				.SingleOrDefault();
		}

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

		public async Task Delete(TKey key)
		{
			await _collection.DeleteOneAsync(GetFilter(key));
		}

		private TKey GetKey(TEntity item) => _keySelectorExpression.Compile()(item);

		private FilterDefinition<TEntity> GetFilter(TKey key)
			=> Builders<TEntity>.Filter.Eq(_keySelectorExpression, key);
	}
}