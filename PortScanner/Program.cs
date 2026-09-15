using System.Text;
using PortScanner.Scanner;
using PortScanner.Services;

namespace PortScanner;

class Program
{
    private static async Task Main(string[] args)
    {
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };
        
        if (args.Length == 0)
        {
            PrintUsage();
            return;
        }

        string target = args[0];

        var options = new ScanOptions();

        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--ports" when i + 1 < args.Length:
                    options.Ports = PortRangeParser.Parse(args[++i]);
                    break;

                case "--timeout" when i + 1 < args.Length:
                    if (int.TryParse(args[++i], out int timeout))
                    {
                        options.TimeoutMs = Math.Clamp(timeout, 50, 30_000);
                    }
                    break;

                case "--threads" when i + 1 < args.Length:
                    if (int.TryParse(args[++i], out int threads))
                    {
                        options.MaxConcurrency = Math.Clamp(threads, 1, 1_000);
                    }
                    break;

                case "--json":
                    options.Json = true;
                    break;

                case "--help":
                case "-h":
                    PrintUsage();
                    return;

                default:
                    Console.Error.WriteLine($"Неизвестный аргумент: {args[i]}");
                    PrintUsage();
                    return;
            }
        }

        try
        {
            var scanner = new TcpPortScanner();
            var serviceDetector = new ServiceDetector();

            Console.WriteLine($"Целевой IP: {target}");
            Console.WriteLine(
                $"Порты: {options.Ports.Count} | " +
                $"Тайм-аут: {options.TimeoutMs} ms | " +
                $"Одновременно: {options.MaxConcurrency}");

            Console.WriteLine();

            var started = DateTime.UtcNow;

            var results = await scanner.ScanAsync(target, options, cts.Token);

            if (options.Json)
            {
                var json = System.Text.Json.JsonSerializer.Serialize(
                    results,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                await File.WriteAllTextAsync("scan_results.json", json, cts.Token);
                Console.WriteLine("Сохранено в json");
                return;
            }

            Console.WriteLine("ПОРТ     СОСТОЯНИЕ    СЕРВИС");
            Console.WriteLine("-------  ---------  ---------------");

            foreach (var result in results
                .Where(r => r.IsOpen)
                .OrderBy(r => r.Port))
            {
                string service = ServiceDetector.GetServiceName(result.Port);

                Console.WriteLine(
                    $"{result.Port,-8} {"OPEN",-10} {service,-16}");
            }

            int openPorts = results.Count(r => r.IsOpen);

            var elapsed = DateTime.UtcNow - started;

            Console.WriteLine();
            Console.WriteLine($"Открытых портов: {openPorts}");
            Console.WriteLine($"Сканирование закончено за {elapsed.TotalSeconds:F2}с");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Ошибка: {ex.Message}");
            Environment.ExitCode = 1;
        }

        return;


        static void PrintUsage()
        {
            Console.WriteLine("""
            PortScanner - TCP port scanner

            Usage:

              PortScanner <host> [options]

            Options:

              --ports <range>     Port range (диапазон портов)
              --timeout <ms>      Connection timeout (тайм-аут соединения)
              --threads <n>       Maximum concurrent connections (максимальное количество одновременных подключений)
              --json              Output JSON (результаты в json)
              --help              Show help (документация)

            Examples:

              PortScanner 127.0.0.1

              PortScanner 192.168.1.10 --ports 1-1024

              PortScanner 192.168.1.10 --ports 1-65535 --timeout 500 --threads 300

              PortScanner 192.168.1.10 --ports 22,80,443,3306,5432

              PortScanner 192.168.1.10 --ports 1-1024 --json
            """);
        }
    }
}



