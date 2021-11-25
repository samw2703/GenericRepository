using System.Threading.Tasks;

namespace GenericRepository.Abstractions
{
	public interface IGenericRepository<T, TKey>
	{
		Task<T> Get(TKey key);
		Task Save(T item);
		Task Delete(TKey key);
	}
}