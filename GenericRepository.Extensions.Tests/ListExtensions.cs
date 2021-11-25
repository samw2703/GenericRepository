using System;
using System.Collections.Generic;

namespace GenericRepository.Extensions.Tests
{
	internal static class ListExtensions
	{
		public static void Replace<T>(this List<T> list, T existing, T @new)
		{
			var index = list.IndexOf(existing);
			if (index == -1)
				throw new ArgumentException("The existing item does not exist");

			list[index] = @new;
		}
	}
}