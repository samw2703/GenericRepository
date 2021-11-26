using System;
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
		{
			return (await _collection.FindAsync(GetFilter(key), _mapFromDocument.ToProjectionFindOptions()))
				.SingleOrDefault();
		}

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

		private TKey GetKey(TEntity item) => _keySelectorExpression.Compile()(_mapToDocument(item));

		private FilterDefinition<TDocument> GetFilter(TKey key)
			=> Builders<TDocument>.Filter.Eq(_keySelectorExpression, key);
	}

	internal static class Extensions
	{
		public static FindOptions<T, TResult> ToProjectionFindOptions<T, TResult>(this Expression<Func<T, TResult>> expression)
			=> new FindOptions<T, TResult>
			{
				Projection = Builders<T>.Projection.Expression(expression)
			};
	}
}