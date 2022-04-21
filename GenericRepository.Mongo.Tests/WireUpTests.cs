using System;
using System.Threading.Tasks;
using GenericRepository.Abstractions;
using GenericRepository.Mongo.TestAssembly;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GenericRepository.Mongo.Tests
{
	public class WireUpTests : MongoTestsBase
	{
        [SetUp]
        public void Setup()
        {
            BsonClassMapHelper.Clear();
        }

		[Test]
		public async Task DoesWireUpGenericMongoRepository()
		{
			var repo = WireUpAndReturnRepository<User, int>();
			var id = 3;
			var user = new User(id, "Billy Joel");
			await repo.Save(user);

			Assertion.AreIdentical(user, await repo.Get(id));
		}

		[Test]
		public async Task DoesWireUpInternalGenericMongoRepository()
		{
			var repo = WireUpAndReturnRepository<Person, Guid>();
			var id = Guid.NewGuid();
			var person = new Person(id, "Steeeeve");
			await repo.Save(person);

			Assertion.AreIdentical(person, await repo.Get(id));
		}

		[Test]
		public async Task DoesWireUpWhenArgsHaveMultipleConstructorsAndOneIsPublicParameterless()
		{
			var repo = WireUpAndReturnRepository<Thing, Guid>();
			var id = Guid.NewGuid();
			var thing = new Thing(id);
			await repo.Save(thing);

			Assertion.AreIdentical(thing, await repo.Get(id));
		}

        private IGenericRepository<T, TKey> WireUpAndReturnRepository<T, TKey>()
		{
			var services = new ServiceCollection();
			services.AddGenericMongoRepositories(Config.ConnectionString, Config.DatabaseName, typeof(TeamEntity).Assembly);
			return services
				.BuildServiceProvider()
				.GetRequiredService<IGenericRepository<T, TKey>>();
		}
	}
}