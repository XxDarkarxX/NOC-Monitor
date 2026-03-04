using System.Net.Http.Json;
using NocMonitor.Models;
using NocMonitor.Core;   // ← IMPORTANTE

namespace NocMonitor.Alerts;

public class DiscordAlert
{
    private string webhook;

    public DiscordAlert(string url)
    {
        webhook = url;
    }

    public async Task Send(HostState state, string hostName, string ip)
    {
        var client = new HttpClient();

        // NO enviar si está en pausa
        if (state.PauseUntil != null && DateTime.Now < state.PauseUntil)
            return;

        for (int i = state.AlertCount; i < 3; i++)
        {
            var payload = new
            {
                content = $"@everyone ALERT: {hostName} [{ip}] is DOWN"
            };

            await client.PostAsJsonAsync(webhook, payload);
            Logger.Log($"ALERT SENT ({i+1}/3) → {hostName} [{ip}]");

            state.AlertCount++;
            state.LastAlert = DateTime.Now;

            if (state.AlertCount < 3)
                await Task.Delay(30000); // 30s entre alertas
        }

        // Después de las 3 alertas → poner pausa 5 min
        state.PauseUntil = DateTime.Now.AddMinutes(5);
        Logger.Log($"COOLDOWN 5 MIN → {hostName} [{ip}]");

        state.AlertCount = 0;
    }
}