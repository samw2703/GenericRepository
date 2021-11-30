using System;
using System.Linq.Expressions;

namespace GenericRepository.Mongo.TestAssembly
{
	public class Person
	{
		public Guid Id { get; }
		public string Name { get; }

		public Person(Guid id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	internal class PersonRepoArgs : ISimpleGenericMongoRepositoryArgs<Person, Guid>
	{
		public Expression<Func<Person, Guid>> KeySelector { get; } = x => x.Id;
	}
}