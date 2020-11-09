namespace R1Engine
{
    public class GBAIsometric_Spyro2_LevelObjects2D : R1Serializable
    {
        public byte[] Bytes_00 { get; set; }

        public uint Obj0Count { get; set; }
        public GBAIsometric_Spyro2_LevelObjects2D_Obj0[] Obj0 { get; set; }

        public uint Obj1Count { get; set; }
        public GBAIsometric_Spyro2_LevelObjects2D_Obj1[] Obj1 { get; set; }

        public uint Obj2Count { get; set; }
        public GBAIsometric_Spyro2_LevelObjects2D_Obj2[] Obj2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Bytes_00 = s.SerializeArray<byte>(Bytes_00, 4, name: nameof(Bytes_00));

            Obj0Count = s.Serialize<uint>(Obj0Count, name: nameof(Obj0Count));
            Obj0 = s.SerializeObjectArray<GBAIsometric_Spyro2_LevelObjects2D_Obj0>(Obj0, Obj0Count, name: nameof(Obj0));

            Obj1Count = s.Serialize<uint>(Obj1Count, name: nameof(Obj1Count));
            Obj1 = s.SerializeObjectArray<GBAIsometric_Spyro2_LevelObjects2D_Obj1>(Obj1, Obj1Count, name: nameof(Obj1));

            Obj2Count = s.Serialize<uint>(Obj2Count, name: nameof(Obj2Count));
            Obj2 = s.SerializeObjectArray<GBAIsometric_Spyro2_LevelObjects2D_Obj2>(Obj2, Obj2Count, name: nameof(Obj2));
        }

        public class GBAIsometric_Spyro2_LevelObjects2D_Obj0 : R1Serializable
        {
            public byte[] Bytes_00 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Bytes_00 = s.SerializeArray<byte>(Bytes_00, 12, name: nameof(Bytes_00));
            }
        }
        public class GBAIsometric_Spyro2_LevelObjects2D_Obj1 : R1Serializable
        {
            public byte[] Bytes_00 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Bytes_00 = s.SerializeArray<byte>(Bytes_00, 16, name: nameof(Bytes_00));
            }
        }
        public class GBAIsometric_Spyro2_LevelObjects2D_Obj2 : R1Serializable
        {
            public byte[] Bytes_00 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Bytes_00 = s.SerializeArray<byte>(Bytes_00, 12, name: nameof(Bytes_00));
            }
        }
    }
}