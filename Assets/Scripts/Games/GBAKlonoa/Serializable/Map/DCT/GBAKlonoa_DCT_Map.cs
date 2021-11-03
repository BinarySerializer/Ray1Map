using BinarySerializer;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_DCT_Map : BinarySerializable
    {
        public bool Pre_SerializeData { get; set; }

        public Pointer[] MapLayerPointers { get; set; }
        public Pointer PalettePointer { get; set; }

        // Serialized from pointers
        public GBAKlonoa_DCT_MapLayer[] MapLayers { get; set; }
        public RGBA5551Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapLayerPointers = s.SerializePointerArray(MapLayerPointers, 4, name: nameof(MapLayerPointers));
            PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));

            if (!Pre_SerializeData)
                return;

            MapLayers ??= new GBAKlonoa_DCT_MapLayer[MapLayerPointers.Length];

            for (int i = 0; i < MapLayerPointers.Length; i++)
                s.DoAt(MapLayerPointers[i], () => MapLayers[i] = s.SerializeObject<GBAKlonoa_DCT_MapLayer>(MapLayers[i], name: $"{nameof(MapLayers)}[{i}]"));

            s.DoAt(PalettePointer, () => s.DoEncoded(new GBAKlonoa_DCT_Encoder(), () => Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, s.CurrentLength / 2, name: nameof(Palette))));
        }
    }
}