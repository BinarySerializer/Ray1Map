﻿using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_ROM_Generic : GBAVV_BaseROM
    {
        // Helpers
        public abstract GBAVV_Generic_MapInfo CurrentMapInfo { get; }
        public virtual int GetTheme => 0;
        public virtual GenericLevelType GetGenericLevelType => GenericLevelType.Map2D;
        public virtual int GetIndex3D => CurrentMapInfo.Index3D;
        public GBAVV_Mode7_LevelInfo CurrentMode7LevelInfo => Mode7_LevelInfos[CurrentMapInfo.Index3D];

        // Mode7
        public GBAVV_Mode7_LevelInfo[] Mode7_LevelInfos { get; set; }
        public RGBA5551Color[] Mode7_TilePalette { get; set; }
        public RGBA5551Color[] Mode7_Crash1_Type0_TilePalette_0F { get; set; }
        public byte[] Mode7_Crash2_Type0_BG1 { get; set; }
        public Pointer[] Mode7_Crash2_Type1_FlamesTileMapsPointers { get; set; }
        public MapTile[][] Mode7_Crash2_Type1_FlamesTileMaps { get; set; }
        public uint[] Mode7_Crash2_Type1_FlamesTileSetLengths { get; set; }
        public Pointer[] Mode7_Crash2_Type1_FlamesTileSetsPointers { get; set; }
        public byte[][] Mode7_Crash2_Type1_FlamesTileSets { get; set; }
        public RGBA5551Color[] Mode7_GetTilePal(GBAVV_Mode7_LevelInfo levInfo)
        {
            RGBA5551Color[] tilePal;

            if (Context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1)
                tilePal = levInfo.TileSetFrames.Palette;
            else if (Context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_SpongeBobRevengeOfTheFlyingDutchman)
                tilePal = levInfo.Crash1_Background.Palette;
            else
                tilePal = Mode7_TilePalette;

            if (Context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1 && levInfo.LevelType == 0)
                tilePal = tilePal.Take(256 - 16).Concat(Mode7_Crash1_Type0_TilePalette_0F).ToArray(); // Over last palette

            return tilePal;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.GBAVV_PointerTable(s.GetR1Settings().GameModeSelection, Offset.File);

            // Serialize level info
            SerializeLevelInfo(s, pointerTable);

            // Serialize graphics if 2D map
            if (GetGenericLevelType == GenericLevelType.Map2D)
                SerializeGraphics(s);

            // Serialize scripts
            SerializeScripts(s);

            // Serialize mode7/isometric data if used by the current level
            if (GetGenericLevelType == GenericLevelType.Mode7)
                SerializeMode7(s, pointerTable);
            else if (GetGenericLevelType == GenericLevelType.Isometric)
                SerializeIsometric(s, pointerTable);

            // Serialize additional data
            SerializeAdditionalData(s, pointerTable);
        }

        public void SerializeMode7(SerializerObject s, Dictionary<DefinedPointer, Pointer> pointerTable)
        {
            // Serialize Mode7 level infos
            s.DoAt(pointerTable[DefinedPointer.Mode7_LevelInfo], () =>
            {
                if (Mode7_LevelInfos == null)
                    Mode7_LevelInfos = new GBAVV_Mode7_LevelInfo[s.GetR1Settings().GetGameManagerOfType<GBAVV_Generic_BaseManager>().Mode7LevelsCount];

                var index3D = GetIndex3D;

                for (int i = 0; i < Mode7_LevelInfos.Length; i++)
                    Mode7_LevelInfos[i] = s.SerializeObject<GBAVV_Mode7_LevelInfo>(Mode7_LevelInfos[i], x => x.SerializeData = i == index3D, name: $"{nameof(Mode7_LevelInfos)}[{i}]");

                // Serialize animations
                Mode7_LevelInfos[index3D].SerializeAnimations(s, Mode7_LevelInfos.SelectMany(x => x.GetAllAnimSets).Where(x => x != null));
            });

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash2)
            {
                DefinedPointer palPointer = CurrentMode7LevelInfo.LevelType == 0 ? DefinedPointer.Mode7_TilePalette_Type0 : DefinedPointer.Mode7_TilePalette_Type1_Flames;

                Mode7_TilePalette = s.DoAt(pointerTable[palPointer], () => s.SerializeObjectArray<RGBA5551Color>(Mode7_TilePalette, CurrentMode7LevelInfo.LevelType == 0 ? 256 : 16, name: nameof(Mode7_TilePalette)));
            }
            else if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1 && CurrentMode7LevelInfo.LevelType == 0)
            {
                Mode7_Crash1_Type0_TilePalette_0F = s.DoAt(pointerTable[DefinedPointer.Mode7_Crash1_Type0_TilePalette_0F], () => s.SerializeObjectArray<RGBA5551Color>(Mode7_Crash1_Type0_TilePalette_0F, 16, name: nameof(Mode7_Crash1_Type0_TilePalette_0F)));
            }

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash2 && CurrentMode7LevelInfo.LevelType == 0)
            {
                Mode7_Crash2_Type0_BG1 = s.DoAt(pointerTable[DefinedPointer.Mode7_Crash2_Type0_BG1], () => s.SerializeArray<byte>(Mode7_Crash2_Type0_BG1, 38 * 9 * 32, name: nameof(Mode7_Crash2_Type0_BG1)));
            }
            else if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash2 && CurrentMode7LevelInfo.LevelType == 1)
            {
                Mode7_Crash2_Type1_FlamesTileMapsPointers = s.DoAt(pointerTable[DefinedPointer.Mode7_Crash2_Type1_FlamesTileMaps], () => s.SerializePointerArray(Mode7_Crash2_Type1_FlamesTileMapsPointers, 20, name: nameof(Mode7_Crash2_Type1_FlamesTileMapsPointers)));

                if (Mode7_Crash2_Type1_FlamesTileMaps == null)
                    Mode7_Crash2_Type1_FlamesTileMaps = new MapTile[20][];

                for (int i = 0; i < Mode7_Crash2_Type1_FlamesTileMaps.Length; i++)
                    Mode7_Crash2_Type1_FlamesTileMaps[i] = s.DoAt(Mode7_Crash2_Type1_FlamesTileMapsPointers[i], () => s.SerializeObjectArray<MapTile>(Mode7_Crash2_Type1_FlamesTileMaps[i], 0x1E * 0x14, name: $"{nameof(Mode7_Crash2_Type1_FlamesTileMaps)}[{i}]"));

                Mode7_Crash2_Type1_FlamesTileSetLengths = s.DoAt(pointerTable[DefinedPointer.Mode7_Crash2_Type1_FlamesTileSetLengths], () => s.SerializeArray<uint>(Mode7_Crash2_Type1_FlamesTileSetLengths, 20, name: nameof(Mode7_Crash2_Type1_FlamesTileSetLengths)));

                Mode7_Crash2_Type1_FlamesTileSetsPointers = s.DoAt(pointerTable[DefinedPointer.Mode7_Crash2_Type1_FlamesTileSets], () => s.SerializePointerArray(Mode7_Crash2_Type1_FlamesTileSetsPointers, 20, name: nameof(Mode7_Crash2_Type1_FlamesTileSetsPointers)));

                if (Mode7_Crash2_Type1_FlamesTileSets == null)
                    Mode7_Crash2_Type1_FlamesTileSets = new byte[20][];

                for (int i = 0; i < Mode7_Crash2_Type1_FlamesTileSets.Length; i++)
                    Mode7_Crash2_Type1_FlamesTileSets[i] = s.DoAt(Mode7_Crash2_Type1_FlamesTileSetsPointers[i], () => s.SerializeArray<byte>(Mode7_Crash2_Type1_FlamesTileSets[i], Mode7_Crash2_Type1_FlamesTileSetLengths[i], name: $"{nameof(Mode7_Crash2_Type1_FlamesTileSets)}[{i}]"));
            }
        }

        public abstract void SerializeLevelInfo(SerializerObject s, Dictionary<DefinedPointer, Pointer> pointerTable);
        public virtual void SerializeAdditionalData(SerializerObject s, Dictionary<DefinedPointer, Pointer> pointerTable) { }
        public virtual void SerializeIsometric(SerializerObject s, Dictionary<DefinedPointer, Pointer> pointerTable) { }

        public enum GenericLevelType
        {
            Map2D,
            Mode7,
            Isometric
        }
    }
}