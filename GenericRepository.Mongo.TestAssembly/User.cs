using System;
using System.Linq.Expressions;

namespace GenericRepository.Mongo.TestAssembly
{
	public class User
	{
		public int Id { get; }
		public string Name { get; }

		public User(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	public class UserRepositoryArgs : GenericMongoRepositoryArgs<User, int>
	{
		public override Expression<Func<User, int>> KeySelector { get; } = x => x.Id;
	}
}