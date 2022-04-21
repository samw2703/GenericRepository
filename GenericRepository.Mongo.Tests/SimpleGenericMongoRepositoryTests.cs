using System;
using GenericRepository.Abstractions;

namespace GenericRepository.Mongo.Tests
{
	public class SimpleGenericMongoRepositoryTests : BaseMongoRepositoryTests
	{
		protected override IGenericRepository<Item, Guid> CreateRepo()
			=> new GenericMongoRepository<Item, Guid>(x => x.Id, GetCollection<Item>());
	}
}