using BinarySerializer;

namespace Ray1Map.GBARRR
{
    public class GBARRR_LevelProperties : BinarySerializable
    {
        public int Int_00 { get; set; } // Set to 2 for the shooting range maps
        public int Int_04 { get; set; } // Set to 2 for sweets and 1 for title screen - some layer prio?
        public int Int_08 { get; set; }
        public int Int_0C { get; set; }
        public int Int_10 { get; set; }
        public int Int_14 { get; set; }
        public int Int_18 { get; set; }
        public int Int_1C { get; set; }
        public int Int_20 { get; set; }
        public int Int_24 { get; set; }
        public byte AlphaBlendingUnk { get; set; } // Set to 0x1F for levels with alpha blending
        public byte AlphaBlendingValue { get; set; }
        public byte Byte_2A { get; set; }
        public byte Byte_2B { get; set; }
        
        public override void SerializeImpl(SerializerObject s)
        {
            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
            Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));
            Int_18 = s.Serialize<int>(Int_18, name: nameof(Int_18));
            Int_1C = s.Serialize<int>(Int_1C, name: nameof(Int_1C));
            Int_20 = s.Serialize<int>(Int_20, name: nameof(Int_20));
            Int_24 = s.Serialize<int>(Int_24, name: nameof(Int_24));
            AlphaBlendingUnk = s.Serialize<byte>(AlphaBlendingUnk, name: nameof(AlphaBlendingUnk));
            AlphaBlendingValue = s.Serialize<byte>(AlphaBlendingValue, name: nameof(AlphaBlendingValue));
            Byte_2A = s.Serialize<byte>(Byte_2A, name: nameof(Byte_2A));
            Byte_2B = s.Serialize<byte>(Byte_2B, name: nameof(Byte_2B));
        }
    }
}