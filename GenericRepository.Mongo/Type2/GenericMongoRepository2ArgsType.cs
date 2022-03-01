using System;
using System.Linq;

namespace GenericRepository.Mongo
{
    internal class GenericMongoRepository2ArgsType
    {
        private readonly Type _value;

        public GenericMongoRepository2ArgsType(Type value)
        {
            Validate(value);
            _value = value;
        }

        private void Validate(Type value)
        {
            var implementationCount = value.GetRepository2ImplementationCount();
            if (implementationCount == 0)
                throw new ArgumentException($"{value.FullName} must implement {Helper.CreateRepository2ArgsGenericTypeDefinition().FullName}");

            if (implementationCount > 1)
                throw new ArgumentException($"{value.FullName} cannot implement {Helper.CreateRepository2ArgsGenericTypeDefinition().FullName} multiple times");

            if (!value.HasParameterlessPublicConstructor())
                throw new NoPublicParameterlessConstructor(value);

            var instance = Activator.CreateInstance(value);
            if (KeySelectorIsNull(value, instance))
                throw new ArgumentException($"KeySelector is not set for {value.FullName}");

            if (MapFromDocumentIsNull(value, instance))
                throw new ArgumentException($"MapFromDocument is not set for {value.FullName}");

            if (MapToDocumentIsNull(value, instance))
                throw new ArgumentException($"MapToDocument is not set for {value.FullName}");

            if (MapKeyIsNull(value, instance))
                throw new ArgumentException($"MapKey is not set for {value.FullName}");
        }

        public Type GetEntityType() => GetGenericMongoRepositoryArgsType().GetGenericArguments()[0];
        public Type GetEntityKeyType() => GetGenericMongoRepositoryArgsType().GetGenericArguments()[1];
        public Type GetDocumentType() => GetGenericMongoRepositoryArgsType().GetGenericArguments()[2];
        public Type GetDocumentKeyType() => GetGenericMongoRepositoryArgsType().GetGenericArguments()[3];

        public object GetKeySelector() => InvokeGet(nameof(IGenericMongoRepositoryArgs<object, int, object, int>.KeySelector));
        public object GetMapFromDocument() => InvokeGet(nameof(IGenericMongoRepositoryArgs<object, int, object, int>.MapFromDocument));
        public object GetMapToDocument() => InvokeGet(nameof(IGenericMongoRepositoryArgs<object, int, object, int>.MapToDocument));
        public object GetMapKey() => InvokeGet(nameof(IGenericMongoRepositoryArgs<object, int, object, int>.MapKey));

        private object InvokeGet(string propertyName)
            => _value
                .GetProperty(propertyName)
                .GetMethod
                .Invoke(Activator.CreateInstance(_value), null);

        public Type CreateGenericMongoRepositoryType() =>
            Helper.CreateGenericMongoRepository2Type(GetEntityType(), GetEntityKeyType(), GetDocumentType(), GetDocumentKeyType());

        private Type GetGenericMongoRepositoryArgsType()
            => _value.GetInterfaces().Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == Helper.CreateRepository2ArgsGenericTypeDefinition());

        private bool KeySelectorIsNull(Type value, object instance)
            => value.GetProperties()
                .Single(x => x.Name == nameof(IGenericMongoRepositoryArgs<object, int, object, int>.KeySelector))
                .GetValue(instance) == null;

        private bool MapFromDocumentIsNull(Type value, object instance)
            => value.GetProperties()
                .Single(x => x.Name == nameof(IGenericMongoRepositoryArgs<object, int, object, int>.MapFromDocument))
                .GetValue(instance) == null;

        private bool MapToDocumentIsNull(Type value, object instance)
            => value.GetProperties()
                .Single(x => x.Name == nameof(IGenericMongoRepositoryArgs<object, int, object, int>.MapToDocument))
                .GetValue(instance) == null;

        private bool MapKeyIsNull(Type value, object instance)
            => value.GetProperties()
                .Single(x => x.Name == nameof(IGenericMongoRepositoryArgs<object, int, object, int>.MapKey))
                .GetValue(instance) == null;
    }
}