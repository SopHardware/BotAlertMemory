using BotAletMemory.Components;
using System;
using System.Threading.Tasks;

namespace BotAletMemory
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Config config = Config.LoadConfig();
            //MemoryMonitor memoryMonitor = new MemoryMonitor(config.MmorySettings.MemoryUsageLimit);
            //SlackNotifier slackNotifier = new SlackNotifier(config.ApiConf.SlackWebhookUrl, Environment.MachineName);

            MemoryMonitor memoryMonitor = new MemoryMonitor(config);
            SlackNotifier slackNotifier = new SlackNotifier(config, Environment.MachineName);

            while (true)
            {
                float memoryUsage = memoryMonitor.GetMemoryUsage();
                Console.WriteLine($"Memoria usada: {memoryUsage} MB");

                if (memoryUsage > config.MmorySettings.AlertThreshold) // Ajusta el límite según tus necesidades
                {
                    await slackNotifier.SendAlert(memoryUsage);
                }
                await Task.Delay(TimeSpan.FromMinutes(config.ApiConf.DelayAlert)); // Espera según la configuración de DelayAlert
            }
        }
    }
}


