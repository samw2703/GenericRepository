using System;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization;

namespace GenericRepository.Mongo
{
    public abstract class GenericMongoRepositoryArgs<T, TKey> where TKey : IEquatable<TKey>
    {
        public abstract Expression<Func<T, TKey>> KeySelector { get; }
        public virtual void RegisterClassMap(BsonClassMap<T> cm)
        {
            cm.AutoMap();
        }

        internal void BaseRegisterClassMap()
        {
            BsonClassMap.RegisterClassMap<T>(cm =>
            {
                cm.MapIdMember(KeySelector);
                RegisterClassMap(cm);
            });
        }
    }
}