using BinarySerializer;

namespace R1Engine
{
    public class GBC_PalmOS_Vignette : BinarySerializable 
    {
        public uint BlockSize { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint BPP { get; set; }
        public byte[] Data { get; set; }
        public BGR565Color[] DataPPC { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
            Width = s.Serialize<uint>(Width, name: nameof(Width));
            Height = s.Serialize<uint>(Height, name: nameof(Height));
            BPP = s.Serialize<uint>(BPP, name: nameof(BPP));
            if (BPP == 16) {
                DataPPC = s.SerializeObjectArray<BGR565Color>(DataPPC, Width * Height, name: nameof(DataPPC));
            } else {
                Data = s.SerializeArray<byte>(Data, Width * Height * BPP / 8, name: nameof(Data));
            }
        }
    }
}