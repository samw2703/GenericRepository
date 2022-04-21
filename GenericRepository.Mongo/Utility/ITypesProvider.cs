using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GenericRepository.Mongo
{
	internal interface ITypesProvider
	{
		List<Type> GetTypes(Assembly[] assemblies);
	}

	internal class TypesProvider : ITypesProvider
	{
		public List<Type> GetTypes(Assembly[] assemblies) => assemblies.SelectMany(x => x.GetTypes()).ToList();
	}
}