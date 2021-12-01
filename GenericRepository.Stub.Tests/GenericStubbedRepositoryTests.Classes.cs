using System;

namespace GenericRepository.Stub.Tests
{
	partial class GenericStubbedRepositoryTests
	{
		private class RepoItem
		{
			public Guid Id { get; }
			public int OtherId { get; }
			public string Value { get; private set; }

			public void SetValue(string newValue)
				=> Value = newValue;

			public RepoItem(Guid id, int otherId = 0, string value = "")
			{
				Id = id;
				OtherId = otherId;
				Value = value;
			}
		}
	}
}
