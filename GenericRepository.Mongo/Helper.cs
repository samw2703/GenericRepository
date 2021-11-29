using System;

namespace GenericRepository.Mongo
{
	internal static class Helper
	{
		public static Type CreateGenericMongoRepositoryType(Type entityType, Type keyType, Type documentType)
			=> typeof(GenericMongoRepository<object, int, object>)
				.GetGenericTypeDefinition()
				.MakeGenericType(entityType, keyType, documentType);

		public static Type CreateRepositoryArgsGenericTypeDefinition()
			=> typeof(IGenericMongoRepositoryArgs<object, int, object>).GetGenericTypeDefinition();

		public static Type CreateSimpleRepositoryArgsGenericTypeDefinition()
			=> typeof(ISimpleGenericMongoRepositoryArgs<object, int>).GetGenericTypeDefinition();
	}
}