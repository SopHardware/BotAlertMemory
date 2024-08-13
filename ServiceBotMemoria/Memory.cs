using ServiceBotMemoria.Componentes;
using System;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ServiceBotMemoria
{
    public partial class Memory : ServiceBase
    {
        private Task _monitoringTask;
        public Memory()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _monitoringTask = Task.Run(() => MonitorMemory());
            }
            catch (Exception ex)
            {
                Stop();
            }
        }

        protected override void OnStop()
        {
            _monitoringTask?.Dispose();
        }

        private async Task MonitorMemory()
        {
            try
            {
                Config config = Config.LoadConfig();
                MemoryMonitor memoryMonitor = new MemoryMonitor(config.TotalMemory);
                SlackNotifier slackNotifier = new SlackNotifier(config.SlackWbHookUrl, Environment.MachineName);

                while (true)
                {
                    try
                    {
                        float memoryUsage = memoryMonitor.GetMemoryUsage();
                        Console.WriteLine($"Memoria usada: {memoryUsage} MB");

                        if (memoryUsage > config.MmLimit)
                        {
                            await slackNotifier.SendAlert(memoryUsage);
                        }

                        await Task.Delay(60000); // Espera 1 minuto antes de volver a comprobar
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error en el bucle principal: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el método MonitorMemory: {ex.Message}");
            }
        }
    }
}
