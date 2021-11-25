using System;

namespace GenericRepository.Stub.Tests
{
	partial class GenericStubbedRepositoryTests
	{
		private class RepoItem
		{
			public Guid Id { get; set; }
			public string Value { get; set; }

			public RepoItem(Guid id, string value)
			{
				Id = id;
				Value = value;
			}
		}

		private class RepoItemRepository : GenericStubbedRepository<RepoItem, Guid>
		{
			protected override Guid GetKey(RepoItem item) => item.Id;
		}
	}
}
