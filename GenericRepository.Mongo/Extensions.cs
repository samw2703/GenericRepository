using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace GenericRepository.Mongo
{
	internal static class Extensions
	{
		public static FindOptions<T, TResult> ToProjectionFindOptions<T, TResult>(this Expression<Func<T, TResult>> expression)
			=> new FindOptions<T, TResult>
			{
				Projection = Builders<T>.Projection.Expression(expression)
			};

		public static bool ImplementsGenericMongoRepositoryArgs(this Type type)
			=> type.ImplementsGenerically<IGenericMongoRepositoryArgs<object, int, object>>();

		public static bool ImplementsSimpleGenericMongoRepositoryArgs(this Type type)
			=> type.ImplementsGenerically<ISimpleGenericMongoRepositoryArgs<object, int>>();

		public static int GetRepositoryImplementationCount(this Type type)
		{
			return type.GetInterfaces()
				.Select(StandardizeType)
				.Count(x => x == Helper.CreateRepositoryArgsGenericTypeDefinition());
		}

		public static int GetSimpleRepositoryImplementationCount(this Type type)
		{
			return type.GetInterfaces()
				.Select(StandardizeType)
				.Count(x => x == Helper.CreateSimpleRepositoryArgsGenericTypeDefinition());
		}

		public static bool HasParameterlessPublicConstructor(this Type type)
		{
			var parameterlessConstructor = type
				.GetConstructors()
				.SingleOrDefault(x => !x.GetParameters().Any());

			if (parameterlessConstructor == null)
				return false;

			return parameterlessConstructor.IsPublic;
		}

		public static bool HasService(this IServiceCollection services, Type serviceType)
			=> services.Any(x => x.ServiceType == serviceType);

		private static bool ImplementsGenerically<T>(this Type type)
		{
			if (!typeof(T).IsInterface)
				throw new ArgumentException("The inputted type to check against must be an interface");

			return type.GetInterfaces()
				.Select(StandardizeType)
				.Any(x => x == StandardizeType(typeof(T)));
		}

		private static Type StandardizeType(this Type type)
		{
			if (type.IsGenericType)
				return type.GetGenericTypeDefinition();

			return type;
		}
	}
}