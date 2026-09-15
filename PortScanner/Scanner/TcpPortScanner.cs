using System.Diagnostics;
using System.Net.Sockets;

namespace PortScanner.Scanner;

public sealed class TcpPortScanner
{
    public async Task<IReadOnlyList<PortScanResult>> ScanAsync(string host, ScanOptions options,
        CancellationToken cancellationToken = default)
    {
        var results = new PortScanResult[options.Ports.Count];

        await Parallel.ForEachAsync(Enumerable.Range(0, options.Ports.Count),

            new ParallelOptions
            { 
                MaxDegreeOfParallelism = options.MaxConcurrency,

                CancellationToken = cancellationToken
            },

            async (index, ct) =>
            {
                results[index] = await ScanPortAsync(host, options.Ports[index], options.TimeoutMs, ct);
            });

        return results;
    }


    private static async Task<PortScanResult> ScanPortAsync(string host, int port, int timeoutMs,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        using var client = new TcpClient();

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        timeoutCts.CancelAfter(timeoutMs);

        try
        {
            await client.ConnectAsync(host, port, timeoutCts.Token);

            return new PortScanResult(port, true, stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new PortScanResult(port, false, stopwatch.ElapsedMilliseconds, "timeout");
        }
        catch (SocketException ex)
        {
            return new PortScanResult(port, false, stopwatch.ElapsedMilliseconds, ex.SocketErrorCode.ToString());
        }
        catch (Exception ex)
        {
            return new PortScanResult(port, false, stopwatch.ElapsedMilliseconds, ex.GetType().Name);
        }
    }
}