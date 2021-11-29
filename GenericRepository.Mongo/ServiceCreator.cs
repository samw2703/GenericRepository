using System;
using System.Linq;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Mongo
{
	internal class ServiceCreator
	{
		private readonly IServiceCollection _services;

		public ServiceCreator(IServiceCollection services)
		{
			_services = services;
		}

		public void CreateServices(GenericMongoRepositoryArgsType argsType)
		{
			Validate(argsType);
		}

		private void Validate(GenericMongoRepositoryArgsType argsType)
		{
			ValidateGenericRepositoryNotAlreadyWired(argsType.GetEntityType(), argsType.GetKeyType());
			ValidateExpressionsAreSet(argsType);
		}

		private void ValidateExpressionsAreSet(GenericMongoRepositoryArgsType argsType)
		{
			var keySelector = argsType.Value.GetProperties().Single(x => x.Name == nameof(IGenericMongoRepositoryArgs<object, int, object>.KeySelector));
			keySelector.GetValue(1, null);
			Activator.CreateInstance(argsType.Value);
		}

		private void ValidateGenericRepositoryNotAlreadyWired(Type entityType, Type keyType)
		{
			var genericRepositoryType = typeof(IGenericRepository<object, int>)
				.GetGenericTypeDefinition()
				.MakeGenericType(entityType, keyType);

			if (_services.HasService(genericRepositoryType))
				throw new ArgumentException($"A repository for {entityType.FullName} with key {keyType.FullName} has already been registered");
		}
	}
}