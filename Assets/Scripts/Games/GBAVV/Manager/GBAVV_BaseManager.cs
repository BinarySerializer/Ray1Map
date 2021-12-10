using BinarySerializer;
using BinarySerializer.GBA;
using BinarySerializer.Image;
using Cysharp.Threading.Tasks;
using Ray1Map.GBARRR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_BaseManager : BaseGameManager
    {
        // Constants
        public const int CellSize = GBAConstants.TileSize;
        public const string GetROMFilePath = "ROM.gba";

        // Scripts
        public virtual Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>();
        public virtual uint[] ScriptPointers => null;

        // Graphics
        public abstract uint[] GraphicsDataPointers { get; }

        // Tools
        public override GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Animation Frames", false, true, (input, output) => ExportAnimFramesAsync(settings, output, false)),
            new GameAction("Export Animations as GIF", false, true, (input, output) => ExportAnimFramesAsync(settings, output, true)),
            new GameAction("Export Cutscenes", false, true, (input, output) => ExportCutscenesAsync(settings, output)),
            new GameAction("Force Export FLC Files", false, true, (input, output) => ForceExportFLCAsync(settings, output)),
            new GameAction("Export Music & Sample Data", false, true, (input, output) => ExportMusicAsync(settings, output)),
        };

        public virtual async UniTask ExportMusicAsync(GameSettings settings, string outputPath) {
            var GAXInfo = GAXHelpers.Info.TryGetItem(settings.GameModeSelection);
            if(GAXInfo == null)
                throw new NotImplementedException("GAX Export isn't defined for this game!");
            var songCount = GAXInfo.MusicCount;
            var fxCount = GAXInfo.FXCount;

            using (var context = new Ray1MapContext(settings)) {
                var s = context.Deserializer;
                
                await LoadFilesAsync(context);

                var file = s.Context.GetFile(GetROMFilePath);

                Pointer SongOffset = songCount > 0 ? new Pointer(GAXInfo.MusicOffset, file) : null;
                Pointer FXOffset = fxCount > 0 ? new Pointer(GAXInfo.FXOffset, file) : null;

                if(SongOffset == null && FXOffset == null)
                    throw new NotImplementedException("GAX Export isn't defined for this game!");

                if (songCount != 0) {
                    Pointer<GAX2_Song>[] Songs = null;
                    s.DoAt(SongOffset, () => {
                        Songs = s.SerializePointerArray<GAX2_Song>(Songs, songCount, resolve: true, name: nameof(Songs));
                    });
                    for (int i = 0; i < Songs.Length; i++) {
                        GAXHelpers.ExportGAX(settings, outputPath, $"music/{i}", Songs[i], 2);
                    }
                }
                if (fxCount != 0) {
                    Pointer<GAX2_Song>[] FX = null;
                    s.DoAt(FXOffset, () => {
                        FX = s.SerializePointerArray<GAX2_Song>(FX, fxCount, resolve: true, name: nameof(FX));
                    });
                    for (int i = 0; i < FX.Length; i++) {
                        GAXHelpers.ExportGAX(settings, outputPath, $"fx/{i}", FX[i], 1);
                    }
                }
            }
        }


        public abstract UniTask ExportCutscenesAsync(GameSettings settings, string outputDir);
        public void ExportCutscenesFromScripts(IEnumerable<GBAVV_Script> scripts, string outputDir)
        {
            // Enumerate every script
            foreach (var script in scripts)
            {
                var index = 0;

                // Enumerate every command which plays an FLC file
                foreach (var flc in script.Commands.Where(x => x.FLC != null))
                {
                    using (var collection = flc.FLC.ToMagickImageCollection())
                        collection.Write(Path.Combine(outputDir, $"{script.DisplayName}-{index++}.gif"));
                }
            }
        }
        public abstract GBAVV_BaseROM LoadROMForExport(Context context);
        public virtual async UniTask ExportAnimFramesAsync(GameSettings settings, string outputDir, bool saveAsGif, bool includePointerInNames = true)
        {
            // Export 2D animations
            using (var context = new Ray1MapContext(settings))
            {
                // Load the files
                await LoadFilesAsync(context);

                // Read the rom
                var rom = LoadROMForExport(context);

                await Controller.WaitFrame();

                await ExportAnimFramesAsync(rom.Map2D_Graphics, outputDir, saveAsGif, includePointerInNames);
            }

            Debug.Log($"Finished exporting animations");
        }
        public async UniTask ExportAnimFramesAsync(GBAVV_Graphics[] graphicsDatas, string outputDir, bool saveAsGif, bool includePointerInNames = true)
        {
            // Enumerate every graphics data
            for (int graphicsIndex = 0; graphicsIndex < graphicsDatas.Length; graphicsIndex++)
            {
                var graphicsData = graphicsDatas[graphicsIndex];

                // Enumerate every anim set
                for (int animSetIndex = 0; animSetIndex < graphicsData.AnimSets.Length; animSetIndex++)
                {
                    var animSet = graphicsData.AnimSets[animSetIndex];

                    // Enumerate every animation
                    for (var animIndex = 0; animIndex < animSet.Animations.Length; animIndex++)
                    {
                        await Controller.WaitFrame();

                        var anim = animSet.Animations[animIndex];
                        var frames = GetAnimFrames(animSet, animIndex, graphicsData.TileSet, graphicsData.Palettes);

                        if (!frames.Any())
                            continue;

                        var dirName = $"2D";
                        var animSetName = $"{animSetIndex}";
                        var animName = $"{animIndex}";

                        if (graphicsDatas.Length > 1)
                            dirName += $"_{graphicsIndex}";

                        if (includePointerInNames)
                        {
                            dirName += $" - 0x{graphicsData.Offset.StringAbsoluteOffset}";
                            animSetName += $" 0x{animSet.Offset.StringAbsoluteOffset}";
                            animName += $" 0x{anim.Offset.StringAbsoluteOffset}";
                        }

                        Util.ExportAnim(
                            frames: frames,
                            speed: anim.GetAnimSpeed,
                            center: false,
                            saveAsGif: saveAsGif,
                            outputDir: Path.Combine(outputDir, dirName),
                            primaryName: animSetName,
                            secondaryName: animName);
                    }
                }
            }
        }
        public async UniTask ForceExportFLCAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Ray1MapContext(settings))
            {
                await LoadFilesAsync(context);

                var s = context.Deserializer;
                var offset = context.FilePointer(GetROMFilePath);
                const int alignment = 4;
                const int minDecompSize = 32;
                
                s.Goto(offset);
                
                var length = s.CurrentLength;

                // Enumerate every byte
                for (int i = 0; i < length; i += alignment)
                {
                    // Go to the offset
                    s.Goto(offset + i);

                    // Check for compression header
                    if (s.Serialize<byte>(default) == 0x10)
                    {
                        // Get the decompressed size
                        var decompressedSize = s.Serialize<UInt24>(default);

                        // Skip if the decompressed size is too low
                        if (decompressedSize < minDecompSize)
                            continue;

                        // Go back to the offset
                        s.Goto(offset + i);

                        // Attempt to decompress
                        try
                        {
                            s.DoEncoded(new GBA_LZSSEncoder(), () =>
                            {
                                // Start by reading the header and check so it's an FLC file
                                var isValid = s.DoAt(s.CurrentPointer, () =>
                                {
                                    var fileSize = s.Serialize<uint>(default);
                                    var format = s.Serialize<FLIC_Format>(default);

                                    return format == FLIC_Format.FLC;
                                });

                                if (!isValid)
                                    return;

                                // Read the .flc file
                                var flc = s.SerializeObject<FLIC>(default);

                                // Export
                                using (var collection = flc.ToMagickImageCollection())
                                    collection.Write(Path.Combine(outputDir, $"0x{(offset + i).StringAbsoluteOffset}.gif"));
                            }, allowLocalPointers: true);
                        }
                        catch (Exception ex)
                        {
                            if (!ex.Message.StartsWith("Cannot go back more than already written"))
                                Debug.Log(ex);
                        }
                    }
                }
            }
        }

        // Load
        public override async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, GBAConstants.Address_ROM);
        public async UniTask<Unity_Level> LoadMap2DAsync(Context context, GBAVV_BaseROM rom, GBAVV_Map map, bool hasAssignedObjTypeGraphics = true)
        {
            Controller.DetailedState = "Loading tilesets";
            await Controller.WaitIfNecessary();

            // Load tilemaps
            var tileMaps = map.MapLayers.Select((x, i) =>
            {
                if (x == null)
                    return null;

                return GetTileMap(x.TileMap, x.MapTiles);
            }).ToArray();

            var tileSet4bpp = LoadTileSet(map.TileSets.TileSet4bpp.TileSet, map.TilePalette, false, context.GetR1Settings().EngineVersion, -1, tileMaps.Take(3).Where(x => x != null).SelectMany(x => x).Select(x => x.Data).ToArray());
            var tileSet8bpp = LoadTileSet(map.TileSets.TileSet8bpp.TileSet, map.TilePalette, true, context.GetR1Settings().EngineVersion, -1, null);

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            var maps = map.MapLayers.Select((x, i) =>
            {
                if (x == null)
                    return null;

                return new
                {
                    Map = new Unity_Map
                    {
                        Width = (ushort)(x.TileMap.Width * 2),
                        Height = (ushort)(x.TileMap.Height * 2),
                        TileSet = new Unity_TileSet[]
                        {
                            i == 3 ? tileSet8bpp : tileSet4bpp
                        },
                        MapTiles = tileMaps[i],
                        Type = Unity_Map.MapType.Graphics,
                        Layer = Unity_Map.MapLayer.Middle,
                    },
                    Prio = x.LayerPrio
                };
            }).Where(x => x != null).OrderByDescending(x => x.Prio).Select(x => x.Map).ToArray();

            List<Unity_CollisionLine> collisionLines = null;

            // Load line collision
            if (map.LineCollision != null)
            {
                collisionLines = new List<Unity_CollisionLine>();
                var dummyCollision = new GBAVV_Fusion_MapCollisionLineData();

                addCollision(map.LineCollision);

                void addCollision(GBAVV_LineCollisionSector mapCollision)
                {
                    // Add every line
                    foreach (var line in mapCollision.CollisionLines ?? new GBAVV_LineCollisionLine[0])
                    {
                        // Get vectors for the points
                        Vector2 p1 = new Vector2(line.GetX1, line.GetY1);
                        Vector2 p2 = new Vector2(line.GetX2, line.GetY2);

                        var c = line.CollisionData ?? dummyCollision;

                        // Add the line
                        collisionLines.Add(new Unity_CollisionLine()
                        {
                            Pos_0 = p1,
                            Pos_1 = p2,
                            LineColor = c.GetColor(),
                            TypeName = c.GetCollisionType().ToString(),
                            DebugText = $"Direction: {line.Direction}{Environment.NewLine}" +
                                        $"Data: {(c.Data != null ? c.Data.ToHexString() : "")}{Environment.NewLine}"
                        });
                    }

                    // Add every sub-sector
                    foreach (var mc in mapCollision.SubSectors.Where(x => x != null))
                        addCollision(mc);
                }
            }

            // Load collision map
            if (map.MapCollision != null)
            {
                var cm = map.MapCollision;

                maps = maps.Append(new Unity_Map
                {
                    Width = cm.Width,
                    Height = cm.Height,
                    TileSet = new Unity_TileSet[0],
                    MapTiles = cm.CollisionMap.Select(x => new Unity_Tile(new MapTile()
                    {
                        CollisionType = x
                    })).ToArray(),
                    Type = Unity_Map.MapType.Collision,
                    Layer = Unity_Map.MapLayer.Middle,
                }).ToArray();
            }

            Controller.DetailedState = "Loading localization";
            await Controller.WaitIfNecessary();

            var loc = LoadLocalization(rom);

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var objmanager = new Unity_ObjectManager_GBAVV(
                context: context, 
                animSets: LoadAnimSets(rom), 
                objData: map.ObjData, 
                scripts: rom.GetAllScripts.ToArray(), 
                graphics: rom.Map2D_Graphics, 
                dialogScripts: rom.DialogScripts, 
                locPointerTable: loc.Item2,
                addDummyAnimSet: !hasAssignedObjTypeGraphics);
            var objects = new List<Unity_SpriteObject>();

            if (map.ObjData?.Objects != null)
                objects.AddRange(map.ObjData.Objects.Select(obj => new Unity_Object_GBAVV(objmanager, obj, -1, -1)));

            // TODO: Collision type for map collision
            return new Unity_Level()
            {
                Maps = maps,
                ObjManager = objmanager,
                EventData = objects,
                CellSize = CellSize,
                Localization = loc.Item1,
                CollisionLines = collisionLines?.ToArray()
            };
        }

        // Tileset
        public Unity_TileSet LoadGenericTileSet(byte[] tileSet, RGBA5551Color[] pal, int palIndex)
        {
            var palettes = Util.ConvertAndSplitGBAPalette(pal);

            var tex = Util.ToTileSetTexture(tileSet, palettes[palIndex], Util.TileEncoding.Linear_4bpp, CellSize, false);

            return new Unity_TileSet(tex, CellSize);
        }
        public Unity_TileSet LoadTileSet(byte[] tileSet, RGBA5551Color[] pal, bool is8bit, EngineVersion engineVersion, int levelTheme, MapTile[] mapTiles_4, GBAVV_NitroKart_TileAnimations nitroKartTileAnimations = null, GBAVV_Generic_PaletteShifts paletteShifts = null)
        {
            Texture2D tex;
            var additionalTiles = new List<Texture2D>();
            var tileSize = is8bit ? 0x40 : 0x20;
            var paletteIndices = Enumerable.Range(0, tileSet.Length / tileSize).Select(x => new List<byte>()).ToArray();
            var tilesCount = tileSet.Length / tileSize;
            var tileSetPaletteIndices = new List<byte>();
            var originalTileSetIndices = new List<int>();
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
                    tileSetPaletteIndices.Add(p);
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
                        tileSetPaletteIndices.Add(p);
                        originalTileSetIndices.Add(tileIndex);
                    }
                }

                // NOTE: Currently this does not support the same tile having multiple palettes, but it's not used in Nitro Kart
                // Add Nitro Kart animated tiles
                if (nitroKartTileAnimations != null)
                {
                    // Enumerate every animation
                    foreach (var anim in nitroKartTileAnimations.Animations)
                    {
                        // Enumerate every tile
                        for (int tileIndex = 0; tileIndex < anim.TilesCount; tileIndex++)
                        {
                            // Make sure the tile is used in the level
                            if (mapTiles_4.All(x => x.TileMapY != anim.TileIndices[tileIndex]))
                                continue;

                            // Get the palette index
                            var palIndex = mapTiles_4.First(x => x.TileMapY == anim.TileIndices[tileIndex]).PaletteIndex;

                            var framesCount = anim.FramesCount - 1; // Don't include the first frame since that's already in the normal tileset

                            // Enumerate every frame
                            for (int frameIndex = 1; frameIndex < anim.FramesCount; frameIndex++)
                            {
                                var tileTex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                                // Create a new tile
                                tileTex.FillInTile(
                                    imgData: anim.Frames[frameIndex].TileSet,
                                    imgDataOffset: tileSize * tileIndex,
                                    pal: palettes[palIndex],
                                    encoding: Util.TileEncoding.Linear_4bpp,
                                    tileWidth: CellSize,
                                    flipTextureY: false,
                                    tileX: 0,
                                    tileY: 0);

                                // Add to additional tiles list
                                additionalTiles.Add(tileTex);
                            }

                            tileAnimations.Add(new Unity_AnimatedTile
                            {
                                AnimationSpeeds = anim.Frames.Select(x => x.Speed / 2f).ToArray(),
                                TileIndices = new int[]
                                {
                                    anim.TileIndices[tileIndex]
                                }.Concat(Enumerable.Range(tilesCount + additionalTiles.Count - framesCount, framesCount)).ToArray()
                            });
                        }
                    }
                }
            }

            // Some levels use animated tile palettes based on the level theme. These are all hard-coded in the level load function.
            RGBA5551Color[][] animatedPalettes = null;
            HashSet<byte> modifiedPaletteIndices = null;
            float animSpeed = 0;
            bool isReversed = false;

            //Debug.Log($"Theme: {levelTheme}");

            // NOTE: Speeds might not be correct
            if (engineVersion == EngineVersion.GBAVV_Crash1)
            {
                if (levelTheme == 1 || levelTheme == 6) // Jungle
                {
                    var anim = new byte[][]
                    {
                        // 0x0816c35e
                        new byte[] { 0x39, 0x3a, 0x3d, 0x3e, 0x75, 0x82, 0xbc, 0xea, 0xfc },
                        // 0x0816c370
                        new byte[] { 0x51, 0x52, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0xf1 }, // Speed on this one is slightly different
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal);

                    isReversed = true;
                    animSpeed = 2;
                }
                else if (levelTheme == 2) // Sewer
                {
                    var anim = new byte[][]
                    {
                        // 0x0816c354
                        new byte[] { 0xb1, 0xb2, 0xb3, 0xb4, 0xb5 },
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal);

                    animSpeed = 5;
                }
                else if (levelTheme == 3) // Underwater
                {
                    var anim = new byte[][]
                    {
                        // 0x0816c382
                        new byte[] { 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f },
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal);

                    isReversed = true;
                    animSpeed = 4;
                }
                else if (levelTheme == 5) // Space
                {
                    var anim = new byte[][]
                    {
                        // 0x0816c3a2
                        new byte[] { 0x97, 0xb4, 0xf7, 0xf8, 0xff },
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal);

                    isReversed = true;
                    animSpeed = 3;
                }
            }
            else if (engineVersion == EngineVersion.GBAVV_Crash2)
            {
                if (levelTheme == 1 || levelTheme == 9) // Egyptian
                {
                    var anim = new byte[][]
                    {
                        // 0x081d26ea
                        new byte[] { 0xfb, 0xfc, 0xfd, 0xfe, 0xff },
                        // 0x081d26f4
                        new byte[] { 0x22, 0x23, 0x24 },
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal).Reverse().ToArray(); // The animation is reversed

                    animSpeed = 4;
                }
                else if (levelTheme == 0) // Arabian
                {
                    var anim = new byte[][]
                    {
                        // 0x081d26e0
                        new byte[] { 0xf3, 0xf4, 0xf5, 0xf6, 0xf7 },
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal);

                    animSpeed = 4;
                }
                // TODO: Theme 2 (Volcanic) uses a different palette animation system
            }
            else if (engineVersion == EngineVersion.GBAVV_CrashNitroKart)
            {
                if (is8bit)
                {
                    var anim = new byte[][]
                    {
                        new byte[] { 0xfb, 0xfc, 0xfd, 0xfe, 0xff },
                    };

                    modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                    animatedPalettes = GetAnimatedPalettes(anim, pal);

                    animSpeed = 2;
                }
            }

            if (paletteShifts != null)
            {
                var anim = paletteShifts.Shifts.Select(x =>
                {
                    var indices = x.ColorIndices;

                    if (x.IsReverse)
                        indices = indices.Reverse().ToArray();

                    return indices;
                }).ToArray();

                modifiedPaletteIndices = new HashSet<byte>(anim.SelectMany(x => x));

                animatedPalettes = GetAnimatedPalettes(anim, pal);

                // TODO: Support different speed for each palette shift
                // We get the average speed for now
                animSpeed = paletteShifts.Shifts.Sum(x => x.ShiftSpeed) / (float)paletteShifts.Shifts.Length;

                // Divide by 2 for now since it's multiplied by 2 later
                animSpeed /= 2f;
            }

            var additionalPalTilesCount = additionalTiles.Count;

            if (animatedPalettes != null)
            {
                if (isReversed)
                    animatedPalettes = animatedPalettes.Reverse().ToArray();

                // Convert palettes
                var animatedPalettes_4 = animatedPalettes.Select(x => Util.ConvertAndSplitGBAPalette(x)).ToArray();
                var animatedPalettes_8 = animatedPalettes.Select(x => Util.ConvertGBAPalette(x)).ToArray();

                for (int tileIndex = 0; tileIndex < tilesCount + additionalPalTilesCount; tileIndex++)
                {
                    var originalTileIndex = tileIndex >= tilesCount ? originalTileSetIndices[tileIndex - tilesCount] : tileIndex;

                    var tilePalette = is8bit ? 0 : tileSetPaletteIndices[tileIndex];

                    // Check if the tile uses any of the animated colors
                    // If the tile doesn't contain an animated color we ignore it
                    bool containsAnimatedPalette = false;
                    for (int i = tileSize * originalTileIndex; i < tileSize * originalTileIndex + tileSize; i++) {
                        if (is8bit) {
                            if (modifiedPaletteIndices.Contains(tileSet[i])) {
                                containsAnimatedPalette = true;
                                break;
                            }
                        } else {
                            var palInd = tilePalette * 16;
                            if (modifiedPaletteIndices.Contains((byte)(palInd + (tileSet[i] & 0xF)))) {
                                containsAnimatedPalette = true;
                                break;
                            }
                            if (modifiedPaletteIndices.Contains((byte)(palInd + (tileSet[i] >> 4)))) {
                                containsAnimatedPalette = true;
                                break;
                            }
                        }
                    }
                    if(!containsAnimatedPalette) continue;

                    // Add animation for the tile
                    tileAnimations.Add(new Unity_AnimatedTile
                    {
                        AnimationSpeed = animSpeed * 2f,
                        TileIndices = new int[]
                        {
                            tileIndex
                        }.Concat(Enumerable.Range(tilesCount + additionalTiles.Count, animatedPalettes.Length)).ToArray()
                    });

                    // Create a new tile for every animated palette frame
                    for (int animFrame = 0; animFrame < animatedPalettes.Length; animFrame++)
                    {
                        var tileTex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                        // Create a new tile
                        tileTex.FillInTile(
                            imgData: tileSet,
                            imgDataOffset: tileSize * originalTileIndex,
                            pal: is8bit ? animatedPalettes_8[animFrame] : animatedPalettes_4[animFrame][tilePalette],
                            encoding: is8bit ? Util.TileEncoding.Linear_8bpp : Util.TileEncoding.Linear_4bpp,
                            tileWidth: CellSize,
                            flipTextureY: false,
                            tileX: 0,
                            tileY: 0);

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

            //Debug.Log($"Count: {tilesCount}");
            //Debug.Log($"TotalCount: {totalTilesCount}");
            //Debug.Log($"AnimFrames: {animatedPalettes?.Length}");
            //Debug.Log($"Tiles: {tiles.Length}");

            return new Unity_TileSet(tiles)
            {
                AnimatedTiles = tileAnimations.Count == 0 ? null : tileAnimations.ToArray()
            };
        }
        public RGBA5551Color[][] GetAnimatedPalettes(byte[][] paletteAnimations, RGBA5551Color[] palette)
        {
            // Get the lowest common multiple
            var length = paletteAnimations.Length == 1 ? paletteAnimations[0].Length : Util.LCM(paletteAnimations.Select(y => y.Length).ToArray());

            var output = new RGBA5551Color[length - 1][];

            // Shift colors and create new palettes for every frame
            for (int i = 0; i < length - 1; i++)
            {
                int frame = i + 1;
                var newPal = new RGBA5551Color[palette.Length];

                // Set to original palette
                for (int j = 0; j < newPal.Length; j++)
                    newPal[j] = palette[j];

                foreach (var anim in paletteAnimations)
                {
                    int shift = frame % anim.Length;
                    if (shift > 0)
                    {
                        for (int k = 0; k < anim.Length; k++)
                        {
                            int targetK = (k + shift) % anim.Length;
                            newPal[anim[targetK]] = palette[anim[k]];
                        }
                    }
                }

                output[i] = newPal;
            }

            return output;
        }

        // Tilemap
        public Unity_Tile[] GetTileMap(GBAVV_TileMap mapLayer, MapTile[] mapTiles)
        {
            if (mapLayer.MapEncoding == GBAVV_TileMap.Encoding.Columns)
            {
                var tileMap = new Unity_Tile[mapLayer.Width * 2 * mapLayer.Height * 2];

                for (int columnIndex = 0; columnIndex < mapLayer.Width; columnIndex++)
                {
                    var column = mapLayer.TileMapSections[columnIndex];
                    var y = 0;

                    foreach (var cmd in column.Commands)
                    {
                        if (cmd.Type == 3)
                        {
                            for (var i = 0; i < cmd.Params.Length; i++)
                            {
                                setTile(cmd.Params[i], i, cmd.Params.Length);
                                y++;
                            }
                        }
                        else if (cmd.Type == 2)
                        {
                            for (int i = 0; i < cmd.Length; i++)
                            {
                                setTile(cmd.Param, i, cmd.Length);
                                y++;
                            }
                        }
                        else
                        {
                            setTile(cmd.Length, 0, 1);
                            y++;
                        }

                        void setTile(ushort value, int index, int length)
                        {
                            var actualX = columnIndex * 2;
                            var actualY = y * 2;

                            setTileAt(0, 0, mapTiles[value * 4 + 0]);
                            setTileAt(1, 0, mapTiles[value * 4 + 1]);
                            setTileAt(0, 1, mapTiles[value * 4 + 2]);
                            setTileAt(1, 1, mapTiles[value * 4 + 3]);

                            void setTileAt(int offX, int offY, MapTile tile)
                            {
                                var outputX = actualX + offX;
                                var outputY = actualY + offY;

                                tileMap[outputY * mapLayer.Width * 2 + outputX] = new Unity_Tile(tile)
                                {
                                    DebugText = Settings.ShowDebugInfo ? ($"CMD: {cmd.Type}{Environment.NewLine}" +
                                                $"Value: {value}{Environment.NewLine}" +
                                                $"Index: {index}{Environment.NewLine}" +
                                                $"Length: {length}{Environment.NewLine}" +
                                                $"Tile: {tile.TileMapY}{Environment.NewLine}" +
                                                $"FlipX: {tile.HorizontalFlip}{Environment.NewLine}" +
                                                $"FlipY: {tile.VerticalFlip}{Environment.NewLine}") : null
                                };
                            }
                        }
                    }
                }

                return tileMap;
            }
            else
            {
                var tileMap = new Unity_Tile[mapLayer.Width * 2 * mapLayer.Height * 2];
                int actualX, actualY;

                for (int y = 0; y < mapLayer.Height; y++)
                {
                    actualY = y * 2;
                    actualX = 0;
                    var chunk = mapLayer.TileMapSections[y];

                    foreach (var cmd in chunk.Commands)
                    {
                        if (cmd.Type == 3)
                        {
                            for (var i = 0; i < cmd.Params.Length; i++)
                            {
                                setTile(cmd.Params[i], i, cmd.Params.Length);
                            }
                        }
                        else if (cmd.Type == 2)
                        {
                            for (int i = 0; i < cmd.Length; i++)
                            {
                                setTile(cmd.Param, i, cmd.Length);
                            }
                        }
                        else
                        {
                            setTile(cmd.Length, 0, 1);
                        }
                    }
                }

                void setTile(ushort value, int index, int length)
                {
                    var val = value * 4;

                    setTileAt(0, 0, mapTiles[val + 0], index, length, value);
                    setTileAt(1, 0, mapTiles[val + 1], index, length, value);
                    setTileAt(0, 1, mapTiles[val + 2], index, length, value);
                    setTileAt(1, 1, mapTiles[val + 3], index, length, value);
                    actualX += 2;
                }

                void setTileAt(int offX, int offY, MapTile tile, int index, int length, ushort value)
                {
                    var outputX = actualX + offX;
                    var outputY = actualY + offY;
                    tileMap[outputY * mapLayer.Width * 2 + outputX] = new Unity_Tile(tile)
                    {
                        /*DebugText = $"CMD: {cmdType}{Environment.NewLine}" +
                                    $"Value: {value}{Environment.NewLine}" +
                                    $"Index: {index}{Environment.NewLine}" +
                                    $"Length: {length}{Environment.NewLine}" +
                                    $"Tile: {tile.TileMapY}{Environment.NewLine}" +
                                    $"FlipX: {tile.HorizontalFlip}{Environment.NewLine}" +
                                    $"FlipY: {tile.VerticalFlip}{Environment.NewLine}"*/
                    };
                }

                return tileMap;
            }
        }

        // Animations
        public Unity_ObjectManager_GBAVV.AnimSet[][] LoadAnimSets(GBAVV_BaseROM rom)
        {
            Unity_ObjectManager_GBAVV.AnimSet.Animation convertAnim(GBAVV_Graphics graphics, GBAVV_AnimSet animSet, GBAVV_Animation anim, int i) => new Unity_ObjectManager_GBAVV.AnimSet.Animation(
                animFrameFunc: () => GetAnimFrames(animSet, i, graphics?.TileSet, graphics?.Palettes).Select(frame => frame.CreateSprite()).ToArray(),
                crashAnim: anim,
                xPos: animSet.GetMinX(i),
                yPos: animSet.GetMinY(i)
            );

            Unity_ObjectManager_GBAVV.AnimSet convertAnimSet(GBAVV_Graphics graphics, GBAVV_AnimSet animSet) => new Unity_ObjectManager_GBAVV.AnimSet(animSet.Animations.Select((anim, i) => convertAnim(graphics, animSet, anim, i)).ToArray());

            var animSets = rom.Map2D_Graphics?.Select(graphics => graphics.AnimSets.Select(animSet => convertAnimSet(graphics, animSet)).ToArray()).ToArray() ?? new Unity_ObjectManager_GBAVV.AnimSet[0][];

            // Create an anim set for Fake Crash for Crash 2
            if (rom.Context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash2)
            {
                var crash = rom.Map2D_Graphics[0].AnimSets[0];

                animSets[0] = animSets[0].Append(convertAnimSet(rom.Map2D_Graphics[0], new GBAVV_AnimSet
                {
                    Animations = crash.Animations.Select(x => new GBAVV_Animation
                    {
                        HitBox = x.HitBox,
                        RenderBox = x.RenderBox,
                        PaletteIndex = 5,
                        AnimSpeed = x.AnimSpeed,
                        FrameIndexTable = x.FrameIndexTable
                    }).ToArray(),
                    AnimationFrames = crash.AnimationFrames
                })).ToArray();
            }

            return animSets;
        }
        public Texture2D[] GetAnimFrames(GBAVV_AnimSet animSet, int animIndex, byte[] tileSet, GBAVV_ObjPalette[] palettes)
        {
            // Get properties
            var shapes = TileShapes;
            var anim = animSet.Animations[animIndex];
            var frames = anim.FrameIndexTable.Select(x => animSet.AnimationFrames[x]).ToArray();
            var pal = palettes != null ? Util.ConvertGBAPalette(palettes[anim.PaletteIndex].Palette) : Util.ConvertGBAPalette(anim.Palette);

            // Return empty animation if there are no frames
            if (!frames.Any(x => x.TilesCount > 0))
                return new Texture2D[0];

            var output = new Texture2D[frames.Length];

            var minX = animSet.GetMinX(animIndex);
            var minY = animSet.GetMinY(animIndex);
            var maxX = frames.SelectMany(f => Enumerable.Range(0, f.TilesCount).Select(x => f.TilePositions[x].XPos + shapes[f.GetTileShape(x)].x)).Max();
            var maxY = frames.SelectMany(f => Enumerable.Range(0, f.TilesCount).Select(x => f.TilePositions[x].YPos + shapes[f.GetTileShape(x)].y)).Max();

            var width = maxX - minX;
            var height = maxY - minY;

            var frameCache = new Dictionary<GBAVV_AnimationFrame, Texture2D>();

            for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
            {
                var frame = frames[frameIndex];

                if (!frameCache.ContainsKey(frame))
                {
                    var tex = TextureHelpers.CreateTexture2D(width, height, clear: true);

                    int offset = (int)frame.TileOffset.Value;

                    for (int i = 0; i < frame.TilesCount; i++)
                    {
                        var shape = shapes[frame.GetTileShape(i)];
                        var pos = frame.TilePositions[i];

                        for (int y = 0; y < shape.y; y += 8)
                        {
                            for (int x = 0; x < shape.x; x += 8)
                            {
                                tex.FillInTile(
                                    imgData: tileSet ?? frame.Fusion_TileSet,
                                    imgDataOffset: offset,
                                    pal: pal,
                                    encoding: Util.TileEncoding.Linear_4bpp,
                                    tileWidth: CellSize,
                                    flipTextureY: true,
                                    tileX: pos.XPos + x - minX,
                                    tileY: pos.YPos + y - minY);

                                offset += 32;
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

        // Localization
        public virtual string[] Languages => null;
        public int DefaultLanguage => Languages?.FindItemIndex(x => x == "English") ?? -1;
        public virtual (KeyValuePair<string, string[]>[], Dictionary<Pointer, int>) LoadLocalization(GBAVV_BaseROM rom)
        {
            return rom.Scripts != null ? LoadLocalization(rom.GetAllScripts) : (null, null);
        }
        public (KeyValuePair<string, string[]>[], Dictionary<Pointer, int>) LoadLocalization(IEnumerable<GBAVV_Script> scripts)
        {
            var languages = Languages;

            var locTables = Enumerable.Range(0, languages.Length).Select(x => new
            {
                Strings = new List<string>(),
                Index = x
            }).ToArray();
            var pointerTable = new Dictionary<Pointer, int>();

            var index = 0;

            // Find strings from scripts
            foreach (var script in scripts.SelectMany(x => x.Commands).Where(x => x.Dialog != null))
            {
                var dialog = script.Dialog;

                if (pointerTable.ContainsKey(dialog.Offset))
                    continue;

                for (int i = 0; i < dialog.Items.Length; i++)
                    locTables[i].Strings.Add(dialog.Items[i]?.Text);

                pointerTable[dialog.Offset] = index++;
            }

            var locList = locTables.Select(x => new KeyValuePair<string, string[]>(languages[x.Index], x.Strings.ToArray())).ToList();

            locList.Sort(delegate (KeyValuePair<string, string[]> x, KeyValuePair<string, string[]> y)
            {
                if (x.Key == "English")
                    return -1;
                if (y.Key == "English")
                    return 1;
                return 0;
            });

            return (locList.ToArray(), pointerTable);
        }

        // Helpers
        public virtual void FindDataInROM(SerializerObject s, Pointer offset = null)
        {
            if (offset == null)
                offset = s.Context.FilePointer(GetROMFilePath);

            // Read ROM as a uint array
            var values = s.DoAt(offset, () => s.SerializeArray<uint>(default, s.CurrentLength / 4, name: "Values"));

            // Helper for getting a pointer
            long getPointer(int index) => GBAConstants.Address_ROM + index * 4;
            bool isValidPointer(uint value) => value >= GBAConstants.Address_ROM && value < GBAConstants.Address_ROM + s.CurrentLength;

            // Keep track of found data
            var foundGraphics = new List<long>();
            var foundScripts = new List<Tuple<long, string>>();

            if (s.GetR1Settings().EngineVersion >= EngineVersion.GBAVV_CrashNitroKart_NGage && s.GetR1Settings().EngineVersion != EngineVersion.GBAVV_KidsNextDoorOperationSODA)
            {
                // Find animation sets by finding pointers which references itself
                for (int i = 0; i < values.Length; i++)
                {
                    var p = getPointer(i);

                    if (values[i] == p)
                        // We found a valid animation set!
                        foundGraphics.Add(p);
                }
            }
            else
            {
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
                            var animSetsCount = s.DoAt(new Pointer(getPointer(i + 3), s.CurrentPointer.File), () => s.Serialize<ushort>(default));
                            var palettesCount = s.DoAt(new Pointer(getPointer(i + 3) + 2, s.CurrentPointer.File), () => s.Serialize<ushort>(default));

                            // Make sure the animSets count and palette counts are reasonable
                            if (animSetsCount < 1000 && palettesCount < 10000)
                                foundGraphics.Add(p);
                        }
                    }
                }
            }

            var scriptCmds = ScriptCommands;

            if (scriptCmds != null && scriptCmds.Any(x => x.Value == GBAVV_ScriptCommand.CommandType.Name))
            {
                var nameCmd = scriptCmds.First(x => x.Value == GBAVV_ScriptCommand.CommandType.Name).Key;
                var primary = nameCmd / 100;
                var secondary = nameCmd % 100;

                // Find scripts by finding the name command which is always the first one
                for (int i = 0; i < values.Length - 2; i++)
                {
                    if (values[i] == primary && values[i + 1] == secondary && isValidPointer(values[i + 2]))
                    {
                        // Serialize the script
                        var script = s.DoAt(new Pointer(getPointer(i), offset.File), () => s.SerializeObject<GBAVV_Script>(default, x => x.BaseFile = s.Context.GetFile(GetROMFilePath)));

                        // If the script is invalid we ignore it
                        if (!script.IsValid)
                        {
                            Debug.Log($"Skipping script {script.DisplayName}");
                            continue;
                        }

                        foundScripts.Add(new Tuple<long, string>(getPointer(i), script.Name));
                    }
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
        public static Vector2Int[] TileShapes { get; } = new Vector2Int[]
        {
            new Vector2Int(0x08, 0x08),
            new Vector2Int(0x10, 0x10),
            new Vector2Int(0x20, 0x20),
            new Vector2Int(0x40, 0x40),
            new Vector2Int(0x10, 0x08),
            new Vector2Int(0x20, 0x08),
            new Vector2Int(0x20, 0x10),
            new Vector2Int(0x40, 0x20),
            new Vector2Int(0x08, 0x10),
            new Vector2Int(0x08, 0x20),
            new Vector2Int(0x10, 0x20),
            new Vector2Int(0x20, 0x40),
        };

        // Obj type
        public virtual int ObjTypesCount => 0;
        public virtual uint ObjTypesPointer => 0;
        public virtual ObjTypeInit[] ObjTypeInitInfos => null;

        public void LogObjTypeInit(SerializerObject s)
        {
            // For now we only support games which use animations directly
            if (!GBAVV_Graphics.UsesAnimationsDirectly(s.Context.GetR1Settings()))
                return;

            // Load the animations
            var graphics = new GBAVV_Graphics();
            graphics.Init(s.Context.FilePointer(GetROMFilePath));
            graphics.SerializeImpl(s);
            var animSets = graphics.AnimSets;

            var str = new StringBuilder();

            var initFunctionPointers = s.DoAt(new Pointer(ObjTypesPointer, s.Context.GetFile(GetROMFilePath)), () => s.SerializePointerArray(default, ObjTypesCount));
            var orderedPointers = initFunctionPointers.OrderBy(x => x.AbsoluteOffset).ToArray();

            // Enumerate every obj init function
            for (int i = 0; i < initFunctionPointers.Length; i++)
            {
                var nextPointer = orderedPointers.ElementAtOrDefault(orderedPointers.FindItemIndex(x => x == initFunctionPointers[i]) + 1);

                s.DoAt(initFunctionPointers[i], () =>
                {
                    var foundPointer = false;

                    // Try and read every int as a pointer until we get a valid one 20 times
                    for (int j = 0; j < 20; j++)
                    {
                        if (nextPointer != null && s.CurrentPointer.AbsoluteOffset >= nextPointer.AbsoluteOffset)
                            break;

                        var p = s.SerializePointer(default);

                        // First we check if the pointer leads directly to an animation
                        tryParseAnim(p);

                        // If we found the pointer we're done
                        if (foundPointer)
                            return;

                        // If not we assume it leads to a struct with the animation pointer
                        s.DoAt(p, () =>
                        {
                            // First pointer here should lead to an animation
                            var animPointer = s.SerializePointer(default);

                            tryParseAnim(animPointer);
                        });

                        if (foundPointer)
                            return;

                        // Later games have structs where the second value is the animation pointer
                        s.DoAt(p, () =>
                        {
                            s.Serialize<int>(default);

                            var animPointer = s.SerializePointer(default);

                            tryParseAnim(animPointer);
                        });

                        void tryParseAnim(Pointer ap)
                        {
                            s.DoAt(ap, () =>
                            {
                                // If it's a valid animation the first pointer will lead to a pointer to itself
                                var animSetPointer = s.SerializePointer(default);

                                s.DoAt(animSetPointer, () =>
                                {
                                    var selfPointer = s.SerializePointer(default);

                                    // If not valid, return
                                    if (selfPointer != animSetPointer)
                                        return;

                                    // Sometimes the pointer after the animation pointer leads to a script, so we check that
                                    var scriptName = tryParseScript(s.DoAt(p + 4, () => s.SerializePointer(default))) ??
                                                     tryParseScript(s.DoAt(p + 36, () => s.SerializePointer(default)));

                                    var animSetIndex = animSets.FindItemIndex(x => x.Offset == animSetPointer);
                                    var animIndex = animSets[animSetIndex].Animations.FindItemIndex(x => x.Offset == ap);

                                    str.AppendLine($"new ObjTypeInit({animSetIndex}, {animIndex}, {(scriptName == null ? "null" : $"\"{scriptName}\"")}), // {i}");
                                    foundPointer = true;
                                });
                            });
                        }

                        string tryParseScript(Pointer scriptPointer)
                        {
                            // Attempt to get the script name
                            return s.DoAt(scriptPointer, () =>
                            {
                                var cmd = s.SerializeObject<GBAVV_ScriptCommand>(default, x => x.BaseFile = scriptPointer.File);

                                if (cmd.Type != GBAVV_ScriptCommand.CommandType.Name)
                                    return null;
                                else
                                    return cmd.Name;
                            });
                        }

                        if (foundPointer)
                            return;
                    }

                    // No pointer found...
                    str.AppendLine($"new ObjTypeInit(-1, -1, null), // {i}");
                });
            }

            str.ToString().CopyToClipboard();
        }

        public class ObjTypeInit
        {
            public ObjTypeInit(int animSetIndex, int animIndex, string scriptName, int? jpAnimIndex = null)
            {
                AnimSetIndex = animSetIndex;
                AnimIndex = animIndex;
                ScriptName = scriptName;
                JPAnimIndex = jpAnimIndex;
            }

            public int AnimSetIndex { get; }
            public int AnimIndex { get; }
            public string ScriptName { get; }
            public int? JPAnimIndex { get; }
        }
    }
}