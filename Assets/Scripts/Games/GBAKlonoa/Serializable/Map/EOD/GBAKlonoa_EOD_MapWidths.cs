using BinarySerializer;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_EOD_MapWidths : BinarySerializable
    {
        public ushort[] Widths { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Widths = s.SerializeArray<ushort>(Widths, 3, name: nameof(Widths));
        }
    }
}