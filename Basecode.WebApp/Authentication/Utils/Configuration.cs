namespace Basecode.WebApp.Utilities;

public class Configuration
{
    static Configuration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
        Config = builder.Build();

        Config = Config;
    }

    public static IConfigurationRoot Config { get; set; }

    public static string DbConnection => Config["DefaultConnection"];
}