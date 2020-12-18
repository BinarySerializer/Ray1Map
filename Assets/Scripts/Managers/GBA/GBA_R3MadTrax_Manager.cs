using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class GBA_R3MadTrax_Manager : GBA_Manager
    {
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new GameAction("Export Mad Trax Sprites", false, true, (input, output) => ExportSpritesAsync(settings, output)), 
            }).ToArray();
        }

        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
            Enumerable.Range(0, 1),
        };

        public override string GetROMFilePath(Context context) => $"{(Files)context.Settings.World}.bin";

        public enum Files
        {
            client_pad_english,
            client_pad_french,
            client_pad_german,
            client_pad_italian,
            client_pad_spanish,

            // EU only
            client_pad145,
            client_pad2,
            client_pad3,
        }

        public override int[] MenuLevels => new int[0];
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => new int[0];
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new NotImplementedException();

        public async UniTask ExportSpritesAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Context(settings))
            {
                var s = context.Deserializer;
                int tileLength = CellSize * CellSize / 2;

                foreach (var world in GetLevels(settings).First().Worlds.Where(x => x.Maps.Length > 0).Select(x => x.Index))
                {
                    settings.World = world;
                    var filePath = GetROMFilePath(context);
                    var file = (Files)world;

                    var pointerTable = PointerTables.GBA_PointerTable(context, await context.AddMemoryMappedFile(filePath, GBA_ROMBase.Address_ROM));

                    s.Goto(pointerTable[GBA_Pointer.MadTrax_Sprites]);

                    int index = 0;

                    exportSprites(width: 8, height: 8, length: 9); // Gumsi
                    exportSprites(width: 8, height: 8, length: 9); // Rayman
                    exportSprites(width: 2, height: 4, length: 5); // Big blocks

                    if (file == Files.client_pad2 || file == Files.client_pad3)
                    {
                        exportSprites(width: 2, height: 4, length: 2); // Big blocks (are these correct?)
                        exportSprites(width: 2, height: 4, length: 5); // Blocks
                        exportSprites(width: 1, height: 2, length: 2); // Effect
                        exportSprites(width: 4, height: 4, length: 1);
                        exportSprites(width: 2, height: 2, length: 4);
                        exportSprites(width: 2, height: 2, length: 1);
                        exportSprites(width: 1, height: 1, length: 3);

                        exportSprites(width: 8, height: 8, length: 1); // ?
                        s.Goto(s.CurrentPointer + 0x20 * 1); // ?

                        exportSprites(width: 4, height: 8, assmbleWidth: 8, assembleHeight: 2, length: 1); // Controls screen
                    }
                    else
                    {
                        exportSprites(width: 4, height: 4, length: 1); // Big blocks
                        exportSprites(width: 2, height: 4, length: 1); // Missile
                        exportSprites(width: 4, height: 4, length: file == Files.client_pad145 ? 6 : 21); // Blocks
                        exportSprites(width: 2, height: 4, length: 6); // Missiles
                        exportSprites(width: 2, height: 2, length: 1); // Missiles
                        exportSprites(width: 2, height: 1, length: 1); // Effect
                        exportSprites(width: 2, height: 4, length: 1); // Vertical bar

                        if (file != Files.client_pad145)
                            exportSprites(width: 2, height: 1, length: 1); // Arrow

                        exportSprites(width: 2, height: 2, length: 4);

                        exportSprites(width: 4, height: 4, length: 7); // Explosion 
                        exportSprites(width: 2, height: 2, length: 1);
                        exportSprites(width: 4, height: 4, length: 1);
                        exportSprites(width: 1, height: 1, length: 3);

                        s.Goto(s.CurrentPointer + 0x20 * 4); // ?

                        exportSprites(width: 4, height: 4, assmbleWidth: 8, assembleHeight: 5, length: 1); // Controls screen
                    }

                    if (file == Files.client_pad145 || file == Files.client_pad2 || file == Files.client_pad3)
                        exportSprites(width: 4, height: 8, assmbleWidth: 8, assembleHeight: 1, length: 1); // Restart

                    if (file == Files.client_pad2 || file == Files.client_pad3)
                        exportSprites(width: 4, height: 8, assmbleWidth: 8, assembleHeight: 1, length: 1); // Try again
                    else
                        exportSprites(width: 8, height: 8, length: 1); // Fail

                    if (file == Files.client_pad145 || file == Files.client_pad2 || file == Files.client_pad3)
                        exportSprites(width: 4, height: 8, assmbleWidth: 8, assembleHeight: 1, length: 1); // Thank you
                    else
                        exportSprites(width: 4, height: 4, assmbleWidth: 8, assembleHeight: 3, length: 1); // Win

                    exportSprites(width: 4, height: 8, assmbleWidth: 8, assembleHeight: 1, length: 1); // Connection failed

                    if (file != Files.client_pad2 && file != Files.client_pad3)
                    {
                        exportSprites(width: 4, height: 8, assmbleWidth: 4, assembleHeight: 1, length: 1); // Pause
                        exportSprites(width: 4, height: 4, length: 1); // Balloon small
                        exportSprites(width: 8, height: 8, length: 1); // Balloon big
                        exportSprites(width: 8, height: 4, length: 4); // 3, 2, 1, Go!
                        exportSprites(width: 2, height: 2, length: 9); // 1-9
                        exportSprites(width: 4, height: 2, assmbleWidth: 2, assembleHeight: 1, length: 1); // Level
                    }

                    void exportSprites(int width, int height, int assmbleWidth = 1, int assembleHeight = 1, int length = 1)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            var pointer = s.CurrentPointer;

                            var pal = Util.ConvertGBAPalette(s.SerializeObjectArray<RGBA5551Color>(default, 16, name: $"Palette[{index}]"));
                            var tileData = s.SerializeArray<byte>(default, width * height * assmbleWidth * assembleHeight * tileLength, name: $"TileData[{index}]");

                            var tex = TextureHelpers.CreateTexture2D(width * assmbleWidth * CellSize, height * assembleHeight * CellSize);

                            var assembleLength = width * height * tileLength;

                            for (int assembleY = 0; assembleY < assembleHeight; assembleY++)
                            {
                                for (int assembleX = 0; assembleX < assmbleWidth; assembleX++)
                                {
                                    var assembleOffset = assembleLength * (assembleY * assmbleWidth + assembleX);

                                    for (int y = 0; y < height; y++)
                                    {
                                        for (int x = 0; x < width; x++)
                                        {
                                            var relTileOffset = tileLength * (y * width + x);

                                            tex.FillInTile(
                                                imgData: tileData,
                                                imgDataOffset: assembleOffset + relTileOffset,
                                                pal: pal,
                                                encoding: Util.TileEncoding.Linear_4bpp,
                                                tileWidth: CellSize,
                                                flipTextureY: true,
                                                tileX: CellSize * (assembleX * width + x),
                                                tileY: CellSize * (assembleY * height + y));
                                        }
                                    }
                                }
                            }


                            Util.ByteArrayToFile(Path.Combine(outputDir, file.ToString(), $"Sprite_{index++}_0x{pointer.AbsoluteOffset:X8}.png"), tex.EncodeToPNG());
                        }
                    }
                }
            }
        }
    }
}