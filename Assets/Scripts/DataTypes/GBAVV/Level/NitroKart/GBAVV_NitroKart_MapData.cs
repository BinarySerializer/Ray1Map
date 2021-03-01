namespace R1Engine
{
    public class GBAVV_NitroKart_MapData : R1Serializable
    {
        public Pointer TileSetPointer { get; set; }
        public Pointer DataBlockPointer { get; set; } // Compressed data block
        public Pointer TilePalettePointer { get; set; }
        public Pointer Pointer_0C { get; set; } // Points to 3 pointers for first level, each with a palette (pal animations?)
        public int Int_10 { get; set; } // 0 or 3
        public Pointer Mode7MapLayerPointer { get; set; }
        public Pointer[] MapLayerPointers { get; set; } // 3 layers
        public Pointer Pointer_24 { get; set; } // To null-terminated pointer array
        public Pointer TileSetIntsPointer { get; set; }
        public int TileSetLength { get; set; }
        public Pointer Objects_Normal_Pointer { get; set; }
        public Pointer Objects_TimeTrial_Pointer { get; set; }
        public Pointer Objects_Unknown_Pointer { get; set; }
        public Pointer Pointer_3C { get; set; }
        public Pointer Pointer_40 { get; set; }

        // Serialized from pointers
        public byte[] TileSet { get; set; }
        public RGBA5551Color[] TilePalette { get; set; }
        public GBAVV_WorldMap_MapLayer Mode7MapLayer { get; set; }
        public int[] TileSetInts { get; set; } // TODO: What is this?
        public GBAVV_NitroKart_Object[] Objects_Normal { get; set; }
        public GBAVV_NitroKart_Object[] Objects_TimeTrial { get; set; }
        public GBAVV_NitroKart_Object[] Objects_Unknown { get; set; } // For multiplayer/arcade? Usually same as normal.

        public override void SerializeImpl(SerializerObject s)
        {
            TileSetPointer = s.SerializePointer(TileSetPointer, name: nameof(TileSetPointer));
            DataBlockPointer = s.SerializePointer(DataBlockPointer, name: nameof(DataBlockPointer));
            TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
            Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Mode7MapLayerPointer = s.SerializePointer(Mode7MapLayerPointer, name: nameof(Mode7MapLayerPointer));
            MapLayerPointers = s.SerializePointerArray(MapLayerPointers, 3, name: nameof(MapLayerPointers));
            Pointer_24 = s.SerializePointer(Pointer_24, name: nameof(Pointer_24));
            TileSetIntsPointer = s.SerializePointer(TileSetIntsPointer, name: nameof(TileSetIntsPointer));
            TileSetLength = s.Serialize<int>(TileSetLength, name: nameof(TileSetLength));
            Objects_Normal_Pointer = s.SerializePointer(Objects_Normal_Pointer, name: nameof(Objects_Normal_Pointer));
            Objects_TimeTrial_Pointer = s.SerializePointer(Objects_TimeTrial_Pointer, name: nameof(Objects_TimeTrial_Pointer));
            Objects_Unknown_Pointer = s.SerializePointer(Objects_Unknown_Pointer, name: nameof(Objects_Unknown_Pointer));
            Pointer_3C = s.SerializePointer(Pointer_3C, name: nameof(Pointer_3C));
            Pointer_40 = s.SerializePointer(Pointer_40, name: nameof(Pointer_40));

            TileSet = s.DoAt(TileSetPointer, () => s.SerializeArray<byte>(TileSet, TileSetLength * 0x40, name: nameof(TileSet)));
            TilePalette = s.DoAt(TilePalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(TilePalette, 256, name: nameof(TilePalette)));
            Mode7MapLayer = s.DoAt(Mode7MapLayerPointer, () => s.SerializeObject<GBAVV_WorldMap_MapLayer>(Mode7MapLayer, name: nameof(Mode7MapLayer)));
            TileSetInts = s.DoAt(TileSetIntsPointer, () => s.SerializeArray<int>(TileSetInts, TileSetLength, name: nameof(TileSetInts)));
            Objects_Normal = s.DoAt(Objects_Normal_Pointer, () => s.SerializeObjectArrayUntil(Objects_Normal, x => x.ObjType == 0, includeLastObj: false, name: nameof(Objects_Normal)));
            Objects_TimeTrial = s.DoAt(Objects_TimeTrial_Pointer, () => s.SerializeObjectArrayUntil(Objects_TimeTrial, x => x.ObjType == 0, includeLastObj: false, name: nameof(Objects_TimeTrial)));
            Objects_Unknown = s.DoAt(Objects_Unknown_Pointer, () => s.SerializeObjectArrayUntil(Objects_Unknown, x => x.ObjType == 0, includeLastObj: false, name: nameof(Objects_Unknown)));
        }
    }
}