using System.Linq;

namespace R1Engine
{
    public class SNES_Proto_ROM : R1Serializable
    {
        public RGBA5551Color[] TilePalette { get; set; }
        public RGBA5551Color[] SpritePalette { get; set; }

        public MapData MapData { get; set; }
        public SNES_Proto_TileDescriptor[] TileDescriptors { get; set; }
        public byte[] TileSet { get; set; }

        public SNES_Proto_State[] States { get; set; }
        public SNES_Proto_ImageDescriptor[] ImageDescriptors { get; set; }
        public byte[] SpriteTileSet { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            s.DoAt(s.CurrentPointer + 0x2ADC4, () =>
            {
                TilePalette = s.SerializeObjectArray<RGBA5551Color>(TilePalette, 8 * 16, name: nameof(TilePalette));
                SpritePalette = s.SerializeObjectArray<RGBA5551Color>(SpritePalette, 8 * 16, name: nameof(SpritePalette));
            });

            MapData = s.DoAt(s.CurrentPointer + 0x28000, () => s.SerializeObject<MapData>(MapData, name: nameof(MapData)));
            TileDescriptors = s.DoAt(s.CurrentPointer + 0x1AAF8, () => s.SerializeObjectArray<SNES_Proto_TileDescriptor>(TileDescriptors, 1024 * 4, name: nameof(TileDescriptors)));
            TileSet = s.DoAt(s.CurrentPointer + 0x30000, () => s.SerializeArray<byte>(TileSet, 0x10000, name: nameof(TileSet)));
            
            States = s.DoAt(s.CurrentPointer + 0x210AC, () => s.SerializeObjectArray<SNES_Proto_State>(States, 5 * 0x15, name: nameof(States)));
            ImageDescriptors = s.DoAt(s.CurrentPointer + 0x20f4c, () => s.SerializeObjectArray<SNES_Proto_ImageDescriptor>(ImageDescriptors, States.Max(state => state.Animation?.Layers.Max(layer => layer.ImageIndex + 1) ?? 0), name: nameof(ImageDescriptors)));
            SpriteTileSet = s.DoAt(s.CurrentPointer + 0x2afc4, () => s.SerializeArray<byte>(SpriteTileSet, 0x3000, name: nameof(SpriteTileSet)));

        }
    }
}