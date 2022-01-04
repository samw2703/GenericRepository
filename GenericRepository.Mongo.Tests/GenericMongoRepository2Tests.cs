using System;
using GenericRepository.Abstractions;

namespace GenericRepository.Mongo.Tests
{
    public class GenericMongoRepository2Tests : BaseMongoRepositoryTests
    {
        protected override IGenericRepository<Item, Guid> CreateRepo()
            => new GenericMongoRepository<Item, Guid, ItemDocument, string>(GetCollection<ItemDocument>(),
                doc => doc.Id,
                doc => new Item(Guid.Parse(doc.Id), doc.OtherId, doc.Value),
                ent => new ItemDocument(ent.Id.ToString(), ent.OtherId, ent.Value),
                x => x.ToString());


        private class ItemDocument
        {
            public string Id { get; }
            public int OtherId { get; }
            public string Value { get; }

            public ItemDocument(string id, int otherId, string value)
            {
                Id = id;
                OtherId = otherId;
                Value = value;
            }
        }
    }
}