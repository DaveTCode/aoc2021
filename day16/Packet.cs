using System.Collections.ObjectModel;

namespace aoc2021day16
{
    public abstract record Packet
    {
        public byte Version {  get; }

        public byte Type { get; }

        public int BitsParsed { get; }

        protected Packet(byte version, byte type, int bitsParsed)
        {
            Version = version;
            Type = type;
            BitsParsed = bitsParsed;
        }

        public abstract int VersionSum();

        public abstract long PacketValue();
    }

    public record LiteralPacket : Packet
    {
        public long Literal { get; }

        public LiteralPacket(byte version, byte type, int bitsParsed, long literal) : base(version, type, bitsParsed)
        {
            Literal = literal;
        }

        public override int VersionSum() => Version;

        public override long PacketValue() => Literal;
    }

    public record OperatorPacket : Packet
    {
        public ReadOnlyCollection<Packet> Packets { get; }

        public OperatorPacket(byte version, byte type, int bitsParsed, List<Packet> subPackets) : base(version, type, bitsParsed)
        {
            Packets = subPackets.AsReadOnly();
        }

        public override int VersionSum() => Version + Packets.Select(p => p.VersionSum()).Sum();

        public override long PacketValue() => Type switch
        {
            0 => Packets.Select(p => p.PacketValue()).Sum(),
            1 => Packets.Select(p => p.PacketValue()).Aggregate(1L, (a, b) => a * b),
            2 => Packets.Select(p => p.PacketValue()).Min(),
            3 => Packets.Select(p => p.PacketValue()).Max(),
            5 => Packets[0].PacketValue() > Packets[1].PacketValue() ? 1 : 0,
            6 => Packets[0].PacketValue() < Packets[1].PacketValue() ? 1 : 0,
            7 => Packets[0].PacketValue() == Packets[1].PacketValue() ? 1 : 0,
            _ => throw new NotImplementedException($"Packet type {Type} is not valid"),
        };
    }
}