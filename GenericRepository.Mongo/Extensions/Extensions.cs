using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Mongo
{
	internal static class Extensions
    {
        public static bool ImplementsGenericMongoRepositoryArgs(this Type type)
            => type.GetGenericInheritanceHierarchy().Contains(typeof(GenericMongoRepositoryArgs<,>));

        public static bool HasParameterlessPublicConstructor(this Type type)
			=> type.GetParameterlessPublicConstructor() != null;

		public static ConstructorInfo GetParameterlessPublicConstructor(this Type type)
			=> type
				.GetConstructors()
				.SingleOrDefault(x => x.IsPublic && !x.GetParameters().Any());

		public static bool HasService(this IServiceCollection services, Type serviceType)
			=> services.Any(x => x.ServiceType == serviceType);

		public static async Task WhenAll(this IEnumerable<Task> tasks)
			=> await Task.WhenAll(tasks);

        public static IEnumerable<Type> GetInheritanceHierarchy(this Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
                yield return current;
        }

		public static Type StandardizeType(this Type type)
		{
			if (type.IsGenericType)
				return type.GetGenericTypeDefinition();

			return type;
		}

        private static IEnumerable<Type> GetGenericInheritanceHierarchy(this Type type)
            => type.GetInheritanceHierarchy().Select(x => x.StandardizeType());
    }
}