using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GenericRepository.Stub.Tests
{
	public class StubbedGenericRepositoryBuilderTests
	{
		[Test]
		public void Add_RepoThatHasNotYetBeenAdded_DoesNotThrow()
		{
			Assert.DoesNotThrow(
				() => CreateBuilder().Add<RepoItem, int>(x => x.Id));
		}

		[Test]
		public void Add_RepoThatHasAlreadyBeenAdded_Throws()
		{
			var ex = Assert.Throws<ArgumentException>(
				() => CreateBuilder()
					.Add<RepoItem, int>(x => x.Id)
					.Add<RepoItem, int>(x => x.Id));
			Assert.AreEqual("A repository for GenericRepository.Stub.Tests.StubbedGenericRepositoryBuilderTests+RepoItem with key System.Int32", ex.Message);
		}

		private StubbedGenericRepositoryBuilder CreateBuilder()
			=> new(new ServiceCollection());

		private class RepoItem
		{
			public int Id { get; set; }
		}

	}
}