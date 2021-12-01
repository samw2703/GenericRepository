using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GenericRepository.Abstractions
{
	public interface IGenericRepository<T, TKey>
	{
		Task<T> Get(TKey key);
		Task<List<T>> GetWhere(Expression<Func<T, bool>> where);
		Task Save(T item);
		Task UpdateWhere(Expression<Action<T>> update, Expression<Func<T, bool>> where);
		Task Delete(TKey key);
		Task DeleteWhere(Expression<Func<T, bool>> where);
	}
}