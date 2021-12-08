static void ProcessFile(string path)
{
    var lines = File.ReadAllLines(path);
    var horizontalPositions = lines[0].Split(",").Select(s => int.Parse(s)).ToList();
    horizontalPositions.Sort();

    var total = horizontalPositions.Sum();
    var average = Math.Floor(horizontalPositions.Average());
    var median = horizontalPositions[(horizontalPositions.Count + 1) / 2];

    var distanceFromMedian = horizontalPositions.Select(i => Math.Abs(i - median)).Sum();
    var distanceFromAverage = horizontalPositions.Select(i => Math.Abs(i - average) / 2 * (Math.Abs(i - average) + 1)).Sum();

    Console.WriteLine("Part 1: {0}", distanceFromMedian);
    Console.WriteLine("Part 2: {0}", distanceFromAverage);
}

ProcessFile("test.txt");
ProcessFile("input.txt");
