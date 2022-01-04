using System;
using GenericRepository.Abstractions;

namespace GenericRepository.Mongo.Tests
{
	public class GenericMongoRepositoryTests : BaseMongoRepositoryTests
	{
		protected override IGenericRepository<Item, Guid> CreateRepo()
			=> new GenericMongoRepository<Item, Guid, ItemDocument>(x => x.Id,
				doc => new Item(doc.Id, doc.OtherId, doc.Value),
				ent => new ItemDocument(ent.Id, ent.OtherId, ent.Value),
				GetCollection<ItemDocument>());


		private class ItemDocument
		{
			public Guid Id { get; }
			public int OtherId { get; }
			public string Value { get; }

			public ItemDocument(Guid id, int otherId, string value)
			{
				Id = id;
				OtherId = otherId;
				Value = value;
			}
		}
	}
}