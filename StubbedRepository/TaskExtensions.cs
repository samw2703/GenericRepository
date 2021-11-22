using System.Threading.Tasks;

namespace StubbedRepository
{
	internal static class TaskExtensions
	{
		public static Task<T> ToTask<T>(this T obj)
			=> Task.FromResult(obj);
	}
}