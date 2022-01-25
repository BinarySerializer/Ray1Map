using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Level3D_Object : BinarySerializable
    {
        public GBAIsometric_Ice_Level3D_Object() { }

        public GBAIsometric_Ice_Level3D_Object(short objectType, GBAIsometric_Ice_Vector position)
        {
            ObjectType = objectType;
            Positions = position;
        }

        public short ObjectType { get; set; }
        public ushort Ushort_02 { get; set; } // Unused?
        public GBAIsometric_Ice_Vector Positions { get; set; }

        public static IEnumerable<GBAIsometric_Ice_Level3D_Object> GetFixedObjects(GBAIsometric_Ice_Vector startPos)
        {
            yield return new GBAIsometric_Ice_Level3D_Object(15, new GBAIsometric_Ice_Vector(0x4000, -0x4000, 0)); // HUD gem
            yield return new GBAIsometric_Ice_Level3D_Object(21, new GBAIsometric_Ice_Vector(0, -0x4000, 0)); // HUD fairy
            yield return new GBAIsometric_Ice_Level3D_Object(1, startPos); // Spyro

            // TODO: What are these? Sparx?
            yield return new GBAIsometric_Ice_Level3D_Object(2, new GBAIsometric_Ice_Vector(-0x19000, -0x19000, -0x19000));
            yield return new GBAIsometric_Ice_Level3D_Object(3, new GBAIsometric_Ice_Vector(0x1b800, -0xf800, 0x800));
            yield return new GBAIsometric_Ice_Level3D_Object(0, new GBAIsometric_Ice_Vector(0, 0, 0));
        }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<short>(ObjectType, name: nameof(ObjectType));
            Ushort_02 = s.Serialize<ushort>(Ushort_02, name: nameof(Ushort_02));
            Positions = s.SerializeObject<GBAIsometric_Ice_Vector>(Positions, name: nameof(Positions));
        }
    }
}