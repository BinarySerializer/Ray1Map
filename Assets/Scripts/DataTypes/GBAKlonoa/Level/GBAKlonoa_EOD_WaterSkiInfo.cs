using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_EOD_WaterSkiInfo : BinarySerializable
    {
        public byte[] Bytes_00 { get; set; }
        public Pointer Pointer_04 { get; set; }
        public byte World { get; set; }
        public byte Level { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Bytes_00 = s.SerializeArray<byte>(Bytes_00, 4, name: nameof(Bytes_00));
            Pointer_04 = s.SerializePointer(Pointer_04, allowInvalid: true, name: nameof(Pointer_04));
            World = s.Serialize<byte>(World, name: nameof(World));
            Level = s.Serialize<byte>(Level, name: nameof(Level));
            s.SerializePadding(2, logIfNotNull: true);
        }
    }
}