using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_RHR_CutsceneDialog : BinarySerializable
    {
        public ushort PortraitIndex { get; set; }
        public GBAIsometric_LocIndex LocIndex { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            PortraitIndex = s.Serialize<ushort>(PortraitIndex, name: nameof(PortraitIndex));
            LocIndex = s.SerializeObject<GBAIsometric_LocIndex>(LocIndex, name: nameof(LocIndex));
            s.Serialize<ushort>(default, name: "Padding");
        }
    }
}