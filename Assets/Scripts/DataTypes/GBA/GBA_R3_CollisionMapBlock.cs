namespace R1Engine
{
    /// <summary>
    /// Collision map data for Rayman 3 (GBA)
    /// </summary>
    public class GBA_R3_CollisionMapBlock : R1Serializable
    {
        // Always 0?
        public uint Unk1 { get; set; }

        public uint BlockSize { get; set; }

        // Flags?
        public uint Unk2 { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public GBA_TileCollisionType[] CollisionData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
            Unk2 = s.Serialize<uint>(Unk2, name: nameof(Unk2));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            s.DoEncoded(new LZSSEncoder(), () => CollisionData = s.SerializeArray<GBA_TileCollisionType>(CollisionData, Width * Height, name: nameof(CollisionData)));
        }
    }
}