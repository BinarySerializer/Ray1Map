using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Level3D_Objects : BinarySerializable
    {
        public int ObjectsCount { get; set; }
        public GBAIsometric_Ice_Level3D_Object[] Objects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectsCount = s.Serialize<int>(ObjectsCount, name: nameof(ObjectsCount));
            Objects = s.SerializeObjectArray<GBAIsometric_Ice_Level3D_Object>(Objects, ObjectsCount, name: nameof(Objects));
        }
    }
}