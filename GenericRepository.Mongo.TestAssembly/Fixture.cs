using System;

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
}