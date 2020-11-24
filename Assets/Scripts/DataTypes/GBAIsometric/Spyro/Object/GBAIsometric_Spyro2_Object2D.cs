namespace R1Engine
{
    public class GBAIsometric_Spyro2_Object2D : R1Serializable
    {
        public ObjCategory Category { get; set; }

        public ushort ObjType { get; set; }
        public ushort ID { get; set; }

        public short LinkedObjectID { get; set; }
        public ushort Unk { get; set; } // Always 0xCCCC?

        public short MinX { get; set; }
        public short MinY { get; set; }
        public short MaxX { get; set; }
        public short MaxY { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjType = s.Serialize<ushort>(ObjType, name: nameof(ObjType));
            ID = s.Serialize<ushort>(ID, name: nameof(ID));

            if (Category == ObjCategory.Character)
            {
                LinkedObjectID = s.Serialize<short>(LinkedObjectID, name: nameof(LinkedObjectID));
                Unk = s.Serialize<ushort>(Unk, name: nameof(Unk));
            }

            MinX = s.Serialize<short>(MinX, name: nameof(MinX));
            MinY = s.Serialize<short>(MinY, name: nameof(MinY));
            MaxX = s.Serialize<short>(MaxX, name: nameof(MaxX));
            MaxY = s.Serialize<short>(MaxY, name: nameof(MaxY));
        }

        public enum ObjCategory
        {
            Door,
            Character,
            Collectible
        }
    }
}