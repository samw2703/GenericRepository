using System;
using System.Linq;
using Callinho;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GenericRepository.Stub.Tests
{
	public class GenericStubbedRepositoryBuilderTests
	{
		[Test]
		public void Add_RepoThatHasNotYetBeenAdded_AddsRepo()
		{
			var serviceCollection = new ServiceCollection();
			new GenericStubbedRepositoryBuilder(serviceCollection)
				.Add<RepoItem, int>(x => x.Id);

			serviceCollection
				.Any(x => x.ServiceType == typeof(IGenericRepository<RepoItem, int>) && x.ImplementationInstance?.GetType() == typeof(GenericStubbedRepository<RepoItem,int>))
				.Use(Assert.True);
		}

		[Test]
		public void Add_RepoThatHasAlreadyBeenAdded_Throws()
		{
			var ex = Assert.Throws<ArgumentException>(
				() => new ServiceCollection()
					.Call(sc => new GenericStubbedRepositoryBuilder(sc))
					.Add<RepoItem, int>(x => x.Id)
					.Add<RepoItem, int>(x => x.Id));
			Assert.AreEqual("A repository for GenericRepository.Stub.Tests.GenericStubbedRepositoryBuilderTests+RepoItem with key System.Int32 has already been registered", ex.Message);
		}

		private class RepoItem
		{
			public int Id { get; set; }
		}

	}
}