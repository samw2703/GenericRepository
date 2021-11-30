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

	public class Team
	{
		public int Id { get; }
		public string Name { get; }

		public Team(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	public class TeamRepoArgs : IGenericMongoRepositoryArgs<TeamEntity, int, Team>
	{
		public Expression<Func<Team, int>> KeySelector { get; } = x => x.Id;
		public Expression<Func<Team, TeamEntity>> MapFromDocument { get; } = x => new TeamEntity(x.Id, x.Name);
		public Func<TeamEntity, Team> MapToDocument { get; } = x => new Team(x.Id, x.Name);
	}
}