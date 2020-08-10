namespace R1Engine
{
    /// <summary>
    /// A map block for Rayman 3 (GBA)
    /// </summary>
    public class GBA_R3_MapBlock : GBA_R3_BaseBlock
    {
        // TODO: Better way to figure this out...
        public bool IsCollisionBlock => Unk1 == 257;

        // Flags?
        public uint Unk1 { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        // Appears to be tilemap related
        public ushort Unk2 { get; set; }

        public ushort Unk3 { get; set; }

        public byte Unk4 { get; set; }

        // ?
        public bool IsBackground { get; set; }

        // Always 0?
        public ushort Unk5 { get; set; }

        // The tile indexes
        public ushort[] MapData { get; set; }
        public GBA_TileCollisionType[] CollisionData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize block header
            base.SerializeImpl(s);

            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            if (!IsCollisionBlock)
            {
                Unk2 = s.Serialize<ushort>(Unk2, name: nameof(Unk2));
                Unk3 = s.Serialize<ushort>(Unk3, name: nameof(Unk3));
                Unk4 = s.Serialize<byte>(Unk4, name: nameof(Unk4));
                IsBackground = s.Serialize<bool>(IsBackground, name: nameof(IsBackground));
                Unk5 = s.Serialize<ushort>(Unk5, name: nameof(Unk5));

                // TODO: It seems the compressed block contains more data than just the tile indexes for BG_2 & 3?
                s.DoEncoded(new LZSSEncoder(), () => MapData = s.SerializeArray<ushort>(MapData, Width * Height, name: nameof(MapData)));
            }
            else
            {
                s.DoEncoded(new LZSSEncoder(), () => CollisionData = s.SerializeArray<GBA_TileCollisionType>(CollisionData, Width * Height, name: nameof(CollisionData)));
            }
        }
    }
}