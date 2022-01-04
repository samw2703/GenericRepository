using System;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace GenericRepository.Mongo.TestAssembly
{
    public class FixtureEntity
    {
        public Guid Id { get; set; }

        public FixtureEntity(Guid id)
        {
            Id = id;
        }
    }

    public class FixtureDocument
    {
        public string Id { get; set; }

        public FixtureDocument(string id)
        {
            Id = id;
        }
    }

    internal class FixtureArgs : IGenericMongoRepositoryArgs<FixtureEntity, Guid, FixtureDocument, string>
    {
        public Expression<Func<FixtureDocument, string>> KeySelector { get; } = x => x.Id;

        public Expression<Func<FixtureDocument, FixtureEntity>> MapFromDocument { get; } =
            x => new FixtureEntity(Guid.Parse(x.Id));
        public Func<FixtureEntity, FixtureDocument> MapToDocument { get; } = x => new FixtureDocument(x.Id.ToString());
        public Func<Guid, string> MapKey { get; } = x => x.ToString();
    }
}