using System;
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
				.Where(x => x.ImplementsGenerically<IGenericMongoRepositoryArgs<object, int, object>>())
				.Select(x => new GenericMongoRepositoryArgsType(x))
				.ToList();

		public List<SimpleGenericMongoRepositoryArgsType> GetSimpleArgsTypes(Assembly[] assemblies)
			=> _typesProvider
				.GetTypes(assemblies)
				.Where(x => x.ImplementsGenerically<ISimpleGenericMongoRepositoryArgs<object, int>>())
				.Select(x => new SimpleGenericMongoRepositoryArgsType(x))
				.ToList();
	}

	internal class GenericMongoRepositoryArgsType
	{
		public Type Value { get; }

		public GenericMongoRepositoryArgsType(Type value)
		{
			Validate(value);
			Value = value;
		}

		private void Validate(Type value)
		{
			if (!value.ImplementsGenerically<IGenericMongoRepositoryArgs<object, int, object>>())
				throw new ArgumentException($"Underlying type must implement {typeof(IGenericMongoRepositoryArgs<object, int, object>).GetGenericTypeDefinition().FullName}");

			if (!value.HasParameterlessPublicConstructor())
				throw new NoPublicParameterlessConstructor(value);
		}
	}

	internal class SimpleGenericMongoRepositoryArgsType
	{
		public Type Value { get; }

		public SimpleGenericMongoRepositoryArgsType(Type value)
		{
			Validate(value);
			Value = value;
		}

		private void Validate(Type value)
		{
			if (!value.ImplementsGenerically<ISimpleGenericMongoRepositoryArgs<object, int>>())
				throw new ArgumentException($"Underlying type must implement {typeof(ISimpleGenericMongoRepositoryArgs<object, int>).GetGenericTypeDefinition().FullName}");

			if (!value.HasParameterlessPublicConstructor())
				throw new NoPublicParameterlessConstructor(value);
		}
	}
}