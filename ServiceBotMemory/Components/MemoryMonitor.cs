using BotAletMemory.Components;
using System.Diagnostics;

public class MemoryMonitor
{
    private readonly Config _config;

    public MemoryMonitor(Config config)
    {
        _config = config;
    }

    public float GetMemoryUsage()
    {
        using (PerformanceCounter pc = new PerformanceCounter("Memory", "Available MBytes"))
        {
            float avMm = pc.NextValue();
            return _config.MmorySettings.AlertThreshold - avMm;
        }
    }
}