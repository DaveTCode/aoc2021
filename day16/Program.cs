namespace aoc2021day16;

public static class Program
{
    public static void Main(string[] args)
    {
        var inputHexString = Decoder.DecodeHexString(File.ReadAllText("input.txt"));
        var packet = Decoder.ParsePacket(inputHexString);
        Console.WriteLine($"{packet.VersionSum()}");
        Console.WriteLine($"{packet.PacketValue()}");
    }
}

