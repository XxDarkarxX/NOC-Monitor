using NocMonitor.Services;

namespace NocMonitor.Core;

public class NetworkHealthService
{
    private PingService ping = new();

    private string[] healthTargets = new[]
    {
        "1.1.1.1",
        "8.8.8.8"
    };

    public async Task<bool> IsNetworkUp()
    {
        var tasks = healthTargets.Select(ip => ping.Check(ip));

        var results = await Task.WhenAll(tasks);

        return results.Any(r => r == true);
    }
}
