using System;
using System.Linq;

namespace GenericRepository.Mongo
{
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

			var instance = Activator.CreateInstance(value);
			if (KeySelectorIsNull(value, instance))
				throw new ArgumentException($"KeySelector is not set for {value.FullName}");
		}

		private bool KeySelectorIsNull(Type value, object instance)
			=> value.GetProperties()
				.Single(x => x.Name == nameof(ISimpleGenericMongoRepositoryArgs<object, int>.KeySelector))
				.GetValue(instance) == null;
	}
}