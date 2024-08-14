using Microsoft.Extensions.Configuration;

namespace BotAletMemory.Components
{
    public class Config
    {
        public ApiConfig ApiConf { get; set; }
        public MemorySettings MmorySettings { get; set; }
        public ServiceConfig ServConfig { get; set; }

        public class ApiConfig
        {
            public string SlackWebhookUrl { get; set; }
            public int DelayAlert { get; set; }
        }

        public class MemorySettings
        {
            public float MemoryUsageLimit { get; set; }
            public float AlertThreshold { get; set; }
        }

        public class ServiceConfig
        {
            public string AppName1 { get; set; }
            public bool IsActive { get; set; }
        }

        public static Config LoadConfig()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("Prod.json", optional: false, reloadOnChange: true)
                .Build();

            return new Config
            {
                ApiConf = new ApiConfig
                {
                    SlackWebhookUrl = config["ApiConfig:SlackWebhookUrl"],
                    DelayAlert = int.Parse(config["ApiConfig:DelayAlert"])
                },
                MmorySettings = new MemorySettings
                {
                    MemoryUsageLimit = float.Parse(config["MemorySettings:MemoryUsageLimit"]),
                    AlertThreshold = float.Parse(config["MemorySettings:AlertThreshold"])
                },

                // Este proceso es para mandar reinicar el agente mediante la clase ResertService
                ServConfig = new ServiceConfig
                {
                    AppName1 = config["ServiceConfig:AppName1"],
                    IsActive = bool.Parse(config["ServiceConfig:IsActive"])
                }
            };
        }
    }
}