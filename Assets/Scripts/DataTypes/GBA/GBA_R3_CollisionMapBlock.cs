namespace R1Engine
{
    /// <summary>
    /// Collision map data for Rayman 3 (GBA)
    /// </summary>
    public class GBA_R3_CollisionMapBlock : GBA_R3_BaseBlock
    {
        // Flags?
        public uint Unk { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public GBA_TileCollisionType[] CollisionData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize block header
            base.SerializeImpl(s);

            Unk = s.Serialize<uint>(Unk, name: nameof(Unk));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            s.DoEncoded(new LZSSEncoder(), () => CollisionData = s.SerializeArray<GBA_TileCollisionType>(CollisionData, Width * Height, name: nameof(CollisionData)));
        }
    }
}