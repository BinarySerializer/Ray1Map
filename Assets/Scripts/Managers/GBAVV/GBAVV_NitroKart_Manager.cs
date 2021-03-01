using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public class GBAVV_NitroKart_Manager : GBAVV_BaseManager
    {
        public override LevInfo[] LevInfos => new LevInfo[]
        {
            new LevInfo(GBAVV_MapInfo.GBAVV_MapType.Kart, 0, ""), 
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

        public uint[] GraphicsDataPointers => new uint[]
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
    }
}