using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Mongo
{
	public static class WireUp
	{
		public static void AddGenericMongoRepositories(this IServiceCollection serviceCollection, params Assembly[] assemblies)
		{

		}
	}
}