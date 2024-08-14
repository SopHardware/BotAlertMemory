using BotAletMemory.Components;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;

public class SlackNotifier
{
    private readonly Config _config;
    private readonly string _machineName;

    public SlackNotifier(Config config, string machineName)
    {
        _config = config;
        _machineName = machineName;
    }

    public async Task SendAlert(float memoryUsage)
    {
        using (HttpClient cl = new HttpClient())
        {
            var payload = new
            {
                text = $"Alerta: El consumo de memoria en la máquina {_machineName} ha superado el límite. Uso actual: {memoryUsage} MB."
            };

            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage rsp = await cl.PostAsync(_config.ApiConf.SlackWebhookUrl, content);
            if (rsp.IsSuccessStatusCode)
            {
                Console.WriteLine("Alerta enviada a Slack.");
            }
            else
            {
                Console.WriteLine("Error al enviar la alerta a Slack.");
            }

            var resetService = new ResetService(_config);
            resetService.CheckAndResetService();
        }
    }
}