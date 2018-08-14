using System.IO;
using Akka.Configuration;

namespace Infrastructure.Config
{
    public class ConfigurationLoader
    {
        public static Akka.Configuration.Config Load()
        {
            var developmentConfig = LoadConfig("akka.Development.conf");
            var config = LoadConfig("akka.conf");
            return developmentConfig.WithFallback(config);
        }

        private static Akka.Configuration.Config LoadConfig(string configFile)
        {
            if (File.Exists(configFile))
            {
                string config = File.ReadAllText(configFile);
                return ConfigurationFactory.ParseString(config);
            }


            return Akka.Configuration.Config.Empty;
        }
    }
}