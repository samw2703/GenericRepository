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
				() => GetSimpleArgs(typeof(SimpleNonParameterlessConsturctorArgs)));

			Assert.AreEqual(ex.Type, typeof(SimpleNonParameterlessConsturctorArgs));
		}

		[Test]
		public void GetSimpleArgsTypes_AnImplementationHasPrivateParameterlessConstructor_Throws()
		{
			var ex = Assert.Throws<NoPublicParameterlessConstructor>(
				() => GetSimpleArgs(typeof(PrivateConstructorSimpleArgs)));

			Assert.AreEqual(ex.Type, typeof(PrivateConstructorSimpleArgs));
		}

		[Test]
		public void GetSimpleArgsTypes_AnImplementationImplementsMultipleSimpleGenericTypeArgs_Throws()
		{
			var ex = Assert.Throws<ArgumentException>(() => GetSimpleArgs(typeof(MultipleSimpleImplementationsArgs)));

			Assert.AreEqual("GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+MultipleSimpleImplementationsArgs cannot implement GenericRepository.Mongo.ISimpleGenericMongoRepositoryArgs`2 multiple times", ex.Message);
		}

		[Test]
		public void GetSimpleArgsTypes_AnImplementationDoesNotSetKeySelector_Throws()
		{
			var ex = Assert.Throws<ArgumentException>(() => GetSimpleArgs(typeof(NokeySelectorSimpleArgs)));

			Assert.AreEqual("KeySelector is not set for GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+NokeySelectorSimpleArgs", ex.Message);
		}

		[Test]
		public void GetSimpleArgsTypes_RetrievesExpectedTypes()
		{
			var args = GetSimpleArgs(typeof(SimpleArgs1), typeof(object), typeof(SimpleArgs2));

			Assert.AreEqual(2, args.Count);
			Assert.True(args.Any(x => Is(x, new SimpleArgs1())));
			Assert.True(args.Any(x => Is(x, new SimpleArgs2())));
		}

        private bool Is<TEntity, TKey>(SimpleGenericMongoRepositoryArgsType argsType, ISimpleGenericMongoRepositoryArgs<TEntity, TKey> args)
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

        private class SimpleNonParameterlessConsturctorArgs : ISimpleGenericMongoRepositoryArgs<SimpleNonParameterlessConsturctorEntity, int>
		{
			public SimpleNonParameterlessConsturctorArgs(int aParam)
			{
			}

			public Expression<Func<SimpleNonParameterlessConsturctorEntity, int>> KeySelector { get; }
		}

		private class SimpleNonParameterlessConsturctorEntity { }


		private class SimpleArgs1 : ISimpleGenericMongoRepositoryArgs<SimpleEntity1, int>
		{
			public Expression<Func<SimpleEntity1, int>> KeySelector { get; } = x => x.Id;
		}

		private class SimpleEntity1
		{
			public int Id { get; set; }
		}

		private class SimpleArgs2 : ISimpleGenericMongoRepositoryArgs<SimpleEntity2, int>
		{
			public Expression<Func<SimpleEntity2, int>> KeySelector { get; } = x => x.Id;
		}

		private class SimpleEntity2
		{
			public int Id { get; set; }
		}

		private class NokeySelectorSimpleArgs : ISimpleGenericMongoRepositoryArgs<NokeySelectorSimpleEntity, int>
		{
			public Expression<Func<NokeySelectorSimpleEntity, int>> KeySelector { get; }
		}

		private class NokeySelectorSimpleEntity { }

		private class MultipleSimpleImplementationsArgs : ISimpleGenericMongoRepositoryArgs<object, int>, ISimpleGenericMongoRepositoryArgs<string, int>
		{
			private Expression<Func<object, int>> _keySelector;
			private Expression<Func<string, int>> _keySelector1;

			Expression<Func<object, int>> ISimpleGenericMongoRepositoryArgs<object, int>.KeySelector => _keySelector;

			Expression<Func<string, int>> ISimpleGenericMongoRepositoryArgs<string, int>.KeySelector => _keySelector1;
		}

		private class PrivateConstructorSimpleEntity
		{
			public int Id { get; set; }
		}

		private class PrivateConstructorSimpleArgs : ISimpleGenericMongoRepositoryArgs<PrivateConstructorSimpleEntity, int>
		{
			private PrivateConstructorSimpleArgs()
			{
				
			}

			public Expression<Func<PrivateConstructorSimpleEntity, int>> KeySelector { get; } = x => x.Id;
		}
    }
}