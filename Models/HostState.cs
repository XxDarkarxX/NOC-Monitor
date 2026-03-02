namespace NocMonitor.Models;

public class HostState
{
    // Estado actual: UNKNOWN | UP | DOWN
    public string State { get; set; } = "UNKNOWN";

    // Conteo de fallos consecutivos de ping
    public int FailCount { get; set; } = 0;

    // Cantidad de alertas enviadas durante el ciclo actual (máx. 3)
    public int AlertCount { get; set; } = 0;

    // Fecha de última alerta enviada
    public DateTime? LastAlert { get; set; }

    // Si este valor no es null → el host está en cooldown (pausa)
    public DateTime? PauseUntil { get; set; }

    // --- Utilidades internas (no obligatorias, pero muy útiles) ---

    // Está en pausa?
    public bool IsPaused =>
        PauseUntil != null && DateTime.Now < PauseUntil.Value;

    // Está considerado DOWN actualmente?
    public bool IsDown => State == "DOWN";

    // Estado inicial?
    public bool IsUnknown => State == "UNKNOWN";

    // Está UP?
    public bool IsUp => State == "UP";
}