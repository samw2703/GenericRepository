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

	public class ThingArgs : GenericMongoRepositoryArgs<Thing, Guid>
	{
		public ThingArgs()
		{
		}

		public ThingArgs(int i)
		{
		}

		public override Expression<Func<Thing, Guid>> KeySelector { get; } = x => x.Id;
	}
}