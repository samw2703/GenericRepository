using System;
using System.Linq.Expressions;

namespace GenericRepository.Mongo
{
    public interface ISimpleGenericMongoRepositoryArgs<TEntity, TKey> where TKey : IEquatable<TKey>
    {
        Expression<Func<TEntity, TKey>> KeySelector { get; }
    }
}