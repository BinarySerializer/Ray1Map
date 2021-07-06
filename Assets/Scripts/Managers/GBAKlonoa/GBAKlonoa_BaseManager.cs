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

        public Unity_TileSet LoadTileSet(byte[] tileSet, RGBA5551Color[] pal, bool is8bit, MapTile[] mapTiles_4)
        {
            Texture2D tex;
            var additionalTiles = new List<Texture2D>();
            var tileSize = is8bit ? 0x40 : 0x20;
            var paletteIndices = Enumerable.Range(0, tileSet.Length / tileSize).Select(x => new List<byte>()).ToArray();
            var tilesCount = tileSet.Length / tileSize;

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

            return new Unity_TileSet(tiles);
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
            if (oamCollection.Length != 1)
                Debug.LogWarning($"Animation uses multiple sprites!");

            // Sometimes the last animation is null
            if (anims.Length > 1 && anims.Last() == null)
                anims = anims.Take(anims.Length - 1).ToArray();

            return new Unity_ObjectManager_GBAKlonoa.AnimSet(
                animations: Enumerable.Range(0, anims.Length).
                    Select(animIndex => anims[animIndex]?.Frames?.Length > 0 ? new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation(
                        animFrameFunc: () => GetAnimFrames(anims[animIndex].Frames, oamCollection, palettes, singlePal: singlePal).Select(x => x.CreateSprite()).ToArray(),
                        oamCollection: oamCollection,
                        animSpeeds: anims[animIndex]?.Frames?.Select(f => (int?)f?.Speed).ToArray()) : null).ToArray(),
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
    }
}