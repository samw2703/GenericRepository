﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
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

        public static bool ImplementsGenericMongoRepository2Args(this Type type)
            => type.ImplementsGenerically<IGenericMongoRepositoryArgs<object, int, object, int>>();

		public static bool ImplementsSimpleGenericMongoRepositoryArgs(this Type type)
			=> type.ImplementsGenerically<ISimpleGenericMongoRepositoryArgs<object, int>>();

        public static int GetRepository2ImplementationCount(this Type type)
        {
            return type.GetInterfaces()
                .Select(StandardizeType)
                .Count(x => x == Helper.CreateRepository2ArgsGenericTypeDefinition());
        }

		public static int GetSimpleRepositoryImplementationCount(this Type type)
		{
			return type.GetInterfaces()
				.Select(StandardizeType)
				.Count(x => x == Helper.CreateSimpleRepositoryArgsGenericTypeDefinition());
		}

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