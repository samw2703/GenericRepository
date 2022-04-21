using System;
using GenericRepository.Abstractions;

namespace GenericRepository.Mongo
{
	internal static class Helper
	{
        public static Type CreateSimpleGenericMongoRepositoryType(Type entityType, Type keyType)
			=> typeof(SimpleGenericMongoRepository<object, int>)
				.GetGenericTypeDefinition()
				.MakeGenericType(entityType, keyType);

        public static Type CreateSimpleRepositoryArgsGenericTypeDefinition()
			=> typeof(ISimpleGenericMongoRepositoryArgs<object, int>).GetGenericTypeDefinition();

		public static Type CreateIGenericRepositoryType(Type entityType, Type keyType)
			=> typeof(IGenericRepository<object, int>)
				.GetGenericTypeDefinition()
				.MakeGenericType(entityType, keyType);
	}
}