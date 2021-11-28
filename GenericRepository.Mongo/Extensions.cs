using System;
using System.Linq;
using System.Linq.Expressions;
using GenericRepository.Abstractions;
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

		public static bool ContainsGenericRepository<T, TKey>(this IServiceCollection sc)
			=> sc.Any(x => x.ServiceType == typeof(IGenericRepository<T, TKey>));

		public static bool ImplementsGenerically<T>(this Type type)
		{
			if (!typeof(T).IsInterface)
				throw new ArgumentException("The inputted type to check against must be an interface");

			return type.GetInterfaces()
				.Select(StandardizeType)
				.Any(x => x == StandardizeType(typeof(T)));
		}

		private static Type StandardizeType(Type type)
		{
			if (type.IsGenericType)
				return type.GetGenericTypeDefinition();

			return type;
		}

		public static bool HasParameterlessPublicConstructor(this Type type)
		{
			var constructors = type.GetConstructors();
			var parameterlessConstructor = type
				.GetConstructors()
				.SingleOrDefault(x => !x.GetParameters().Any());

			if (parameterlessConstructor == null)
				return false;

			return parameterlessConstructor.IsPublic;
		}

	}
}