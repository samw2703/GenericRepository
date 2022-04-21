using System;
using GenericRepository.Abstractions;

namespace GenericRepository.Mongo
{
	internal static class Helper
	{
        public static Type CreateGenericMongoRepositoryType(Type entityType, Type keyType)
			=> typeof(GenericMongoRepository<,>).MakeGenericType(entityType, keyType);

        public static Type CreateIGenericRepositoryType(Type entityType, Type keyType)
			=> typeof(IGenericRepository<,>).MakeGenericType(entityType, keyType);
	}
}