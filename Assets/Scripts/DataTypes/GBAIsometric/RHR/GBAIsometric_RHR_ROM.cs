using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;

namespace R1Engine
{
    public class GBAIsometric_RHR_ROM : GBA_ROMBase
    {
        public GBAIsometric_RHR_Localization Localization { get; set; }

        public GBAIsometric_RHR_PaletteAnimationTable[] PaletteAnimations { get; set; }
        public GBAIsometric_RHR_LevelInfo[] LevelInfos { get; set; }
        public GBAIsometric_ObjectType[] ObjectTypes { get; set; }
        public GBAIsometric_ObjectTypeData[] AdditionalObjectTypes { get; set; }
        public GBAIsometric_RHR_AnimSet[] AdditionalAnimSets { get; set; }
        public GBAIsometric_RHR_SpriteSet[] SpriteSets { get; set; }
        public GBAIsometric_RHR_Sprite[] Sprites { get; set; }
        public Pointer[] SpriteIconPointers { get; set; }
        public GBAIsometric_RHR_Sprite[] SpriteIcons { get; set; }
        public GBAIsometric_RHR_FlagSprite[] FlagSpritesUS { get; set; }
        public GBAIsometric_RHR_FlagSprite[] FlagSpritesEU { get; set; }

        public GBAIsometric_RHR_Font Font0 { get; set; }
        public GBAIsometric_RHR_Font Font1 { get; set; }
        public GBAIsometric_RHR_Font Font2 { get; set; }

        public Pointer[] PortraitPointers { get; set; }
        public GBAIsometric_RHR_AnimSet[] Portraits { get; set; }

        public GBAIsometric_RHR_MapLayer[] MenuMaps { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var pointerTable = PointerTables.GBAIsometric_RHR_PointerTable(s.GetR1Settings().GameModeSelection, Offset.File);

            // Serialize localization
            Localization = s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.Localization], () => s.SerializeObject<GBAIsometric_RHR_Localization>(Localization, name: nameof(Localization)));

