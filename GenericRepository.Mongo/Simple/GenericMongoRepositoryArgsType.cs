using System;
using System.Linq;

namespace GenericRepository.Mongo
{
	internal class GenericMongoRepositoryArgsType
	{
		private readonly Type _value;

        public Type KeyType { get; }
        public Type EntityType { get; }

		public GenericMongoRepositoryArgsType(Type value)
		{
			Validate(value);

            var genericMongoRepositoryArgsType = value.GetInheritanceHierarchy().Single(x => x.StandardizeType() == typeof(GenericMongoRepositoryArgs<,>));
            KeyType = genericMongoRepositoryArgsType.GetGenericArguments()[1];
            EntityType = genericMongoRepositoryArgsType.GetGenericArguments()[0];
			_value = value;
		}

        public Type CreateGenericMongoRepositoryType() =>
			Helper.CreateGenericMongoRepositoryType(EntityType, KeyType);

		public object GetKeySelector() => _value
			.GetProperty(nameof(GenericMongoRepositoryArgs<object, int>.KeySelector))
			.GetMethod
			.Invoke(Activator.CreateInstance(_value), null);

        private static void Validate(Type value)
        {
            if (!value.ImplementsGenericMongoRepositoryArgs())
                throw new ArgumentException($"{value.FullName} must implement {typeof(GenericMongoRepositoryArgs<,>).FullName}");

            if (!value.HasParameterlessPublicConstructor())
                throw new NoPublicParameterlessConstructor(value);

            var instance = Activator.CreateInstance(value);
            if (KeySelectorIsNull(value, instance))
                throw new ArgumentException($"KeySelector is not set for {value.FullName}");
        }

		private static bool KeySelectorIsNull(Type value, object instance)
			=> value.GetProperties()
				.Single(x => x.Name == nameof(GenericMongoRepositoryArgs<object, int>.KeySelector))
				.GetValue(instance) == null;
	}
}