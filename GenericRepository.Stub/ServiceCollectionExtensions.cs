using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Stub
{
	internal static class ServiceCollectionExtensions
	{
		public static bool HasService<TService>(this IServiceCollection sc)
			=> sc.Any(x => x.ServiceType == typeof(TService));
	}
}