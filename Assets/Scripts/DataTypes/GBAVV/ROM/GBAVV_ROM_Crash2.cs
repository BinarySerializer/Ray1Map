using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_ROM_Crash2 : GBAVV_ROM_Crash
    {
        // Helpers
        public GBAVV_Isometric_MapData CurrentIsometricMapData => Isometric_MapDatas[CurrentIsometricIndex];
        public GBAVV_Isometric_ObjectData CurrentIsometricObjData => Isometric_ObjectDatas[CurrentIsometricIndex];
        public int CurrentIsometricIndex => CurrentMapInfo.Index3D + 4;

        // Cutscenes
        public GBAVV_Crash2_CutsceneEntry[] CutsceneTable { get; set; }
        public GBAVV_Crash2_FLCTableEntry[] FLCTable { get; set; }

        // Isometric
        public GBAVV_Isometric_MapData[] Isometric_MapDatas { get; set; }
        public GBAVV_Isometric_ObjectData[] Isometric_ObjectDatas { get; set; }
        public GBAVV_Isometric_CharacterInfo[] Isometric_CharacterInfos { get; set; }
        public GBAVV_Isometric_CharacterIcon[] Isometric_CharacterIcons { get; set; }
        public GBAVV_Isometric_Animation[] Isometric_ObjAnimations { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_0 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_1 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_2 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_4 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_11 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_12 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_13 { get; set; }
        public RGBA5551Color[] Isometric_GetObjPalette =>
            Isometric_ObjPalette_0.
                Concat(Isometric_ObjPalette_1).
                Concat(Isometric_ObjPalette_2).
                Concat(Enumerable.Repeat(new RGBA5551Color(), 16 * 1)).
                Concat(Isometric_ObjPalette_4).
                Concat(Enumerable.Repeat(new RGBA5551Color(), 16 * 6)).
                Concat(Isometric_ObjPalette_11).
                Concat(Isometric_ObjPalette_12).
                Concat(Isometric_ObjPalette_13).
                Concat(Enumerable.Repeat(new RGBA5551Color(), 16 * 2)).
                ToArray();
        public GBAVV_Isometric_Animation[] Isometric_AdditionalAnimations { get; set; }
        public IEnumerable<GBAVV_Isometric_Animation> Isometric_GetAnimations => Isometric_ObjAnimations.Concat(Isometric_AdditionalAnimations);

        // WorldMap
        public GBAVV_Map WorldMap { get; set; }

        public override void SerializeAdditionalData(SerializerObject s, Dictionary<GBAVV_Pointer, Pointer> pointerTable)
        {
            var isJp = s.GetR1Settings().GameModeSelection == GameModeSelection.Crash2GBAJP;

            CutsceneTable = s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.Crash2_CutsceneTable), () => s.SerializeObjectArray<GBAVV_Crash2_CutsceneEntry>(CutsceneTable, isJp ? 55 : 54, name: nameof(CutsceneTable)));

            if (SerializeFLC)
                FLCTable = s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.Crash2_FLCTable), () => s.SerializeObjectArray<GBAVV_Crash2_FLCTableEntry>(FLCTable, isJp ? 25 : 24, name: nameof(FLCTable)));

            if (CurrentLevInfo.IsWorldMap)
                WorldMap = s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.Crash2_WorldMap), () => s.SerializeObject<GBAVV_Map>(WorldMap, name: nameof(WorldMap)));
        }

        public override void SerializeIsometric(SerializerObject s, Dictionary<GBAVV_Pointer, Pointer> pointerTable)
        {
            var index3D = CurrentMapInfo.Index3D;

            s.DoAt(pointerTable[GBAVV_Pointer.Isometric_MapDatas], () =>
            {
                if (Isometric_MapDatas == null)
                    Isometric_MapDatas = new GBAVV_Isometric_MapData[7];

                for (int i = 0; i < Isometric_MapDatas.Length; i++)
                    Isometric_MapDatas[i] = s.SerializeObject<GBAVV_Isometric_MapData>(Isometric_MapDatas[i], x => x.SerializeData = i == index3D + 4, name: $"{nameof(Isometric_MapDatas)}[{i}]");
            });

            s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjectDatas], () =>
            {
                if (Isometric_ObjectDatas == null)
                    Isometric_ObjectDatas = new GBAVV_Isometric_ObjectData[7];

                for (int i = 0; i < Isometric_ObjectDatas.Length; i++)
                    Isometric_ObjectDatas[i] = s.SerializeObject<GBAVV_Isometric_ObjectData>(Isometric_ObjectDatas[i], x =>
                    {
                        x.SerializeData = i == index3D + 4;
                        x.IsMultiplayer = i < 4;
                    }, name: $"{nameof(Isometric_ObjectDatas)}[{i}]");
            });

            Isometric_CharacterInfos = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_Characters], () => s.SerializeObjectArray<GBAVV_Isometric_CharacterInfo>(Isometric_CharacterInfos, 12, name: nameof(Isometric_CharacterInfos)));
            Isometric_CharacterIcons = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_CharacterIcons], () => s.SerializeObjectArray<GBAVV_Isometric_CharacterIcon>(Isometric_CharacterIcons, 11, name: nameof(Isometric_CharacterIcons)));
            Isometric_ObjAnimations = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjAnimations], () => s.SerializeObjectArray<GBAVV_Isometric_Animation>(Isometric_ObjAnimations, 22, name: nameof(Isometric_ObjAnimations)));

            Isometric_ObjPalette_0 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_0], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_0, 16, name: nameof(Isometric_ObjPalette_0)));
            Isometric_ObjPalette_1 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_1], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_1, 16, name: nameof(Isometric_ObjPalette_1)));
            Isometric_ObjPalette_2 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_2], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_2, 16, name: nameof(Isometric_ObjPalette_2)));
            Isometric_ObjPalette_4 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_4], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_4, 16, name: nameof(Isometric_ObjPalette_4)));
            Isometric_ObjPalette_11 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_11], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_11, 16, name: nameof(Isometric_ObjPalette_11)));
            Isometric_ObjPalette_12 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_12], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_12, 16, name: nameof(Isometric_ObjPalette_12)));
            Isometric_ObjPalette_13 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_13], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_13, 16, name: nameof(Isometric_ObjPalette_13)));

            // These animations are all hard-coded from functions:
            Isometric_AdditionalAnimations = new GBAVV_Isometric_Animation[]
            {
                GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim0_Frames], 0x03, 4, 4, 2), // Green barrel
                GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim1_Frames], 0x03, 4, 4, 2), // Laser beam
                GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim2_Frames], 0x06, 4, 4, 1), // Crate breaks
                GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim3_Frames], 0x07, 4, 4, 1), // Checkpoint breaks
                GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim4_Frames], 0x18, 8, 4, 0), // Checkpoint text
                GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim5_Frames], 0x08, 4, 4, 2), // Nitro explosion
                GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim6_Frames], 0x08, 4, 4, 2), // Nitro switch
                GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim7_Frames], 0x0E, 4, 4, 0), // Wumpa HUD
                GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim8_Frames], 0x0A, 8, 8, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim8_Palette]), // Crystal collected
                GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim9_Frames], 0x03, 4, 4, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim9_Palette]), // Multiplayer base
                GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim10_Frames], 0x0A, 2, 2, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim10_Palette]), // Multiplayer item
            };
        }
    }
}