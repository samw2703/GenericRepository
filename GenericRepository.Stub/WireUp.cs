using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Stub
{
	public static class WireUp
	{
		public static GenericStubbedRepositoryBuilder UseStubbedGenericRepositories(this IServiceCollection sc)
		{
			return new GenericStubbedRepositoryBuilder(sc);
		}
	}
}