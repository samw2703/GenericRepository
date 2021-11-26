using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Stub
{
	internal static class Extensions
	{
		public static void Replace<T>(this List<T> list, T existing, T @new)
		{
			var index = list.IndexOf(existing);
			if (index == -1)
				throw new ArgumentException("The existing item does not exist");

			list[index] = @new;
		}

		public static Task<T> ToTask<T>(this T obj)
			=> Task.FromResult(obj);

		public static bool ContainsGenericRepository<T, TKey>(this IServiceCollection sc)
			=> sc.Any(x => x.ServiceType == typeof(IGenericRepository<T, TKey>));
	}
}