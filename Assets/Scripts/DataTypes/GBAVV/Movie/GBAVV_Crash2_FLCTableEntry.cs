using BinarySerializer;
using BinarySerializer.Image;

namespace R1Engine
{
    public class GBAVV_Crash2_FLCTableEntry : BinarySerializable
    {
        public Pointer FLCPointer { get; set; }
        public bool Loop { get; set; }

        public FLIC FLC { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FLCPointer = s.SerializePointer(FLCPointer, name: nameof(FLCPointer));
            Loop = s.Serialize<bool>(Loop, name: nameof(Loop));
            s.Align();

            s.DoAt(FLCPointer, () => s.DoEncoded(new GBA_LZSSEncoder(), () => FLC = s.SerializeObject<FLIC>(FLC, name: nameof(FLC)), allowLocalPointers: true));
        }
    }
}