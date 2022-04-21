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
		public void GetArgsTypes_AnImplementationDoesNotHaveParameterlessConstructor_Throws()
		{
			var ex = Assert.Throws<NoPublicParameterlessConstructor>(
				() => GetArgs(typeof(NonParameterlessConsturctorArgs)));

			Assert.AreEqual(ex.Type, typeof(NonParameterlessConsturctorArgs));
		}

		[Test]
		public void GetArgsTypes_AnImplementationHasPrivateParameterlessConstructor_Throws()
		{
			var ex = Assert.Throws<NoPublicParameterlessConstructor>(
				() => GetArgs(typeof(PrivateConstructorArgs)));

			Assert.AreEqual(ex.Type, typeof(PrivateConstructorArgs));
		}

        [Test]
		public void GetArgsTypes_AnImplementationDoesNotSetKeySelector_Throws()
		{
			var ex = Assert.Throws<ArgumentException>(() => GetArgs(typeof(NokeySelectorArgs)));

			Assert.AreEqual("KeySelector is not set for GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+NokeySelectorArgs", ex.Message);
		}

		[Test]
		public void GetArgsTypes_RetrievesExpectedTypes()
		{
			var args = GetArgs(typeof(Args1), typeof(object), typeof(Args2));

			Assert.AreEqual(2, args.Count);
			Assert.True(args.Any(x => Is(x, new Args1())));
			Assert.True(args.Any(x => Is(x, new Args2())));
		}

        private bool Is<TEntity, TKey>(GenericMongoRepositoryArgsType argsType, GenericMongoRepositoryArgs<TEntity, TKey> args)
			where TKey : IEquatable<TKey>
			=> argsType.KeyType == typeof(TKey)
			   && argsType.EntityType == typeof(TEntity);

        private List<GenericMongoRepositoryArgsType> GetArgs(params Type[] types)
			=> MockTypesProvider(types).Call(x => new GenericMongoRepositoryArgsProvider(x).GetArgsTypes(null));

		private ITypesProvider MockTypesProvider(Type[] types)
		{
			var mock = new Mock<ITypesProvider>();
			mock
				.Setup(x => x.GetTypes(It.IsAny<Assembly[]>()))
				.Returns(types.ToList());

			return mock.Object;
		}

        private class NonParameterlessConsturctorArgs : GenericMongoRepositoryArgs<NonParameterlessConsturctorEntity, int>
		{
			public NonParameterlessConsturctorArgs(int aParam)
			{
			}

			public override Expression<Func<NonParameterlessConsturctorEntity, int>> KeySelector { get; }
		}

		private class NonParameterlessConsturctorEntity { }


		private class Args1 : GenericMongoRepositoryArgs<Entity1, int>
		{
			public override Expression<Func<Entity1, int>> KeySelector { get; } = x => x.Id;
		}

		private class Entity1
		{
			public int Id { get; set; }
		}

		private class Args2 : GenericMongoRepositoryArgs<Entity2, int>
		{
			public override Expression<Func<Entity2, int>> KeySelector { get; } = x => x.Id;
		}

		private class Entity2
		{
			public int Id { get; set; }
		}

		private class NokeySelectorArgs : GenericMongoRepositoryArgs<NokeySelectorEntity, int>
		{
			public override Expression<Func<NokeySelectorEntity, int>> KeySelector { get; }
		}

		private class NokeySelectorEntity { }

        private class PrivateConstructorEntity
		{
			public int Id { get; set; }
		}

		private class PrivateConstructorArgs : GenericMongoRepositoryArgs<PrivateConstructorEntity, int>
		{
			private PrivateConstructorArgs()
			{
				
			}

			public override Expression<Func<PrivateConstructorEntity, int>> KeySelector { get; } = x => x.Id;
		}
    }
}