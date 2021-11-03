using BinarySerializer;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_LevelObject : BinarySerializable
    {
        public SectorState[] SectorStates { get; set; }
        public byte OAMIndex { get; set; }
        public byte DCT_GraphicsIndex { get; set; }
        public byte ObjType { get; set; }

        public GBAKlonoa_LoadedObject ToLoadedObject(short index, int sectorIndex) => new GBAKlonoa_LoadedObject(
            index: index, 
            oamIndex: OAMIndex, 
            xPos: SectorStates[sectorIndex].XPos, 
            yPos: SectorStates[sectorIndex].YPos, 
            param_1: SectorStates[sectorIndex].Param_1, 
            value6: 0, 
            param_2: SectorStates[sectorIndex].Param_2, 
            value8: SectorStates[sectorIndex].Byte_06, 
            objType: ObjType,
            levelObj: this,
            dct_GraphicsIndex: DCT_GraphicsIndex);

        public override void SerializeImpl(SerializerObject s)
        {
            SectorStates = s.SerializeObjectArray<SectorState>(SectorStates, 5, name: nameof(SectorStates));

            if (s.GetR1Settings().EngineVersion == EngineVersion.KlonoaGBA_DCT)
            {
                s.SerializePadding(2, logIfNotNull: true);

                ObjType = s.Serialize<byte>(ObjType, name: nameof(ObjType));
                DCT_GraphicsIndex = s.Serialize<byte>(DCT_GraphicsIndex, name: nameof(DCT_GraphicsIndex));
            }
            else
            {
                OAMIndex = s.Serialize<byte>(OAMIndex, name: nameof(OAMIndex));
                ObjType = s.Serialize<byte>(ObjType, name: nameof(ObjType));

                s.SerializePadding(2, logIfNotNull: true);
            }
        }

        public class SectorState : BinarySerializable
        {
            public short XPos { get; set; }
            public short YPos { get; set; }
            public byte Param_1 { get; set; }
            public byte Param_2 { get; set; }
            public byte Byte_06 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                YPos = s.Serialize<short>(YPos, name: nameof(YPos));

                Param_1 = s.Serialize<byte>(Param_1, name: nameof(Param_1));
                Param_2 = s.Serialize<byte>(Param_2, name: nameof(Param_2));
                Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));

                s.SerializePadding(1, logIfNotNull: true);
            }
        }
    }
}