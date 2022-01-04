using System;
using System.Linq.Expressions;

namespace GenericRepository.Mongo
{
	public interface IGenericMongoRepositoryArgs<TEntity, TKey, TDocument> where TKey : IEquatable<TKey>
	{
		Expression<Func<TDocument, TKey>> KeySelector { get; }
		Expression<Func<TDocument, TEntity>> MapFromDocument { get; }
		Func<TEntity, TDocument> MapToDocument { get; }
	}

    public interface IGenericMongoRepositoryArgs<TEntity, in TEntityKey, TDocument, TDocumentKey> 
        where TDocumentKey : IEquatable<TDocumentKey>
    {
        Expression<Func<TDocument, TDocumentKey>> KeySelector { get; }
        Expression<Func<TDocument, TEntity>> MapFromDocument { get; }
        Func<TEntity, TDocument> MapToDocument { get; }
		Func<TEntityKey, TDocumentKey> MapKey { get; }
    }

	public interface ISimpleGenericMongoRepositoryArgs<TEntity, TKey> where TKey : IEquatable<TKey>
	{
		Expression<Func<TEntity, TKey>> KeySelector { get; }
	}
}