using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Stub
{
	public static class WireUp
	{
		public static StubbedGenericRepositoryBuilder UseStubbedGenericRepositories(this IServiceCollection sc)
		{
			return new StubbedGenericRepositoryBuilder(sc);
		}
	}
}