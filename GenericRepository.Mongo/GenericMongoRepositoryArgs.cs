using System;
using System.Linq.Expressions;

namespace GenericRepository.Mongo
{
    public abstract class GenericMongoRepositoryArgs<TEntity, TKey> where TKey : IEquatable<TKey>
    {
        public abstract Expression<Func<TEntity, TKey>> KeySelector { get; }
    }
}