using Ray1Map.GBA;

namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class AnimationFrame : BinarySerializable
    {
        public uint ChannelsCount { get; set; }
        public AnimationChannel[] Channels { get; set; }
        
        public uint AffineMatricesCount { get; set; }
        public GBA_AffineMatrix[] AffineMatrices { get; set; }

        public uint UnknownCount { get; set; }
        public byte[] Unknown { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ChannelsCount = s.Serialize<uint>(ChannelsCount, name: nameof(ChannelsCount));
            Channels = s.SerializeObjectArray<AnimationChannel>(Channels, ChannelsCount, name: nameof(Channels));

            AffineMatricesCount = s.Serialize<uint>(AffineMatricesCount, name: nameof(AffineMatricesCount));
            AffineMatrices = s.SerializeObjectArray<GBA_AffineMatrix>(AffineMatrices, AffineMatricesCount, name: nameof(AffineMatrices));

            UnknownCount = s.Serialize<uint>(UnknownCount, name: nameof(UnknownCount));
            Unknown = s.SerializeArray<byte>(Unknown, UnknownCount, name: nameof(Unknown));
        }
    }
}