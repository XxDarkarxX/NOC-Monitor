using NocMonitor.Models;

namespace NocMonitor.Services;

public class StateManager
{
    private readonly Dictionary<string, HostState> states = new();

    public HostState Get(string ip)
    {
        if (!states.ContainsKey(ip))
            states[ip] = new HostState();

        return states[ip];
    }

    public bool Update(string ip, bool isUp, int threshold, out string newState)
    {
        var state = Get(ip);
        newState = string.Empty;

        // Si está en cooldown → no procesar, no alertar, no contar fail
        if (state.PauseUntil != null && DateTime.Now < state.PauseUntil)
        {
            return false;
        }

        // ---------------------
        //      HOST UP
        // ---------------------
        if (isUp)
        {
            // Si estaba DOWN → recuperar
            if (state.State == "DOWN")
            {
                newState = "RECOVERY";
                state.AlertCount = 0;
                state.FailCount = 0;
                state.PauseUntil = null;
            }

            // Si estaba UNKNOWN → marcar como UP por primera vez
            if (state.State == "UNKNOWN")
            {
                newState = "UP";
            }

            state.State = "UP";
            state.FailCount = 0;
            return newState != string.Empty;
        }

        // ---------------------
        //     HOST FAIL
        // ---------------------
        state.FailCount++;

        // Si ya estaba DOWN → no repetir alerta
        if (state.State == "DOWN")
        {
            return false;
        }

        // Cuando cruza el umbral → primera transición a DOWN
        if (state.FailCount >= threshold)
        {
            state.State = "DOWN";
            newState = "DOWN";
            return true;
        }

        return false;
    }
}