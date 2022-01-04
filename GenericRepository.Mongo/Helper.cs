using System;
using GenericRepository.Abstractions;

namespace GenericRepository.Mongo
{
	internal static class Helper
	{
		public static Type CreateGenericMongoRepositoryType(Type entityType, Type keyType, Type documentType)
			=> typeof(GenericMongoRepository<object, int, object>)
				.GetGenericTypeDefinition()
				.MakeGenericType(entityType, keyType, documentType);

        public static Type CreateGenericMongoRepository2Type(Type entityType, Type keyType, Type documentType, Type documentKeyType)
            => typeof(GenericMongoRepository<object, int, object, int>)
                .GetGenericTypeDefinition()
                .MakeGenericType(entityType, keyType, documentType, documentKeyType);

		public static Type CreateSimpleGenericMongoRepositoryType(Type entityType, Type keyType)
			=> typeof(SimpleGenericMongoRepository<object, int>)
				.GetGenericTypeDefinition()
				.MakeGenericType(entityType, keyType);

		public static Type CreateRepositoryArgsGenericTypeDefinition()
			=> typeof(IGenericMongoRepositoryArgs<object, int, object>).GetGenericTypeDefinition();

        public static Type CreateRepository2ArgsGenericTypeDefinition()
            => typeof(IGenericMongoRepositoryArgs<object, int, object, int>).GetGenericTypeDefinition();

		public static Type CreateSimpleRepositoryArgsGenericTypeDefinition()
			=> typeof(ISimpleGenericMongoRepositoryArgs<object, int>).GetGenericTypeDefinition();

		public static Type CreateIGenericRepositoryType(Type entityType, Type keyType)
			=> typeof(IGenericRepository<object, int>)
				.GetGenericTypeDefinition()
				.MakeGenericType(entityType, keyType);
	}
}