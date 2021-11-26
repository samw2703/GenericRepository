using System;
using System.Linq;
using Callinho;
using GenericRepository.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GenericRepository.Stub.Tests
{
	public class StubbedGenericRepositoryBuilderTests
	{
		[Test]
		public void Add_RepoThatHasNotYetBeenAdded_AddsRepo()
		{
			var serviceCollection = new ServiceCollection();
			new StubbedGenericRepositoryBuilder(serviceCollection)
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
					.Call(sc => new StubbedGenericRepositoryBuilder(sc))
					.Add<RepoItem, int>(x => x.Id)
					.Add<RepoItem, int>(x => x.Id));
			Assert.AreEqual("A repository for GenericRepository.Stub.Tests.StubbedGenericRepositoryBuilderTests+RepoItem with key System.Int32", ex.Message);
		}

		private class RepoItem
		{
			public int Id { get; set; }
		}

	}
}