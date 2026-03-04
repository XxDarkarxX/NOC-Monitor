using System.Net.NetworkInformation;
using NocMonitor.Core;   // ← IMPORTANTE para Logger

namespace NocMonitor.Services;

public class PingService
{
    public async Task<bool> Check(string ip)
    {
        using var ping = new Ping();

        try
        {
            var reply = await ping.SendPingAsync(ip, 1000);

            bool ok = reply.Status == IPStatus.Success;

            Logger.Log($"Ping {(ok ? "OK" : "FAIL")} → {ip}");

            return ok;
        }
        catch (Exception ex)
        {
            Logger.Log($"Ping ERROR → {ip} | {ex.Message}");
            return false;
        }
    }
}