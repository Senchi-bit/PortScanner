namespace PortScanner.Scanner;

public sealed class ScanOptions
{
    public IReadOnlyList<int> Ports { get; set; } = Enumerable.Range(1, 1024).ToArray();

    public int TimeoutMs { get; set; } = 1000;

    public int MaxConcurrency { get; set; } = 200;

    public bool Json { get; set; }
}