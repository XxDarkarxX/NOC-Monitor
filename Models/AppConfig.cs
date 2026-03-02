namespace NocMonitor.Models;

public class AppConfig
{
    // Intervalo mínimo seguro (2 segundos)
    private int _intervalSeconds = 2;

    // Fallos mínimos requeridos para evitar falsas alarmas (5)
    private int _failThreshold = 5;

    public int IntervalSeconds
    {
        get => _intervalSeconds;
        set => _intervalSeconds = value < 1 ? 2 : value;
    }

    public int FailThreshold
    {
        get => _failThreshold;
        set => _failThreshold = value < 1 ? 5 : value;
    }

    // Webhook obligatorio
    public string DiscordWebhook { get; set; } = string.Empty;

    // Lista de hosts configurados
    public List<HostConfig> Hosts { get; set; } = new();

    // Valida que la configuración esté correcta
    public bool IsValid(out string error)
    {
        if (string.IsNullOrWhiteSpace(DiscordWebhook))
        {
            error = "DiscordWebhook is missing in config.json";
            return false;
        }

        if (Hosts == null || Hosts.Count == 0)
        {
            error = "No hosts defined in config.json";
            return false;
        }

        foreach (var host in Hosts)
        {
            if (string.IsNullOrWhiteSpace(host.Ip))
            {
                error = "A host is missing its IP address.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(host.Name))
            {
                error = $"Host {host.Ip} is missing a name.";
                return false;
            }
        }

        error = string.Empty;
        return true;
    }
}