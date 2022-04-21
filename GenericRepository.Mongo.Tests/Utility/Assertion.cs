using Newtonsoft.Json;
using NUnit.Framework;

namespace GenericRepository.Mongo.Tests
{
	internal static class Assertion
	{
		public static void AreIdentical(object expected, object actual)
		{
			Assert.AreEqual(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
		}
	}
}