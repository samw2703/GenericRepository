using System;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization;

namespace GenericRepository.Mongo
{
    public abstract class GenericMongoRepositoryArgs<TEntity, TKey> where TKey : IEquatable<TKey>
    {
        public abstract Expression<Func<TEntity, TKey>> KeySelector { get; }
        public virtual void RegisterClassMap(BsonClassMap<TEntity> cm)
        {
            cm.AutoMap();
        }

        internal void BaseRegisterClassMap()
        {
            BsonClassMap.RegisterClassMap<TEntity>(cm =>
            {
                cm.MapIdMember(KeySelector);
                RegisterClassMap(cm);
            });
        }
    }
}