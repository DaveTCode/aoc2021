namespace aoc2021day16;

public static class Decoder
{
    public static string DecodeHexString(string hex) => string.Join("", hex.ToCharArray().Select(c => c switch
        {
            '0' => "0000",
            '1' => "0001",
            '2' => "0010",
            '3' => "0011",
            '4' => "0100",
            '5' => "0101",
            '6' => "0110",
            '7' => "0111",
            '8' => "1000",
            '9' => "1001",
            'A' => "1010",
            'B' => "1011",
            'C' => "1100",
            'D' => "1101",
            'E' => "1110",
            'F' => "1111",
            _ => throw new NotImplementedException(),
        }));

    public static Packet ParsePacket(string binary)
    {
        var version = (byte)Convert.ToInt32(binary[..3], 2);
        var packetType = (byte)Convert.ToInt32(binary[3..6], 2);
        var bitsParsed = 6;

        switch (packetType)
        {
            case 4:
                var (literalValue, literalBitsParsed) = ParseLiteralNumber(binary[6..]);
                return new LiteralPacket(version, packetType, bitsParsed + literalBitsParsed, literalValue);
            default:
                var lengthTypeId = binary[6];
                bitsParsed++;
                var subPackets = new List<Packet>();
                var subbitsParsed = 0;
                switch (lengthTypeId)
                {
                    case '0':
                        var totalLength = Convert.ToInt32(binary[7..22], 2);
                        bitsParsed += 15;

                        while (subbitsParsed < totalLength)
                        {
                            var subPacket = ParsePacket(binary[(22 + subbitsParsed)..]);
                            subPackets.Add(subPacket);
                            subbitsParsed += subPacket.BitsParsed;
                        }
                        break;
                    case '1':
                        var numSubPackets = Convert.ToInt32(binary[7..18], 2);
                        bitsParsed += 11;

                        for (var ii = 0; ii < numSubPackets; ii++)
                        {
                            var subPacket = ParsePacket(binary[(18 + subbitsParsed)..]);
                            subPackets.Add(subPacket);
                            subbitsParsed += subPacket.BitsParsed;
                        }
                        break;
                }
                bitsParsed += subbitsParsed;
                return new OperatorPacket(version, packetType, bitsParsed, subPackets);
        }
    }

    private static (long, int) ParseLiteralNumber(string binary)
    {
        var chunks = binary
            .Chunk(5)
            .TakeWhile(c => c[0] != '0')
            .Append(binary.Chunk(5).First(c => c[0] == '0'))
            .ToList();
        var str = new string(chunks
            .SelectMany(chrs => chrs.AsSpan()[1..].ToArray()) // Remove first bit from each literal
            .ToArray());
        return (Convert.ToInt64(str, 2), chunks.SelectMany(c => c).Count());
    }
}
