using System.Text.RegularExpressions;

namespace aoc2021day19;

internal readonly record struct Position(int X, int Y, int Z)
{
    public static Position operator +(Position a, Position b) =>
         new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Position operator -(Position a, Position b) =>
         new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public int Distance(Position a) =>
        Math.Abs(X - a.X) + Math.Abs(Y - a.Y) + Math.Abs(Z - a.Z);
}

internal class Scanner
{
    internal int Id { get; }
    internal HashSet<Position> Beacons { get; } = new();

    internal Scanner(int id)
    {
        Id = id;
    }

    public override string ToString()
    {
        return $"{Id}";
    }
}

internal class Program
{
    internal readonly static Regex ScannerLineRegex = new(@"--- scanner (\d+) ---");

    internal static List<Scanner> ParseScanners(string path)
    {
        var lines = File.ReadAllLines(path);

        var scanners = new List<Scanner>();
        Scanner? scanner = null;
        foreach (var line in lines)
        {
            if (ScannerLineRegex.IsMatch(line))
            {
                var scannerId = int.Parse(ScannerLineRegex.Match(line).Groups[1].Value);
                if (scanner != null) scanners.Add(scanner);
                scanner = new Scanner(scannerId);
            }
            else if (line.Contains(','))
            {
                var parts = line.Split(',');
                var position = new Position(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
                scanner?.Beacons.Add(position);
            }
        }

        if (scanner != null) scanners.Add(scanner);

        return scanners;
    }

    internal static IEnumerable<HashSet<Position>> PossibleOrientations(HashSet<Position> scanner)
    {
        for (var i = 0; i < 24; i++)
        {
            yield return scanner.Select(position =>
            {
                var (x, y, z) = position;
                return i switch
                {
                    0 => new Position(x, y, z),
                    1 => new Position(-y, x, z),
                    2 => new Position(-x, -y, z),
                    3 => new Position(y, -x, z),

                    4 => new Position(-x, y, -z),
                    5 => new Position(y, x, -z),
                    6 => new Position(x, -y, -z),
                    7 => new Position(-y, -x, -z),

                    8 => new Position(-z, y, x),
                    9 => new Position(-z, x, -y),
                    10 => new Position(-z, -y, -x),
                    11 => new Position(-z, -x, y),

                    12 => new Position(z, y, -x),
                    13 => new Position(z, x, y),
                    14 => new Position(z, -y, x),
                    15 => new Position(z, -x, -y),

                    16 => new Position(x, -z, y),
                    17 => new Position(-y, -z, x),
                    18 => new Position(-x, -z, -y),
                    19 => new Position(y, -z, -x),

                    20 => new Position(x, z, -y),
                    21 => new Position(-y, z, -x),
                    22 => new Position(-x, z, y),
                    23 => new Position(y, z, x),
                    _ => throw new Exception()
                };
            }).ToHashSet();
        }
    }

    internal static HashSet<Position> TransformBeacons(HashSet<Position> beacons, Position transform) =>
        beacons.Select(b => b + transform).ToHashSet();

    internal static IEnumerable<(HashSet<Position>, Position)> ValidTransforms(HashSet<Position> a, HashSet<Position> b)
    {
        var uniqueTransforms = a.SelectMany(x => b, (x, y) => x - y).ToHashSet();

        foreach (var transform in uniqueTransforms)
        {
            yield return (b.Select(y => y + transform).ToHashSet(), transform);
        }
    }

    internal static (HashSet<Position>?, Position?) AlignScanners(HashSet<Position> a, HashSet<Position> b)
    {
        foreach (var option in PossibleOrientations(b))
        {
            foreach (var (beacons, transform) in ValidTransforms(a, option))
            {
                if (a.Intersect(beacons).Count() >= 12)
                {
                    return (a.Union(beacons).ToHashSet(), transform);
                }
            }
        }

        return (null, null);
    }

    internal static void Solve(List<Scanner> scanners)
    {
        var positions = new Dictionary<int, Position>
        {
            { 0, new Position(0, 0, 0) },
        };
        var scanner0 = scanners[0].Beacons;
        var scannerQueue = new Queue<Scanner>();

        foreach (var scanner in scanners.Skip(1))
        {
            scannerQueue.Enqueue(scanner);
        }

        while (scannerQueue.Any())
        {
            var scanner = scannerQueue.Dequeue();

            var (result, transform) = AlignScanners(scanner0, scanner.Beacons);
            if (result == null || transform == null)
            {
                Console.WriteLine($"Requeueing {scanner}");
                scannerQueue.Enqueue(scanner);
            }
            else
            {
                positions[scanner.Id] = transform.Value;
                scanner0 = result;
            }
        }

        

        Console.WriteLine($"Part 1: {scanner0.Count}");
        foreach (var (s, t) in positions)
        {
            Console.WriteLine($"{s},{t}");
        }

        Console.WriteLine($"Part 2: {positions.Values.SelectMany(x => positions.Values, (x, y) => x.Distance(y)).OrderByDescending(x => x).First()}");
    }

    internal static void Main(string[] args)
    {
        //var testData = ParseScanners("test.txt");
        var inputData = ParseScanners("input.txt");

        //Solve(testData);
        Solve(inputData);
    }
}