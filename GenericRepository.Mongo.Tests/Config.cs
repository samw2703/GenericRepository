using Microsoft.Extensions.Configuration;

namespace GenericRepository.Mongo.Tests
{
	internal static class Config
	{
		public static IConfiguration Init()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.Build();
			return config;
		}
	}
}