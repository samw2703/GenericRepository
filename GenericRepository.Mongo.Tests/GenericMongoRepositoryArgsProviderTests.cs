using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Callinho;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;

namespace GenericRepository.Mongo.Tests
{
	public class GenericMongoRepositoryArgsProviderTests
	{
        [Test]
		public void GetSimpleArgsTypes_AnImplementationDoesNotHaveParameterlessConstructor_Throws()
		{
			var ex = Assert.Throws<NoPublicParameterlessConstructor>(
				() => GetSimpleArgs(typeof(NonParameterlessConsturctorArgs)));

			Assert.AreEqual(ex.Type, typeof(NonParameterlessConsturctorArgs));
		}

		[Test]
		public void GetSimpleArgsTypes_AnImplementationHasPrivateParameterlessConstructor_Throws()
		{
			var ex = Assert.Throws<NoPublicParameterlessConstructor>(
				() => GetSimpleArgs(typeof(PrivateConstructorArgs)));

			Assert.AreEqual(ex.Type, typeof(PrivateConstructorArgs));
		}

        [Test]
		public void GetSimpleArgsTypes_AnImplementationDoesNotSetKeySelector_Throws()
		{
			var ex = Assert.Throws<ArgumentException>(() => GetSimpleArgs(typeof(NokeySelectorArgs)));

			Assert.AreEqual("KeySelector is not set for GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+NokeySelectorArgs", ex.Message);
		}

		[Test]
		public void GetSimpleArgsTypes_RetrievesExpectedTypes()
		{
			var args = GetSimpleArgs(typeof(Args1), typeof(object), typeof(Args2));

			Assert.AreEqual(2, args.Count);
			Assert.True(args.Any(x => Is(x, new Args1())));
			Assert.True(args.Any(x => Is(x, new Args2())));
		}

        private bool Is<TEntity, TKey>(SimpleGenericMongoRepositoryArgsType argsType, GenericMongoRepositoryArgs<TEntity, TKey> args)
			where TKey : IEquatable<TKey>
			=> argsType.GetKeyType() == typeof(TKey)
			   && argsType.GetEntityType() == typeof(TEntity);

        private List<SimpleGenericMongoRepositoryArgsType> GetSimpleArgs(params Type[] types)
			=> MockTypesProvider(types).Call(x => new GenericMongoRepositoryArgsProvider(x).GetSimpleArgsTypes(null));

		private ITypesProvider MockTypesProvider(Type[] types)
		{
			var mock = new Mock<ITypesProvider>();
			mock
				.Setup(x => x.GetTypes(It.IsAny<Assembly[]>()))
				.Returns(types.ToList());

			return mock.Object;
		}

        private class NonParameterlessConsturctorArgs : GenericMongoRepositoryArgs<SimpleNonParameterlessConsturctorEntity, int>
		{
			public NonParameterlessConsturctorArgs(int aParam)
			{
			}

			public override Expression<Func<SimpleNonParameterlessConsturctorEntity, int>> KeySelector { get; }
		}

		private class SimpleNonParameterlessConsturctorEntity { }


		private class Args1 : GenericMongoRepositoryArgs<SimpleEntity1, int>
		{
			public override Expression<Func<SimpleEntity1, int>> KeySelector { get; } = x => x.Id;
		}

		private class SimpleEntity1
		{
			public int Id { get; set; }
		}

		private class Args2 : GenericMongoRepositoryArgs<SimpleEntity2, int>
		{
			public override Expression<Func<SimpleEntity2, int>> KeySelector { get; } = x => x.Id;
		}

		private class SimpleEntity2
		{
			public int Id { get; set; }
		}

		private class NokeySelectorArgs : GenericMongoRepositoryArgs<NokeySelectorSimpleEntity, int>
		{
			public override Expression<Func<NokeySelectorSimpleEntity, int>> KeySelector { get; }
		}

		private class NokeySelectorSimpleEntity { }

        private class PrivateConstructorSimpleEntity
		{
			public int Id { get; set; }
		}

		private class PrivateConstructorArgs : GenericMongoRepositoryArgs<PrivateConstructorSimpleEntity, int>
		{
			private PrivateConstructorArgs()
			{
				
			}

			public override Expression<Func<PrivateConstructorSimpleEntity, int>> KeySelector { get; } = x => x.Id;
		}
    }
}