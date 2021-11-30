using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using MongoDB.Driver;

namespace GenericRepository.Mongo
{
	internal class GenericMongoRepository<TEntity, TKey, TDocument> : IGenericRepository<TEntity, TKey>
		where TKey : IEquatable<TKey>
	{
		private readonly Expression<Func<TDocument, TKey>> _keySelectorExpression;
		private readonly IMongoCollection<TDocument> _collection;
		private readonly Expression<Func<TDocument, TEntity>> _mapFromDocument;
		private readonly Func<TEntity, TDocument> _mapToDocument;

		public GenericMongoRepository(Expression<Func<TDocument, TKey>> keySelectorExpression,
			Expression<Func<TDocument, TEntity>> mapFromDocument,
			Func<TEntity, TDocument> mapToDocument,
			IMongoCollection<TDocument> collection)
		{
			_keySelectorExpression = keySelectorExpression;
			_collection = collection;
			_mapFromDocument = mapFromDocument;
			_mapToDocument = mapToDocument;
		}

		public async Task<TEntity> Get(TKey key)
			=> (await _collection.FindAsync(GetFilter(key), _mapFromDocument.ToProjectionFindOptions()))
				.SingleOrDefault();


		public async Task<List<TEntity>> GetWhere(Expression<Func<TEntity, bool>> where)
			=> (await _collection.FindAsync(Builders<TDocument>.Filter.Where(_ => true), _mapFromDocument.ToProjectionFindOptions()))
				.ToEnumerable()
				.Where(where.Compile())
				.ToList();

		public async Task Save(TEntity item)
		{
			var existingItem = await Get(GetKey(item));
			if (existingItem == null)
			{
				await _collection.InsertOneAsync(_mapToDocument(item));
				return;
			}

			await _collection.ReplaceOneAsync(GetFilter(GetKey(item)), _mapToDocument(item));
		}

		public async Task Delete(TKey key)
		{
			await _collection.DeleteOneAsync(GetFilter(key));
		}

		public async Task DeleteWhere(Expression<Func<TEntity, bool>> where)
		{
			await (await GetWhere(where))
				.Select(x => _keySelectorExpression.Compile()(_mapToDocument(x)))
				.Select(Delete)
				.WhenAll();
		}

		private TKey GetKey(TEntity item) => _keySelectorExpression.Compile()(_mapToDocument(item));

		private FilterDefinition<TDocument> GetFilter(TKey key)
			=> Builders<TDocument>.Filter.Eq(_keySelectorExpression, key);
	}
}