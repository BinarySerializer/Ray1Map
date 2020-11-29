namespace R1Engine
{
    public class SNES_Proto_ROM : R1Serializable
    {
        public MapData MapData { get; set; }
        public RGBA5551Color[] Palettes { get; set; }
        public SNES_Proto_TileDescriptor[] TileDescriptors { get; set; }
        public byte[] TileMap { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            MapData = s.DoAt(s.CurrentPointer + 0x28000, () => s.SerializeObject<MapData>(MapData, name: nameof(MapData)));
            Palettes = s.DoAt(s.CurrentPointer + 0x2ADC4, () => s.SerializeObjectArray<RGBA5551Color>(Palettes, 16* 16, name: nameof(Palettes)));
            TileDescriptors = s.DoAt(s.CurrentPointer + 0x1AAF8, () => s.SerializeObjectArray<SNES_Proto_TileDescriptor>(TileDescriptors, 1024 * 4, name: nameof(TileDescriptors)));
            TileMap = s.DoAt(s.CurrentPointer + 0x30000, () => s.SerializeArray<byte>(TileMap, 0x10000, name: nameof(TileMap)));
        }
    }
}