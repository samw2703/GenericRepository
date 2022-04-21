using System;
using System.Linq.Expressions;

namespace GenericRepository.Mongo.TestAssembly
{
	public class TeamEntity
	{
		public int Id { get; }
		public string Name { get; }

		public TeamEntity(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}