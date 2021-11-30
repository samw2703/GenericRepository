using System;
using GenericRepository.Abstractions;

namespace GenericRepository.Mongo.Tests
{
	public class GenericMongoRepositoryTests : BaseMongoRepositoryTests
	{
		protected override IGenericRepository<Item, Guid> CreateRepo()
			=> new GenericMongoRepository<Item, Guid, ItemDocuemnt>(x => x.Id,
				doc => new Item(doc.Id, doc.OtherId, doc.Value),
				ent => new ItemDocuemnt(ent.Id, ent.OtherId, ent.Value),
				GetCollection<ItemDocuemnt>());


		private class ItemDocuemnt
		{
			public Guid Id { get; }
			public int OtherId { get; }
			public string Value { get; }

			public ItemDocuemnt(Guid id, int otherId, string value)
			{
				Id = id;
				OtherId = otherId;
				Value = value;
			}
		}
	}
}