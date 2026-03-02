using System.Text.Json;
using NocMonitor.Models;
using NocMonitor.Core;

try
{
    Logger.Log("=== NOC Monitor starting up ===");

    // -----------------------------
    // VALIDAR EXISTENCIA DEL CONFIG
    // -----------------------------
    if (!File.Exists("config.json"))
    {
        var msg = "ERROR: config.json not found.";
        Console.WriteLine(msg);
        Logger.Log(msg);
        return;
    }

    // -----------------------------
    // CARGAR CONFIG
    // -----------------------------
    var json = File.ReadAllText("config.json");

    AppConfig? config = null;

    try
    {
        config = JsonSerializer.Deserialize<AppConfig>(json);
    }
    catch (Exception ex)
    {
        var msg = $"ERROR: Failed to parse config.json → {ex.Message}";
        Console.WriteLine(msg);
        Logger.Log(msg);
        return;
    }

    if (config == null)
    {
        var msg = "ERROR: config.json is empty or invalid.";
        Console.WriteLine(msg);
        Logger.Log(msg);
        return;
    }

    // -----------------------------
    // VALIDAR CONFIG INTERNA
    // -----------------------------
    if (!config.IsValid(out string configError))
    {
        Console.WriteLine("CONFIG ERROR → " + configError);
        Logger.Log("CONFIG ERROR → " + configError);
        return;
    }

    Logger.Log("Config validated OK.");

    // -----------------------------
    // INICIAR MOTOR PRINCIPAL
    // -----------------------------
    var engine = new MonitorEngine(config, config.DiscordWebhook);

    Logger.Log("MonitorEngine starting loop...");

    await engine.Run();
}
catch (Exception ex)
{
    var msg = $"FATAL ERROR → {ex.Message}";
    Console.WriteLine(msg);
    Logger.Log(msg);
}
finally
{
    Logger.Log("=== NOC Monitor stopped ===");
}