using Microsoft.Extensions.Configuration;

namespace GenericRepository.Mongo.Tests
{
	internal static class Config
	{
		static Config()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.Build();
			ConnectionString = config.GetSection("ConnectionString").Value;
			DatabaseName = config.GetSection("DatabaseName").Value;
		}

		public static string ConnectionString { get; }
		public static string DatabaseName { get; }
	}
}