using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GenericRepository.Mongo
{
    public interface IGenericMongoRepositoryArgs<TEntity, in TEntityKey, TDocument, TDocumentKey>
        where TDocumentKey : IEquatable<TDocumentKey>
    {
        Expression<Func<TDocument, TDocumentKey>> KeySelector { get; }
        Expression<Func<TDocument, TEntity>> MapFromDocument { get; }
        Func<TEntity, TDocument> MapToDocument { get; }
        Func<TEntityKey, TDocumentKey> MapKey { get; }
    }
}
