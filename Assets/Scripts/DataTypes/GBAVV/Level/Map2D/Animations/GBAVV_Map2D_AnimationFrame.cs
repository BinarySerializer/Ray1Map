namespace R1Engine
{
    public class GBAVV_Map2D_AnimationFrame : R1Serializable
    {
        public Pointer TilePositionsPointer { get; set; }
        public Pointer TileShapesPointer { get; set; }
        public UInt24 TileOffset { get; set; } // Offset in the global tileset
        public byte TilesCount { get; set; }

        // Serialized from pointers
        
        public TilePosition[] TilePositions { get; set; }
        public TileShape[] TileShapes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TilePositionsPointer = s.SerializePointer(TilePositionsPointer, name: nameof(TilePositionsPointer));
            TileShapesPointer = s.SerializePointer(TileShapesPointer, name: nameof(TileShapesPointer));
            TileOffset = s.Serialize<UInt24>(TileOffset, name: nameof(TileOffset));
            TilesCount = s.Serialize<byte>(TilesCount, name: nameof(TilesCount));

            TilePositions = s.DoAt(TilePositionsPointer, () => s.SerializeObjectArray<TilePosition>(TilePositions, TilesCount, name: nameof(TilePositions)));
            TileShapes = s.DoAt(TileShapesPointer, () => s.SerializeObjectArray<TileShape>(TileShapes, TilesCount, name: nameof(TileShapes)));
        }

        public class TilePosition : R1Serializable
        {
            public short XPos { get; set; }
            public short YPos { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            }
        }

        public class TileShape : R1Serializable
        {
            public byte ShapeIndex { get; set; }
            public byte Unknown { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                s.SerializeBitValues<byte>(bitFunc =>
                {
                    ShapeIndex = (byte)bitFunc(ShapeIndex, 4, name: nameof(ShapeIndex));
                    Unknown = (byte)bitFunc(Unknown, 4, name: nameof(Unknown));
                });
            }
        }
    }
}