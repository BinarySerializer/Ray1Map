using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_Spyro2_LevelObjects2D : BinarySerializable
    {
        public byte[] Bytes_00 { get; set; }

        public uint DoorObjectsCount { get; set; }
        public GBAIsometric_Spyro2_Object2D[] DoorObjects { get; set; }

        public uint CharacterObjectsCount { get; set; }
        public GBAIsometric_Spyro2_Object2D[] CharacterObjects { get; set; }

        public uint CollectibleObjectsCount { get; set; }
        public GBAIsometric_Spyro2_Object2D[] CollectibleObjects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Bytes_00 = s.SerializeArray<byte>(Bytes_00, 4, name: nameof(Bytes_00));

            DoorObjectsCount = s.Serialize<uint>(DoorObjectsCount, name: nameof(DoorObjectsCount));
            DoorObjects = s.SerializeObjectArray<GBAIsometric_Spyro2_Object2D>(DoorObjects, DoorObjectsCount, onPreSerialize: x => x.Category = GBAIsometric_Spyro2_Object2D.ObjCategory.Door, name: nameof(DoorObjects));

            CharacterObjectsCount = s.Serialize<uint>(CharacterObjectsCount, name: nameof(CharacterObjectsCount));
            CharacterObjects = s.SerializeObjectArray<GBAIsometric_Spyro2_Object2D>(CharacterObjects, CharacterObjectsCount, onPreSerialize: x => x.Category = GBAIsometric_Spyro2_Object2D.ObjCategory.Character, name: nameof(CharacterObjects));

            CollectibleObjectsCount = s.Serialize<uint>(CollectibleObjectsCount, name: nameof(CollectibleObjectsCount));
            CollectibleObjects = s.SerializeObjectArray<GBAIsometric_Spyro2_Object2D>(CollectibleObjects, CollectibleObjectsCount, onPreSerialize: x => x.Category = GBAIsometric_Spyro2_Object2D.ObjCategory.Collectible, name: nameof(CollectibleObjects));
        }
    }
}