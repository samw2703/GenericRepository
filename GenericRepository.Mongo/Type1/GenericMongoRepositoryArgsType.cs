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
			var implementationCount = value.GetRepositoryImplementationCount();
			if (implementationCount == 0)
				throw new ArgumentException($"{value.FullName} must implement {Helper.CreateRepositoryArgsGenericTypeDefinition().FullName}");

			if (implementationCount > 1)
				throw new ArgumentException($"{value.FullName} cannot implement {Helper.CreateRepositoryArgsGenericTypeDefinition().FullName} multiple times");

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

		public Type GetEntityType() => GetGenericMongoRepositoryArgsType().GetGenericArguments()[0];
		public Type GetKeyType() => GetGenericMongoRepositoryArgsType().GetGenericArguments()[1];
		public Type GetDocumentType() => GetGenericMongoRepositoryArgsType().GetGenericArguments()[2];

		public object GetKeySelector() => InvokeGet(nameof(IGenericMongoRepositoryArgs<object, int, object>.KeySelector));
		public object GetMapFromDocument() => InvokeGet(nameof(IGenericMongoRepositoryArgs<object, int, object>.MapFromDocument));
		public object GetMapToDocument() => InvokeGet(nameof(IGenericMongoRepositoryArgs<object, int, object>.MapToDocument));

		private object InvokeGet(string propertyName)
			=> _value
				.GetProperty(propertyName)
				.GetMethod
				.Invoke(Activator.CreateInstance(_value), null);

		public Type CreateGenericMongoRepositoryType() =>
			Helper.CreateGenericMongoRepositoryType(GetEntityType(), GetKeyType(), GetDocumentType());

		private Type GetGenericMongoRepositoryArgsType()
			=> _value.GetInterfaces().Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == Helper.CreateRepositoryArgsGenericTypeDefinition());

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