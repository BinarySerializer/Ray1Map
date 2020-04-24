using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_Manager : PS1_Manager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 16;

        /// <summary>
        /// The file info to use
        /// </summary>
        protected override Dictionary<string, PS1FileInfo> FileInfo => PS1FileInfo.fileInfoR2PS1;

        // TODO: Is this needed?
        protected override PS1MemoryMappedFile.InvalidPointerMode InvalidPointerMode => PS1MemoryMappedFile.InvalidPointerMode.Allow;

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Enumerable.Range(1, w == World.Jungle ? 4 : 0).ToArray())).ToArray();

        /// <summary>
        /// Gets the name for the specified map
        /// </summary>
        /// <param name="map">The map</param>
        /// <returns>The map name</returns>
        public virtual string GetMapName(int map)
        {
            switch (map)
            {
                case 1:
                    return "PL1";

                case 2:
                    return "PL2";

                case 3:
                    return "FD1";

                case 4:
                    return "FD2";

                default:
                    throw new ArgumentOutOfRangeException(nameof(map));
            }
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Common_Tileset GetTileSet(Context context) {
            var tileSetPath = $"JUNGLE/{GetMapName(context.Settings.Level)}.RAW";
            var palettePath = $"JUNGLE/{GetMapName(context.Settings.Level)}.PAL";
            var tileSet = FileFactory.Read<Array<byte>>(tileSetPath, context, (s, x) => x.Length = s.CurrentLength);
            var palette = FileFactory.Read<ObjectArray<ARGB1555Color>>(palettePath, context, (s, x) => x.Length = s.CurrentLength / 2);

            return new Common_Tileset(tileSet.Value.Select(ind => palette.Value[ind]).ToArray(), TileSetWidth, CellSize);
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The filled v-ram</returns>
        public override void FillVRAM(Context context)
        { }

        public async Task<uint> LoadFile(Context context, string path, uint baseAddress) {
            await FileSystem.PrepareFile(context.BasePath + path);

            Dictionary<string, PS1FileInfo> fileInfo = FileInfo;
            if (baseAddress != 0) {
                PS1MemoryMappedFile file = new PS1MemoryMappedFile(context, baseAddress, InvalidPointerMode) {
                    filePath = path,
                    Length = fileInfo[path].Size
                };
                context.AddFile(file);

                return fileInfo[path].Size;
            } else {
                LinearSerializedFile file = new LinearSerializedFile(context) {
                    filePath = path,
                    length = fileInfo.ContainsKey(path) ? fileInfo[path].Size : 0
                };
                context.AddFile(file);
                return 0;
            }
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            uint baseAddress = 0x80018000;

            // TODO: Move these to methods to avoid hard-coding
            var fixDTAPath = $"RAY.DTA";
            var fixGRPPath = $"RAY.GRP";
            var sprPLSPath = $"SPR.PLS";
            var levelDTAPath = $"JUNGLE/{GetWorldName(context.Settings.World)}01.DTA";
            var levelSPRPath = $"JUNGLE/{GetWorldName(context.Settings.World)}01.SPR"; // SPRites?
            var levelGRPPath = $"JUNGLE/{GetWorldName(context.Settings.World)}01.GRP"; // GRaPhics/graphismes
            // TODO: Load submaps based on levelDTA file
            var tileSetPath = $"JUNGLE/{GetMapName(context.Settings.Level)}.RAW";
            var palettePath = $"JUNGLE/{GetMapName(context.Settings.Level)}.PAL";
            var mapPath = $"JUNGLE/{GetMapName(context.Settings.Level)}.MPU";


            baseAddress += await LoadFile(context, fixDTAPath, baseAddress);
            baseAddress -= 0x5E; // FIX.DTA header size
            Pointer fixDTAHeader = new Pointer(baseAddress, context.FilePointer(fixDTAPath).file);
            context.Deserializer.DoAt(fixDTAHeader, () => {
                // TODO: Read header here (0x5E bytes). Should be done now because these bytes will be overwritten

            });
            await LoadFile(context, fixGRPPath, 0);
            await LoadFile(context, sprPLSPath, 0);
            baseAddress += await LoadFile(context, levelSPRPath, baseAddress);
            baseAddress += await LoadFile(context, levelDTAPath, baseAddress);
            await LoadFile(context, levelGRPPath, 0);
            await LoadFile(context, tileSetPath, 0);
            await LoadFile(context, palettePath, 0);
            await LoadFile(context, mapPath, 0); // TODO: Load all maps for this level

            // Read the level data
            var lvlData = FileFactory.Read<PS1_R2Demo_LevDataFile>(levelDTAPath, context);

            // Read the map block
            var map = FileFactory.Read<PS1_R1_MapBlock>(mapPath, context);

            // Temporary to see which pointers match
            var pointers1 = lvlData.Events.Select(x => x.UnkPointer1).Distinct().OrderBy(x => x?.AbsoluteOffset).ToArray();
            var pointers2 = lvlData.Events.Select(x => x.UnkPointer2).Distinct().OrderBy(x => x?.AbsoluteOffset).ToArray();
            var pointers3 = lvlData.Events.Select(x => x.UnkPointer3).Distinct().OrderBy(x => x?.AbsoluteOffset).ToArray();

            // Load the level
            var level = await LoadAsync(context, map, null, null, loadTextures);

            var index = 0;

            // Add every event
            foreach (var e in lvlData.Events)
            {
                // Add the event
                level.Level.EventData.Add(new Common_EventData
                {
                    //Type = e.Type,
                    //Etat = e.Etat,
                    //SubEtat = e.SubEtat,
                    XPosition = e.XPosition,
                    YPosition = e.YPosition,
                    DESKey = String.Empty,
                    ETAKey = String.Empty,
                    //OffsetBX = e.OffsetBX,
                    //OffsetBY = e.OffsetBY,
                    //OffsetHY = e.OffsetHY,
                    //FollowSprite = e.FollowSprite,
                    //HitPoints = e.Hitpoints,
                    Layer = e.Layer,
                    //HitSprite = e.HitSprite,
                    //FollowEnabled = e.GetFollowEnabled(context.Settings),
                    //LabelOffsets = e.LabelOffsets,
                    //CommandCollection = e.Commands,
                    LinkIndex = lvlData.EventLinkTable[index],
                    DebugText = $"Unk1: {String.Join("-", e.Unk1)}{Environment.NewLine}" +
                                $"Unk2: {String.Join("-", e.Unk2)}{Environment.NewLine}" +
                                $"Unk3: {String.Join("-", e.Unk3)}{Environment.NewLine}" +
                                $"Unk4: {String.Join("-", e.Unk4)}{Environment.NewLine}" +
                                $"Unk5: {String.Join("-", e.Unk5)}{Environment.NewLine}" +
                                $"PointerGroup1: {pointers1.FindItemIndex(y => y == e.UnkPointer1)}{Environment.NewLine}" +
                                $"PointerGroup2: {pointers2.FindItemIndex(y => y == e.UnkPointer2)}{Environment.NewLine}" +
                                $"PointerGroup3: {pointers3.FindItemIndex(y => y == e.UnkPointer3)}{Environment.NewLine}"
                });

                index++;
            }
            
            // Return the level
            return level;
        }

        /// <summary>
        /// Gets the vignette file info
        /// </summary>
        /// <returns>The vignette file info</returns>
        protected override PS1VignetteFileInfo[] GetVignetteInfo() => new PS1VignetteFileInfo[]
        {
            new PS1VignetteFileInfo("LOGO_UBI.TIM", 320),
            new PS1VignetteFileInfo("JUNGLE/NEWFND16.TIM", 320),
        };
    }
}