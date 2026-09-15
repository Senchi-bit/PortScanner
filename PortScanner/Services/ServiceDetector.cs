namespace PortScanner.Services;

public sealed class ServiceDetector
{
    private static readonly IReadOnlyDictionary<int, string> Services = new Dictionary<int, string>
    {
        [20] = "FTP-data",
        [21] = "FTP",
        [22] = "SSH",
        [23] = "Telnet",
        [25] = "SMTP",
        [53] = "DNS",
        [80] = "HTTP",
        [110] = "POP3",
        [111] = "rpcbind",
        [123] = "NTP",
        [135] = "MSRPC",
        [139] = "NetBIOS",
        [143] = "IMAP",
        [161] = "SNMP",
        [389] = "LDAP",
        [443] = "HTTPS",
        [445] = "SMB",
        [465] = "SMTPS",
        [587] = "SMTP submission",
        [636] = "LDAPS",
        [993] = "IMAPS",
        [995] = "POP3S",
        [1433] = "MS SQL Server",
        [1521] = "Oracle",
        [2049] = "NFS",
        [2375] = "Docker",
        [2376] = "Docker TLS",
        [3306] = "MySQL",
        [3389] = "RDP",
        [5432] = "PostgreSQL",
        [5672] = "AMQP",
        [5900] = "VNC",
        [6379] = "Redis",
        [6443] = "Kubernetes API",
        [8080] = "HTTP-alt",
        [8443] = "HTTPS-alt",
        [9200] = "Elasticsearch",
        [11211] = "Memcached"
    };


    public static string GetServiceName(int port)
    {
        return Services.GetValueOrDefault(port, "unknown");
    }
}