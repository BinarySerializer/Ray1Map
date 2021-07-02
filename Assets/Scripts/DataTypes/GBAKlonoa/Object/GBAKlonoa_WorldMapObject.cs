using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_WorldMapObject : BinarySerializable
    {
        public sbyte XPos { get; set; }
        public sbyte YPos { get; set; }
        public byte ZPos { get; set; }
        public byte OAMIndex { get; set; }
        public byte ObjType { get; set; }

        public GBAKlonoa_LoadedObject ToLoadedObject(short index) => new GBAKlonoa_LoadedObject(
            index: index,
            oamIndex: OAMIndex,
            xPos: XPos,
            yPos: YPos,
            param_1: ZPos,
            value6: 1,
            param_2: 0,
            value8: 0x1C,
            objType: ObjType,
            worldMapObj: this);

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.Serialize<sbyte>(XPos, name: nameof(XPos));
            YPos = s.Serialize<sbyte>(YPos, name: nameof(YPos));
            ZPos = s.Serialize<byte>(ZPos, name: nameof(ZPos));
            OAMIndex = s.Serialize<byte>(OAMIndex, name: nameof(OAMIndex));
            ObjType = s.Serialize<byte>(ObjType, name: nameof(ObjType));
        }
    }
}