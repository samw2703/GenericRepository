using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Callinho;
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
			Assert.True(args.Any(x => x.Value == typeof(Args1)));
			Assert.True(args.Any(x => x.Value == typeof(Args2)));
		}

		[Test]
		public void GetSimpleArgsTypes_AnImplementationDoesNotHaveParameterlessConstructor_Throws()
		{
			var ex = Assert.Throws<NoPublicParameterlessConstructor>(
				() => GetSimpleArgs(typeof(SimpleNonParameterlessConsturctorArgs)));

			Assert.AreEqual(ex.Type, typeof(SimpleNonParameterlessConsturctorArgs));
		}

		[Test]
		public void GetSimpleArgsTypes_RetrievesExpectedTypes()
		{
			var args = GetSimpleArgs(typeof(SimpleArgs1), typeof(object), typeof(SimpleArgs2));

			Assert.AreEqual(2, args.Count);
			Assert.True(args.Any(x => x.Value == typeof(SimpleArgs1)));
			Assert.True(args.Any(x => x.Value == typeof(SimpleArgs2)));
		}

		private List<GenericMongoRepositoryArgsType> GetArgs(params Type[] types)
			=> MockTypesProvider(types).Call(x => new GenericMongoRepositoryArgsProvider(x).GetArgsTypes(null));

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

		private class NonParameterlessConsturctorArgs : IGenericMongoRepositoryArgs<NonParameterlessConsturctorEntity, int, NonParameterlessConsturctorDocument>
		{
			public NonParameterlessConsturctorArgs(int aParam)
			{
			}

			public Expression<Func<NonParameterlessConsturctorDocument, int>> KeySelector { get; }
			public Expression<Func<NonParameterlessConsturctorDocument, NonParameterlessConsturctorEntity>> MapFromDocument { get; }

			public Expression<Func<NonParameterlessConsturctorEntity, NonParameterlessConsturctorDocument>> MapToDocument { get; }
		}

		private class NonParameterlessConsturctorEntity{}

		private class NonParameterlessConsturctorDocument{}

		private class Args1 : IGenericMongoRepositoryArgs<Entity1, int, Document1>
		{
			public Expression<Func<Document1, int>> KeySelector { get; } = x => x.Id;
			public Expression<Func<Document1, Entity1>> MapFromDocument { get; } = x => new Entity1();
			public Expression<Func<Entity1, Document1>> MapToDocument { get; } = x => new Document1();
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
			public Expression<Func<Entity2, Document2>> MapToDocument { get; } = x => new Document2();
		}

		private class Entity2 { }

		private class Document2
		{
			public int Id { get; set; }
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
			public Expression<Func<SimpleEntity1, int>> KeySelector { get; }
		}

		private class SimpleEntity1 { }

		private class SimpleArgs2 : ISimpleGenericMongoRepositoryArgs<SimpleEntity2, int>
		{
			public Expression<Func<SimpleEntity2, int>> KeySelector { get; }
		}

		private class SimpleEntity2 { }

		private class NokeySelectorArgs : IGenericMongoRepositoryArgs<NokeySelectorEntity, int, NokeySelectorDocument>
		{
			public Expression<Func<NokeySelectorDocument, int>> KeySelector { get; }
			public Expression<Func<NokeySelectorDocument, NokeySelectorEntity>> MapFromDocument { get; } 
				= x => new NokeySelectorEntity();
			public Expression<Func<NokeySelectorEntity, NokeySelectorDocument>> MapToDocument { get; }
				= x => new NokeySelectorDocument();
		}

		private class NokeySelectorEntity { }

		private class NokeySelectorDocument { }

		private class NoMapFromDocumentArgs : IGenericMongoRepositoryArgs<NoMapFromDocumentEntity, int, NoMapFromDocumentDocument>
		{
			public Expression<Func<NoMapFromDocumentDocument, int>> KeySelector { get; } = x => x.Id;
			public Expression<Func<NoMapFromDocumentDocument, NoMapFromDocumentEntity>> MapFromDocument { get; }
			public Expression<Func<NoMapFromDocumentEntity, NoMapFromDocumentDocument>> MapToDocument { get; }
				= x => new NoMapFromDocumentDocument();
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
			public Expression<Func<NoMapToDocumentEntity, NoMapToDocumentDocument>> MapToDocument { get; }
		}

		private class NoMapToDocumentEntity { }

		private class NoMapToDocumentDocument
		{
			public int Id { get; set; }
		}
	}
}