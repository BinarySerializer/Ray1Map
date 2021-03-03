using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine
{
    public abstract class GBAVV_NitroKart_Manager : GBAVV_BaseManager
    {
        public override LevInfo[] LevInfos => Levels;

        public static LevInfo[] Levels = new LevInfo[]
        {
            new LevInfo(0, "Terra Hub"), 
            new LevInfo(1, "Inferno Island"),
            new LevInfo(2, "Jungle Boogie"),
            new LevInfo(3, "Tiny's Temple"),

            new LevInfo(4, "Barin Hub"),
            new LevInfo(5, "Meteor Gorge"),
            new LevInfo(6, "Barin Ruins"),
            new LevInfo(7, "Deep Sea Driving"),
            
            new LevInfo(8, "Fenomena Hub"),
            new LevInfo(9, "Out of Time"),
            new LevInfo(10, "Clockwork Wumpa"),
            new LevInfo(11, "Thunder Struck"),
            
            new LevInfo(12, "Teknee Hub"),
            new LevInfo(13, "Assembly Lane"),
            new LevInfo(14, "Android Alley"),
            new LevInfo(15, "Electron Avenue"),

            new LevInfo(16, "Velo's Citadel"),
            new LevInfo(17, "Velo's Challenge"),

            // TODO: Names
            new LevInfo(18, "Battle 1"),
            new LevInfo(19, "Battle 2"),
            new LevInfo(20, "Battle 3"),
            new LevInfo(21, "Battle 4"),
            new LevInfo(22, "Battle 5"),
            new LevInfo(23, "Battle 6"),
            new LevInfo(24, "Battle 7"),
            new LevInfo(24, "Battle 7 (duplicate)"),
            new LevInfo(25, "Battle 8"),
        };

        public override UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            //FindDataInROM(context.Deserializer, context.FilePointer(GetROMFilePath));
            //FindObjTypeData(context);
            return base.LoadAsync(context, loadTextures);
        }

        public void FindDataInROM(SerializerObject s, Pointer offset)
        {
            // Read ROM as a uint array
            var values = s.DoAt(offset, () => s.SerializeArray<uint>(default, s.CurrentLength / 4, name: "Values"));

            // Helper for getting a pointer
            long getPointer(int index) => GBA_ROMBase.Address_ROM + index * 4;
            bool isValidPointer(uint value) => value >= GBA_ROMBase.Address_ROM && value < GBA_ROMBase.Address_ROM + s.CurrentLength;

            // Keep track of found data
            var foundGraphics = new List<long>();
            var foundScripts = new List<Tuple<long, string>>();

            // Find graphics datas
            for (int i = 0; i < values.Length - 3; i++)
            {
                var p = getPointer(i);

                // The animSets pointer always points to 12 bytes ahead
                if (values[i] == p + 16)
                {
                    // Make sure we've got valid pointers for the tiles and palettes
                    if (isValidPointer(values[i + 1]) && isValidPointer(values[i + 2]))
                    {
                        var animSetsCount = s.DoAt(new Pointer((uint)getPointer(i + 3), s.CurrentPointer.file), () => s.Serialize<ushort>(default));
                        var palettesCount = s.DoAt(new Pointer((uint)(getPointer(i + 3) + 2), s.CurrentPointer.file), () => s.Serialize<ushort>(default));

                        // Make sure the animSets count and palette counts are reasonable
                        if (animSetsCount < 10000 && palettesCount < 10000)
                            foundGraphics.Add(p);
                    }
                }
            }

            // Find scripts by finding the name command which is always the first one
            for (int i = 0; i < values.Length - 2; i++)
            {
                if (values[i] == 9 && values[i + 1] == 7 && isValidPointer(values[i + 2]))
                {
                    foundScripts.Add(new Tuple<long, string>(getPointer(i), s.DoAt(new Pointer(values[i + 2], s.CurrentPointer.file), () => s.SerializeString(default))));
                }
            }

            // Log found data to clipboard
            var str = new StringBuilder();

            str.AppendLine($"Graphics:");

            foreach (var g in foundGraphics)
                str.AppendLine($"0x{g:X8},");

            str.AppendLine();
            str.AppendLine($"Scripts:");

            foreach (var (p, name) in foundScripts)
                str.AppendLine($"0x{p:X8}, // {name}");

            str.ToString().CopyToClipboard();
        }

        public void FindObjTypeData(Context context)
        {
            var rom = FileFactory.Read<GBAVV_ROM>(GetROMFilePath, context, (o, r) => r.CurrentLevInfo = LevInfos[context.Settings.Level]);
            var s = context.Deserializer;

            var str = new StringBuilder();

            var initFunctionPointers = s.DoAt(new Pointer(ObjTypesPointer, s.Context.GetFile(GetROMFilePath)), () => s.SerializePointerArray(default, ObjTypesCount));
            var orderedPointers = initFunctionPointers.OrderBy(x => x.AbsoluteOffset).Distinct().ToArray();

            // Enumerate every obj init function
            for (int i = 0; i < initFunctionPointers.Length; i++)
            {
                var nextPointer = orderedPointers.ElementAtOrDefault(orderedPointers.FindItemIndex(x => x == initFunctionPointers[i]) + 1);

                s.DoAt(initFunctionPointers[i], () =>
                {
                    s.Align();

                    var foundPointer = false;

                    // Try and read every int as a pointer until we get a valid one 25 times
                    for (int j = 0; j < 25; j++)
                    {
                        if (nextPointer != null && s.CurrentPointer.AbsoluteOffset >= nextPointer.AbsoluteOffset)
                            break;

                        var p = s.SerializePointer(default);

                        s.DoAt(p, () =>
                        {
                            s.Goto(s.CurrentPointer + 20);
                            var graphicsPointer = s.SerializePointer(default);

                            if (rom.Map2D_Graphics.Any(x => x.Offset == graphicsPointer))
                            {
                                str.AppendLine($"0x{p.AbsoluteOffset:X8}, // {i}");
                                foundPointer = true;
                            }
                        });

                        if (foundPointer)
                            return;
                    }

                    // No pointer found...
                    str.AppendLine($"null, // {i}");
                });
            }

            str.ToString().CopyToClipboard();
        }

        public virtual long ObjTypesCount => 114;
        public abstract uint ObjTypesPointer { get; }
        public abstract uint?[] ObjTypesDataPointers { get; }

        public abstract uint[] GraphicsDataPointers { get; }

        public abstract uint[] ScriptPointers { get; }
    }
    public class GBAVV_NitroKartUS_Manager : GBAVV_NitroKart_Manager
    {
        public override uint ObjTypesPointer => 0x080080d0;
        public override uint?[] ObjTypesDataPointers => new uint?[]
        {
            0x08086D94, // 0
            0x08086D94, // 1
            0x08086D94, // 2
            null, // 3
            null, // 4
            null, // 5
            null, // 6
            null, // 7
            null, // 8
            null, // 9
            null, // 10
            null, // 11
            null, // 12
            0x08086D94, // 13
            null, // 14
            null, // 15
            0x08086D94, // 16
            0x08086D94, // 17
            0x08086D94, // 18
            0x08086D94, // 19
            0x08086D94, // 20
            0x08086D94, // 21
            0x08086D94, // 22
            0x08086D94, // 23
            0x08086D94, // 24
            0x08086D94, // 25
            0x08086D94, // 26
            0x08086D94, // 27
            0x08086D94, // 28
            0x08086D94, // 29
            0x08086D94, // 30
            0x080867D8, // 31
            0x0808680C, // 32
            0x08086840, // 33
            0x08033A94, // 34
            0x08033AD8, // 35
            0x08033B1C, // 36
            0x08033A50, // 37
            0x080338FC, // 38
            0x08033940, // 39
            0x08033B60, // 40
            0x08033984, // 41
            0x080339C8, // 42
            0x08033A0C, // 43
            0x080608A4, // 44
            0x080602F4, // 45
            0x0806035C, // 46
            0x08060738, // 47
            0x08060328, // 48
            0x08060704, // 49
            0x08060390, // 50
            0x080603C4, // 51
            0x080603F8, // 52
            0x0806042C, // 53
            0x08060460, // 54
            0x08060494, // 55
            0x080604C8, // 56
            0x0806069C, // 57
            0x080606D0, // 58
            0x080604FC, // 59
            0x0806076C, // 60
            0x080607A0, // 61
            0x080607D4, // 62
            0x08060808, // 63
            0x08060530, // 64
            0x0806083C, // 65
            0x08060870, // 66
            0x08060564, // 67
            0x08060598, // 68
            0x08060634, // 69
            0x08060668, // 70
            0x080605CC, // 71
            0x08060600, // 72
            0x08060940, // 73
            0x08060974, // 74
            0x080609A8, // 75
            0x080609E8, // 76
            0x08033BE4, // 77
            0x08033C18, // 78
            0x08086D94, // 79
            0x08086D94, // 80
            0x08086D94, // 81
            0x08086D94, // 82
            0x08086D94, // 83
            0x08086D94, // 84
            0x08086D94, // 85
            0x080868B4, // 86
            0x080868E8, // 87
            0x0808691C, // 88
            0x08086950, // 89
            0x08086984, // 90
            0x080869B8, // 91
            0x080869EC, // 92
            0x08086A20, // 93
            0x08086A54, // 94
            0x08086A88, // 95
            0x08086ABC, // 96
            0x08086AF0, // 97
            0x08086B24, // 98
            0x08086B58, // 99
            0x08086880, // 100
            0x08086B8C, // 101
            0x08086BC0, // 102
            0x08086C90, // 103
            0x08086C5C, // 104
            0x08086CC4, // 105
            0x08086CF8, // 106
            0x08086D2C, // 107
            0x08086D60, // 108
            null, // 109
            0x08086D94, // 110
            0x080608D8, // 111
            0x0806090C, // 112
            0x08086D94, // 113
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0808709C,
            0x08089A80,
            0x0808D3DC,
            0x0808F464,
            0x0808F4FC,
            0x0808FC88,
            0x08090428,
            0x08090B90,
            0x08090C78,
            0x08091404,
            0x08091874,
            0x08092014,
            0x08093DA4,
            0x08094510,
            0x08094D08,
            0x0809588C,
            0x08096018,
            0x080967A8,
            0x08096F64,
            0x0809770C,
            0x08098570,
            0x0809CE8C,
            0x0809D600,
            0x0809DD8C,
            0x080A05AC,
            0x080A0C9C,
            0x080A143C,
            0x080A14F4
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08032FBC, // script_waitForInputOrTime
            0x0805CA50, // movie_intro
            0x0805CBEC, // movie_garage
            0x0805CC48, // movie_credits
            0x0805CCDC, // movie_gameIntro
            0x0805D240, // movie_earthBossIntro
            0x0805D3D8, // movie_earthBossCrashWin
            0x0805D624, // movie_earthBossEvilWin
            0x0805D810, // movie_barinBossIntro
            0x0805D9A8, // movie_barinBossCrashWin
            0x0805DB94, // movie_barinBossEvilWin
            0x0805DDE0, // movie_fenomBossIntro
            0x0805DF9C, // movie_fenomBossCrashWin
            0x0805E1B8, // movie_fenomBossEvilWin
            0x0805E3F8, // movie_tekneeBossIntro
            0x0805E570, // movie_tekneeBossCrashWin
            0x0805E6A8, // movie_tekneeBossEvilWin
            0x0805E7DC, // movie_veloBossIntro
            0x0805E89C, // movie_veloBossCrashWin
            0x0805EBFC, // movie_veloBossEvilWin
            0x0805EDE4, // movie_findFakeCrash
            0x0805EF2C, // SCRIPT_pagedTextLoop
            0x08064F1C, // script_license
            0x08064F5C, // script_intro
            0x08064F9C, // script_credits
            0x08064FE4, // script_findFakeCrash
            0x0806502C, // script_earthBossIntro
            0x08065078, // script_earthBossCrashWin
            0x080650C0, // script_earthBossEvilWin
            0x08065108, // script_barinBossIntro
            0x08065154, // script_barinBossCrashWin
            0x0806519C, // script_barinBossEvilWin
            0x080651E4, // script_fenomBossIntro
            0x08065230, // script_fenomBossCrashWin
            0x08065278, // script_fenomBossEvilWin
            0x080652C0, // script_tekneeBossIntro
            0x0806530C, // script_tekneeBossCrashWin
            0x08065358, // script_tekneeBossEvilWin
            0x080653A0, // script_veloBossIntro
            0x080653E8, // script_veloBossCrashWin
            0x08065430, // script_veloBossEvilWin
        };
    }
}