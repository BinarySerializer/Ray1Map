namespace R1Engine
{
    public class GBAVV_NitroKart_MapData : R1Serializable
    {
        public Pointer Mode7TileSetPointer { get; set; }
        public Pointer BackgroundTileSetPointer { get; set; }
        public Pointer TilePalettePointer { get; set; }
        public Pointer UnknownPalettesPointer { get; set; } // Points to 3 pointers for first level, each with a palette (pal animations?)
        public int Int_10 { get; set; } // 0 or 3
        public Pointer Mode7MapLayerPointer { get; set; }
        public Pointer[] BackgroundMapLayerPointers { get; set; }
        public Pointer Pointer_24 { get; set; } // To null-terminated pointer array
        public Pointer Mode7TileSetIntsPointer { get; set; }
        public int Mode7TileSetLength { get; set; }
        public Pointer Objects_Normal_Pointer { get; set; }
        public Pointer Objects_TimeTrial_Pointer { get; set; }
        public Pointer Objects_Unknown_Pointer { get; set; }
        public Pointer Pointer_3C { get; set; }
        public Pointer Pointer_40 { get; set; }

        // Serialized from pointers
        public byte[] Mode7TileSet { get; set; }
        public GBAVV_Map2D_TileSet BackgroundTileSet { get; set; }
        public RGBA5551Color[] TilePalette { get; set; }
        public GBAVV_NitroKart_UnknownPalettes UnknownPalettes { get; set; }
        public GBAVV_WorldMap_MapLayer Mode7MapLayer { get; set; }
        public GBAVV_NitroKart_BackgroundMapLayer[] BackgroundMapLayers { get; set; }
        public int[] Mode7TileSetInts { get; set; } // TODO: What is this?
        public GBAVV_NitroKart_Object[] Objects_Normal { get; set; }
        public GBAVV_NitroKart_Object[] Objects_TimeTrial { get; set; }
        public GBAVV_NitroKart_Object[] Objects_Unknown { get; set; } // For multiplayer/arcade? Usually same as normal.

        public override void SerializeImpl(SerializerObject s)
        {
            Mode7TileSetPointer = s.SerializePointer(Mode7TileSetPointer, name: nameof(Mode7TileSetPointer));
            BackgroundTileSetPointer = s.SerializePointer(BackgroundTileSetPointer, name: nameof(BackgroundTileSetPointer));
            TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
            UnknownPalettesPointer = s.SerializePointer(UnknownPalettesPointer, name: nameof(UnknownPalettesPointer));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Mode7MapLayerPointer = s.SerializePointer(Mode7MapLayerPointer, name: nameof(Mode7MapLayerPointer));
            BackgroundMapLayerPointers = s.SerializePointerArray(BackgroundMapLayerPointers, 3, name: nameof(BackgroundMapLayerPointers));
            Pointer_24 = s.SerializePointer(Pointer_24, name: nameof(Pointer_24));
            Mode7TileSetIntsPointer = s.SerializePointer(Mode7TileSetIntsPointer, name: nameof(Mode7TileSetIntsPointer));
            Mode7TileSetLength = s.Serialize<int>(Mode7TileSetLength, name: nameof(Mode7TileSetLength));
            Objects_Normal_Pointer = s.SerializePointer(Objects_Normal_Pointer, name: nameof(Objects_Normal_Pointer));
            Objects_TimeTrial_Pointer = s.SerializePointer(Objects_TimeTrial_Pointer, name: nameof(Objects_TimeTrial_Pointer));
            Objects_Unknown_Pointer = s.SerializePointer(Objects_Unknown_Pointer, name: nameof(Objects_Unknown_Pointer));
            Pointer_3C = s.SerializePointer(Pointer_3C, name: nameof(Pointer_3C));
            Pointer_40 = s.SerializePointer(Pointer_40, name: nameof(Pointer_40));

            Mode7TileSet = s.DoAt(Mode7TileSetPointer, () => s.SerializeArray<byte>(Mode7TileSet, Mode7TileSetLength * 0x40, name: nameof(Mode7TileSet)));
            BackgroundTileSet = s.DoAt(BackgroundTileSetPointer, () => s.SerializeObject<GBAVV_Map2D_TileSet>(BackgroundTileSet, name: nameof(BackgroundTileSet)));
            TilePalette = s.DoAt(TilePalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(TilePalette, 256, name: nameof(TilePalette)));
            UnknownPalettes = s.DoAt(UnknownPalettesPointer, () => s.SerializeObject<GBAVV_NitroKart_UnknownPalettes>(UnknownPalettes, name: nameof(UnknownPalettes)));
            Mode7MapLayer = s.DoAt(Mode7MapLayerPointer, () => s.SerializeObject<GBAVV_WorldMap_MapLayer>(Mode7MapLayer, name: nameof(Mode7MapLayer)));

            if (BackgroundMapLayers == null)
                BackgroundMapLayers = new GBAVV_NitroKart_BackgroundMapLayer[BackgroundMapLayerPointers.Length];

            for (int i = 0; i < BackgroundMapLayers.Length; i++)
                BackgroundMapLayers[i] = s.DoAt(BackgroundMapLayerPointers[i], () => s.SerializeObject(BackgroundMapLayers[i], name: $"{nameof(BackgroundMapLayers)}[{i}]"));

            Mode7TileSetInts = s.DoAt(Mode7TileSetIntsPointer, () => s.SerializeArray<int>(Mode7TileSetInts, Mode7TileSetLength, name: nameof(Mode7TileSetInts)));
            Objects_Normal = s.DoAt(Objects_Normal_Pointer, () => s.SerializeObjectArrayUntil(Objects_Normal, x => x.ObjType == 0, includeLastObj: false, name: nameof(Objects_Normal)));
            Objects_TimeTrial = s.DoAt(Objects_TimeTrial_Pointer, () => s.SerializeObjectArrayUntil(Objects_TimeTrial, x => x.ObjType == 0, includeLastObj: false, name: nameof(Objects_TimeTrial)));
            Objects_Unknown = s.DoAt(Objects_Unknown_Pointer, () => s.SerializeObjectArrayUntil(Objects_Unknown, x => x.ObjType == 0, includeLastObj: false, name: nameof(Objects_Unknown)));
        }
    }
}