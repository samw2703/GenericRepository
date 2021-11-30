using System;
using System.Linq.Expressions;

namespace GenericRepository.Mongo.TestAssembly
{
	public class Thing
	{
		public Guid Id { get; }

		public Thing(Guid id)
		{
			Id = id;
		}
	}

	public class ThingArgs : ISimpleGenericMongoRepositoryArgs<Thing, Guid>
	{
		public ThingArgs()
		{
		}

		public ThingArgs(int i)
		{
		}

		public Expression<Func<Thing, Guid>> KeySelector { get; } = x => x.Id;
	}
}