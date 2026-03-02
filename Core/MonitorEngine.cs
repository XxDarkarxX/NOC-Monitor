using NocMonitor.Models;
using NocMonitor.Services;
using NocMonitor.Alerts;

namespace NocMonitor.Core;

public class MonitorEngine
{
    private AppConfig config;
    private PingService pingService = new();
    private StateManager stateManager = new();
    private NetworkHealthService networkHealth = new();

    private DiscordAlert discord;

    public MonitorEngine(AppConfig cfg, string webhook)
    {
        config = cfg;
        discord = new DiscordAlert(webhook);
    }

    public async Task Run()
    {
        Logger.Log("Monitor Engine Started.");

        while (true)
        {
            try
            {
                await LoopOnce();
            }
            catch (Exception ex)
            {
                Logger.Log($"CRITICAL ENGINE ERROR: {ex.Message}");
                await Task.Delay(5000); // Previene loops rápidos en caso de crash
            }
        }
    }

    private async Task LoopOnce()
    {
        bool networkUp = await networkHealth.IsNetworkUp();

        if (!networkUp)
        {
            Logger.Log("NETWORK OUTAGE DETECTED");
            await Task.Delay(config.IntervalSeconds * 1000);
            return;
        }

        var tasks = config.Hosts.Select(async host =>
        {
            bool ok = await pingService.Check(host.Ip);

            if (stateManager.Update(host.Ip, ok, config.FailThreshold, out string newState))
            {
                var state = stateManager.Get(host.Ip);

                // DOWN transition → alert + cooldown logic
                if (newState == "DOWN")
                {
                    await discord.Send(state, host.Name, host.Ip);
                    Logger.Log($"{host.Name} [{host.Ip}] is DOWN");
                }

                // RECOVERY transition
                if (newState == "RECOVERY")
                {
                    Logger.Log($"{host.Name} [{host.Ip}] RECOVERED");
                }

                // Optional Log for UP (first detection)
                if (newState == "UP")
                {
                    Logger.Log($"{host.Name} [{host.Ip}] is UP");
                }
            }
        });

        await Task.WhenAll(tasks);

        await Task.Delay(config.IntervalSeconds * 1000);
    }
}