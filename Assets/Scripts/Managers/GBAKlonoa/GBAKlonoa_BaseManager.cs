﻿using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    public abstract class GBAKlonoa_BaseManager : BaseGameManager
    {
        public const int CellSize = GBAConstants.TileSize;
        public const string GetROMFilePath = "ROM.gba";

        public const string CompressedObjTileBlockName = "CompressedObjTileBlock";
        public const string CompressedWorldObjTileBlockName = "CompressedWorldObjTileBlock";

        public Unity_TileSet LoadTileSet(byte[] tileSet, RGBA5551Color[] pal, bool is8bit, MapTile[] mapTiles_4, GBAKlonoa_ObjectGraphics[] mapGraphics = null)
        {
            Texture2D tex;
            var additionalTiles = new List<Texture2D>();
            var tileSize = is8bit ? 0x40 : 0x20;
            var paletteIndices = Enumerable.Range(0, tileSet.Length / tileSize).Select(x => new List<byte>()).ToArray();
            var tilesCount = tileSet.Length / tileSize;

            var tileAnimations = new List<Unity_AnimatedTile>();

            if (is8bit)
            {
                tex = Util.ToTileSetTexture(tileSet, Util.ConvertGBAPalette(pal), Util.TileEncoding.Linear_8bpp, CellSize, false);
            }
            else
            {
                var palettes = Util.ConvertAndSplitGBAPalette(pal);

                foreach (var m in mapTiles_4)
                {
                    if (m.TileMapY < paletteIndices.Length && !paletteIndices[m.TileMapY].Contains(m.PaletteIndex))
                        paletteIndices[m.TileMapY].Add(m.PaletteIndex);
                }

                tex = Util.ToTileSetTexture(tileSet, palettes[0], Util.TileEncoding.Linear_4bpp, CellSize, false, getPalFunc: x =>
                {
                    var p = paletteIndices[x].ElementAtOrDefault(0);
                    return palettes[p];
                });

                // Add additional tiles for tiles with multiple palettes
                for (int tileIndex = 0; tileIndex < paletteIndices.Length; tileIndex++)
                {
                    for (int palIndex = 1; palIndex < paletteIndices[tileIndex].Count; palIndex++)
                    {
                        var p = paletteIndices[tileIndex][palIndex];

                        var tileTex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                        // Create a new tile
                        tileTex.FillInTile(
                            imgData: tileSet,
                            imgDataOffset: tileSize * tileIndex,
                            pal: palettes[p],
                            encoding: Util.TileEncoding.Linear_4bpp,
                            tileWidth: CellSize,
                            flipTextureY: false,
                            tileX: 0,
                            tileY: 0);

                        // Modify all tiles where this is used
                        foreach (MapTile t in mapTiles_4.Where(x => x.TileMapY == tileIndex && x.PaletteIndex == p))
                        {
                            t.TileMapY = (ushort)(tilesCount + additionalTiles.Count);
                        }

                        // Add to additional tiles list
                        additionalTiles.Add(tileTex);
                    }
                }

                // Add animated tiles, currently only supported for 4-bit
                if (mapGraphics != null)
                {
                    foreach (GBAKlonoa_ObjectGraphics graphics in mapGraphics)
                    {
                        var count = graphics.ImgDataLength / tileSize;
                        var anim = graphics.Animations[0];

                        for (int i = 0; i < count; i++)
                        {
                            var tileAnim = new Unity_AnimatedTile();

                            var tileOffset = i * tileSize;
                            var framesCount = anim.Frames.Length;

                            // We assume the tileset we're parsing is located at the start of the VRAM
                            var tileIndex = (int)((graphics.VRAMPointer - GBAConstants.Address_VRAM + tileOffset) / tileSize);

                            tileAnim.AnimationSpeeds = new float[]
                            {
                                1
                            }.Concat(anim.Frames.Select(x => (float)(x.Speed + 1))).ToArray();
                            tileAnim.TileIndices = new int[]
                            {
                                tileIndex
                            }.Concat(Enumerable.Range(tilesCount + additionalTiles.Count, framesCount)).ToArray();
                            tileAnim.IgnoreFirstTile = true;

                            foreach (var frame in anim.Frames)
                            {
                                var tileTex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                                var tilePals = paletteIndices[tileIndex];

                                if (tilePals.Count > 1)
                                    Debug.LogWarning($"Animated tile has multiple palettes!");

                                // Create a new tile
                                tileTex.FillInTile(
                                    imgData: frame.ImgData,
                                    imgDataOffset: tileOffset,
                                    pal: palettes[tilePals.First()],
                                    encoding: Util.TileEncoding.Linear_4bpp,
                                    tileWidth: CellSize,
                                    flipTextureY: false,
                                    tileX: 0,
                                    tileY: 0);

                                // Add to additional tiles list
                                additionalTiles.Add(tileTex);
                            }

                            tileAnimations.Add(tileAnim);
                        }
                    }
                }
            }

            // Create the tile array
            var tiles = new Unity_TileTexture[tilesCount + additionalTiles.Count];

            // Keep track of the index
            var index = 0;

            // Add every normal tile
            for (int y = 0; y < tex.height; y += CellSize)
            {
                for (int x = 0; x < tex.width; x += CellSize)
                {
                    if (index >= tilesCount)
                        break;

                    // Create a tile
                    tiles[index++] = tex.CreateTile(new Rect(x, y, CellSize, CellSize));
                }
            }

            // Add additional tiles
            foreach (Texture2D t in additionalTiles)
                tiles[index++] = t.CreateTile();

            return new Unity_TileSet(tiles)
            {
                AnimatedTiles = tileAnimations.ToArray()
            };
        }

        public Unity_CollisionLine[] GetSectorCollisionLines(GBAKlonoa_MapSectors sectors)
        {
            // Add line collision for the sector bounds
            var collisionLines = new List<Unity_CollisionLine>();

            var sectorIndex = 0;

            foreach (var sector in sectors.Sectors)
            {
                var color = new Color(1, 0.92f - (0.1f * sectorIndex), 0.016f + (0.15f * sectorIndex));

                collisionLines.Add(new Unity_CollisionLine(
                    new Vector2(sector.X, sector.Y),
                    new Vector2(sector.X, sector.Y + sector.Height),
                    color));

                collisionLines.Add(new Unity_CollisionLine(
                    new Vector2(sector.X, sector.Y + sector.Height),
                    new Vector2(sector.X + sector.Width, sector.Y + sector.Height),
                    color));

                collisionLines.Add(new Unity_CollisionLine(
                    new Vector2(sector.X + sector.Width, sector.Y),
                    new Vector2(sector.X + sector.Width, sector.Y + sector.Height),
                    color));

                collisionLines.Add(new Unity_CollisionLine(
                    new Vector2(sector.X, sector.Y),
                    new Vector2(sector.X + sector.Width, sector.Y),
                    color));

                sectorIndex++;
            }

            return collisionLines.ToArray();
        }

        public Texture2D[] GetAnimFrames(GBAKlonoa_AnimationFrame[] frames, GBAKlonoa_OAM[] oamCollection, Color[][] palettes, int imgDataOffset = 0, bool singlePal = false)
        {
            var output = new Texture2D[frames.Length];

            var frameCache = new Dictionary<GBAKlonoa_AnimationFrame, Texture2D>();

            var shapes = GBAConstants.SpriteShapes;

            for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
            {
                GBAKlonoa_AnimationFrame frame = frames[frameIndex];

                if (!frameCache.ContainsKey(frame))
                {
                    var spriteRects = oamCollection.Select(x =>
                        new RectInt(x.XPos - oamCollection[0].XPos, x.YPos - oamCollection[0].YPos, shapes[x.Shape].Width, shapes[x.Shape].Height)).ToArray();

                    var tex = TextureHelpers.CreateTexture2D(spriteRects.Max(x => x.xMax), spriteRects.Max(x => x.yMax));

                    var tileIndex = 0;

                    for (int spriteIndex = 0; spriteIndex < oamCollection.Length; spriteIndex++)
                    {
                        var oam = oamCollection[spriteIndex];
                        var shape = shapes[oam.Shape];

                        Color[] pal;

                        if (singlePal)
                            pal = palettes.First();
                        else
                            pal = palettes.ElementAtOrDefault(oam.PaletteIndex) ?? Util.CreateDummyPalette(16).Select(c => c.GetColor()).ToArray();

                        for (int y = 0; y < shape.Height; y += CellSize)
                        {
                            for (int x = 0; x < shape.Width; x += CellSize)
                            {
                                tex.FillInTile(
                                    imgData: frame.ImgData,
                                    imgDataOffset: imgDataOffset + tileIndex * 0x20,
                                    pal: pal,
                                    encoding: Util.TileEncoding.Linear_4bpp,
                                    tileWidth: CellSize,
                                    flipTextureY: true,
                                    flipTextureX: false,
                                    tileX: x + spriteRects[spriteIndex].x,
                                    tileY: y + spriteRects[spriteIndex].y);

                                tileIndex++;
                            }
                        }
                    }

                    tex.Apply();

                    frameCache.Add(frame, tex);
                }

                output[frameIndex] = frameCache[frame];
            }

            return output;
        }

        public Unity_ObjectManager_GBAKlonoa.AnimSet LoadAnimSet(GBAKlonoa_Animation[] anims, GBAKlonoa_OAM[] oamCollection, Color[][] palettes, bool singlePal = false, int dct_GraphicsIndex = -1)
        {
            // Sometimes the last animation is null
            if (anims.Length > 1 && anims.Last() == null)
                anims = anims.Take(anims.Length - 1).ToArray();

            return new Unity_ObjectManager_GBAKlonoa.AnimSet(
                animations: Enumerable.Range(0, anims.Length).
                    Select(animIndex => anims[animIndex]?.Frames?.Length > 0 ? new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation(
                        animFrameFunc: () => GetAnimFrames(anims[animIndex].Frames, oamCollection, palettes, singlePal: singlePal).Select(x => x.CreateSprite()).ToArray(),
                        oamCollection: oamCollection,
                        animSpeeds: anims[animIndex]?.Frames?.Select(f => (int?)f?.Speed).ToArray(),
                        linkedAnimIndex: anims[animIndex]?.SerializedFrames?.LastOrDefault()?.LinkedAnimIndex) : null).ToArray(),
                displayName: dct_GraphicsIndex != -1 ? $"{dct_GraphicsIndex}" : $"0x{anims.First().Offset.StringAbsoluteOffset}",
                oamCollections: new GBAKlonoa_OAM[][]
                {
                    oamCollection
                },
                dct_GraphisIndex: dct_GraphicsIndex);
        }

        public void LoadAnimSets_ObjGraphics(List<Unity_ObjectManager_GBAKlonoa.AnimSet> loadedAnimSets, IEnumerable<GBAKlonoa_ObjectGraphics> animSets, GBAKlonoa_ObjectOAMCollection[] oamCollections, Color[][] palettes, IList<GBAKlonoa_LoadedObject> objects)
        {
            var loadedAnimSetsDictionary = new Dictionary<Pointer, Unity_ObjectManager_GBAKlonoa.AnimSet>();

            // Load animations declared through the graphics data
            foreach (var animSet in animSets)
            {
                var oam = oamCollections.ElementAtOrDefault(objects.ElementAtOrDefault(animSet.ObjIndex)?.OAMIndex ?? -1);

                if (animSet.AnimationsPointer == null || oam == null)
                    continue;

                if (!loadedAnimSetsDictionary.ContainsKey(animSet.AnimationsPointer))
                {
                    loadedAnimSetsDictionary[animSet.AnimationsPointer] = LoadAnimSet(animSet.Animations, oam.OAMs, palettes);
                    loadedAnimSets.Add(loadedAnimSetsDictionary[animSet.AnimationsPointer]);
                }
                else
                {
                    loadedAnimSetsDictionary[animSet.AnimationsPointer].OAMCollections.Add(oam.OAMs);
                }
            }
        }

        public void LoadAnimSets_SpecialAnims(List<Unity_ObjectManager_GBAKlonoa.AnimSet> loadedAnimSets, Context context, IEnumerable<SpecialAnimation> specialAnims, Color[][] palettes, IList<GBAKlonoa_LoadedObject> objects, GBAKlonoa_ObjectOAMCollection[] oamCollections, int fixCount)
        {
            var romFile = context.GetFile(GetROMFilePath);
            var s = context.Deserializer;
            var specialAnimIndex = 0;
            var shapes = GBAConstants.SpriteShapes;

            // Load special animations. These are all manually loaded into VRAM by the game.
            foreach (var specialAnim in specialAnims)
            {
                // If a group count is specified we read x sprites one after another assumed to be stored in VRAM in the same order
                if (specialAnim.GroupCount != null)
                {
                    var oams = objects.Where(x => x != null && specialAnim.MatchesObject(x)).Select(x => oamCollections[x.OAMIndex]).ToArray();

                    if (!oams.Any())
                        continue;

                    // We assume each object only has a single sprite (OAM)
                    int tileIndex = oams.Min(x => x.OAMs[0].TileIndex);

                    // We assume every animation has a single frame
                    var offset = specialAnim.FrameOffsets[0];

                    for (int groupSprite = 0; groupSprite < specialAnim.GroupCount.Value; groupSprite++)
                    {
                        var oam = oams.First(x => x.OAMs[0].TileIndex == tileIndex);

                        loadSpecialAnimation(new GBAKlonoa_OAM[][]
                        {
                            oam.OAMs
                        }, null, new long[]
                        {
                            offset
                        }, specialAnim.FramesCount);

                        var tilesCount = (shapes[oam.OAMs[0].Shape].Width / CellSize) * (shapes[oam.OAMs[0].Shape].Height / CellSize);
                        tileIndex += tilesCount;
                        offset += tilesCount * 0x20;
                    }
                }
                else
                {
                    // Find an object which uses this animation (filter out 0,0 objects here too since some unused objects get placed there with wrong graphics...)
                    var oams = objects.Where(x => x != null && (!(x.XPos == 0 && x.YPos == 0) || x.Index < fixCount) && specialAnim.MatchesObject(x)).Select(x => oamCollections[x.OAMIndex].OAMs).ToArray();

                    if (!oams.Any())
                        continue;

                    loadSpecialAnimation(oams, specialAnim.Offset, specialAnim.FrameOffsets, specialAnim.FramesCount);
                }

                void loadSpecialAnimation(IReadOnlyList<GBAKlonoa_OAM[]> oams, long? offset, IReadOnlyList<long> frameOffsets, int framesCount)
                {
                    var imgDataLength = oams[0].Select(o => (shapes[o.Shape].Width / CellSize) * (shapes[o.Shape].Height / CellSize) * 0x20).Sum();
                    var animsPointer = new Pointer(offset ?? frameOffsets[0], romFile);

                    Pointer[] framePointers;

                    if (offset != null)
                        framePointers = s.DoAt(animsPointer, () => s.SerializePointerArray(null, framesCount, name: $"SpecialAnimations[{specialAnimIndex}]"));
                    else
                        framePointers = frameOffsets.Select(p => new Pointer(p, romFile)).ToArray();

                    var frames = new GBAKlonoa_AnimationFrame[framePointers.Length];

                    for (int frameIndex = 0; frameIndex < framePointers.Length; frameIndex++)
                    {
                        var imgData = s.DoAt(framePointers[frameIndex], () => s.SerializeArray<byte>(null, imgDataLength, name: $"SpecialAnimations[{specialAnimIndex}][{frameIndex}]"));

                        if (imgData == null)
                            throw new Exception($"Image data is null for special animation! Pointer: {framePointers[frameIndex]}");

                        frames[frameIndex] = new GBAKlonoa_AnimationFrame()
                        {
                            ImgData = imgData
                        };
                    }

                    var animSet = new Unity_ObjectManager_GBAKlonoa.AnimSet(new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation[]
                    {
                        new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation(() => GetAnimFrames(frames, oams[0], palettes).Select(x => x.CreateSprite()).ToArray(), oams[0])
                    }, $"0x{animsPointer.StringAbsoluteOffset}", oams);

                    loadedAnimSets.Add(animSet);

                    specialAnimIndex++;
                }
            }
        }

        public void CorrectWorldMapObjectPositions(IEnumerable<Unity_Object> objects, int mapWidth, int mapHeight)
        {
            foreach (var o in objects)
            {
                var x = o.XPosition;
                var y = o.YPosition;
                var fx = mapWidth * CellSize;
                var fy = mapHeight * CellSize;

                var cos = Mathf.Cos(((y / 128f) + 0.5f) * Mathf.PI) * x;
                var sin = Mathf.Sin(((y / 128f) + 0.5f) * Mathf.PI) * x;
                o.XPosition = (short)(cos + fx / 2);
                o.YPosition = (short)(sin + fy / 2);
            }
        }

        public override async UniTask LoadFilesAsync(Context context) => await context.AddMemoryMappedFile(GetROMFilePath, GBAConstants.Address_ROM);

        public abstract AnimSetInfo[] AnimSetInfos { get; }

        public class AnimSetInfo
        {
            public AnimSetInfo(long offset, int animCount)
            {
                Offset = offset;
                AnimCount = animCount;
            }

            public long Offset { get; }
            public int AnimCount { get; }
        }

        public class SpecialAnimation
        {
            public SpecialAnimation(long framesArrayOffset, int framesCount, bool isFix, int index)
            {
                Offset = framesArrayOffset;
                FramesCount = framesCount;
                IsFix = isFix;
                Index = index;
            }
            public SpecialAnimation(long frameOffset, bool isFix, int index, int? groupCount = null, int? objParam_1 = null, int? objParam_2 = null)
            {
                FrameOffsets = new long[]
                {
                    frameOffset
                };
                IsFix = isFix;
                Index = index;
                GroupCount = groupCount;
                ObjParam_1 = objParam_1;
                ObjParam_2 = objParam_2;
            }
            public SpecialAnimation(long[] frameOffsets, bool isFix, int index)
            {
                FrameOffsets = frameOffsets;
                IsFix = isFix;
                Index = index;
            }

            /// <summary>
            /// The offset for every frame in the animation
            /// </summary>
            public long[] FrameOffsets { get; }

            /// <summary>
            /// If specified this is the offset to the frames pointer array, otherwise <see cref="FrameOffsets"/> should be set
            /// </summary>
            public long? Offset { get; }

            /// <summary>
            /// The amount of frames in the animation if <see cref="Offset"/> is specified
            /// </summary>
            public int FramesCount { get; }

            /// <summary>
            /// Indicates if the animation is for a fix object, otherwise it's for a level object
            /// </summary>
            public bool IsFix { get; }

            /// <summary>
            /// If <see cref="IsFix"/> this is the object index (0-12), otherwise it's the object type
            /// </summary>
            public int Index { get; }

            /// <summary>
            /// If specified this indicates how many sprites are used for this group of sprites. A very hacky solution to the wind leaves.
            /// </summary>
            public int? GroupCount { get; }

            /// <summary>
            /// The param the object has to be set to for this animation to be applied
            /// </summary>
            public int? ObjParam_1 { get; }

            public int? ObjParam_2 { get; }

            public bool MatchesObject(GBAKlonoa_LoadedObject obj)
            {
                if (ObjParam_1 != null && ObjParam_1 != obj.Param_1)
                    return false;

                if (ObjParam_2 != null && ObjParam_2 != obj.Param_2)
                    return false;

                return IsFix && obj.Index == Index ||
                       !IsFix && obj.ObjType == Index;
            }
        }
    }
}