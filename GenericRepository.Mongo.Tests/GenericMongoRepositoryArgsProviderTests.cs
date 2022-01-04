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
		public void GetArgsTypes_AnImplementationImplementsMultipleGenericTypeArgs_Throws()
		{
			var ex = Assert.Throws<ArgumentException>(() => GetArgs(typeof(MultipleImplementationsArgs)));

			Assert.AreEqual("GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+MultipleImplementationsArgs cannot implement GenericRepository.Mongo.IGenericMongoRepositoryArgs`3 multiple times", ex.Message);
		}

		[Test]
		public void GetArgsTypes_ImplementationDoesNotHaveKeySelectorSet_Throws()
		{
			var ex = Assert.Throws<ArgumentException>(() => GetArgs(typeof(NokeySelectorArgs)));

			Assert.AreEqual("KeySelector is not set for GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+NokeySelectorArgs", ex.Message);
		}

		[Test]
		public void GetArgsTypes_ImplementationDoesNotHaveMapFromDocumentSet_Throws()
		{
			var ex = Assert.Throws<ArgumentException>(() => GetArgs(typeof(NoMapFromDocumentArgs)));

			Assert.AreEqual("MapFromDocument is not set for GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+NoMapFromDocumentArgs", ex.Message);
		}

		[Test]
		public void GetArgsTypes_ImplementationDoesNotHaveMapToDocumentSet_Throws()
		{
			var ex = Assert.Throws<ArgumentException>(() => GetArgs(typeof(NoMapToDocumentArgs)));

			Assert.AreEqual("MapToDocument is not set for GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+NoMapToDocumentArgs", ex.Message);
		}

		[Test]
		public void GetArgsTypes_RetrievesExpectedTypes()
		{
			var args = GetArgs(typeof(Args1), typeof(object), typeof(Args2));

			Assert.AreEqual(2, args.Count);
			Assert.True(args.Any(x => Is(x, new Args1())));
			Assert.True(args.Any(x => Is(x, new Args2())));
		}

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

        [Test]
        public void GetArgsTypes2_AnImplementationDoesNotHaveParameterlessConstructor_Throws()
        {
			var ex = Assert.Throws<NoPublicParameterlessConstructor>(
                () => GetArgs2(typeof(NonParameterlessConsturctorArgs2)));

            Assert.AreEqual(ex.Type, typeof(NonParameterlessConsturctorArgs2));
		}

        [Test]
        public void GetArgsTypes2_AnImplementationImplementsMultipleGenericTypeArgs_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() => GetArgs2(typeof(MultipleImplementationsArgs2)));

            Assert.AreEqual("GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+MultipleImplementationsArgs2 cannot implement GenericRepository.Mongo.IGenericMongoRepositoryArgs`4 multiple times", ex.Message);
		}

        [Test]
        public void GetArgsTypes2_ImplementationDoesNotHaveKeySelectorSet_Throws()
        {
			var ex = Assert.Throws<ArgumentException>(() => GetArgs2(typeof(NokeySelectorArgs2)));

            Assert.AreEqual("KeySelector is not set for GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+NokeySelectorArgs2", ex.Message);
		}

        [Test]
        public void GetArgsTypes2_ImplementationDoesNotHaveMapFromDocumentSet_Throws()
        {
			var ex = Assert.Throws<ArgumentException>(() => GetArgs2(typeof(NoMapFromDocumentArgs2)));

            Assert.AreEqual("MapFromDocument is not set for GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+NoMapFromDocumentArgs2", ex.Message);
		}

        [Test]
        public void GetArgsTypes2_ImplementationDoesNotHaveMapToDocumentSet_Throws()
        {
			var ex = Assert.Throws<ArgumentException>(() => GetArgs2(typeof(NoMapToDocumentArgs2)));

            Assert.AreEqual("MapToDocument is not set for GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+NoMapToDocumentArgs2", ex.Message);
		}

        [Test]
        public void GetArgsTypes2_ImplementationDoesNotHaveMapKeySet_Throws()
        {
			var ex = Assert.Throws<ArgumentException>(() => GetArgs2(typeof(NoMapKeyArgs)));

            Assert.AreEqual("MapKey is not set for GenericRepository.Mongo.Tests.GenericMongoRepositoryArgsProviderTests+NoMapKeyArgs", ex.Message);
		}

        [Test]
        public void GetArgsTypes2_RetrievesExpectedTypes()
        {
			var args = GetArgs2(typeof(Args3), typeof(object), typeof(Args4));

            Assert.AreEqual(2, args.Count);
            Assert.True(args.Any(x => Is(x, new Args3())));
            Assert.True(args.Any(x => Is(x, new Args4())));
		}

		private bool Is<TEntity, TKey, TDocument>(GenericMongoRepositoryArgsType argsType, IGenericMongoRepositoryArgs<TEntity, TKey, TDocument> args)
			where TKey : IEquatable<TKey>
			=> argsType.GetKeyType() == typeof(TKey)
			   && argsType.GetEntityType() == typeof(TEntity)
			   && argsType.GetDocumentType() == typeof(TDocument);

        private bool Is<TEntity, TEntityKey, TDocument, TDocumentKey>(GenericMongoRepository2ArgsType argsType,
            IGenericMongoRepositoryArgs<TEntity, TEntityKey, TDocument, TDocumentKey> args)
            where TDocumentKey : IEquatable<TDocumentKey>
            => argsType.GetEntityKeyType() == typeof(TEntityKey)
               && argsType.GetEntityType() == typeof(TEntity)
               && argsType.GetDocumentType() == typeof(TDocument)
               && argsType.GetDocumentKeyType() == typeof(TDocumentKey);

		private bool Is<TEntity, TKey>(SimpleGenericMongoRepositoryArgsType argsType, ISimpleGenericMongoRepositoryArgs<TEntity, TKey> args)
			where TKey : IEquatable<TKey>
			=> argsType.GetKeyType() == typeof(TKey)
			   && argsType.GetEntityType() == typeof(TEntity);

		private List<GenericMongoRepositoryArgsType> GetArgs(params Type[] types)
			=> MockTypesProvider(types).Call(x => new GenericMongoRepositoryArgsProvider(x).GetArgsTypes(null));

        private List<GenericMongoRepository2ArgsType> GetArgs2(params Type[] types)
            => MockTypesProvider(types).Call(x => new GenericMongoRepositoryArgsProvider(x).GetArgsTypes2(null));

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

		private class NonParameterlessConsturctorArgs : IGenericMongoRepositoryArgs<NonParameterlessConstuctorEntity, int, NonParameterlessConstuctorDocument>
		{
			public NonParameterlessConsturctorArgs(int aParam)
			{
			}

			public Expression<Func<NonParameterlessConstuctorDocument, int>> KeySelector { get; }
			public Expression<Func<NonParameterlessConstuctorDocument, NonParameterlessConstuctorEntity>> MapFromDocument { get; }

			public Func<NonParameterlessConstuctorEntity, NonParameterlessConstuctorDocument> MapToDocument { get; }
		}

		private class NonParameterlessConstuctorEntity{}

		private class NonParameterlessConstuctorDocument{}

		private class Args1 : IGenericMongoRepositoryArgs<Entity1, int, Document1>
		{
			public Expression<Func<Document1, int>> KeySelector { get; } = x => x.Id;
			public Expression<Func<Document1, Entity1>> MapFromDocument { get; } = x => new Entity1();
			public Func<Entity1, Document1> MapToDocument { get; } = x => new Document1();
		}

		private class Entity1 { }

		private class Document1
		{
			public int Id { get; set; }
		}

		private class Args2 : IGenericMongoRepositoryArgs<Entity2, int, Document2>
		{
			public Expression<Func<Document2, int>> KeySelector { get; } = x => x.Id;
			public Expression<Func<Document2, Entity2>> MapFromDocument { get; } = x => new Entity2();
			public Func<Entity2, Document2> MapToDocument { get; } = x => new Document2();
		}

		private class Entity2 { }

		private class Document2
		{
			public int Id { get; set; }
		}

		private class NokeySelectorArgs : IGenericMongoRepositoryArgs<NokeySelectorEntity, int, NokeySelectorDocument>
		{
			public Expression<Func<NokeySelectorDocument, int>> KeySelector { get; }
			public Expression<Func<NokeySelectorDocument, NokeySelectorEntity>> MapFromDocument { get; } 
				= x => new NokeySelectorEntity();
			public Func<NokeySelectorEntity, NokeySelectorDocument> MapToDocument { get; } = x => new NokeySelectorDocument();
		}

		private class NokeySelectorEntity { }

		private class NokeySelectorDocument { }

		private class NoMapFromDocumentArgs : IGenericMongoRepositoryArgs<NoMapFromDocumentEntity, int, NoMapFromDocumentDocument>
		{
			public Expression<Func<NoMapFromDocumentDocument, int>> KeySelector { get; } = x => x.Id;
			public Expression<Func<NoMapFromDocumentDocument, NoMapFromDocumentEntity>> MapFromDocument { get; }
			public Func<NoMapFromDocumentEntity, NoMapFromDocumentDocument> MapToDocument { get; } = x => new NoMapFromDocumentDocument();
		}

		private class NoMapFromDocumentEntity { }

		private class NoMapFromDocumentDocument
		{
			public int Id { get; set; }
		}

		private class NoMapToDocumentArgs : IGenericMongoRepositoryArgs<NoMapToDocumentEntity, int, NoMapToDocumentDocument>
		{
			public Expression<Func<NoMapToDocumentDocument, int>> KeySelector { get; } = x => x.Id;
			public Expression<Func<NoMapToDocumentDocument, NoMapToDocumentEntity>> MapFromDocument { get; }
				= x => new NoMapToDocumentEntity();
			public Func<NoMapToDocumentEntity, NoMapToDocumentDocument> MapToDocument { get; }
		}

		private class NoMapToDocumentEntity { }

		private class NoMapToDocumentDocument
		{
			public int Id { get; set; }
		}

		private class MultipleImplementationsArgs : IGenericMongoRepositoryArgs<object, int, object>, 
            IGenericMongoRepositoryArgs<string, int, string>
        {
			private Expression<Func<object, int>> _keySelector;
			private Expression<Func<object, object>> _mapFromDocument;
			private Func<object, object> _mapToDocument;
			private Expression<Func<string, int>> _keySelector1;
			private Expression<Func<string, string>> _mapFromDocument1;
			private Func<string, string> _mapToDocument1;

			Expression<Func<object, int>> IGenericMongoRepositoryArgs<object, int, object>.KeySelector => _keySelector;
            Expression<Func<string, string>> IGenericMongoRepositoryArgs<string, int, string>.MapFromDocument => _mapFromDocument1;
            Func<string, string> IGenericMongoRepositoryArgs<string, int, string>.MapToDocument => _mapToDocument1;
            Expression<Func<string, int>> IGenericMongoRepositoryArgs<string, int, string>.KeySelector => _keySelector1;
            Expression<Func<object, object>> IGenericMongoRepositoryArgs<object, int, object>.MapFromDocument => _mapFromDocument;
            Func<object, object> IGenericMongoRepositoryArgs<object, int, object>.MapToDocument => _mapToDocument;
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

        private class NonParameterlessConsturctorArgs2 : IGenericMongoRepositoryArgs<NonParameterlessConstuctorEntity, int, NonParameterlessConstuctorDocument, int>
        {
            public NonParameterlessConsturctorArgs2(int aParam)
            {
            }

            public Expression<Func<NonParameterlessConstuctorDocument, int>> KeySelector { get; }
            public Expression<Func<NonParameterlessConstuctorDocument, NonParameterlessConstuctorEntity>> MapFromDocument { get; }
            public Func<NonParameterlessConstuctorEntity, NonParameterlessConstuctorDocument> MapToDocument { get; }
            public Func<int, int> MapKey { get; }
        }

        private class NokeySelectorArgs2 : IGenericMongoRepositoryArgs<NokeySelectorEntity, int, NokeySelectorDocument, int>
        {
            public Expression<Func<NokeySelectorDocument, int>> KeySelector { get; }
            public Expression<Func<NokeySelectorDocument, NokeySelectorEntity>> MapFromDocument { get; }
                = x => new NokeySelectorEntity();
            public Func<NokeySelectorEntity, NokeySelectorDocument> MapToDocument { get; } = x => new NokeySelectorDocument();
            public Func<int, int> MapKey { get; } = x => 1;
        }

        private class NoMapFromDocumentArgs2 : IGenericMongoRepositoryArgs<NoMapFromDocumentEntity, int, NoMapFromDocumentDocument, int>
        {
            public Expression<Func<NoMapFromDocumentDocument, int>> KeySelector { get; } = x => x.Id;
            public Expression<Func<NoMapFromDocumentDocument, NoMapFromDocumentEntity>> MapFromDocument { get; }
            public Func<NoMapFromDocumentEntity, NoMapFromDocumentDocument> MapToDocument { get; } = x => new NoMapFromDocumentDocument();
            public Func<int, int> MapKey { get; } = x => 1;
        }

        private class NoMapToDocumentArgs2 : IGenericMongoRepositoryArgs<NoMapToDocumentEntity, int, NoMapToDocumentDocument, int>
        {
            public Expression<Func<NoMapToDocumentDocument, int>> KeySelector { get; } = x => x.Id;
            public Expression<Func<NoMapToDocumentDocument, NoMapToDocumentEntity>> MapFromDocument { get; }
                = x => new NoMapToDocumentEntity();
            public Func<NoMapToDocumentEntity, NoMapToDocumentDocument> MapToDocument { get; }
			public Func<int, int> MapKey { get; } = x => 1;
		}

        private class NoMapKeyArgs : IGenericMongoRepositoryArgs<object, int, object, int>
        {
            public Expression<Func<object, int>> KeySelector { get; } = x => 1;
            public Expression<Func<object, object>> MapFromDocument { get; } = x => new object();
			public Func<object, object> MapToDocument { get; } = x => new object();
            public Func<int, int> MapKey { get; }
        }

        private class MultipleImplementationsArgs2 : IGenericMongoRepositoryArgs<object, int, object, int>,
            IGenericMongoRepositoryArgs<string, int, string, int>
        {
            private Expression<Func<object, int>> _keySelector;
            private Expression<Func<object, object>> _mapFromDocument;
            private Func<object, object> _mapToDocument;
            private Expression<Func<string, int>> _keySelector1;
            private Expression<Func<string, string>> _mapFromDocument1;
            private Func<string, string> _mapToDocument1;

            Expression<Func<object, int>> IGenericMongoRepositoryArgs<object, int, object, int>.KeySelector => _keySelector;
            Expression<Func<string, string>> IGenericMongoRepositoryArgs<string, int, string, int>.MapFromDocument => _mapFromDocument1;
            Func<string, string> IGenericMongoRepositoryArgs<string, int, string, int>.MapToDocument => _mapToDocument1;
            Expression<Func<string, int>> IGenericMongoRepositoryArgs<string, int, string, int>.KeySelector => _keySelector1;
            Expression<Func<object, object>> IGenericMongoRepositoryArgs<object, int, object, int>.MapFromDocument => _mapFromDocument;
            Func<object, object> IGenericMongoRepositoryArgs<object, int, object, int>.MapToDocument => _mapToDocument;
            public Func<int, int> MapKey { get; } = x => 1;
        }

        private class Args3 : IGenericMongoRepositoryArgs<object, int, object, int>
        {
            public Expression<Func<object, int>> KeySelector { get; } = x => 1;
            public Expression<Func<object, object>> MapFromDocument { get; } = x => new object();
            public Func<object, object> MapToDocument { get; } = x => new object();
			public Func<int, int> MapKey { get; } = x => 1;
		}

        private class Args4 : IGenericMongoRepositoryArgs<string, int, string, int>
        {
            public Expression<Func<string, int>> KeySelector { get; } = x => 1;
            public Expression<Func<string, string>> MapFromDocument { get; } = x => "";
            public Func<string, string> MapToDocument { get; } = x => "";
			public Func<int, int> MapKey { get; } = x => 1;
		}
	}
}