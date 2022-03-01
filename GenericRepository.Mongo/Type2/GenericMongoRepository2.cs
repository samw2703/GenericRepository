using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using MongoDB.Driver;

namespace GenericRepository.Mongo
{
	internal class GenericMongoRepository<TEntity, TEntityKey, TDocument, TDocumentKey> : IGenericRepository<TEntity, TEntityKey>
        where TDocumentKey : IEquatable<TDocumentKey>
    {
        private readonly IMongoCollection<TDocument> _collection;
        private readonly Expression<Func<TDocument, TDocumentKey>> _keySelector;
        private readonly Expression<Func<TDocument, TEntity>> _mapFromDocument;
        private readonly Func<TEntity, TDocument> _mapToDocument;
        private readonly Func<TEntityKey, TDocumentKey> _mapKey;

        public GenericMongoRepository(IMongoCollection<TDocument> collection, 
            Expression<Func<TDocument, TDocumentKey>> keySelector, 
            Expression<Func<TDocument, TEntity>> mapFromDocument, 
            Func<TEntity, TDocument> mapToDocument, 
            Func<TEntityKey, TDocumentKey> mapKey)
        {
            _collection = collection;
            _keySelector = keySelector;
            _mapFromDocument = mapFromDocument;
            _mapToDocument = mapToDocument;
            _mapKey = mapKey;
        }

        public async Task<TEntity> Get(TEntityKey key)
            => (await _collection.FindAsync(GetFilter(key), _mapFromDocument.ToProjectionFindOptions()))
                .SingleOrDefault();

        public async Task<List<TEntity>> GetWhere(Expression<Func<TEntity, bool>> @where)
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

        public async Task UpdateWhere(Expression<Action<TEntity>> update, Expression<Func<TEntity, bool>> @where)
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

        public async Task Delete(TEntityKey key)
            => await _collection.DeleteOneAsync(GetFilter(key));

        public async Task DeleteWhere(Expression<Func<TEntity, bool>> @where)
        {
            var documentKeys = (await GetWhere(where))
                .Select(x => _keySelector.Compile()(_mapToDocument(x)));
            var filter = Builders<TDocument>.Filter.In(_keySelector, documentKeys);
            await _collection.DeleteManyAsync(filter);
        }

        private TDocumentKey GetKey(TEntity item) => _keySelector.Compile()(_mapToDocument(item));

        private FilterDefinition<TDocument> GetFilter(TEntityKey key)
        {
            var documentKey = _mapKey(key);
            return Builders<TDocument>.Filter.Eq(_keySelector, documentKey);
        }

        private FilterDefinition<TDocument> GetFilter(TDocumentKey key)
            => Builders<TDocument>.Filter.Eq(_keySelector, key);

        private async Task<TDocument> Get(TDocumentKey key)
            => (await _collection.FindAsync(GetFilter(key))).SingleOrDefault();
    }
}