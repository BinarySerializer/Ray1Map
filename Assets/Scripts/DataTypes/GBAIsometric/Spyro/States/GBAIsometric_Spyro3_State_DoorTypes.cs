using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_Spyro3_State_DoorTypes : BinarySerializable
    {
        public ushort ObjectType { get; set; }
        public ushort LevelID { get; set; }
        public ushort GraphicsStateID1 { get; set; }
        public ushort Ushort_06 { get; set; }
        public ushort GraphicsStateID2 { get; set; }
        public byte[] UnkBytes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<ushort>(ObjectType, name: nameof(ObjectType));
            LevelID = s.Serialize<ushort>(LevelID, name: nameof(LevelID));
            GraphicsStateID1 = s.Serialize<ushort>(GraphicsStateID1, name: nameof(GraphicsStateID1));
            Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
            GraphicsStateID2 = s.Serialize<ushort>(GraphicsStateID2, name: nameof(GraphicsStateID2));
            UnkBytes = s.SerializeArray<byte>(UnkBytes, 6, name: nameof(UnkBytes));
        }
    }
}