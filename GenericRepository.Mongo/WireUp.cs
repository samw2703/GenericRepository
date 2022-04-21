using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Mongo
{
	public static class WireUp
	{
		public static void AddGenericMongoRepositories(this IServiceCollection serviceCollection, 
			string connectionString, 
			string databaseName, 
			params Assembly[] assemblies)
		{
			var serviceCreator = new ServiceCreator(serviceCollection, connectionString, databaseName);
			var argsProvider = new GenericMongoRepositoryArgsProvider(new TypesProvider());
            argsProvider
				.GetSimpleArgsTypes(assemblies)
				.ForEach(x => serviceCreator.CreateSimpleServices(x));
        }
	}
}