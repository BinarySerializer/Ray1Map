using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_ROM_Crash : GBAVV_ROM_Generic
    {
        public GBAVV_Crash_BaseManager.CrashLevInfo CurrentLevInfo { get; set; } // Set before serializing

        // Helpers
        public override GBAVV_Generic_MapInfo CurrentMapInfo
        {
            get
            {
                GBAVV_Generic_MapInfo map;

                if (CurrentLevInfo.IsSpecialMap)
                    map = new GBAVV_Generic_MapInfo
                    {
                        Crash_MapType = CurrentLevInfo.SpecialMapType ?? GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Normal,
                        Index3D = CurrentLevInfo.Index3D
                    };
                else if (CurrentLevInfo.MapType == GBAVV_Crash_BaseManager.CrashLevInfo.Type.Normal)
                    map = LevelInfos[CurrentLevInfo.LevelIndex].LevelData.Maps[CurrentLevInfo.MapIndex];
                else if (CurrentLevInfo.MapType == GBAVV_Crash_BaseManager.CrashLevInfo.Type.Bonus)
                    map = LevelInfos[CurrentLevInfo.LevelIndex].LevelData.BonusMap;
                else
                    map = LevelInfos[CurrentLevInfo.LevelIndex].LevelData.ChallengeMap;

                return map;
            }
        }
        public override int GetTheme => LevelInfos[CurrentLevInfo.LevelIndex].LevelTheme;
        public override GenericLevelType GetGenericLevelType
        {
            get
            {
                switch (CurrentMapInfo.Crash_MapType)
                {
                    case GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Normal:
                    case GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Normal_Vehicle_0:
                    case GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Normal_Vehicle_1:
                        return GenericLevelType.Map2D;

                    case GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Mode7:
                        return GenericLevelType.Mode7;

                    case GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Isometric:
                        return GenericLevelType.Isometric;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // Localization
        public Pointer[] LocTablePointers { get; set; }
        public GBAVV_Crash_LocTable[] LocTables { get; set; }

        // Common
        public GBAVV_Crash_LevelInfo[] LevelInfos { get; set; }

        public override void SerializeLevelInfo(SerializerObject s, Dictionary<GBAVV_Pointer, Pointer> pointerTable)
        {
            // Serialize localization
            if (pointerTable.ContainsKey(GBAVV_Pointer.Localization))
            {
                var multipleLanguages = s.GetR1Settings().GameModeSelection == GameModeSelection.Crash1GBAEU || s.GetR1Settings().GameModeSelection == GameModeSelection.Crash2GBAEU;

                if (multipleLanguages)
                    LocTablePointers = s.DoAt(pointerTable[GBAVV_Pointer.Localization], () => s.SerializePointerArray(LocTablePointers, 6, name: nameof(LocTablePointers)));
                else
                    LocTablePointers = new Pointer[]
                    {
                        pointerTable[GBAVV_Pointer.Localization]
                    };

                if (LocTables == null)
                    LocTables = new GBAVV_Crash_LocTable[LocTablePointers.Length];

                for (int i = 0; i < LocTables.Length; i++)
                    LocTables[i] = s.DoAt(LocTablePointers[i], () => s.SerializeObject<GBAVV_Crash_LocTable>(LocTables[i], name: $"{nameof(LocTables)}[{i}]"));
            }

            // TODO: Make common localization storage system for games - access using language ID and loc ID
            // Store the localization in the context so we can access it
            s.Context.StoreObject(GBAVV_Crash_BaseManager.LocTableID, LocTables?.FirstOrDefault());

            if (CurrentLevInfo.IsWorldMap)
                return;

            // Get the manager
            var manager = s.GetR1Settings().GetGameManagerOfType<GBAVV_Crash_BaseManager>();

            // Serialize level infos
            s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.LevelInfo), () =>
            {
                if (LevelInfos == null)
                    LevelInfos = new GBAVV_Crash_LevelInfo[manager.LevInfos.Max(x => x.LevelIndex) + 1];

                for (int i = 0; i < LevelInfos.Length; i++)
                    LevelInfos[i] = s.SerializeObject<GBAVV_Crash_LevelInfo>(LevelInfos[i], x => x.LevInfo = i == CurrentLevInfo.LevelIndex ? CurrentLevInfo : null, name: $"{nameof(LevelInfos)}[{i}]");
            });
        }
    }
}