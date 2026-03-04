using System.Net;

namespace NocMonitor.Models;

public class HostConfig
{
    // Nombre del servidor (obligatorio)
    private string _name = string.Empty;

    // IP del servidor (obligatorio)
    private string _ip = string.Empty;

    public string Name
    {
        get => _name;
        set => _name = (value ?? string.Empty).Trim();
    }

    public string Ip
    {
        get => _ip;
        set => _ip = (value ?? string.Empty).Trim();
    }

    // Campo opcional para futura UI / dashboard
    public string Description { get; set; } = string.Empty;

    // Overrides opcionales (si no se usan, el motor usa la config general)
    public int? CustomIntervalSeconds { get; set; } = null;
    public int? CustomFailThreshold { get; set; } = null;

    // Validar host individual
    public bool IsValid(out string error)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            error = "A host has no Name.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Ip))
        {
            error = $"Host '{Name}' has no IP.";
            return false;
        }

        if (!IPAddress.TryParse(Ip, out _))
        {
            error = $"Host '{Name}' has an invalid IP: {Ip}";
            return false;
        }

        error = string.Empty;
        return true;
    }
}