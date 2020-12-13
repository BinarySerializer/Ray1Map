using Cysharp.Threading.Tasks;
using ImageMagick;
using R1Engine.Serialize;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class SNES_Prototype_Manager : IGameManager
    {
        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, new int[]
            {
                0
            })
        });

        public virtual string GetROMFilePath => $"ROM.sfc";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Sprites", false, true, (input, output) => ExportSpritesAsync(settings, output)),
            new GameAction("Export Animation Frames", false, true, (input, output) => ExportAnimFramesAsync(settings, output, false)),
            new GameAction("Export Animations as GIF", false, true, (input, output) => ExportAnimFramesAsync(settings, output, true)),
        };

        public async UniTask ExportSpritesAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Context(settings))
            {
                // Load rom
                await LoadFilesAsync(context);
                var rom = FileFactory.Read<SNES_Proto_ROM>(GetROMFilePath, context);

                var sprites = GetSprites(rom);

                // Export every sprite
                for (int i = 0; i < sprites.Length; i++)
                {
                    var spriteIndex = i % rom.Rayman.ImageDescriptors.Length;
                    var vramConfig = i / rom.Rayman.ImageDescriptors.Length;
                    var imgDescriptor = rom.Rayman.ImageDescriptors[spriteIndex];
                    var sprite = sprites[i];
                    if(sprite == null) continue;

                    var xPos = imgDescriptor.TileIndex % 16;
                    var yPos = (imgDescriptor.TileIndex - xPos) / 16;
                    
                    var width = (int)sprite.rect.width;
                    var height = (int)sprite.rect.height;

                    var newTex = TextureHelpers.CreateTexture2D(width, height);

                    var flipX = imgDescriptor.FlipX;
                    var flipY = !imgDescriptor.FlipY;
                    
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            newTex.SetPixel(flipX ? width - x - 1 : x, (!flipY) ? height - y - 1 : y, sprite.texture.GetPixel((int)sprite.rect.x + x, (int)sprite.rect.y + y));
                        }
                    }

                    newTex.Apply();

                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{spriteIndex} - {vramConfig}.png"), newTex.EncodeToPNG());
                }
            }
        }

        public async UniTask ExportAnimFramesAsync(GameSettings settings, string outputDir, bool saveAsGif)
        {
            using (var context = new Context(settings))
            {
                // Load rom
                await LoadFilesAsync(context);
                var rom = FileFactory.Read<SNES_Proto_ROM>(GetROMFilePath, context);

                var sprites = GetSprites(rom);

                var animIndex = 0;

                foreach (var stateGroup in rom.Rayman.States.GroupBy(x => x.Animation))
                {
                    // Get the animation
                    var anim = stateGroup.Key;
                    var layersPerFrame = anim.LayersPerFrame;
                    var frameCount = anim.FrameCount;
                    string animPointer = String.Format("{0:X4}", (stateGroup.Key.Offset.FileOffset + 4) % 0x8000 + 0x8000);
                    int vramConfig = stateGroup.First().VRAMConfigIndex;
                    var spriteOffset = rom.Rayman.ImageDescriptors.Length * vramConfig;

                    // Calculate frame size
                    int minX = anim.Layers.Where(x => sprites[x.ImageIndex] != null).Min(x => x.XPosition);
                    int minY = anim.Layers.Where(x => sprites[x.ImageIndex] != null).Min(x => x.YPosition);
                    int frameWidth = (int)anim.Layers.Where(x => sprites[x.ImageIndex] != null).Max(x => sprites[x.ImageIndex].rect.width + x.XPosition);
                    int frameHeight = (int)anim.Layers.Where(x => sprites[x.ImageIndex] != null).Max(x => sprites[x.ImageIndex].rect.height + x.YPosition);

                    // Create frame textures
                    var frames = new Texture2D[frameCount];

                    // Create each animation frame
                    for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                    {
                        var tex = TextureHelpers.CreateTexture2D(frameWidth - minX, frameHeight - minY, clear: true);

                        // Write each layer
                        for (var layerIndex = 0; layerIndex < layersPerFrame; layerIndex++)
                        {
                            var animationLayer = anim.Layers[frameIndex * layersPerFrame + layerIndex];

                            if ((spriteOffset + animationLayer.ImageIndex) >= sprites.Length)
                                continue;

                            // Get the sprite
                            var sprite = sprites[spriteOffset + animationLayer.ImageIndex];

                            if (sprite == null)
                                continue;

                            // Set every pixel
                            for (int y = 0; y < sprite.rect.height; y++)
                            {
                                for (int x = 0; x < sprite.rect.width; x++)
                                {
                                    var c = sprite.texture.GetPixel((int)sprite.rect.x + x, (int)sprite.rect.y + y);

                                    var xPosition = (animationLayer.IsFlippedHorizontally ? (sprite.rect.width - 1 - x) : x) + animationLayer.XPosition;
                                    var yPosition = (!animationLayer.IsFlippedVertically ? (sprite.rect.height - 1 - y) : y) + animationLayer.YPosition;

                                    xPosition -= minX;
                                    yPosition -= minY;

                                    if (c.a != 0)
                                        tex.SetPixel((int)xPosition, (int)(tex.height - yPosition - 1), c);
                                }
                            }
                        }

                        tex.Apply();

                        frames[frameIndex] = tex;
                    }

                    // Export animation
                    if (saveAsGif)
                    {
                        var speeds = stateGroup.Select(x => x.AnimSpeed).Distinct();

                        foreach (var speed in speeds)
                        {
                            using (MagickImageCollection collection = new MagickImageCollection())
                            {
                                int index = 0;

                                foreach (var tex in frames)
                                {
                                    var img = tex.ToMagickImage();
                                    collection.Add(img);
                                    collection[index].AnimationDelay = speed;
                                    collection[index].AnimationTicksPerSecond = 60;
                                    collection[index].Trim();

                                    collection[index].GifDisposeMethod = GifDisposeMethod.Background;
                                    index++;
                                }

                                // Save gif
                                collection.Write(Path.Combine(outputDir, $"{animIndex} ({speed}) - {animPointer}.gif"));
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < frames.Length; i++)
                            Util.ByteArrayToFile(Path.Combine(outputDir, $"{animIndex}", $"{i}.png"), frames[i].EncodeToPNG());
                    }

                    animIndex++;
                }
            }
        }

        public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = $"Loading data";
            await Controller.WaitIfNecessary();

            // Read the rom
            var rom = FileFactory.Read<SNES_Proto_ROM>(GetROMFilePath, context);

            Controller.DetailedState = $"Loading maps";
            await Controller.WaitIfNecessary();

            // Get the tilesets
            var tileSet_0000 = LoadTileSet(rom.TileSet_0000, rom.TilePalette, false, true);
            var tileSet_8000 = LoadTileSet(rom.TileSet_8000, rom.TilePalette, true, false);

            // Load the primary map
            var map = LoadMap(rom.BG1_Map, rom.BG1_Tiles).Select(x =>
            {
                x.TileMapY = (ushort)(x.PaletteIndex * tileSet_0000.SNES_BaseLength + x.TileMapY);
                return new Unity_Tile(x);
            }).ToArray();

            var maps = new Unity_Map[]
            {
                // Background
                new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics,
                    Width = 32,
                    Height = 32,
                    TileSet = new Unity_TileSet[]
                    {
                        tileSet_8000
                    },
                    MapTiles = rom.BG3_Tiles.Select(x =>
                    {
                        x.TileMapY = (ushort)(x.PaletteIndex * tileSet_8000.SNES_BaseLength + x.TileMapY);
                        return new Unity_Tile(x);
                    }).ToArray(),
                },
                // Map (no priority)
                new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics,
                    Width = (ushort)(rom.BG1_Map.Width * 2),
                    Height = (ushort)(rom.BG1_Map.Height * 2),
                    TileSet = new Unity_TileSet[]
                    {
                        tileSet_0000
                    },
                    MapTiles = map,
                },
                // Map (priority)
                new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics,
                    Width = (ushort)(rom.BG1_Map.Width * 2),
                    Height = (ushort)(rom.BG1_Map.Height * 2),
                    TileSet = new Unity_TileSet[]
                    {
                        tileSet_0000
                    },
                    MapTiles = map.Select(x => !x.Data.Priority ? new Unity_Tile(new MapTile()) : x).ToArray(),
                    Layer = Unity_Map.MapLayer.Front,
                    Alpha = 0.5f
                },
                // Foreground
                new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics,
                    Width = 32,
                    Height = 32,
                    TileSet = new Unity_TileSet[]
                    {
                        tileSet_0000
                    },
                    MapTiles = rom.BG2_Tiles.Select(x =>
                    {
                        x.TileMapY = (ushort)(x.PaletteIndex * tileSet_0000.SNES_BaseLength + x.TileMapY);
                        return new Unity_Tile(x);
                    }).ToArray(),
                },
                // Collision
                new Unity_Map()
                {
                    Type = Unity_Map.MapType.Collision,
                    Width = rom.BG1_Map.Width,
                    Height = rom.BG1_Map.Height,
                    TileSet = new Unity_TileSet[0],
                    MapTiles = rom.BG1_Map.Tiles.Select(x => new Unity_Tile(x)).ToArray()
                },
            };

            Controller.DetailedState = $"Loading sprites";
            await Controller.WaitIfNecessary();

            // Load sprites
            var sprites = GetSprites(rom);

            var objManager = new Unity_ObjectManager_SNES(context,
                rom.Rayman.States.Select(x => new Unity_ObjectManager_SNES.State(x,
                x.Animation.ToCommonAnimation(baseSpriteIndex: x.VRAMConfigIndex * rom.Rayman.ImageDescriptors.Length))).ToArray(), sprites);

            // Create Rayman
            var rayman = new Unity_Object_SNES(rom.Rayman, objManager);

            // Convert levelData to common level format
            Unity_Level level = new Unity_Level(
                maps: maps, 
                objManager: new Unity_ObjectManager(context),
                getCollisionTypeNameFunc: x => ((R1Jaguar_TileCollisionType)x).ToString(),
                getCollisionTypeGraphicFunc: x => ((R1Jaguar_TileCollisionType)x).GetCollisionTypeGraphic(),
                rayman: rayman,
                cellSize: 8)
            {
                CellSizeOverrideCollision = 16
            };

            return level;
        }

        public Sprite[] GetSprites(SNES_Proto_ROM rom) {
            var sprites = new Sprite[rom.Rayman.ImageDescriptors.Length * 3];

            var pal = Util.ConvertAndSplitGBAPalette(rom.SpritePalette);
            var buffer = new byte[rom.SpriteTileSet.Length];
            for (int addBlock = 0; addBlock < 3; addBlock++) {
                Array.Copy(rom.SpriteTileSet, buffer, buffer.Length);
                switch (addBlock) {
                    case 0:
                        Array.Copy(rom.SpriteTileSetAdd0, 0, buffer, 0xC00, 0x400);
                        Array.Copy(rom.SpriteTileSetAdd0, 0x400, buffer, 0x1000, 0x100);
                        Array.Copy(rom.SpriteTileSetAdd0, 0x500, buffer, 0x1200, 0x100);
                        break;
                    case 1:
                        Array.Copy(rom.SpriteTileSetAdd1, 0, buffer, 0xC00, 0x400);
                        Array.Copy(rom.SpriteTileSetAdd1, 0x400, buffer, 0x1000, 0x100);
                        Array.Copy(rom.SpriteTileSetAdd1, 0x500, buffer, 0x1200, 0x100);
                        break;
                    case 2:
                        Array.Copy(rom.SpriteTileSetAdd2, 0, buffer, 0xC00, 0x400);
                        break;
                }



                var tileSets = pal.Select(x => Util.ToTileSetTexture(buffer, x, Util.TileEncoding.Planar_4bpp, 8, true, wrap: 16, flipTileX: true)).ToArray();

                for (int i = 0; i < rom.Rayman.ImageDescriptors.Length; i++) {
                    if (i == 0) {
                        sprites[i] = null;
                        continue;
                    }

                    var imgDescriptor = rom.Rayman.ImageDescriptors[i];

                    var xPos = imgDescriptor.TileIndex % 16;
                    var yPos = (imgDescriptor.TileIndex - xPos) / 16;
                    var size = imgDescriptor.IsLarge ? 16 : 8;

                    sprites[addBlock * rom.Rayman.ImageDescriptors.Length + i] = tileSets[imgDescriptor.Palette].CreateSprite(new Rect(xPos * 8, tileSets[imgDescriptor.Palette].height - yPos * 8 - size, size, size));
                }
            }
            return sprites;
        }

        public MapTile[] LoadMap(MapData map, MapTile[] tiles8)
        {
            var output = new MapTile[map.Width * 2 * map.Height * 2];

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var actualX = x * 2;
                    var actualY = y * 2;

                    var mapTile = map.Tiles[y * map.Width + x];

                    setTileAt(actualX, actualY, 0, 0, tiles8[mapTile.TileMapY * 4 + 0]);
                    setTileAt(actualX, actualY, 1, 0, tiles8[mapTile.TileMapY * 4 + 1]);
                    setTileAt(actualX, actualY, 0, 1, tiles8[mapTile.TileMapY * 4 + 2]);
                    setTileAt(actualX, actualY, 1, 1, tiles8[mapTile.TileMapY * 4 + 3]);

                    void setTileAt(int baseX, int baseY, int offX, int offY, MapTile tile)
                    {
                        var newTile = new MapTile()
                        {
                            TileMapY = tile.TileMapY,
                            Priority = tile.Priority,
                            PaletteIndex = tile.PaletteIndex,
                            HorizontalFlip = tile.HorizontalFlip,
                            VerticalFlip = tile.VerticalFlip
                        };

                        if (mapTile.HorizontalFlip)
                        {
                            offX = offX == 1 ? 0 : 1;
                            newTile.HorizontalFlip = !newTile.HorizontalFlip;
                        }

                        if (mapTile.VerticalFlip)
                        {
                            offY = offY == 1 ? 0 : 1;
                            newTile.VerticalFlip = !newTile.VerticalFlip;
                        }

                        var outputX = baseX + offX;
                        var outputY = baseY + offY;

                        output[outputY * (map.Width * 2) + outputX] = newTile;
                    }
                }
            }

            return output;
        }

        public Unity_TileSet LoadTileSet(byte[] tileSet, RGBA5551Color[] palette, bool is2bpp, bool flipX)
        {
            var pal = is2bpp ? Util.ConvertAndSplitGBCPalette(palette) : Util.ConvertAndSplitGBAPalette(palette);

            int numPalettes = pal.Length;
            const int wrap = 64;
            int bpp = is2bpp ? 2 : 4;
            const int tileWidth = 8;
            int tileSize = tileWidth * tileWidth * bpp / 8;
            int tilesetLength = tileSet.Length / tileSize;

            int tilesX = Math.Min(tilesetLength * numPalettes, wrap);
            int tilesY = Mathf.CeilToInt(tilesetLength * numPalettes / (float)wrap);

            var tex = TextureHelpers.CreateTexture2D(tilesX * tileWidth, tilesY * tileWidth);

            for (int p = 0; p < numPalettes; p++)
            {
                for (int i = 0; i < tilesetLength; i++)
                {
                    int tileInd = i + p * tilesetLength;
                    int tileY = (tileInd / wrap) * tileWidth;
                    int tileX = (tileInd % wrap) * tileWidth;

                    tex.FillInTile(
                        imgData: tileSet, 
                        imgDataOffset: i * tileSize, 
                        pal: pal[p], 
                        encoding: is2bpp ? Util.TileEncoding.Planar_2bpp : Util.TileEncoding.Planar_4bpp, 
                        tileWidth: tileWidth, 
                        flipTextureY: false, 
                        tileX: tileX, 
                        tileY: tileY,
                        flipTileX: flipX);
                }
            }

            tex.Apply();

            return new Unity_TileSet(tex, tileWidth)
            {
                SNES_BaseLength = tilesetLength
            };
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context) => await context.AddLinearSerializedFileAsync(GetROMFilePath);
    }
}