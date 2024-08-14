using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BotAletMemory.Components
{
    public class ResetService
    {
        private readonly Config _config;
        private readonly int _maxAlerts = 3;
        private int _alertCount = 0;

        public ResetService(Config config)
        {
            _config = config;
        }

        public void CheckAndResetService()
        {
            if (_alertCount >= _maxAlerts && _config.ServConfig.IsActive)
            {
                try
                {
                    var serviceName = _config.ServConfig.AppName1;
                    var serviceController = new ServiceController(serviceName);
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                    serviceController.Start();
                    serviceController.WaitForStatus(ServiceControllerStatus.Running);
                    Console.WriteLine($"Servicio {serviceName} reiniciado con éxito.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al reiniciar el servicio: {ex.Message}");
                }
                finally
                {
                    _alertCount = 0;
                }
            }
            else
            {
                _alertCount++;
            }
        }
    }
}
