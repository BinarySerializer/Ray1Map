using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_LevelData : BinarySerializable
    {
        public bool Is2D { get; set; }
        public int SerializeDataForID { get; set; }
        public static bool ForceSerializeAll { get; set; }

        public Pointer[] MapLayerPointers { get; set; }

        public GBAIsometric_IceDragon_ResourceRef TilePaletteIndex { get; set; }
        public GBAIsometric_IceDragon_ResourceRef Collision2DIndex { get; set; }
        public GBAIsometric_IceDragon_ResourceRef Collision3DIndex { get; set; }
        public GBAIsometric_IceDragon_ResourceRef ObjPaletteIndex { get; set; }
        public GBAIsometric_IceDragon_ResourceRef Index3 { get; set; } // 2D map of 2 byte structs
        
        public uint ID { get; set; } // Levels are referenced by ID instead of index

        // Parsed
        public GBAIsometric_Spyro_MapLayer[] MapLayers { get; set; }
        public RGBA5551Color[] TilePalette { get; set; }
        public RGBA5551Color[] ObjPalette { get; set; }
        public GBAIsometric_Spyro_Collision2DMapData Collision2D { get; set; }
        public GBAIsometric_Spyro_Collision3DMapData Collision3D { get; set; }
        //public GBAIsometric_Spyro_SpriteMap Index3Map { get; set; } // TODO: What is this?

        public override void SerializeImpl(SerializerObject s)
        {
            MapLayerPointers = s.SerializePointerArray(MapLayerPointers, 4, name: nameof(MapLayerPointers));

            if (Is2D)
            {
                if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2)
                    Collision2DIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(Collision2DIndex, x => x.Pre_HasPadding = true, name: nameof(Collision2DIndex));

                TilePaletteIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(TilePaletteIndex, x => x.Pre_HasPadding = true, name: nameof(TilePaletteIndex));

                if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro3)
                    Collision2DIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(Collision2DIndex, x => x.Pre_HasPadding = true, name: nameof(Collision2DIndex));

                ObjPaletteIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(ObjPaletteIndex, x => x.Pre_HasPadding = true, name: nameof(ObjPaletteIndex));

                if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro3)
                    ID = s.Serialize<uint>(ID, name: nameof(ID));
            }
            else
            {
                TilePaletteIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(TilePaletteIndex, x => x.Pre_HasPadding = true, name: nameof(TilePaletteIndex));
                Collision3DIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(Collision3DIndex, x => x.Pre_HasPadding = true, name: nameof(Collision3DIndex));
                ObjPaletteIndex = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(ObjPaletteIndex, x => x.Pre_HasPadding = true, name: nameof(ObjPaletteIndex));
                Index3 = s.SerializeObject<GBAIsometric_IceDragon_ResourceRef>(Index3, x => x.Pre_HasPadding = true, name: nameof(Index3));
                ID = s.Serialize<uint>(ID, name: nameof(ID));
            }

            if (SerializeDataForID == ID || ForceSerializeAll)
            {
                if (MapLayers == null)
                    MapLayers = new GBAIsometric_Spyro_MapLayer[MapLayerPointers.Length];

                for (int i = 0; i < MapLayers.Length; i++)
                    MapLayers[i] = s.DoAt(MapLayerPointers[i], () => s.SerializeObject<GBAIsometric_Spyro_MapLayer>(MapLayers[i], name: $"{nameof(MapLayers)}[{i}]"));

                TilePaletteIndex.DoAt(size => TilePalette = s.SerializeObjectArray<RGBA5551Color>(TilePalette, 256, name: nameof(TilePalette)));
                ObjPaletteIndex.DoAt(size => ObjPalette = s.SerializeObjectArray<RGBA5551Color>(ObjPalette, 256, name: nameof(ObjPalette)));
                Collision3DIndex?.DoAt(size => Collision3D = s.SerializeObject<GBAIsometric_Spyro_Collision3DMapData>(Collision3D, name: nameof(Collision3D)));
                Collision2DIndex?.DoAt(size => Collision2D = s.SerializeObject<GBAIsometric_Spyro_Collision2DMapData>(Collision2D, name: nameof(Collision2D)));
                //Index3Map = Index3?.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_SpriteMap>(Index3Map, name: nameof(Index3Map)));
            }
        }
    }
}