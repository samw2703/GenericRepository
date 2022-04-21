using Callinho;
using MongoDB.Driver;
using NUnit.Framework;

namespace GenericRepository.Mongo.Tests
{
	public abstract class MongoTestsBase
	{
		private IMongoDatabase _db;

		[SetUp]
		public void Setup()
		{
			new MongoClient(Config.ConnectionString)
				.Use(x => ResetDatabase(x, Config.DatabaseName));
		}

		private void ResetDatabase(MongoClient client, string dbName)
		{
			client.DropDatabase(dbName);
			_db = client.GetDatabase(dbName);
		}

		protected IMongoCollection<T> GetCollection<T>()
			=> _db.GetCollection<T>(typeof(T).Name);
	}
}