using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_Crash2_Manager : GBAVV_Crash_BaseManager
    {
        // Metadata
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new GameAction("Export isometric character icons", false, true, (input, output) => ExportIsometricCharacterIcons(settings, output)),
            }).ToArray();
        }

        // Exports
        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_Crash2>(context, GetROMFilePath, (s, d) => d.CurrentLevInfo = LevInfos.First());
        public override GBAVV_ROM_Generic LoadROMForMode7Export(Context context, int level) => FileFactory.Read<GBAVV_ROM_Crash2>(context, GetROMFilePath, (s, d) => d.CurrentLevInfo = new CrashLevInfo(GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Mode7, (short)level, null));

        public override async UniTask ExportAnimFramesAsync(GameSettings settings, string outputDir, bool saveAsGif, bool includePointerInNames = true)
        {
            // Export 2D and Mode7 animations
            await base.ExportAnimFramesAsync(settings, outputDir, saveAsGif, includePointerInNames);

            // Export isometric animations
            using (var context = new Ray1MapContext(settings))
            {
                // Load the files
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBAVV_ROM_Crash2>(context, GetROMFilePath, (s, d) => d.CurrentLevInfo = new CrashLevInfo(GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Isometric, 0, null));

                var pal = Util.ConvertAndSplitGBAPalette(rom.Isometric_GetObjPalette);

                var animations = rom.Isometric_GetAnimations.ToArray();

                // Enumerate every animation
                for (var i = 1; i < animations.Length; i++)
                {
                    await Controller.WaitFrame();

                    var frames = GetIsometricAnimFrames(animations[i], pal);

                    Util.ExportAnim(
                        frames: frames,
                        speed: 4,
                        center: true,
                        saveAsGif: saveAsGif,
                        outputDir: Path.Combine(outputDir, "Isometric"),
                        primaryName: $"{i}",
                        secondaryName: $"0");
                }
            }

            Debug.Log($"Finished exporting isometric animations");
        }
        public override async UniTask ExportCutscenesAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Ray1MapContext(settings))
            {
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBAVV_ROM_Crash2>(context, GetROMFilePath, (s, d) => {
					d.CurrentLevInfo = LevInfos.First();
					d.SerializeFLC = true;
				});

                var index = 0;

                // Enumerate every .flc entry
                foreach (var entry in rom.FLCTable)
                {
                    using (var collection = entry.FLC.ToMagickImageCollection())
                        collection.Write(Path.Combine(outputDir, $"{index++}.gif"));
                }
            }
        }
        public async UniTask ExportIsometricCharacterIcons(GameSettings settings, string outputDir)
        {
            using (var context = new Ray1MapContext(settings))
            {
                // Load the files
                await LoadFilesAsync(context);

                // Read the rom
                var rom = FileFactory.Read<GBAVV_ROM_Crash2>(context, GetROMFilePath, (s, d) => d.CurrentLevInfo = new CrashLevInfo(GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Isometric, 0, null));

                var pal = Util.ConvertAndSplitGBAPalette(rom.Isometric_GetObjPalette);

                // Enumerate every character
                for (int i = 0; i < rom.Isometric_CharacterIcons.Length; i++)
                {
                    var tex = Util.ToTileSetTexture(rom.Isometric_CharacterIcons[i].TileSet.TileSet, pal[rom.Isometric_CharacterIcons[i].PaletteIndex], Util.TileEncoding.Linear_4bpp, CellSize, true, wrap: 2);

                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{i:00}_{rom.Isometric_CharacterInfos[i].Name}.png"), tex.EncodeToPNG());
                }
            }
        }

        // Load
        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM_Crash2>(context, GetROMFilePath, (s, r) => r.CurrentLevInfo = LevInfos[context.GetR1Settings().Level]);
            var levInfo = rom.CurrentLevInfo;
            var map = rom.CurrentMapInfo;

            if (map.Crash_MapType == GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Mode7)
                return await LoadMode7Async(context, rom, rom.CurrentMode7LevelInfo);
            else if (map.Crash_MapType == GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Isometric)
                return await LoadIsometricAsync(context, rom);
            else if (levInfo.IsWorldMap)
                return await LoadMap2DAsync(context, rom, rom.WorldMap);
            else
                return await LoadMap2DAsync(context, rom, rom.CurrentMapInfo, rom.GetTheme);
        }
        public async UniTask<Unity_Level> LoadIsometricAsync(Context context, GBAVV_ROM_Crash2 rom)
        {
            var mapData = rom.CurrentIsometricMapData;
            var objData = rom.CurrentIsometricObjData;

            Controller.DetailedState = "Loading maps & tilesets";
            await Controller.WaitIfNecessary();

            var tileSet = LoadIsometricTileSet(mapData.TileSet, mapData.TilePalette);

            var maps = mapData.MapLayers.Select((map, i) => new Unity_Map
            {
                Width = (ushort)(map.Width * 2),
                Height = (ushort)(map.Height * 2),
                TileSet = new Unity_TileSet[]
                {
                    tileSet,
                },
                MapTiles = GetTileMap(map, mapData.MapTiles),
                Type = Unity_Map.MapType.Graphics,
                Layer = i == 0 ? Unity_Map.MapLayer.Front : Unity_Map.MapLayer.Middle
            }).Reverse().ToArray();

            Controller.DetailedState = "Loading localization";
            await Controller.WaitIfNecessary();

            var loc = LoadLocalization(rom);

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();
            int baseType = (int)Unity_IsometricCollisionTile.CollisionType.GBAVV_Solid_0;

            float minHeight = Mathf.Min(0, mapData.CollisionMap.Min(c => mapData.CollisionTiles[c].Height.AsFloat));
            var collision = mapData.CollisionMap.Select(c => new Unity_IsometricCollisionTile()
            {
                GBAVV_Rotation = mapData.CollisionTiles[c].Shape % 4,
                GBAVV_AdditionalHeight = mapData.CollisionTypes[mapData.CollisionTiles[c].TypeIndex].AdditionalHeight,
                DebugText = Settings.ShowDebugInfo ? ($"Height: {(mapData.CollisionTiles[c].Height - minHeight)}{Environment.NewLine}" +
                $"Tile index: {c}{Environment.NewLine}" +
                $"Offset: {mapData.CollisionTiles[c].Offset}{Environment.NewLine}" +
                $"Type: {mapData.CollisionTiles[c].TypeIndex}{Environment.NewLine}" +
                $"Shape: {mapData.CollisionTiles[c].Shape}") : null,
                Height = (mapData.CollisionTiles[c].Height - minHeight),
                Type = (Unity_IsometricCollisionTile.CollisionType)(baseType + GetIsometricCollisionType(rom.CurrentIsometricIndex, mapData.CollisionTiles[c].TypeIndex))
            }).ToArray();
            var mirroredCollision = new Unity_IsometricCollisionTile[mapData.CollisionWidth * mapData.CollisionHeight];
            for (int x = 0; x < mapData.CollisionWidth; x++)
            {
                for (int y = 0; y < mapData.CollisionHeight; y++)
                {
                    mirroredCollision[x * mapData.CollisionHeight + y] = collision[y * mapData.CollisionWidth + x];
                }
            }

            // X/Z dimensions: the diagonal of one collision tile is 12 graphics tiles. Height is 6 which matches 12 * sin(30 deg).
            // Height: a 0,1875 is 6 tiles => 6/0.1875 = 32 => viewed at an angle, so divide by cos(angle)
            float tileDiagonal = 12 / 2f;
            float tileWidth = Mathf.Sqrt(tileDiagonal * tileDiagonal / 2);
            float heightScale = 32f / Mathf.Cos(Mathf.Deg2Rad * 30f) / 2f;

            // Create the object manager and load animations
            var objManager = new Unity_ObjectManager_GBAVVIsometric(context, LoadIsometricAnimations(rom), mapData, x => GetIsometricCollisionType(rom.CurrentIsometricIndex, x));

            // Load normal objects
            var objects = objData.Objects.Select(x => (Unity_Object_BaseGBAVVIsometric)new Unity_Object_GBAVVIsometric_Obj(x, objManager));

            // Load target objects
            var index = objData.Objects.Length;

            foreach (var targetObj in objData.TargetObjects)
            {
                objects = objects.Append(new Unity_Object_GBAVVIsometric_TargetObj(targetObj, objManager, index + 1));
                objects = objects.Append(new Unity_Object_GBAVVIsometric_TargetObjTarget(targetObj, objManager));

                index += 2;
            }

            // Load start positions
            for (int i = 0; i < objData.StartPositions.Length; i++)
                objects = objects.Append(new Unity_Object_GBAVVIsometric_StartPos(objData.StartPositions[i], i, objManager));

            // Load multiplayer flags
            objects = objData.MultiplayerFlags.Aggregate(objects, (current, o) => current.Append(new Unity_Object_GBAVVIsometric_MultiplayerFlag(new GBAVV_Isometric_Position()
            {
                XPos = new FixedPointInt32() { Value = o.XPos * 0x3000 + 0x1800 },
                YPos = new FixedPointInt32() { Value = o.YPos * 0x3000 + 0x1800 },
            }, objManager)));

            // Load multiplayer crowns
            objects = objData.MultiplayerCrowns.Aggregate(objects, (current, o) => current.Append(new Unity_Object_GBAVVIsometric_MultiplayerCrown(new GBAVV_Isometric_Position()
            {
                XPos = new FixedPointInt32() { Value = o.XPos << 8 },
                YPos = new FixedPointInt32() { Value = o.YPos << 8 },
            }, objManager)));

            float w = mapData.MapWidth * 0.5f;
            float h = mapData.MapHeight * 0.5f;

            return new Unity_Level()
            {
                Maps = maps,
                ObjManager = objManager,
                EventData = new List<Unity_SpriteObject>(objects),
                CellSize = CellSize,
                IsometricData = new Unity_IsometricData()
                {
                    CollisionMapWidth = mapData.CollisionHeight,
                    CollisionMapHeight = mapData.CollisionWidth,
                    TilesWidth = mapData.MapWidth,
                    TilesHeight = mapData.MapHeight,
                    CollisionMap = mirroredCollision,
                    Scale = new Vector3(tileWidth, heightScale, tileWidth),
                    // Multiply X & Y displacement by 2 as it is divided by 2 later
                    CalculateXDisplacement = () => w - 16 * mapData.XPosition * 2,
                    CalculateYDisplacement = () => h - 16 * mapData.YPosition * 2 + (minHeight * heightScale * 2 * Mathf.Cos(Mathf.Deg2Rad * 30f)),
                    ObjectScale = Vector3.one * 12 / 64f
                },
                Localization = loc.Item1,
            };
        }

        // Mode7
        public override int[] Mode7AnimSetCounts => new int[]
        {
            41, 55
        };
        public override int Mode7LevelsCount => 7;

        // Isometric animations
        public Unity_ObjectManager_GBAVVIsometric.GraphicsData[] LoadIsometricAnimations(GBAVV_ROM_Crash2 rom)
        {
            var pal = Util.ConvertAndSplitGBAPalette(rom.Isometric_GetObjPalette);

            return rom.Isometric_GetAnimations.Select(anim => new Unity_ObjectManager_GBAVVIsometric.GraphicsData(
                animFrames: GetIsometricAnimFrames(anim, pal).Select(x => x.CreateSprite()).ToArray(),
                animSpeed: 4,
                crashAnim: anim)).ToArray();
        }
        public Texture2D[] GetIsometricAnimFrames(GBAVV_Isometric_Animation anim, Color[][] palette)
        {
            if (anim.AnimFrames[0] == null)
                return new Texture2D[0];

            var output = new Texture2D[anim.AnimFrames.Length];
            var pal = anim.Palette != null ? Util.ConvertGBAPalette(anim.Palette) : palette[anim.PaletteIndex];

            for (int frameIndex = 0; frameIndex < anim.AnimFrames.Length; frameIndex++)
                output[frameIndex] = Util.ToTileSetTexture(anim.AnimFrames[frameIndex], pal, Util.TileEncoding.Linear_4bpp, CellSize, true, wrap: anim.Width / CellSize, flipX: anim.IsFlipped);

            return output;
        }

        // Isometric tileset
        public Unity_TileSet LoadIsometricTileSet(GBAVV_Isometric_TileSet tileSet, RGBA5551Color[] tilePal)
        {
            var pal = Util.ConvertGBAPalette(tilePal);

            // The game converts the 4bpp tileset to an 8bpp tileset using the convert data

            var convertedTileSet = new byte[tileSet.TileSetCount_4bpp * 0x40];

            for (int i = 0; i < tileSet.TileSetCount_4bpp; i++)
            {
                var convertData = tileSet.TileSet_4bpp_ConvertDatas[tileSet.TileSet_4bpp_ConvertIndexTable[i]];

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        var index = y * 8 + x;

                        var b = convertData.Data[BitHelpers.ExtractBits(tileSet.TileSet_4bpp[i * 0x20 + Mathf.FloorToInt(index / 2f)], 4, index % 2 == 0 ? 0 : 4)];
                        convertedTileSet[i * 0x40 + (y * 8 + (8 - x - 1))] = b;
                    }
                }
            }

            var completeTileSet = convertedTileSet.Concat(tileSet.TileSet_8bpp).ToArray();

            var tex = Util.ToTileSetTexture(completeTileSet, pal, Util.TileEncoding.Linear_8bpp, CellSize, false);

            // Palette animation
            var palIndices = new byte[] { 0x61, 0x86, 0x96 };
            var animatedPalettes = GetAnimatedPalettes(new byte[][]
            {
                palIndices.Reverse().ToArray()
            }, tilePal);
            var palIndicesLookup = new HashSet<byte>(palIndices);

            var additionalTiles = new List<Texture2D>();
            var tileAnimations = new List<Unity_AnimatedTile>();
            var tilesCount = tileSet.TileSetCount_Total;
            const int tileSize = 0x40;
            const int animSpeed = 3; // Note: Might not be correct

            var animatedPalettes_8 = animatedPalettes.Select(x => Util.ConvertGBAPalette(x)).ToArray();

            for (int tileIndex = 0; tileIndex < tilesCount; tileIndex++)
            {
                // Check if the tile uses any of the animated colors

                // Check if the tile uses any of the animated colors
                // If the tile doesn't contain an animated color we ignore it
                bool containsAnimatedPalette = false;
                for (int i = tileSize * tileIndex; i < tileSize * tileIndex + tileSize; i++)
                {
                    if (palIndicesLookup.Contains(completeTileSet[i]))
                    {
                        containsAnimatedPalette = true;
                        break;
                    }
                }
                if (!containsAnimatedPalette) continue;

                // Add animation for the tile
                tileAnimations.Add(new Unity_AnimatedTile
                {
                    AnimationSpeed = animSpeed * 2,
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
                        imgData: completeTileSet,
                        imgDataOffset: tileSize * tileIndex,
                        pal: animatedPalettes_8[animFrame],
                        encoding: Util.TileEncoding.Linear_8bpp,
                        tileWidth: CellSize,
                        flipTextureY: false,
                        tileX: 0,
                        tileY: 0);

                    // Add to additional tiles list
                    additionalTiles.Add(tileTex);
                }
            }

            // Create the tile array
            var tiles = new Unity_TileTexture[tilesCount + additionalTiles.Count];

            // Keep track of the index
            var finalTileIndex = 0;

            // Add every normal tile
            for (int y = 0; y < tex.height; y += CellSize)
            {
                for (int x = 0; x < tex.width; x += CellSize)
                {
                    if (finalTileIndex >= tilesCount)
                        break;

                    // Create a tile
                    tiles[finalTileIndex++] = tex.CreateTile(new RectInt(x, y, CellSize, CellSize));
                }
            }

            // Add additional tiles
            foreach (Texture2D t in additionalTiles)
                tiles[finalTileIndex++] = t.CreateTile();

            return new Unity_TileSet(tiles)
            {
                AnimatedTiles = tileAnimations.Count == 0 ? null : tileAnimations.ToArray()
            };
        }

        // Isometric collision
        public static byte GetIsometricCollisionType(int level, int index)
        {
            switch (level)
            {
                case 0:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 1;
                        case 2: return 2;
                    }
                    break;
                case 1:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 3;
                        case 2: return 1;
                        case 3: return 4;
                        case 4: return 5;
                    }
                    break;
                case 2:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 3;
                    }
                    break;
                case 3:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 6;
                        case 2: return 7;
                        case 3: return 8;
                        case 4: return 9;
                        case 5: return 10;
                        case 6: return 11;
                    }
                    break;
                case 4:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 12;
                        case 2: return 13;
                        case 3: return 1;
                        case 4: return 14;
                        case 5: return 15;
                        case 6: return 16;
                        case 7: return 17;
                        case 8: return 18;
                        case 9: return 19;
                        case 10: return 20;
                        case 11: return 21;
                        case 12: return 22;
                        case 13: return 23;
                        case 14: return 24;
                        case 15: return 25;
                        case 16: return 26;
                    }
                    break;
                case 5:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 27;
                        case 2: return 12;
                        case 3: return 13;
                        case 4: return 26;
                        case 5: return 28;
                        case 6: return 1;
                        case 7: return 29;
                        case 8: return 30;
                        case 9: return 19;
                        case 10: return 31;
                        case 11: return 32;
                        case 12: return 24;
                        case 13: return 21;
                    }
                    break;
                case 6:
                    switch (index)
                    {
                        case 0: return 0;
                        case 1: return 1;
                        case 2: return 24;
                        case 3: return 17;
                        case 4: return 28;
                        case 5: return 32;
                        case 6: return 33;
                        case 7: return 25;
                        case 8: return 26;
                        case 9: return 34;
                        case 10: return 12;
                        case 11: return 19;
                        case 12: return 21;
                        case 13: return 35;
                    }
                    break;
            }

            throw new Exception($"Invalid collision type {index} in level {level}");
        }

        // Localization
        public override int LocTableCount => 191;

        // Levels
        public override CrashLevInfo[] LevInfos => Levels;
        public static CrashLevInfo[] Levels = new CrashLevInfo[]
        {
            new CrashLevInfo(0, 0, "Island Intro"), 
            new CrashLevInfo(1, 0, "Prints of Persia"), 
            new CrashLevInfo(1, CrashLevInfo.Type.Bonus, "Prints of Persia"), 
            new CrashLevInfo(2, 0, "Lagoony Tunes"), 
            new CrashLevInfo(3, 0, "Globe Trottin'"), 
            new CrashLevInfo(4, 0, "Pharaoh's Funhouse"), 
            new CrashLevInfo(4, CrashLevInfo.Type.Bonus, "Pharaoh's Funhouse"), 
            new CrashLevInfo(5, 0, "Runaway Rug"), 
            new CrashLevInfo(5, CrashLevInfo.Type.Bonus, "Runaway Rug"), 
            new CrashLevInfo(6, 0, "Tiki Torture"), 
            new CrashLevInfo(6, CrashLevInfo.Type.Bonus, "Tiki Torture"), 
            new CrashLevInfo(7, 0, "Hoppin' Coffins"), 
            new CrashLevInfo(7, CrashLevInfo.Type.Bonus, "Hoppin' Coffins"), 
            new CrashLevInfo(8, 0, "Barrel Roll"), 
            new CrashLevInfo(9, 0, "Flockful of Seagulls"), 
            new CrashLevInfo(10, 0, "Magma Mania"), 
            new CrashLevInfo(10, CrashLevInfo.Type.Bonus, "Magma Mania"), 
            new CrashLevInfo(11, 0, "Run from the Sun"), 
            new CrashLevInfo(12, 0, "Now it's Istanbul"), 
            new CrashLevInfo(12, CrashLevInfo.Type.Bonus, "Now it's Istanbul"), 
            new CrashLevInfo(13, 0, "Mister Lava Lava"), 
            new CrashLevInfo(13, CrashLevInfo.Type.Bonus, "Mister Lava Lava"), 
            new CrashLevInfo(14, 0, "Water Logged"), 
            new CrashLevInfo(15, 0, "Slip-n-slidin' Sphinx"), 
            new CrashLevInfo(15, CrashLevInfo.Type.Bonus, "Slip-n-slidin' Sphinx"), 
            new CrashLevInfo(16, 0, "Rocks can Roll"), 
            new CrashLevInfo(17, 0, "Rock the Casaba"), 
            new CrashLevInfo(17, CrashLevInfo.Type.Bonus, "Rock the Casaba"), 
            new CrashLevInfo(18, 0, "Eruption Disruption"), 
            new CrashLevInfo(18, CrashLevInfo.Type.Bonus, "Eruption Disruption"), 
            new CrashLevInfo(19, 0, "Spaced Out"), 
            new CrashLevInfo(20, 0, "King too Uncommon"), 
            new CrashLevInfo(20, CrashLevInfo.Type.Bonus, "King too Uncommon"), 
            new CrashLevInfo(21, 0, "Wild Nile Ride"), 
            new CrashLevInfo(22, 0, "101 Arabian Kites"), 
            new CrashLevInfo(23, 0, "Fire Walker"), 
            new CrashLevInfo(24, 0, "Evil Crunch"), 
            new CrashLevInfo(25, 0, "Evil Coco"), 
            new CrashLevInfo(26, 0, "Fake Crash"), 
            new CrashLevInfo(27, 0, "N. Trance - Part 1"), 
            new CrashLevInfo(27, 1, "N. Trance - Part 2"), 
            new CrashLevInfo(28, 0, "N. Tropy - Part 1"), 
            new CrashLevInfo(28, 1, "N. Tropy - Part 2"), 
            new CrashLevInfo(28, 2, "N. Tropy - Part 3"), 

            // Duplicates of Mode7 level 2 - probably here since the Mode7 array was copies from the previous game which has 7 entries
            new CrashLevInfo(GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Mode7, 5, "Mode7 - Duplicate Level 5"), 
            new CrashLevInfo(GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Mode7, 6, "Mode7 - Duplicate Level 6"), 

            new CrashLevInfo(GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Isometric, 0 - 4, "Isometric - Test"), 
            new CrashLevInfo(GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Isometric, 1 - 4, "Isometric - Prototype"), 
            new CrashLevInfo(GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Isometric, 2 - 4, "Isometric - Standin"), 
            new CrashLevInfo(GBAVV_Generic_MapInfo.GBAVV_Crash_MapType.Isometric, 3 - 4, "Isometric - Demo"), 

            new CrashLevInfo(null, 0, "World Map"),
        };
    }

    public class GBAVV_Crash2EU_Manager : GBAVV_Crash2_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x084768b4
        };
    }
    public class GBAVV_Crash2US_Manager : GBAVV_Crash2_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0846dd74
        };
    }
    public class GBAVV_Crash2JP_Manager : GBAVV_Crash2_Manager
    {
        public override int LocTableCount => 202;
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x08460b70
        };
    }
}