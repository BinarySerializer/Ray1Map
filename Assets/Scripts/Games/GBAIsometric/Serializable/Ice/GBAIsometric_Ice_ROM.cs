using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_ROM : GBAIsometric_IceDragon_BaseROM
    {
        public bool Pre_SerializeLevel3D { get; set; }
        public int Pre_Level3DIndex { get; set; } = -1;
        public bool Pre_SerializeSparx { get; set; }
        public int Pre_SparxIndex { get; set; } = -1;
        public bool Pre_SerializePortraits { get; set; }
        public bool Pre_SerializeSprites { get; set; }

        // TODO: Mode7 levels

        // Levels 3D
        public Pointer<Palette>[] Level3D_Palettes { get; set; }
        public GBAIsometric_Ice_Level3D_MapLayers[] Level3D_MapLayers { get; set; }
        public uint[] Level3D_TileSetLengths { get; set; }
        public Pointer<Array<byte>>[] Level3D_TileSets { get; set; }
        public Pointer<GBAIsometric_Ice_Level3D_MapCollision>[] Level3D_MapCollision { get; set; }
        public GBAIsometric_Ice_Level3D_LevelMap[] Level3D_LevelMaps { get; set; } // JP only
        public Pointer<GBAIsometric_Ice_Level3D_Objects>[] Level3D_Objects { get; set; }
        public GBAIsometric_Ice_Vector[] Level3D_StartPositions { get; set; }

        // Sparx
        public GBAIsometric_Ice_Sparx_LevelData[] Sparx_Levels { get; set; }
        public GBAIsometric_Ice_Sparx_LevelData Sparx_MenuMap { get; set; } // Unused
        public Sparx_ObjectType[] Sparx_ObjectTypes { get; set; }
        public Palette Sparx_ObjPalette { get; set; }

        // Portraits
        public Pointer<Palette>[] PortraitPalettes { get; set; }
        public Pointer<ObjectArray<BinarySerializer.Nintendo.GBA.MapTile>>[] PortraitTileMaps { get; set; }
        public ushort[] PortraitTileSetLengths { get; set; }
        public Pointer<Array<byte>>[] PortraitTileSets { get; set; }

        // Sprites
        public Pointer<GBAIsometric_Ice_SpriteSet>[] SpriteSets { get; set; }
        public Pointer<Palette>[] SpriteSetPalettes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header and base data
            base.SerializeImpl(s);

            Dictionary<Spyro_DefinedPointer, Pointer> pointerTable = PointerTables.GBAIsometric_Spyro_PointerTable(
                s.GetR1Settings().GameModeSelection, Offset.File);

            if (Pre_SerializeLevel3D)
                SerializeLevel3D(s, pointerTable);

            if (Pre_SerializeSparx)
                SerializeSparx(s, pointerTable);

            if (Pre_SerializePortraits)
                SerializePortraits(s, pointerTable);

            if (Pre_SerializeSprites)
                SerializeSpriteSets(s, s.GetSettings<GBAIsometricSettings>());
        }

        private void SerializeLevel3D(
            SerializerObject s,
            Dictionary<Spyro_DefinedPointer, Pointer> pointerTable)
        {
            const int count = 17;

            // Serialize palettes
            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_Level3D_Palettes], () =>
                Level3D_Palettes = s.SerializePointerArray<Palette>(Level3D_Palettes, count, name: nameof(Level3D_Palettes)));

            // Serialize map layers
            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_Level3D_MapLayers], () =>
                Level3D_MapLayers = s.SerializeObjectArray<GBAIsometric_Ice_Level3D_MapLayers>(Level3D_MapLayers, count, 
                    (x, i) => x.Pre_Resolve = i == Pre_Level3DIndex || Pre_Level3DIndex == -1, name: nameof(Level3D_MapLayers)));

            // Serialize tile set lengths
            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_Level3D_TileSetLengths], () =>
                Level3D_TileSetLengths = s.SerializeArray<uint>(Level3D_TileSetLengths, count, name: nameof(Level3D_TileSetLengths)));

            // Serialize tile sets
            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_Level3D_TileSets], () =>
                Level3D_TileSets = s.SerializePointerArray<Array<byte>>(Level3D_TileSets, count, name: nameof(Level3D_TileSets)));

            // Serialize collision
            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_Level3D_MapCollision], () =>
                Level3D_MapCollision = s.SerializePointerArray<GBAIsometric_Ice_Level3D_MapCollision>(Level3D_MapCollision, count, name: nameof(Level3D_MapCollision)));

            // Although these are all directly referenced from the load function it is easier parsing it as an array as they're
            // stored one after another in the correct order. These are only in the JP version.
            s.DoAt(pointerTable.TryGetItem(Spyro_DefinedPointer.LevelMaps), () =>
                Level3D_LevelMaps = s.SerializeObjectArray<GBAIsometric_Ice_Level3D_LevelMap>(Level3D_LevelMaps, 15, name: nameof(Level3D_LevelMaps)));

            // Serialize objects
            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_Level3D_Objects], () =>
                Level3D_Objects = s.SerializePointerArray<GBAIsometric_Ice_Level3D_Objects>(Level3D_Objects, count, name: nameof(Level3D_Objects)));

            // Serialize start positions
            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_Level3D_StartPositions], () =>
                Level3D_StartPositions = s.SerializeObjectArray<GBAIsometric_Ice_Vector>(Level3D_StartPositions, count, name: nameof(Level3D_StartPositions)));

            // Resolve level data
            if (Pre_Level3DIndex != -1) {
                // For one level
                Level3D_Palettes?[Pre_Level3DIndex]?.ResolveObject(s, x => x.Pre_Is8Bit = true);
                Level3D_TileSets?[Pre_Level3DIndex]?.ResolveObject(s, x => x.Pre_Length = Level3D_TileSetLengths[Pre_Level3DIndex]);
                Level3D_MapCollision?[Pre_Level3DIndex]?.ResolveObject(s);
                Level3D_Objects?[Pre_Level3DIndex]?.ResolveObject(s);
            } else {
                // For all levels
                Level3D_Palettes?.ResolveObject(s, onPreSerialize: (x, _) => x.Pre_Is8Bit = true);
                Level3D_TileSets?.ResolveObject(s, onPreSerialize: (x, i) => x.Pre_Length = Level3D_TileSetLengths[i]);
                Level3D_MapCollision?.ResolveObject(s);
                Level3D_Objects?.ResolveObject(s);
            }
        }

        private void SerializeSparx(
            SerializerObject s,
            Dictionary<Spyro_DefinedPointer, Pointer> pointerTable)
        {
            // Serialize unused MENU map as index 4
            if (Pre_SparxIndex == 4 || Pre_SparxIndex == -1)
            {
                s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_Sparx_MenuMap], () =>
                {
                    Sparx_MenuMap ??= new GBAIsometric_Ice_Sparx_LevelData();

                    Sparx_MenuMap.Maps = new Pointer<GBAIsometric_Ice_Sparx_MapLayer>[]
                    {
                        Pointer<GBAIsometric_Ice_Sparx_MapLayer>.FromObject(
                            s.SerializeObject<GBAIsometric_Ice_Sparx_MapLayer>(Sparx_MenuMap.Maps?.FirstOrDefault(), name: nameof(Sparx_MenuMap.Maps)))
                    };
                    s.Align();

                    Sparx_MenuMap.Palette = Pointer<Palette>.FromObject(
                        s.SerializeObject<Palette>(Sparx_MenuMap.Palette,
                        x => x.Pre_Is8Bit = true, name: nameof(Sparx_MenuMap.Palette)));
                    Sparx_MenuMap.TileSetMap = Pointer<GBAIsometric_Ice_Sparx_TileSetMap>.FromObject(
                        s.SerializeObject<GBAIsometric_Ice_Sparx_TileSetMap>(Sparx_MenuMap.TileSetMap,
                        x => x.Pre_TilesCount = 150, name: nameof(Sparx_MenuMap.TileSetMap)));
                    Sparx_MenuMap.TileSet = Pointer<GBAIsometric_Ice_Sparx_TileSet>.FromObject(
                        s.SerializeObject<GBAIsometric_Ice_Sparx_TileSet>(Sparx_MenuMap.TileSet, name: nameof(Sparx_MenuMap.TileSet)));
                });
            }

            // Serialize levels
            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_Sparx_Levels], () =>
                Sparx_Levels = s.SerializeObjectArray<GBAIsometric_Ice_Sparx_LevelData>(Sparx_Levels, 4, 
                    (x, i) => x.Pre_Resolve = i == Pre_SparxIndex || Pre_SparxIndex == -1, name: nameof(Sparx_Levels)));

            // Serialize object types
            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_Sparx_ObjectTypes], () =>
                Sparx_ObjectTypes = s.SerializeObjectArray<Sparx_ObjectType>(Sparx_ObjectTypes, 49, name: nameof(Sparx_ObjectTypes)));

            // Serialize object palette
            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_Sparx_ObjectPalette], () =>
                Sparx_ObjPalette = s.SerializeObject<Palette>(Sparx_ObjPalette, x => x.Pre_Is8Bit = true, name: nameof(Sparx_ObjPalette)));
        }

        private void SerializePortraits(
            SerializerObject s,
            Dictionary<Spyro_DefinedPointer, Pointer> pointerTable) {
            const int count = 24;

            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_PortraitPalettes], () => {
                PortraitPalettes = s.SerializePointerArray<Palette>(PortraitPalettes, count, name: nameof(PortraitPalettes))
                ?.ResolveObject(s);
            });

            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_PortraitTileMaps], () => {
                PortraitTileMaps = s.SerializePointerArray<ObjectArray<BinarySerializer.Nintendo.GBA.MapTile>>(PortraitTileMaps, count, name: nameof(PortraitTileMaps))
                ?.ResolveObject(s, onPreSerialize: (x, _) => x.Pre_Length = 4 * 4);
            });

            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_PortraitTileSetLengths], () =>
                PortraitTileSetLengths = s.SerializeArray<ushort>(PortraitTileSetLengths, count, name: nameof(PortraitTileSetLengths)));

            s.DoAt(pointerTable[Spyro_DefinedPointer.Ice_PortraitTileSets], () => {
                PortraitTileSets = s.SerializePointerArray<Array<byte>>(PortraitTileSets, count, name: nameof(PortraitTileSets))
                ?.ResolveObject(s, onPreSerialize: (x, i) => x.Pre_Length = PortraitTileSetLengths[i]);
            });
        }

        private void SerializeSpriteSets(SerializerObject s, GBAIsometricSettings settings)
        {
            SpriteSets ??= settings.Ice_SpriteSetOffsets.
                Select(x => new Pointer<GBAIsometric_Ice_SpriteSet>(new Pointer(x.SpriteSetOffset, Offset.File))).
                ToArray();

            SpriteSets?.ResolveObject(s);

            SpriteSetPalettes ??= settings.Ice_SpriteSetOffsets.
                Select(x => x.PaletteOffset == 0 
                    ? null 
                    : new Pointer<Palette>(new Pointer(x.PaletteOffset, Offset.File))).
                ToArray();

            SpriteSetPalettes?.ResolveObject(s);
        }
    }
}