            if (s.GetR1Settings().World == 0)
            {
                // Serialize level infos
                s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.Levels], () =>
                {
                    if (LevelInfos == null)
                        LevelInfos = new GBAIsometric_RHR_LevelInfo[20];

                    for (int i = 0; i < LevelInfos.Length; i++)
                        LevelInfos[i] = s.SerializeObject(LevelInfos[i], x => x.SerializeData = i == s.GetR1Settings().Level, name: $"{nameof(LevelInfos)}[{i}]");
                });

                PaletteAnimations = new GBAIsometric_RHR_PaletteAnimationTable[3];
                s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.PaletteAnimations0], () => {
                    PaletteAnimations[0] = s.SerializeObject<GBAIsometric_RHR_PaletteAnimationTable>(PaletteAnimations[0], name: $"{nameof(PaletteAnimations)}[0]");
                });
                s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.PaletteAnimations1], () => {
                    PaletteAnimations[1] = s.SerializeObject<GBAIsometric_RHR_PaletteAnimationTable>(PaletteAnimations[1], name: $"{nameof(PaletteAnimations)}[1]");
                });
                s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.PaletteAnimations2], () => {
                    PaletteAnimations[2] = s.SerializeObject<GBAIsometric_RHR_PaletteAnimationTable>(PaletteAnimations[2], name: $"{nameof(PaletteAnimations)}[2]");
                });

                // Serialize object types
                ObjectTypes = s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.ObjTypes], () => s.SerializeObjectArray<GBAIsometric_ObjectType>(ObjectTypes, 105, name: nameof(ObjectTypes)));

                // Serialize the crab type and add to the array (the crab entry points to memory)
                ObjectTypes[13].Data = s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.CrabObjType], () => s.SerializeObject<GBAIsometric_ObjectTypeData>(ObjectTypes[13].Data, name: $"CrabObjectTypeData"));

                // Serialize additional object types
                var additionalObjTypePointers = ObjTypePointers[s.GetR1Settings().GameModeSelection];
                if (AdditionalObjectTypes == null)
                    AdditionalObjectTypes = new GBAIsometric_ObjectTypeData[additionalObjTypePointers.Length];

                for (int i = 0; i < AdditionalObjectTypes.Length; i++)
                    AdditionalObjectTypes[i] = s.DoAt(new Pointer(additionalObjTypePointers[i], Offset.File), () => s.SerializeObject<GBAIsometric_ObjectTypeData>(AdditionalObjectTypes[i], name: $"{nameof(AdditionalObjectTypes)}[{i}]"));

                // Serialize additional animation sets
                var additionalAnimSetPointers = AnimSetPointers[s.GetR1Settings().GameModeSelection];
                if (AdditionalAnimSets == null)
                    AdditionalAnimSets = new GBAIsometric_RHR_AnimSet[additionalAnimSetPointers.Length];

                for (int i = 0; i < AdditionalAnimSets.Length; i++)
                    AdditionalAnimSets[i] = s.DoAt(new Pointer(additionalAnimSetPointers[i], Offset.File), () => s.SerializeObject<GBAIsometric_RHR_AnimSet>(AdditionalAnimSets[i], name: $"{nameof(AdditionalAnimSets)}[{i}]"));

                // Serialize sprite sets
                var spriteSetPointers = SpriteSetPointers[s.GetR1Settings().GameModeSelection];
                if (SpriteSets == null)
                    SpriteSets = new GBAIsometric_RHR_SpriteSet[spriteSetPointers.Length];

                for (int i = 0; i < SpriteSets.Length; i++)
                    SpriteSets[i] = s.DoAt(new Pointer(spriteSetPointers[i], Offset.File), () => s.SerializeObject<GBAIsometric_RHR_SpriteSet>(SpriteSets[i], name: $"{nameof(SpriteSets)}[{i}]"));

                // Serialize sprites
                var spritePointers = SpritePointers[s.GetR1Settings().GameModeSelection];
                if (Sprites == null)
                    Sprites = new GBAIsometric_RHR_Sprite[spritePointers.Length];

                for (int i = 0; i < Sprites.Length; i++)
                    Sprites[i] = s.DoAt(new Pointer(spritePointers[i], Offset.File), () => s.SerializeObject<GBAIsometric_RHR_Sprite>(Sprites[i], name: $"{nameof(Sprites)}[{i}]"));

                // Serialize sprite icons
                SpriteIconPointers = s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.SpriteIcons], () => s.SerializePointerArray(SpriteIconPointers, 84, name: nameof(SpriteIconPointers)));

                if (SpriteIcons == null)
                    SpriteIcons = new GBAIsometric_RHR_Sprite[SpriteIconPointers.Length];

                for (int i = 0; i < SpriteIcons.Length; i++)
                    SpriteIcons[i] = s.DoAt(SpriteIconPointers[i], () => s.SerializeObject<GBAIsometric_RHR_Sprite>(SpriteIcons[i], name: $"{nameof(SpriteIcons)}[{i}]"));

                // Serialize flag sprites
                FlagSpritesUS = s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.SpriteFlagsUS], () => s.SerializeObjectArray<GBAIsometric_RHR_FlagSprite>(FlagSpritesUS, 3, name: nameof(FlagSpritesUS)));
                FlagSpritesEU = s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.SpriteFlagsEU], () => s.SerializeObjectArray<GBAIsometric_RHR_FlagSprite>(FlagSpritesEU, 6, name: nameof(FlagSpritesEU)));

                // Serialize font
                Font0 = s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.Font0], () => s.SerializeObject<GBAIsometric_RHR_Font>(Font0, name: nameof(Font0)));
                Font1 = s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.Font1], () => s.SerializeObject<GBAIsometric_RHR_Font>(Font1, name: nameof(Font1)));
                Font2 = s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.Font2], () => s.SerializeObject<GBAIsometric_RHR_Font>(Font2, name: nameof(Font2)));

                // Serialize portraits
                PortraitPointers = s.DoAt(pointerTable[GBAIsometric_RHR_Pointer.Portraits], () => s.SerializePointerArray(PortraitPointers, 10, name: nameof(PortraitPointers)));

                if (Portraits == null)
                    Portraits = new GBAIsometric_RHR_AnimSet[PortraitPointers.Length];

                for (int i = 0; i < Portraits.Length; i++)
                    Portraits[i] = s.DoAt(PortraitPointers[i], () => s.SerializeObject<GBAIsometric_RHR_AnimSet>(Portraits[i], name: $"{nameof(Portraits)}[{i}]"));

                // Serialize alternative anim set palettes
                var alternativeAnimSetPalettes = AlternativeAnimSetPalettes[s.GetR1Settings().GameModeSelection];

                foreach (GBAIsometric_RHR_AnimSet animSet in GetAllAnimSets().Where(x => alternativeAnimSetPalettes.ContainsKey(x.Name)))
                    animSet.SerializeAlternativePalettes(s, alternativeAnimSetPalettes[animSet.Name]);
            }
            else
            {
                var maps = ((GBAIsometric_RHR_Manager)s.GetR1Settings().GetGameManager).GetMenuMaps(s.GetR1Settings().Level);
                MenuMaps = new GBAIsometric_RHR_MapLayer[maps.Length];

                for (int i = 0; i < MenuMaps.Length; i++)
                    MenuMaps[i] = s.DoAt(pointerTable[maps[i]], () => s.SerializeObject<GBAIsometric_RHR_MapLayer>(default, name: $"{maps[i]}"));

                /*
                 * Palette shift for Digital Eclipse logo (StartIndex,EndIndex,speed)
                 *  ShiftPaletteDigitalEclipse(0x12,0x20,1);
                    ShiftPaletteDigitalEclipse(0x22,0x30,1);
                    ShiftPaletteDigitalEclipse(0x32,0x40,1);
                    ShiftPaletteDigitalEclipse(0x42,0xd0,1);
                    ShiftPaletteDigitalEclipse(0xd1,0xf0,1);
                 * */
            }
        }

        public IEnumerable<GBAIsometric_RHR_AnimSet> GetAllAnimSets() => ObjectTypes.Select(x => x?.Data).Concat(AdditionalObjectTypes).Select(x => x?.AnimSetPointer.Value).Concat(AdditionalAnimSets).Concat(Portraits).Where(x => x != null).Distinct();
        public IEnumerable<GBAIsometric_RHR_Sprite> GetAllSprites() => Sprites.Concat(SpriteIcons).Distinct().ToArray();

        public Dictionary<GameModeSelection, uint[]> ObjTypePointers => new Dictionary<GameModeSelection, uint[]>()
        {
            [GameModeSelection.RaymanHoodlumsRevengeEU] = new uint[]
            {
                0x080e6c0c, // waterSplashAnimSet
                0x080e6b9c, // cutsceneTeensieAnimSet
                0x080e6bb8, // murfyAnimSet
                0x080e6bd4, // sheftAnimSet
                0x080e6bf0, // lavaSteamAnimSet
                0x080e6c28, // begoniaxSmokeAnimSet
                0x080e6b60, // crabStarAnimSet
                0x080e77d8, // plumTumblingAnimSet
                0x080e77f4, // barrel1AnimSet
                0x080e8454, // raymanFistAnimSet
                0x080e8470, // raymanFistAnimSet
                0x080e8588, // refluxOrbAnimSet
                //0x, // {null}
                0x080e856c, // cloneBarrelAnimSet
                0x080e85a4, // refluxOrbAnimSet
                0x080e86e4, // sparkAnimSet
                0x080e8700, // gemSparkAnimSet
                0x080e8854, // teensieCageAnimSet
                0x080e8920, // raymanToadAnimSet
                0x080e893c, // globoxToadAnimSet
                0x080e8958, // begoToadAnimSet
            },
            [GameModeSelection.RaymanHoodlumsRevengeUS] = new uint[]
            {
                0x080e6b68, // waterSplashAnimSet
                0x080e6af8, // cutsceneTeensieAnimSet
                0x080e6b14, // murfyAnimSet
                0x080e6b30, // sheftAnimSet
                0x080e6b4c, // lavaSteamAnimSet
                0x080e6b84, // begoniaxSmokeAnimSet
                0x080e6abc, // crabStarAnimSet
                0x080e7734, // plumTumblingAnimSet
                0x080e7750, // barrel1AnimSet
                0x080e83cc, // raymanFistAnimSet
                0x080e83b0, // raymanFistAnimSet
                0x080e84e4, // refluxOrbAnimSet
                0x080e851c, // {null}
                0x080e84c8, // cloneBarrelAnimSet
                0x080e8500, // refluxOrbAnimSet
                0x080e8640, // sparkAnimSet
                0x080e865c, // gemSparkAnimSet
                0x080e8740, // teensieCageAnimSet
                0x080e887c, // raymanToadAnimSet
                0x080e8898, // globoxToadAnimSet
                0x080e88b4, // begoToadAnimSet
            }
        };
        public Dictionary<GameModeSelection, uint[]> AnimSetPointers => new Dictionary<GameModeSelection, uint[]>()
        {
            [GameModeSelection.RaymanHoodlumsRevengeEU] = new uint[]
            {
                0x081e4a60, // shadowAnimSet
                0x081e4d84, // targetAnimSet
                0x080f0a0c, // portraitTeensie_3low
                0x080f0ad0, // portraitTeensie_4low
                0x0854b408, // fireMonsterFlamesAnimSet
                0x08421598, // gearBlockAnimSet
                0x0844963c, // raftShadowAnimSet
                0x08481efc, // dialogFrame
                0x084822d8, // eyes
                0x08481fa8, // piston
                0x080f0078, // bezel
                0x08482780, // mapIcon

                // Unused
                0x0810f27c, // raymanPafAnimSet
                0x081fc330, // greenPowerupAnimSet
                0x081e7020, // spikeAnimSet

                // New for menu
                0x084e460c, // tagLine
            },
            [GameModeSelection.RaymanHoodlumsRevengeUS] = new uint[]
            {
                0x081e49bc, // shadowAnimSet
                0x081e4ce0, // targetAnimSet
                0x080f0968, // portraitTeensie_3low
                0x080f0a2c, // portraitTeensie_4low
                0x08549ed0, // fireMonsterFlamesAnimSet
                0x084214f4, // gearBlockAnimSet
                0x08449598, // raftShadowAnimSet
                0x08481e58, // dialogFrame
                0x08482234, // eyes
                0x08481f04, // piston
                0x080effd4, // bezel
                0x084826dc, // mapIcon

                // Unused
                0x0810f1d8, // raymanPafAnimSet
                0x081fc28c, // greenPowerupAnimSet
                0x081e6f7c, // spikeAnimSet
            }
        };
        public Dictionary<GameModeSelection, uint[]> SpriteSetPointers => new Dictionary<GameModeSelection, uint[]>()
        {
            [GameModeSelection.RaymanHoodlumsRevengeEU] = new uint[] {
                0x080f00f0, // meterSpriteSet
                0x080F013C, // bossMeterSpriteSet
                0x084e8470, // soundMeterTopSpriteSet
                0x084e84b8, // soundMeterBottomSpriteSet
            },
            [GameModeSelection.RaymanHoodlumsRevengeUS] = new uint[]
            {
                0x080f004c, // meterSpriteSet
                0x080f0098, // bossMeterSpriteSet
                0x084e6f38, // soundMeterTopSpriteSet
                0x084e6f80, // soundMeterBottomSpriteSet
            }
        };
        public Dictionary<GameModeSelection, uint[]> SpritePointers => new Dictionary<GameModeSelection, uint[]>()
        {
            [GameModeSelection.RaymanHoodlumsRevengeEU] = new uint[] {  // TODO: Check if there are any new Sprites for EU
                0x080EB474, // aButton
                0x080EB494, // bButton
                0x080EB4B4, // dPadUp
                0x080EB4D4, // dPadDown
                0x080EB4F8, // dPadLeft
                0x080EB51C, // dPadRight
                0x080EB540, // selector
                0x080EB56C, // selector_yn
                0x080EB590, // cursor
                0x080EB5B0, // dlgAButton
                0x080EB5D4, // dlgBButton
                0x080EB5F8, // dlgRButton
                0x080EB61C, // dlgLButton
                0x080EB640, // dlgStart
                0x080EB664, // dlgSelect
                0x080EB688, // dlgDpadUp
                0x080EB6AC, // dlgDpadDown
                0x080EB6D0, // dlgDpadLeft
                0x080EB6F4, // dlgDpadRight
                0x080EB71C, // dlgDpad
                0x080EF8C0, // scoreCounterFrame
                0x080EF924, // scoreComboFrame
                0x080EF954, // comboText1
                0x080EF974, // comboText2
                0x080EF998, // teensyIcon
                0x080EF9BC, // lumIcon
                0x080EF9DC, // singleCounterFrame
                0x080EFA10, // doubleCounterFrame
                0x080EFA3C, // runeIcon1
                0x080EFA60, // runeIcon2
                0x080EFA84, // runeIcon3
                0x080EFAA8, // runeIcon4
                0x080EFACC, // runeIcon5
                0x080EFAF0, // currentIconNE
                0x080EFB18, // currentIconNW
                0x080EFB40, // currentIconSE
                0x080EFB68, // currentIconSW
                0x080EFBA8, // fireResistanceIcon
                0x080EFBEC, // copterIcon
                0x080EFC28, // metalFistIcon
                0x080EFC68, // plumIcon
                0x080EFCA4, // frogIcon
                0x080EFCC8, // frameOverrunIcon
                0x080EFCF4, // murfyIconSmall
                0x080EFD34, // murfyStamp
                0x080EFD90, // stampFrame1
                0x080EFDCC, // stampFrame2
                0x080F00B8, // meterLeftCap
                0x080F0B5C, // parchmentLeft
                0x080F0B9C, // parchmentRight
                0x080F0BDC, // parchmentCenter
                0x080F0C9C, // ingameDialogFrame

                0x08482064, // bottleHighlight0
                0x08482108, // bottleHighlight1
                0x084821AC, // bottleHighlight2
                0x084826DC, // mapIconComplete
                0x08482700, // mapIconBetween
                // Until here, all offsets match US offset + 0xA4
                
                0x084e83a0, // RLArrow
                0x084e83c0, // leftButton
                0x084e83e4, // rightButton
                0x084e851c, // mapIconRayman
                0x084e854c, // cartouche
            },
            [GameModeSelection.RaymanHoodlumsRevengeUS] = new uint[]
            {
                0x080eb3d0, // aButton
                0x080eb3f0, // bButton
                0x080eb410, // dPadUp
                0x080eb430, // dPadDown
                0x080eb454, // dPadLeft
                0x080eb478, // dPadRight
                0x080eb49c, // selector
                0x080eb4c8, // selector_yn
                0x080eb4ec, // cursor
                0x080eb50c, // dlgAButton
                0x080eb530, // dlgBButton
                0x080eb554, // dlgRButton
                0x080eb578, // dlgLButton
                0x080eb59c, // dlgStart
                0x080eb5c0, // dlgSelect
                0x080eb5e4, // dlgDpadUp
                0x080eb608, // dlgDpadDown
                0x080eb62c, // dlgDpadLeft
                0x080eb650, // dlgDpadRight
                0x080eb678, // dlgDpad
                0x080ef81c, // scoreCounterFrame
                0x080ef880, // scoreComboFrame
                0x080ef8b0, // comboText1
                0x080ef8d0, // comboText2
                0x080ef8f4, // teensyIcon
                0x080ef918, // lumIcon
                0x080ef938, // singleCounterFrame
                0x080ef96c, // doubleCounterFrame
                0x080ef998, // runeIcon1
                0x080ef9bc, // runeIcon2
                0x080ef9e0, // runeIcon3
                0x080efa04, // runeIcon4
                0x080efa28, // runeIcon5
                0x080efa4c, // currentIconNE
                0x080efa74, // currentIconNW
                0x080efa9c, // currentIconSE
                0x080efac4, // currentIconSW
                0x080efb04, // fireResistanceIcon
                0x080efb48, // copterIcon
                0x080efb84, // metalFistIcon
                0x080efbc4, // plumIcon
                0x080efc00, // frogIcon
                0x080efc24, // frameOverrunIcon
                0x080efc50, // murfyIconSmall
                0x080efc90, // murfyStamp
                0x080efcec, // stampFrame1
                0x080efd28, // stampFrame2
                0x080f0014, // meterLeftCap
                0x080f0ab8, // parchmentLeft
                0x080f0af8, // parchmentRight
                0x080f0b38, // parchmentCenter
                0x080f0bf8, // ingameDialogFrame
                0x08481fc0, // bottleHighlight0
                0x08482064, // bottleHighlight1
                0x08482108, // bottleHighlight2
                0x08482638, // mapIconComplete
                0x0848265c, // mapIconBetween
                0x084e6e68, // RLArrow
                0x084e6e88, // leftButton
                0x084e6eac, // rightButton
                0x084e6fe4, // mapIconRayman
                0x084e7014, // cartouche
            }
        };

        public Dictionary<GameModeSelection, Dictionary<string, uint>> SpritePalettes
        {
            get
            {
                var d = new Dictionary<GameModeSelection, Dictionary<string, uint>>()
                {
                    [GameModeSelection.RaymanHoodlumsRevengeEU] = new Dictionary<string, uint>()
                    {
                        ["bottleHighlight0"] = 0x084821D0,
                        ["bottleHighlight1"] = 0x084821D0,
                        ["bottleHighlight2"] = 0x084821D0,
                        ["RLArrow"] = 0x084E8400,
                        ["selector_yn"] = 0x084E8400,
                        ["leftButton"] = 0x084E8440,
                        ["rightButton"] = 0x084E8440,
                        ["parchmentCenter"] = 0x080F0B1C,
                        ["parchmentRight"] = 0x080F0B1C,
                        ["parchmentLeft"] = 0x080F0B1C,
                        ["teensyIcon"] = 0x080EF820,
                        ["lumIcon"] = 0x080EF820,

                        ["_SpriteIcons"] = 0x080E9A7C,
                        ["murfyStamp"] = 0x080EF840,
                        ["stampFrame1"] = 0x080EF860,
                        ["stampFrame2"] = 0x080EF860,

                        ["mapIconBetween"] = 0x080EF820,
                        ["mapIconComplete"] = 0x084826B4,

                        ["aButton"] = 0x080EF800,
                        ["bButton"] = 0x080EF800,
                        ["comboText1"] = 0x080EF800,
                        ["comboText2"] = 0x080EF800,
                        ["scoreCounterFrame"] = 0x080EF800,
                        ["scoreComboFrame"] = 0x080EF800,
                        ["singleCounterFrame"] = 0x080EF800,
                        ["doubleCounterFrame"] = 0x080EF800,

                        ["ingameDialogFrame"] = 0x080F0BFC,

                        ["dlgDpad"] = 0x080EF820,
                        ["dlgAButton"] = 0x080EF820,
                        ["dlgBButton"] = 0x080EF820,
                        ["dlgRButton"] = 0x080EF820,
                        ["dlgLButton"] = 0x080EF820,
                        ["dlgStart"] = 0x080EF820,
                        ["dlgSelect"] = 0x080EF820,
                        ["dlgDpadUp"] = 0x080EF820,
                        ["dlgDpadDown"] = 0x080EF820,
                        ["dlgDpadLeft"] = 0x080EF820,
                        ["dlgDpadRight"] = 0x080EF820,
                        ["metalFistIcon"] = 0x080EF820,
                        ["fireResistanceIcon"] = 0x080EF820,
                        ["copterIcon"] = 0x080EF820,
                        ["frameOverrunIcon"] = 0x080EF820,

                        ["murfyIconSmall"] = 0x080EF840,

                        ["cartouche"] = 0x084E8400,
                        ["mapIconRayman"] = 0x084E84F4,

                        ["selector"] = 0x080E9B1C,
                        ["cursor"] = 0x080E9B1C,

                        ["dPadUp"] = 0x080EF820,
                        ["dPadDown"] = 0x080EF820,
                        ["dPadLeft"] = 0x080EF820,
                        ["dPadRight"] = 0x080EF820,

                        ["runeIcon1"] = 0x080EF820,
                        ["runeIcon2"] = 0x080EF820,
                        ["runeIcon3"] = 0x080EF820,
                        ["runeIcon4"] = 0x080EF820,
                        ["runeIcon5"] = 0x080EF820,
                        ["currentIconNE"] = 0x080EF820,
                        ["currentIconNW"] = 0x080EF820,
                        ["currentIconSE"] = 0x080EF820,
                        ["currentIconSW"] = 0x080EF820,
                        ["plumIcon"] = 0x080EF820,
                        ["frogIcon"] = 0x080EF800,
                        ["meterLeftCap"] = 0x080EF800,

                        ["meterSpriteSet"] = 0x080EF800,
                        ["bossMeterSpriteSet"] = 0x080EF800,
                        ["soundMeterTopSpriteSet_0"] = 0x084E8420,
                        ["soundMeterTopSpriteSet_1"] = 0x084E8400,
                        ["soundMeterBottomSpriteSet_0"] = 0x084E8420,
                        ["soundMeterBottomSpriteSet_1"] = 0x084E8400,
                    },
                    [GameModeSelection.RaymanHoodlumsRevengeUS] = new Dictionary<string, uint>()
                    {
                        ["bottleHighlight0"] = 0x0848212c, // FUN_0801e178, 2
                        ["bottleHighlight1"] = 0x0848212c, // FUN_0801e178, 2
                        ["bottleHighlight2"] = 0x0848212c, // FUN_0801e178, 2
                        ["RLArrow"] = 0x084e6ec8, // FUN_08027328 (PauseGame), 2
                        ["selector_yn"] = 0x084e6ec8, // FUN_08027328, 2
                        ["leftButton"] = 0x084e6f08, // FUN_08027328, 6
                        ["rightButton"] = 0x084e6f08, // FUN_08027328, 6
                        ["parchmentCenter"] = 0x080f0a78, // FUN_080209b0, 7
                        ["parchmentRight"] = 0x080f0a78, // FUN_080209b0, 7
                        ["parchmentLeft"] = 0x080f0a78, // FUN_080209b0, 7
                        ["teensyIcon"] = 0x080ef77c, // FUN_080209b0, 9
                        ["lumIcon"] = 0x080ef77c, // FUN_080209b0, 9

                        ["_SpriteIcons"] = 0x080e99d8, // FUN_0802d63c, 2
                        ["murfyStamp"] = 0x080ef79c, // FUN_0802d63c, 3
                        ["stampFrame1"] = 0x080ef7bc, // FUN_0802d63c, 4
                        ["stampFrame2"] = 0x080ef7bc, // FUN_0802d63c, 4

                        ["mapIconBetween"] = 0x080ef77c, // FUN_0802fb9c, 2
                        ["mapIconComplete"] = 0x08482610, // FUN_0802fb9c, 5

                        ["aButton"] = 0x080EF75C,
                        ["bButton"] = 0x080EF75C,
                        ["comboText1"] = 0x080EF75C,
                        ["comboText2"] = 0x080EF75C,
                        ["scoreCounterFrame"] = 0x080EF75C,
                        ["scoreComboFrame"] = 0x080EF75C,
                        ["singleCounterFrame"] = 0x080EF75C,
                        ["doubleCounterFrame"] = 0x080EF75C,

                        ["ingameDialogFrame"] = 0x080F0B58,

                        ["dlgDpad"] = 0x080EF77C,
                        ["dlgAButton"] = 0x080EF77C,
                        ["dlgBButton"] = 0x080EF77C,
                        ["dlgRButton"] = 0x080EF77C,
                        ["dlgLButton"] = 0x080EF77C,
                        ["dlgStart"] = 0x080EF77C,
                        ["dlgSelect"] = 0x080EF77C,
                        ["dlgDpadUp"] = 0x080EF77C,
                        ["dlgDpadDown"] = 0x080EF77C,
                        ["dlgDpadLeft"] = 0x080EF77C,
                        ["dlgDpadRight"] = 0x080EF77C,
                        ["metalFistIcon"] = 0x080EF77C,
                        ["fireResistanceIcon"] = 0x080EF77C,
                        ["copterIcon"] = 0x080EF77C,
                        ["frameOverrunIcon"] = 0x080EF77C,

                        ["murfyIconSmall"] = 0x080ef79c,

                        ["cartouche"] = 0x084E6EC8,
                        ["mapIconRayman"] = 0x084E6FBC,

                        ["selector"] = 0x080E9A78,
                        ["cursor"] = 0x080E9A78,

                        ["dPadUp"] = 0x080EF77C,
                        ["dPadDown"] = 0x080EF77C,
                        ["dPadLeft"] = 0x080EF77C,
                        ["dPadRight"] = 0x080EF77C,

                        ["runeIcon1"] = 0x080EF77C,
                        ["runeIcon2"] = 0x080EF77C,
                        ["runeIcon3"] = 0x080EF77C,
                        ["runeIcon4"] = 0x080EF77C,
                        ["runeIcon5"] = 0x080EF77C,
                        ["currentIconNE"] = 0x080EF77C,
                        ["currentIconNW"] = 0x080EF77C,
                        ["currentIconSE"] = 0x080EF77C,
                        ["currentIconSW"] = 0x080EF77C,
                        ["plumIcon"] = 0x080EF77C,
                        ["frogIcon"] = 0x080EF75C,
                        ["meterLeftCap"] = 0x080EF75C,

                        ["meterSpriteSet"] = 0x080EF75C,
                        ["bossMeterSpriteSet"] = 0x080EF75C,
                        ["soundMeterTopSpriteSet_0"] = 0x084e6ee8,
                        ["soundMeterTopSpriteSet_1"] = 0x084e6ec8,
                        ["soundMeterBottomSpriteSet_0"] = 0x084e6ee8,
                        ["soundMeterBottomSpriteSet_1"] = 0x084e6ec8,
                    }
                };

                foreach (var dd in d.Values.Where(x => x.ContainsKey("_SpriteIcons")))
                {
                    // Fill spritePalettesUInt
                    foreach (var sprIcon in SpriteIcons)
                        dd[sprIcon.Name] = dd["_SpriteIcons"];
                }

                return d;
            }
        }
        public Dictionary<GameModeSelection, Dictionary<string, uint[]>> AlternativeAnimSetPalettes => new Dictionary<GameModeSelection, Dictionary<string, uint[]>>()
        {
            [GameModeSelection.RaymanHoodlumsRevengeUS] = new Dictionary<string, uint[]>()
            {
                ["teensieAnimSet"] = new uint[]
                {
                    0x08415aac,
                    0x08415acc,
                    0x08415aec,
                    0x08415b0c,
                    0x08415b2c
                },
                ["raymanAnimSet"] = new uint[]
                {
                    0x080f9ab8,
                    0x080f9cb8,
                    0x080f9eb8,
                },
                ["globoxAnimSet"] = new uint[]
                {
                    0x08202124,
                    0x08202144,
                },
                ["cloneAnimSet"] = new uint[]
                {
                    0x085ccf48,
                },
            },
            [GameModeSelection.RaymanHoodlumsRevengeEU] = new Dictionary<string, uint[]>()
            {
                ["teensieAnimSet"] = new uint[]
                {
                    0x08415b50,
                    0x08415b70,
                    0x08415b90,
                    0x08415bB0,
                    0x08415bD0
                },
                ["raymanAnimSet"] = new uint[]
                {
                    0x080f9b5c,
                    0x080f9d5c,
                    0x080f9f5c,
                },
                ["globoxAnimSet"] = new uint[]
                {
                    0x082021c8,
                    0x082021e8,
                },
                ["cloneAnimSet"] = new uint[]
                {
                    0x085ce480,
                },
            }
        };
    }
}