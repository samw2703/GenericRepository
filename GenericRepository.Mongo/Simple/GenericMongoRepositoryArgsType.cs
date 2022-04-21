using System;
using System.Linq;

namespace GenericRepository.Mongo
{
	internal class GenericMongoRepositoryArgsType
	{
		private readonly Type _value;

		public GenericMongoRepositoryArgsType(Type value)
		{
			Validate(value);
			_value = value;
		}

		private void Validate(Type value)
		{
            if (!value.ImplementsSimpleGenericMongoRepositoryArgs())
                throw new ArgumentException($"{value.FullName} must implement {typeof(GenericMongoRepositoryArgs<,>).FullName}");

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
			.GetProperty(nameof(GenericMongoRepositoryArgs<object, int>.KeySelector))
			.GetMethod
			.Invoke(Activator.CreateInstance(_value), null);

        private Type GetGenericMongoRepositoryArgsType()
            => _value.GetInheritanceHierarchy().Single(x => x.StandardizeType() == typeof(GenericMongoRepositoryArgs<,>));

        private bool KeySelectorIsNull(Type value, object instance)
			=> value.GetProperties()
				.Single(x => x.Name == nameof(GenericMongoRepositoryArgs<object, int>.KeySelector))
				.GetValue(instance) == null;
	}
}