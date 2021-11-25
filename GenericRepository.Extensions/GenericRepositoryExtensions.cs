using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericRepository.Abstractions;

namespace GenericRepository.Extensions
{
	public static class GenericRepositoryExtensions
	{
		public static async Task Save<T, TKey>(this IGenericRepository<T, TKey> genericRepository, IEnumerable<T> items)
			=> await Task.WhenAll(items.Select(genericRepository.Save));

		public static async Task<IEnumerable<T>> Get<T, TKey>(this IGenericRepository<T, TKey> genericRepository, IEnumerable<TKey> keys)
			=> await Task.WhenAll(keys.Select(genericRepository.Get));

		public static async Task Delete<T, TKey>(this IGenericRepository<T, TKey> genericRepository, IEnumerable<TKey> keys)
			=> await Task.WhenAll(keys.Select(genericRepository.Delete));
	}
}