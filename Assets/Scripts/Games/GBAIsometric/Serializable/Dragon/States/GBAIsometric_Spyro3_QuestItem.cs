using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro3_QuestItem : BinarySerializable
    {
        public ushort ObjectType { get; set; }
        public QuestItemType ItemType { get; set; }
        public byte AnimFrameIndex { get; set; }
        public byte Byte_04 { get; set; }
        public byte Byte_05 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<ushort>(ObjectType, name: nameof(ObjectType));
            ItemType = s.Serialize<QuestItemType>(ItemType, name: nameof(ItemType));
            AnimFrameIndex = s.Serialize<byte>(AnimFrameIndex, name: nameof(AnimFrameIndex));
            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
            s.Serialize<ushort>(default, name: "Padding");
        }

        public enum QuestItemType : byte
        {
            Unk0 = 0,
            Normal = 1,
            RedChest = 2,
            GreenChest = 3,
            PinkChest = 4,
            YellowChest = 5
        }
    }
}