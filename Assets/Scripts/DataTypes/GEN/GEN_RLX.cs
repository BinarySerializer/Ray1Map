using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine
{
    public class GEN_RLX : BinarySerializable
    {
        public byte Type { get; set; }
        public byte Unknown { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
			Type = s.Serialize<byte>(Type, name: nameof(Type));
			Unknown = s.Serialize<byte>(Unknown, name: nameof(Unknown));
			Width = s.Serialize<ushort>(Width, name: nameof(Width));
			Height = s.Serialize<ushort>(Height, name: nameof(Height));

		}
    }
}