
using System.Linq;

public static class Program
{
    static List<(string[], string[])> ProcessFile(string path)
    {
        var results = new List<(string[], string[])>();
        var lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            var inputOutput = line.Split(" | ");
            var input = inputOutput[0].Split(" ");
            var output = inputOutput[1].Split(" ");
            results.Add((input, output));
        }

        return results;
    }

    static int Part1(List<(string[], string[])> data) => data
        .Select(d => d.Item2)
        .SelectMany(outputs => outputs.Where(output => 
            output.Length == DisplayNumber.One.Segments().Length || 
            output.Length == DisplayNumber.Four.Segments().Length || 
            output.Length == DisplayNumber.Seven.Segments().Length || 
            output.Length == DisplayNumber.Eight.Segments().Length))
        .Count();

    static int Part2(List<(string[], string[])> data)
    {
        var chars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' };
        int SolveRow(HashSet<char>[] inputs, HashSet<char>[] outputs)
        {
            // Basic mappings which have only one option
            var numberMappings = new Dictionary<DisplayNumber, HashSet<char>>
            { 
                { DisplayNumber.One, inputs.First(i => i.Count == DisplayNumber.One.Segments().Length) },
                { DisplayNumber.Four, inputs.First(i => i.Count == DisplayNumber.Four.Segments().Length) },
                { DisplayNumber.Seven, inputs.First(i => i.Count == DisplayNumber.Seven.Segments().Length) },
                { DisplayNumber.Eight, inputs.First(i => i.Count == DisplayNumber.Eight.Segments().Length) },
            };

            // Mappings for 2,3,5 which have 5 segments each
            var optionsWith5Segments = inputs.Where(i => i.Count == 5).ToList();
            // 3 is the only one of those which overlaps entirely with 1
            numberMappings[DisplayNumber.Three] = optionsWith5Segments
                .First(o => numberMappings[DisplayNumber.One].All(c => o.Contains(c)));
            optionsWith5Segments.Remove(numberMappings[DisplayNumber.Three]);
            // 5 has 3 overlaps with 4 whereas 2 only has 2
            numberMappings[DisplayNumber.Five] = optionsWith5Segments
                .First(o => numberMappings[DisplayNumber.Four].Count(c => o.Contains(c)) == 3);
            numberMappings[DisplayNumber.Two] = optionsWith5Segments
                .First(o => numberMappings[DisplayNumber.Four].Count(c => o.Contains(c)) == 2);

            // Mappings for 0, 6, 9 all have 6 segments
            var optionsWith6Segments = inputs.Where(i => i.Count == 6).ToList();
            // 9 is the only one of the three that overlaps entirely with 4
            numberMappings[DisplayNumber.Nine] = optionsWith6Segments
                .First(o => numberMappings[DisplayNumber.Four].All(c => o.Contains(c)));
            // Of the remaining two only 0 overlaps entirely with 1
            optionsWith6Segments.Remove(numberMappings[DisplayNumber.Nine]);
            numberMappings[DisplayNumber.Zero] = optionsWith6Segments
                .First(o => numberMappings[DisplayNumber.One].All(c => o.Contains(c)));
            optionsWith6Segments.Remove(numberMappings[DisplayNumber.Zero]);
            // 6 is then the only remaining option
            numberMappings[DisplayNumber.Six] = optionsWith6Segments[0];

            var outputMapped = outputs.Select(o => numberMappings.First(kv => kv.Value.SetEquals(o))).Select(kv => (int)kv.Key);
            return int.Parse(string.Join(string.Empty, outputMapped));
        }

        return data.Select(d => SolveRow(d.Item1.Select(s => s.ToCharArray().ToHashSet()).ToArray(), d.Item2.Select(s => s.ToHashSet()).ToArray())).Sum();
    }

    public static int Main()
    {
        var testData = ProcessFile("test.txt");
        var inputData = ProcessFile("input.txt");

        Console.WriteLine(Part1(testData));
        Console.WriteLine(Part1(inputData));

        Console.WriteLine(Part2(testData));
        Console.WriteLine(Part2(inputData));

        return 0;
    }
}

public enum DisplayNumber
{
    Zero,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
}

public static class DisplayNumberExtensions
{
    public static string Segments(this DisplayNumber displayNumber) => displayNumber switch
    {
        DisplayNumber.Zero => "abcefg",
        DisplayNumber.One => "cf",
        DisplayNumber.Two => "acdeg",
        DisplayNumber.Three => "acdfg",
        DisplayNumber.Four => "bcdf",
        DisplayNumber.Five => "abdfg",
        DisplayNumber.Six => "abdefg",
        DisplayNumber.Seven => "acf",
        DisplayNumber.Eight => "abcdefg",
        DisplayNumber.Nine => "abcdfg",
        _ => throw new ArgumentOutOfRangeException(nameof(displayNumber))
    };
}