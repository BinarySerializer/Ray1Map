using System.Linq;
using System.Text;

namespace R1Engine
{
    public class GBC_PalmOS_Vignette : GBC_Block {
        public uint BlockSize { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint BitDepth { get; set; }
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
            Width = s.Serialize<uint>(Width, name: nameof(Width));
            Height = s.Serialize<uint>(Height, name: nameof(Height));
            BitDepth = s.Serialize<uint>(BitDepth, name: nameof(BitDepth));
            Data = s.SerializeArray<byte>(Data, Width * Height / (BitDepth == 8 ? 1 : 2), name: nameof(Data));
        }
    }
}