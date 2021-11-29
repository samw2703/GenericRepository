using System;
using System.Linq;

namespace GenericRepository.Mongo
{
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

			var instance = Activator.CreateInstance(value);
			if (KeySelectorIsNull(value, instance))
				throw new ArgumentException($"KeySelector is not set for {value.FullName}");

			if (MapFromDocumentIsNull(value, instance))
				throw new ArgumentException($"MapFromDocument is not set for {value.FullName}");

			if (MapToDocumentIsNull(value, instance))
				throw new ArgumentException($"MapToDocument is not set for {value.FullName}");
		}

		public Type GetEntityType() => Value.GetGenericArguments()[0];

		public Type GetKeyType() => Value.GetGenericArguments()[1];

		private bool KeySelectorIsNull(Type value, object instance)
			=> value.GetProperties()
				.Single(x => x.Name == nameof(IGenericMongoRepositoryArgs<object, int, object>.KeySelector))
				.GetValue(instance) == null;

		private bool MapFromDocumentIsNull(Type value, object instance)
			=> value.GetProperties()
				.Single(x => x.Name == nameof(IGenericMongoRepositoryArgs<object, int, object>.MapFromDocument))
				.GetValue(instance) == null;

		private bool MapToDocumentIsNull(Type value, object instance)
			=> value.GetProperties()
				.Single(x => x.Name == nameof(IGenericMongoRepositoryArgs<object, int, object>.MapToDocument))
				.GetValue(instance) == null;
	}
}