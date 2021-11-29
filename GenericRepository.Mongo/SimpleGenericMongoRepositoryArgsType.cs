using System;
using System.Linq;

namespace GenericRepository.Mongo
{
	internal class SimpleGenericMongoRepositoryArgsType
	{
		private readonly Type _simpleGenericMongoRepositoryArgsType;

		public SimpleGenericMongoRepositoryArgsType(Type value)
		{
			Validate(value);
			_simpleGenericMongoRepositoryArgsType = value
				.GetInterfaces()
				.Single(x => x.StandardizeType() == Helper.CreateSimpleRepositoryArgsGenericTypeDefinition());
		}

		private void Validate(Type value)
		{
			var implementationCount = value.GetSimpleRepositoryImplementationCount();
			if (implementationCount == 0)
				throw new ArgumentException($"{value.FullName} must implement {Helper.CreateSimpleRepositoryArgsGenericTypeDefinition().FullName}");

			if (implementationCount > 1)
				throw new ArgumentException($"{value.FullName} cannot implement {Helper.CreateSimpleRepositoryArgsGenericTypeDefinition().FullName} multiple times");

			if (!value.HasParameterlessPublicConstructor())
				throw new NoPublicParameterlessConstructor(value);

			var instance = Activator.CreateInstance(value);
			if (KeySelectorIsNull(value, instance))
				throw new ArgumentException($"KeySelector is not set for {value.FullName}");
		}

		public Type GetEntityType() => _simpleGenericMongoRepositoryArgsType.GetGenericArguments()[0];
		public Type GetKeyType() => _simpleGenericMongoRepositoryArgsType.GetGenericArguments()[1];

		private bool KeySelectorIsNull(Type value, object instance)
			=> value.GetProperties()
				.Single(x => x.Name == nameof(ISimpleGenericMongoRepositoryArgs<object, int>.KeySelector))
				.GetValue(instance) == null;
	}
}