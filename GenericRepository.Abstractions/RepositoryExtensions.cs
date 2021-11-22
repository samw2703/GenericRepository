using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericRepository.Abstractions
{
	public static class RepositoryExtensions
	{
		public static async Task Save<T, TKey>(this IRepository<T, TKey> repository, IEnumerable<T> items)
			=> await Task.WhenAll(items.Select(repository.Save));

		public static async Task<IEnumerable<T>> Get<T, TKey>(this IRepository<T, TKey> repository, IEnumerable<TKey> keys)
			=> await Task.WhenAll(keys.Select(repository.Get));

		public static async Task Delete<T, TKey>(this IRepository<T, TKey> repository, IEnumerable<TKey> keys)
			=> await Task.WhenAll(keys.Select(repository.Delete));
	}
}