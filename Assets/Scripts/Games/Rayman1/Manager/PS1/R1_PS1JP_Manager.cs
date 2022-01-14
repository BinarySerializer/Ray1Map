using BinarySerializer;
using BinarySerializer.PS1;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.Rayman1
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1 - Japan)
    /// </summary>
    public class R1_PS1JP_Manager : R1_PS1BaseXXXManager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 1;

        protected override PS1_MemoryMappedFile.InvalidPointerMode InvalidPointerMode => PS1_MemoryMappedFile.InvalidPointerMode.Allow;

        public string GetSpecialTileSetPath(GameSettings settings) => GetWorldFolderPath(settings.R1_World) + $"{GetWorldName(settings.R1_World)}{settings.Level:00}.BLC";

        /// <summary>
        /// Gets the tile set colors to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set colors to use</returns>
        public IList<BaseColor> GetTileSetColors(Context context)
        {
            var levelTileSetFileName = GetSpecialTileSetPath(context.GetR1Settings());

            if (FileSystem.FileExists(context.GetAbsoluteFilePath(levelTileSetFileName)))
            {
                ObjectArray<RGBA5551Color> cols = FileFactory.Read<ObjectArray<RGBA5551Color>>(context, levelTileSetFileName, onPreSerialize: (s, x) => x.Pre_Length = s.CurrentLength / 2);
                return cols.Value;
            }

            // Get the file name
            var filename = GetWorldFilePath(context.GetR1Settings());

            // Read the file
            var worldJPFile = FileFactory.Read<PS1_WorldFile>(context, filename);

            // Return the tile set
            return worldJPFile.RawTiles;
        }

        public override string ExeFilePath => "PSX.EXE";
        public override uint? ExeBaseAddress => 0x80128000 - 0x800;
        protected override PS1_ExecutableConfig GetExecutableConfig => PS1_ExecutableConfig.PS1_JP;

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override Unity_TileSet GetTileSet(Context context)
        {
            if (context.GetR1Settings().R1_World == World.Menu)
                return new Unity_TileSet(Settings.CellSize);

            return new Unity_TileSet(GetTileSetColors(context), TileSetWidth, Settings.CellSize);
        }

        /// <summary>
        /// Fills the PS1 v-ram and returns it
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="mode">The blocks to fill</param>
        /// <returns>The filled v-ram</returns>
        protected override void FillVRAM(Context context, PS1VramHelpers.VRAMMode mode)
        {
            // TODO: Support BigRay + font

            // Read the files
            var allFix = mode != PS1VramHelpers.VRAMMode.BigRay ? FileFactory.Read<PS1_AllfixFile>(context, GetAllfixFilePath(context.GetR1Settings())) : null;
            var world = mode == PS1VramHelpers.VRAMMode.Level ? FileFactory.Read<PS1_WorldFile>(context, GetWorldFilePath(context.GetR1Settings())) : null;
            var lev = mode == PS1VramHelpers.VRAMMode.Level ? FileFactory.Read<PS1_LevFile>(context, GetLevelFilePath(context.GetR1Settings())) : null;
            var bigRay = mode == PS1VramHelpers.VRAMMode.BigRay ? FileFactory.Read<PS1_BigRayFile>(context, GetBigRayFilePath(context.GetR1Settings())) : null;
            var font = mode == PS1VramHelpers.VRAMMode.Menu ? FileFactory.Read<Array<byte>>(context, GetFontFilePath(context.GetR1Settings()), (s, o) => o.Length = s.CurrentLength) : null;

            var vram = PS1VramHelpers.PS1_JP_FillVRAM(mode, allFix, world, bigRay, lev, font?.Value, mode == PS1VramHelpers.VRAMMode.Level ? GetTileSetColors(context).Count : 0);

            context.StoreObject("vram", vram);
        }

        /// <summary>
        /// Loads the level specified by the settings for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The level</returns>
        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            Controller.DetailedState = $"Loading allfix";

            // Read the allfix file
            await LoadExtraFile(context, GetAllfixFilePath(context.GetR1Settings()), false);
            FileFactory.Read<PS1_AllfixFile>(context, GetAllfixFilePath(context.GetR1Settings()));

            PS1_ObjBlock objBlock = null;
            MapData mapData;

            if (context.GetR1Settings().R1_World != World.Menu)
            {
                Controller.DetailedState = $"Loading world file";

                await Controller.WaitIfNecessary();

                // Read the world file
                await LoadExtraFile(context, GetWorldFilePath(context.GetR1Settings()), false);
                FileFactory.Read<PS1_WorldFile>(context, GetWorldFilePath(context.GetR1Settings()));

                Controller.DetailedState = $"Loading map data";

                // Read the level data
                await LoadExtraFile(context, GetLevelFilePath(context.GetR1Settings()), true);
                var level = FileFactory.Read<PS1_LevFile>(context, GetLevelFilePath(context.GetR1Settings()));

                objBlock = level.ObjData;
                mapData = level.MapData;

                // Load special tile set file
                await LoadExtraFile(context, GetSpecialTileSetPath(context.GetR1Settings()), true);
            }
            else
            {
                await LoadExtraFile(context, GetFontFilePath(context.GetR1Settings()), false);

                mapData = MapData.GetEmptyMapData(384 / Settings.CellSize, 288 / Settings.CellSize);
            }

            // Load the level
            return await LoadAsync(context, mapData, objBlock?.Objects, objBlock?.ObjectLinkingTable.Select(x => (ushort)x).ToArray());
        }

        public override void AddContextPointers(Context context)
        {
            context.AddPreDefinedPointers(PS1_DefinedPointers.PS1_JP);
        }
    }
}