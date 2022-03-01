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
}