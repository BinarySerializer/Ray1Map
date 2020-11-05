namespace R1Engine
{
    public class GBAIsometric_RHR_AnimSet_Pattern : R1Serializable
    {
        public byte SizeX { get; set; }
        public byte SizeY { get; set; }
        public byte PositionX { get; set; }
        public byte PositionY { get; set; }
        public byte Unknown { get; set; }
        public bool IsLastPattern { get; set; }

        public int NumTiles => (1 << SizeX) * (1 << SizeY);

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<ushort>(bitFunc => {
                SizeX = (byte)bitFunc(SizeX, 2, name: nameof(SizeX));
                SizeY = (byte)bitFunc(SizeY, 2, name: nameof(SizeY));
                PositionX = (byte)bitFunc(PositionX, 5, name: nameof(PositionX));
                PositionY = (byte)bitFunc(PositionY, 5, name: nameof(PositionY));
                Unknown = (byte)bitFunc(Unknown, 1, name: nameof(Unknown));
                IsLastPattern = bitFunc(IsLastPattern ? 1 : 0, 1, name: nameof(IsLastPattern)) == 1;
            });
        }
    }
}