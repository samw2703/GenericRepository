using System.Collections.Generic;
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

		public List<GenericMongoRepositoryArgsType> GetArgsTypes(Assembly[] assemblies)
			=> _typesProvider
				.GetTypes(assemblies)
				.Where(x => x.ImplementsGenericMongoRepositoryArgs())
				.Select(x => new GenericMongoRepositoryArgsType(x))
				.ToList();

        public List<GenericMongoRepository2ArgsType> GetArgsTypes2(Assembly[] assemblies)
            => _typesProvider
                .GetTypes(assemblies)
                .Where(x => x.ImplementsGenericMongoRepository2Args())
                .Select(x => new GenericMongoRepository2ArgsType(x))
                .ToList();

		public List<SimpleGenericMongoRepositoryArgsType> GetSimpleArgsTypes(Assembly[] assemblies)
			=> _typesProvider
				.GetTypes(assemblies)
				.Where(x => x.ImplementsSimpleGenericMongoRepositoryArgs())
				.Select(x => new SimpleGenericMongoRepositoryArgsType(x))
				.ToList();
	}
}