using System;
using System.Linq;

namespace GenericRepository.Mongo
{
	internal class SimpleGenericMongoRepositoryArgsType
	{
		private readonly Type _value;

		public SimpleGenericMongoRepositoryArgsType(Type value)
		{
			Validate(value);
			_value = value;
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

		public Type GetEntityType() => GetGenericMongoRepositoryArgsType().GetGenericArguments()[0];
		public Type GetKeyType() => GetGenericMongoRepositoryArgsType().GetGenericArguments()[1];

		public Type CreateSimpleGenericMongoRepositoryType() =>
			Helper.CreateSimpleGenericMongoRepositoryType(GetEntityType(), GetKeyType());

		public object GetKeySelector() => _value
			.GetProperty(nameof(ISimpleGenericMongoRepositoryArgs<object, int>.KeySelector))
			.GetMethod
			.Invoke(Activator.CreateInstance(_value), null);

		private Type GetGenericMongoRepositoryArgsType()
			=> _value.GetInterfaces().Single(x => x.StandardizeType() == Helper.CreateSimpleRepositoryArgsGenericTypeDefinition());
		private bool KeySelectorIsNull(Type value, object instance)
			=> value.GetProperties()
				.Single(x => x.Name == nameof(ISimpleGenericMongoRepositoryArgs<object, int>.KeySelector))
				.GetValue(instance) == null;
	}
}