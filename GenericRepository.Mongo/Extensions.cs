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
	}
}