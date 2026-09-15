namespace PortScanner.Scanner;

public static class PortRangeParser
{
    public static IReadOnlyList<int> Parse(string input)
    {
        var ports = new SortedSet<int>();

        var parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            if (part.Contains('-'))
            {
                var range = part.Split(
                    '-',
                    2,
                    StringSplitOptions.TrimEntries);

                if (!int.TryParse(range[0], out int start) || !int.TryParse(range[1], out int end))
                {
                    throw new ArgumentException(
                        $"Неверный диапазон портов: {part}");
                }

                if (start > end)
                {
                    (start, end) = (end, start);
                }

                Validate(start);
                Validate(end);

                for (int port = start; port <= end; port++)
                {
                    ports.Add(port);
                }
            }
            else
            {
                if (!int.TryParse(part, out int port))
                {
                    throw new ArgumentException(
                        $"Invalid port: {part}");
                }

                Validate(port);

                ports.Add(port);
            }
        }

        return ports.ToArray();
    }

    private static void Validate(int port)
    {
        if (port is < 1 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(port), $"Порт должен быть между 1 и 65535: {port}");
        }
    }
}