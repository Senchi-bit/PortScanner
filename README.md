# PortScanner

TCP port scanner.

Usage:

```bash
PortScanner <host> [options]
```

Options:

| Option | Description                    |
|--------|--------------------------------|
| `--ports <range>` | Port range (диапазон портов)   |
| `--timeout <ms>` | Connection timeout (тайм-аут соединения) |
| `--threads <n>` | Maximum concurrent connections (максимальное количество одновременных подключений) |
| `--json` | Output JSON (результаты в json) |
| `--help` | Show help  (документация)      |

Examples:

```bash
PortScanner 127.0.0.1

PortScanner 192.168.1.10 --ports 1-1024

PortScanner 192.168.1.10 --ports 1-65535 --timeout 500 --threads 300

PortScanner 192.168.1.10 --ports 22,80,443,3306,5432

PortScanner 192.168.1.10 --ports 1-1024 --json
```

Or


```bash
dotnet run -- 192.168.1.10 --ports 1-1024
```

Requirements:

- .NET 10