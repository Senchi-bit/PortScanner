namespace PortScanner.Scanner;

public sealed record PortScanResult(
    int Port,
    bool IsOpen,
    long ElapsedMilliseconds,
    string? Error = null);