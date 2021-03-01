using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public abstract class GBAVV_NitroKart_Manager : GBAVV_BaseManager
    {
        public override LevInfo[] LevInfos => Levels;

        public static LevInfo[] Levels = new LevInfo[]
        {
            // TODO: Names
            new LevInfo(0, "Terra Hub"), 
            new LevInfo(1, "Inferno Island"),
            new LevInfo(2, ""),
            new LevInfo(3, ""),
            new LevInfo(4, ""),
            new LevInfo(5, ""),
            new LevInfo(6, ""),
            new LevInfo(7, ""),
            new LevInfo(8, ""),
            new LevInfo(9, ""),
            new LevInfo(10, ""),
            new LevInfo(11, ""),
            new LevInfo(12, ""),
            new LevInfo(13, ""),
            new LevInfo(14, ""),
            new LevInfo(15, ""),
            new LevInfo(16, ""),
            new LevInfo(17, ""),
            new LevInfo(18, ""),
            new LevInfo(19, ""),
            new LevInfo(20, ""),
            new LevInfo(21, ""),
            new LevInfo(22, ""),
            new LevInfo(23, ""),
            new LevInfo(24, ""),
            new LevInfo(24, ""),
            new LevInfo(25, ""),
        };

        public override UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            //FindDataInROM(context.Deserializer, context.FilePointer(GetROMFilePath));
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

        public abstract uint[] GraphicsDataPointers { get; }

        public abstract uint[] ScriptPointers { get; }
    }
    public class GBAVV_NitroKartUS_Manager : GBAVV_NitroKart_Manager
    {
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