using Cysharp.Threading.Tasks;
using ImageMagick;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
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

                sprites = GetSprites(rom, CustomImageDescriptors);

                foreach (var anim in CustomAnimations) {
                    // Get the animation
                    var layersPerFrame = anim.LayersPerFrame;
                    var frameCount = anim.FrameCount;
                    int vramConfig = 0;
                    var spriteOffset = rom.Rayman.ImageDescriptors.Length * vramConfig;

                    // Calculate frame size
                    int minX = anim.Layers.Where(x => sprites[x.ImageIndex] != null).Min(x => x.XPosition);
                    int minY = anim.Layers.Where(x => sprites[x.ImageIndex] != null).Min(x => x.YPosition);
                    int frameWidth = (int)anim.Layers.Where(x => sprites[x.ImageIndex] != null).Max(x => sprites[x.ImageIndex].rect.width + x.XPosition);
                    int frameHeight = (int)anim.Layers.Where(x => sprites[x.ImageIndex] != null).Max(x => sprites[x.ImageIndex].rect.height + x.YPosition);

                    // Create frame textures
                    var frames = new Texture2D[frameCount];

                    // Create each animation frame
                    for (int frameIndex = 0; frameIndex < frameCount; frameIndex++) {
                        var tex = TextureHelpers.CreateTexture2D(frameWidth - minX, frameHeight - minY, clear: true);

                        // Write each layer
                        for (var layerIndex = 0; layerIndex < layersPerFrame; layerIndex++) {
                            var animationLayer = anim.Layers[frameIndex * layersPerFrame + layerIndex];

                            if ((spriteOffset + animationLayer.ImageIndex) >= sprites.Length)
                                continue;

                            // Get the sprite
                            var sprite = sprites[spriteOffset + animationLayer.ImageIndex];

                            if (sprite == null)
                                continue;

                            // Set every pixel
                            for (int y = 0; y < sprite.rect.height; y++) {
                                for (int x = 0; x < sprite.rect.width; x++) {
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
                    if (saveAsGif) {
                        var speed = 8;
                        using (MagickImageCollection collection = new MagickImageCollection()) {
                            int index = 0;

                            foreach (var tex in frames) {
                                var img = tex.ToMagickImage();
                                collection.Add(img);
                                collection[index].AnimationDelay = speed;
                                collection[index].AnimationTicksPerSecond = 60;
                                collection[index].Trim();

                                collection[index].GifDisposeMethod = GifDisposeMethod.Background;
                                index++;
                            }

                            // Save gif
                            collection.Write(Path.Combine(outputDir, $"{animIndex} ({speed}).gif"));
                        }
                    } else {
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
            var tileSet_0000_shadow = LoadTileSet(rom.TileSet_0000, rom.TilePalette, false, true, animatedTiles: rom.AnimatedTiles, shadow: true);
            var tileSet_0000 = LoadTileSet(rom.TileSet_0000, rom.TilePalette, false, true, animatedTiles: rom.AnimatedTiles);
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
                    MapTiles = map.Select(x => x.Data.Priority ? new Unity_Tile(new MapTile()) : x).ToArray(),
                },
                // Map (no priority)
                new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics,
                    Width = (ushort)(rom.BG1_Map.Width * 2),
                    Height = (ushort)(rom.BG1_Map.Height * 2),
                    TileSet = new Unity_TileSet[]
                    {
                        tileSet_0000_shadow
                    },
                    MapTiles = map.Select(x => x.Data.Priority ? x : new Unity_Tile(new MapTile())).ToArray(),
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
                    IsAdditive = true,
                    Alpha = 1f
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
                    Layer = Unity_Map.MapLayer.Front,
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

        public Sprite[] GetSprites(SNES_Proto_ROM rom, SNES_Proto_ImageDescriptor[] imageDescriptors = null) {
            if(imageDescriptors == null) imageDescriptors = rom.Rayman.ImageDescriptors;
            var sprites = new Sprite[imageDescriptors.Length * 3];

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

                for (int i = 0; i < imageDescriptors.Length; i++) {
                    if (i == 0) {
                        sprites[i] = null;
                        continue;
                    }

                    var imgDescriptor = imageDescriptors[i];

                    var xPos = imgDescriptor.TileIndex % 16;
                    var yPos = (imgDescriptor.TileIndex - xPos) / 16;
                    var size = imgDescriptor.IsLarge ? 16 : 8;

                    sprites[addBlock * imageDescriptors.Length + i] = tileSets[imgDescriptor.Palette].CreateSprite(new Rect(xPos * 8, tileSets[imgDescriptor.Palette].height - yPos * 8 - size, size, size));
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

        public Unity_TileSet LoadTileSet(byte[] tileSet, RGBA5551Color[] palette, bool is2bpp, bool flipX, SNES_Proto_AnimatedTileEntry[] animatedTiles = null, bool shadow = false)
        {
            var pal = is2bpp ? Util.ConvertAndSplitGBCPalette(palette) : Util.ConvertAndSplitGBAPalette(palette);

            int numPalettes = pal.Length;
            const int wrap = 64;
            int bpp = is2bpp ? 2 : 4;
            const int tileWidth = 8;
            int tileSize = tileWidth * tileWidth * bpp / 8;
            int tilesetLength = tileSet.Length / tileSize;
            int animatedTilesLength = animatedTiles?.Sum(at => at.GraphicsBuffer.Length / tileSize) ?? 0;
            int totalTilesetLength = tilesetLength + animatedTilesLength;

            int tilesX = Math.Min(totalTilesetLength * numPalettes, wrap);
            int tilesY = Mathf.CeilToInt(totalTilesetLength * numPalettes / (float)wrap);

            var tex = TextureHelpers.CreateTexture2D(tilesX * tileWidth, tilesY * tileWidth);

            for (int p = 0; p < numPalettes; p++)
            {
                for (int i = 0; i < tilesetLength; i++) {
                    int tileInd = i + p * totalTilesetLength;
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

                if (animatedTiles != null) {
                    int curAnimatedTile = 0;
                    for (int i = 0; i < animatedTiles.Length; i++) {
                        int numTiles = animatedTiles[i].GraphicsBuffer.Length / tileSize;
                        for (int t = 0; t < numTiles; t++) {
                            int tileInd = tilesetLength + curAnimatedTile + p * totalTilesetLength;
                            int tileY = (tileInd / wrap) * tileWidth;
                            int tileX = (tileInd % wrap) * tileWidth;

                            tex.FillInTile(
                                imgData: animatedTiles[i].GraphicsBuffer,
                                imgDataOffset: t * tileSize,
                                pal: pal[p],
                                encoding: is2bpp ? Util.TileEncoding.Planar_2bpp : Util.TileEncoding.Planar_4bpp,
                                tileWidth: tileWidth,
                                flipTextureY: false,
                                tileX: tileX,
                                tileY: tileY,
                                flipTileX: flipX);
                            curAnimatedTile++;
                        }
                    }
                }
            }
            if (shadow) {
                var colors = tex.GetPixels();
                colors = colors.Select(c => c.a == 0 ? c : Color.black).ToArray();
                tex.SetPixels(colors);
            }

            tex.Apply();
            Unity_AnimatedTile[] unityAnimatedTiles = null;
            if (animatedTiles != null) {
                int curAnimatedTile = 0;
                var animTilesDict = new Dictionary<int, List<int>>();
                foreach (var at in animatedTiles) {
                    var tileInd = (at.VRAMAddress * 2) / 0x20;
                    int numTiles = at.GraphicsBuffer.Length / tileSize;
                    for (int i = 0; i < numTiles; i++) {
                        int key = tileInd + i;
                        if (!animTilesDict.ContainsKey(key)) {
                            animTilesDict[key] = new List<int>();
                            animTilesDict[key].Add(key);
                        }
                        animTilesDict[key].Add(tilesetLength + curAnimatedTile);
                        curAnimatedTile++;
                    }
                }
                var unityAnimTilesList = new List<Unity_AnimatedTile>();
                for (int p = 0; p < numPalettes; p++) {
                    foreach (var kv in animTilesDict) {
                        Unity_AnimatedTile newAT = new Unity_AnimatedTile() {
                            AnimationSpeed = 2,
                            TileIndices = kv.Value.Select(t => t + p * totalTilesetLength).ToArray()
                        };
                        //Debug.Log(string.Join(",",newAT.TileIndices));
                        unityAnimTilesList.Add(newAT);
                    }
                }
                unityAnimatedTiles = unityAnimTilesList.ToArray();
            }
            return new Unity_TileSet(tex, tileWidth)
            {
                SNES_BaseLength = totalTilesetLength,
                AnimatedTiles = unityAnimatedTiles
            };
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context) => await context.AddLinearSerializedFileAsync(GetROMFilePath);

        protected SNES_Proto_ImageDescriptor[] CustomImageDescriptors => new SNES_Proto_ImageDescriptor[] {
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0x0 }, // NULL
            // Enemy (start index 1)
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE0 }, // Body 1
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE1 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE2 }, // Body 2
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE3 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF0 }, // Body 3
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF1 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF2 }, // Body 4
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF3 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE4 }, // Body 5 (vertical)
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF4 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE5 }, // Body 6 (vertical)
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF5 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE6 }, // Body surprised
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xE7 },
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF6 }, // Stinger
            new SNES_Proto_ImageDescriptor() { Palette = 5, Priority = 2, TileIndex = 0xF7 }, // Back

            // Start index 17
        };

        protected R1Jaguar_AnimationDescriptor[] CustomAnimations => new R1Jaguar_AnimationDescriptor[] {
            // Enemy normal
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 3,
                FrameCount = 8, // 4 frames, pingponged
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 12, YPosition = 4, ImageIndex = 16 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 1 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 2 },
                    },
                    new R1_AnimationLayer[] { // Frame 1
                        new R1_AnimationLayer() { XPosition = 11, YPosition = 3, ImageIndex = 16 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 3 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 4 },
                    },
                    new R1_AnimationLayer[] { // Frame 2
                        new R1_AnimationLayer() { XPosition = 10, YPosition = 3, ImageIndex = 16 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 5 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 6 },
                    },
                    new R1_AnimationLayer[] { // Frame 3
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 3, ImageIndex = 16 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 7 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 8 },
                    },
                    new R1_AnimationLayer[] { // Frame 3
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 16 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 7 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 8 },
                    },
                    new R1_AnimationLayer[] { // Frame 2
                        new R1_AnimationLayer() { XPosition = 10, YPosition = 4, ImageIndex = 16 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 3, ImageIndex = 5 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 3, ImageIndex = 6 },
                    },
                    new R1_AnimationLayer[] { // Frame 1
                        new R1_AnimationLayer() { XPosition = 11, YPosition = 4, ImageIndex = 16 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 3, ImageIndex = 3 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 3, ImageIndex = 4 },
                    },
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 12, YPosition = 4, ImageIndex = 16 },
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 3, ImageIndex = 1 },
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 3, ImageIndex = 2 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            // Enemy surprised / stinger
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 4,
                FrameCount = 6,
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 12, YPosition = 3, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 16, YPosition = 3, ImageIndex = 15 }, // Stinger
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 2, ImageIndex = 13 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 2, ImageIndex = 14 },
                    },
                    new R1_AnimationLayer[] { // Frame 1
                        new R1_AnimationLayer() { XPosition = 12, YPosition = 2, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 17, YPosition = 3, ImageIndex = 15 }, // Stinger
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 1, ImageIndex = 13 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 1, ImageIndex = 14 },
                    },
                    new R1_AnimationLayer[] { // Frame 2
                        new R1_AnimationLayer() { XPosition = 12, YPosition = 2, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 18, YPosition = 2, ImageIndex = 15 }, // Stinger
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 2, ImageIndex = 13 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 2, ImageIndex = 14 },
                    },
                    new R1_AnimationLayer[] { // Frame 3
                        new R1_AnimationLayer() { XPosition = 12, YPosition = 3, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 18, YPosition = 2, ImageIndex = 15 }, // Stinger
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 13 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 14 },
                    },
                    new R1_AnimationLayer[] { // Frame 4
                        new R1_AnimationLayer() { XPosition = 12, YPosition = 4, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 17, YPosition = 2, ImageIndex = 15 }, // Stinger
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 13 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 14 },
                    },
                    new R1_AnimationLayer[] { // Frame 5
                        new R1_AnimationLayer() { XPosition = 12, YPosition = 4, ImageIndex = 16 }, // Back
                        new R1_AnimationLayer() { XPosition = 18, YPosition = 3, ImageIndex = 0 }, // Stinger
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 4, ImageIndex = 5 }, // Body
                        new R1_AnimationLayer() { XPosition = 8, YPosition = 4, ImageIndex = 6 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 2,
                FrameCount = 1,
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 0, ImageIndex = 9 }, // Body
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 8, ImageIndex = 10 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },
            new R1Jaguar_AnimationDescriptor() {
                LayersPerFrame = 2,
                FrameCount = 1,
                Layers = new R1_AnimationLayer[][] {
                    new R1_AnimationLayer[] { // Frame 0
                        new R1_AnimationLayer() { XPosition = 4, YPosition = 4, ImageIndex = 11 }, // Body
                        new R1_AnimationLayer() { XPosition = 0, YPosition = 8, ImageIndex = 12 },
                    },
                }.SelectMany(ls => ls).ToArray()
            },
        };
    }
}