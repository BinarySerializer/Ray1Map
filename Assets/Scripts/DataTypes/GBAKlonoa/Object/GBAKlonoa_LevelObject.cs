using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_LevelObject : BinarySerializable
    {
        public SectorState[] SectorStates { get; set; }
        public byte OAMIndex { get; set; }
        public byte ObjType { get; set; }

        public GBAKlonoa_LoadedObject ToLoadedObject(short index, int sectorIndex) => new GBAKlonoa_LoadedObject(
            index: index, 
            oamIndex: OAMIndex, 
            xPos: SectorStates[sectorIndex].XPos, 
            yPos: SectorStates[sectorIndex].YPos, 
            value5: SectorStates[sectorIndex].Byte_04, 
            value6: 0, 
            value7: SectorStates[sectorIndex].Byte_05, 
            value8: SectorStates[sectorIndex].Byte_06, 
            objType: ObjType,
            levelObj: this);

        public override void SerializeImpl(SerializerObject s)
        {
            SectorStates = s.SerializeObjectArray<SectorState>(SectorStates, 5, name: nameof(SectorStates));
            OAMIndex = s.Serialize<byte>(OAMIndex, name: nameof(OAMIndex));
            ObjType = s.Serialize<byte>(ObjType, name: nameof(ObjType));

            s.SerializePadding(2, logIfNotNull: true);
        }

        public class SectorState : BinarySerializable
        {
            public short XPos { get; set; }
            public short YPos { get; set; }
            public byte Byte_04 { get; set; }
            public byte Byte_05 { get; set; }
            public byte Byte_06 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                YPos = s.Serialize<short>(YPos, name: nameof(YPos));

                Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
                Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
                Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));

                s.SerializePadding(1, logIfNotNull: true);
            }
        }
    }
}