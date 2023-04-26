using BinarySerializer;

namespace Ray1Map.Jade
{
    public class GAO_ModifierPreDepthPass : MDF_Modifier {
        public uint Version { get; set; }
        public short Group { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Version = s.Serialize<uint>(Version, name: nameof(Version));
            Group = s.Serialize<short>(Group, name: nameof(Group));
        }
    }
}
