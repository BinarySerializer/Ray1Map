using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_Isometric_CollisionType : BinarySerializable
    {
        public Pointer FunctionPointer_0 { get; set; }
        public Pointer FunctionPointer_1 { get; set; }
        public Pointer FunctionPointer_2 { get; set; }
        public byte[] Bytes_10 { get; set; }
        public FixedPointInt32 AdditionalHeight { get; set; }
        public byte[] Bytes_18 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FunctionPointer_0 = s.SerializePointer(FunctionPointer_0, name: nameof(FunctionPointer_0));
            FunctionPointer_1 = s.SerializePointer(FunctionPointer_1, name: nameof(FunctionPointer_1));
            FunctionPointer_2 = s.SerializePointer(FunctionPointer_2, name: nameof(FunctionPointer_2));
            Bytes_10 = s.SerializeArray<byte>(Bytes_10, 4, name: nameof(Bytes_10));
            AdditionalHeight = s.SerializeObject<FixedPointInt32>(AdditionalHeight, name: nameof(AdditionalHeight));
            Bytes_18 = s.SerializeArray<byte>(Bytes_18, 20, name: nameof(Bytes_18));
        }
    }
}