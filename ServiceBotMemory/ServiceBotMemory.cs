using BotAletMemory.Components;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ServiceBotMemory
{
    public partial class ServiceBotMemory : ServiceBase
    {
        private Task _monitoringTask;
        private EventLog _eventLog;

        public ServiceBotMemory()
        {
            InitializeComponent();
            _eventLog = new EventLog("Application");
            _eventLog.Source = "ServiceBotMemory";
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _monitoringTask = Task.Run(() => MonitorMemory());
                _eventLog.WriteEntry("El servicio ha iniciado correctamente.", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry($"Error al iniciar el servicio: {ex.Message}", EventLogEntryType.Error);
                Stop();
            }
        }

        protected override void OnStop()
        {
            _monitoringTask?.Dispose();
            _eventLog.WriteEntry("El servicio ha detenido.", EventLogEntryType.Information);
        }

        private async Task MonitorMemory()
        {
            try
            {
                Config config = Config.LoadConfig();
                MemoryMonitor memoryMonitor = new MemoryMonitor(config);
                SlackNotifier slackNotifier = new SlackNotifier(config, Environment.MachineName);

                while (true)
                {
                    try
                    {
                        float memoryUsage = memoryMonitor.GetMemoryUsage();
                        _eventLog.WriteEntry($"Memoria usada: {memoryUsage} MB", EventLogEntryType.Information);

                        if (memoryUsage > config.MmorySettings.AlertThreshold)
                        {
                            await slackNotifier.SendAlert(memoryUsage);
                            _eventLog.WriteEntry($"Alerta de memoria enviada: {memoryUsage} MB", EventLogEntryType.Warning);
                        }

                           await Task.Delay(TimeSpan.FromMinutes(config.ApiConf.DelayAlert)); // Espera 1 minuto antes de volver a comprobar
                    }
                    catch (Exception ex)
                    {
                        _eventLog.WriteEntry($"Error en el bucle principal: {ex.Message}", EventLogEntryType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el método MonitorMemory: {ex.Message}");
                _eventLog.WriteEntry($"Error en el método MonitorMemory: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
