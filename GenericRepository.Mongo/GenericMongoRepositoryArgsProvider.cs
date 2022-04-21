﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GenericRepository.Mongo
{
	internal class GenericMongoRepositoryArgsProvider
	{
		private readonly ITypesProvider _typesProvider;

		public GenericMongoRepositoryArgsProvider(ITypesProvider typesProvider)
		{
			_typesProvider = typesProvider;
		}

        public List<SimpleGenericMongoRepositoryArgsType> GetSimpleArgsTypes(Assembly[] assemblies)
			=> _typesProvider
				.GetTypes(assemblies)
				.Where(x => x.ImplementsSimpleGenericMongoRepositoryArgs())
				.Select(x => new SimpleGenericMongoRepositoryArgsType(x))
				.ToList();
	}
}