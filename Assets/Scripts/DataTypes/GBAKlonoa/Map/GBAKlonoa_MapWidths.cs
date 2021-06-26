using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_MapWidths : BinarySerializable
    {
        public ushort[] Widths { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Widths = s.SerializeArray<ushort>(Widths, 3, name: nameof(Widths));
        }
    }
}