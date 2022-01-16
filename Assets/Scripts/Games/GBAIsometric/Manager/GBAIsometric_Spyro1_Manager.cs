using System;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro1_Manager : GBAIsometric_IceDragon_BaseManager
    {
        private const int World_Cutscenes = 1;

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            // TODO: Isometric levels
            // TODO: Mode7 levels
            // TODO: Sparx levels
            new GameInfo_World(World_Cutscenes, ValueRange.EnumerateRanges(new ValueRange(0, 20)).ToArray()), // Cutscenes
        });

        public override GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Resources", false, true, (input, output) => ExportResourcesAsync(settings, output, false)),
            new GameAction("Export Resources (categorized)", false, true, (input, output) => ExportResourcesAsync(settings, output, true)),
            new GameAction("Export Cutscene Maps", false, true, (input, output) => ExportCutsceneMapsAsync(settings, output)),
            new GameAction("Export Sprites (full, no pal)", false, true, (input, output) => ExportAllSpritesAsync(settings, output)),
            new GameAction("Export Font", false, true, (input, output) => ExportFont(settings, output)),
            new GameAction("Export Localization", false, true, (input, output) => ExportLocalization(settings, output)),
        };

        public async UniTask ExportResourcesAsync(GameSettings settings, string outputPath, bool categorize)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, (rom, context) =>
            {
                ExportResources(context, rom.Resources, outputPath, categorize, false);
            });
        }

        public async UniTask ExportCutsceneMapsAsync(GameSettings settings, string outputPath)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, (rom, _) =>
            {
                for (var i = 0; i < rom.CutsceneMaps.Length; i++)
                {
                    GBAIsometric_IceDragon_CutsceneMap cutscene = rom.CutsceneMaps[i];
                    Util.ByteArrayToFile(Path.Combine(outputPath, "Cutscenes", $"{i}.png"), cutscene.ToTexture2D().EncodeToPNG());
                }
            });
        }

        public async UniTask ExportAllSpritesAsync(GameSettings settings, string outputPath)
        {
            using var context = new Ray1MapContext(settings);

            BinaryDeserializer s = context.Deserializer;
            await LoadFilesAsync(context);

            Color[] pal4 = PaletteHelpers.CreateDummyPalette(16).Select(x => x.GetColor()).ToArray();

            s.Goto(context.FilePointer(GetROMFilePath) + 3);

            while (s.CurrentFileOffset < s.CurrentLength - 4)
            {
                string str = s.SerializeString(default, 3);
                s.Goto(s.CurrentPointer + 1);
                
                if (str != "CRS")
                    continue;

                try
                {
                    s.DoAt(s.CurrentPointer - 7, () =>
                    {
                        GBAIsometric_Ice_AnimSet animSet = s.SerializeObject<GBAIsometric_Ice_AnimSet>(default);

                        for (int i = 0; i < animSet.Sprites.Length; i++)
                        {
                            Texture2D tex = GetSpriteTexture(animSet, i, pal4);
                            Util.ByteArrayToFile(Path.Combine(outputPath, $"0x{animSet.Offset.StringAbsoluteOffset}", $"{i}.png"), tex.EncodeToPNG());
                        }
                    });
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"{s.CurrentPointer}: {ex}");
                }
            }
        }

        public async UniTask ExportFont(GameSettings settings, string outputPath)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, (rom, _) =>
            {
                ExportFont(rom.Localization, outputPath);
            });
        }

        public async UniTask ExportLocalization(GameSettings settings, string outputPath)
        {
            await DoGameActionAsync<GBAIsometric_Ice_ROM>(settings, (rom, context) =>
            {
                JsonHelpers.SerializeToFile(LoadLocalization(context, rom.Localization), Path.Combine(outputPath, "Localization.json"));
            });
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            Controller.DetailedState = $"Loading data";
            await Controller.WaitIfNecessary();

            GBAIsometric_Ice_ROM rom = FileFactory.Read<GBAIsometric_Ice_ROM>(context, GetROMFilePath);

            if (context.GetR1Settings().World == World_Cutscenes)
            {
                return await LoadCutsceneMapAsync(context, rom);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public Texture2D GetSpriteTexture(GBAIsometric_Ice_AnimSet animSet, int spriteIndex, Color[] pal)
        {
            // 8-bit sprites are never used
            if (animSet.Is8Bit)
                throw new InvalidOperationException("8-bit sprites are currently not supported");

            GBAIsometric_Ice_Sprite sprite = animSet.Sprites[spriteIndex];
            int shape = 0;

            if (sprite.Height < sprite.Width)
                shape = 1;
            else if (sprite.Width < sprite.Height)
                shape = 2;

            GBAConstants.Size size = GBAConstants.GetSpriteShape(shape, sprite.SpriteSize);

            Texture2D tex = TextureHelpers.CreateTexture2D(size.Width, size.Height, clear: true);

            int imgDataOffset = sprite.TileIndex * (animSet.Is8Bit ? 0x40 : 0x20);
            int tileIndex = animSet.SpriteMapLength * spriteIndex;

            for (int y = 0; y < size.Height / GBAConstants.TileSize; y++)
            {
                for (int x = 0; x < size.Width / GBAConstants.TileSize; x++)
                {
                    if (!animSet.SpriteMaps[tileIndex])
                    {
                        tileIndex++;
                        continue;
                    }

                    tex.FillInTile(
                        imgData: animSet.ImgData,
                        imgDataOffset: imgDataOffset,
                        pal: pal,
                        encoding: Util.TileEncoding.Linear_4bpp,
                        tileWidth: GBAConstants.TileSize,
                        flipTextureY: true,
                        tileX: x * GBAConstants.TileSize,
                        tileY: y * GBAConstants.TileSize);

                    imgDataOffset += animSet.Is8Bit ? 0x40 : 0x20;

                    tileIndex++;
                }
            }

            return tex;
        }
    }
}