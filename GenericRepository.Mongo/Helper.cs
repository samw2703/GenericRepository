using System;
using GenericRepository.Abstractions;

namespace GenericRepository.Mongo
{
	internal static class Helper
	{
        public static Type CreateSimpleGenericMongoRepositoryType(Type entityType, Type keyType)
			=> typeof(GenericMongoRepository<object, int>)
				.GetGenericTypeDefinition()
				.MakeGenericType(entityType, keyType);

        public static Type CreateSimpleRepositoryArgsGenericTypeDefinition()
			=> typeof(GenericMongoRepositoryArgs<object, int>).GetGenericTypeDefinition();

		public static Type CreateIGenericRepositoryType(Type entityType, Type keyType)
			=> typeof(IGenericRepository<object, int>)
				.GetGenericTypeDefinition()
				.MakeGenericType(entityType, keyType);
	}
}