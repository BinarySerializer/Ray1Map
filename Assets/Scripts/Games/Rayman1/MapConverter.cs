using System.Linq;
using BinarySerializer;
using BinarySerializer.Ray1;
using BinarySerializer.Ray1.PC;
using Cysharp.Threading.Tasks;
using Ray1Map.Rayman1;

namespace Ray1Map
{
    public static class MapConverter
    {
        public static async UniTask MapperToKITAsync(GameSettings inputSettings, GameSettings outputSettings)
        {
            using var inputContext = new Ray1MapContext(inputSettings);
            using var outputContext = new Ray1MapContext(outputSettings);

            // Create managers
            var mapperManager = new R1_Mapper_Manager();
            var rdManager = new R1_Kit_Manager();

            // Load files to context
            await mapperManager.LoadFilesAsync(inputContext);
            await rdManager.LoadFilesAsync(outputContext);

            // Load the mapper level
            var inputLev = (await mapperManager.LoadAsync(inputContext));
            var inputMap = inputLev.Maps[0];
            var inputObjManager = (Unity_ObjectManager_R1)inputLev.ObjManager;

            // Load the editor manager data for the output level
            var outData = await rdManager.LoadAsync(outputContext);
            var outputObjManager = (Unity_ObjectManager_R1)outData.ObjManager;

            // Load a dummy PC level as a base
            var outLev = FileFactory.Read<LevelFile>(outputContext, rdManager.GetLevelFilePath(outputSettings));

            // TODO: Set background data

            // Set map data
            outLev.MapInfo.Width = inputMap.Width;
            outLev.MapInfo.Height = inputMap.Height;
            //outLev.MapData.ColorPalettes = new RGB666Color[][]
            //{
            //    FileFactory.Read<PCX>(mapperManager.GetPCXFilePath(inputSettings), inputContext).VGAPalette.Select(x => new RGB666Color(x.Red, x.Green, x.Blue)).ToArray()
            //};

            // Set tiles while keeping track of the size changes
            var prevTileSize = outLev.MapInfo.Blocks.Length * 6;
            outLev.MapInfo.Blocks = inputMap.MapTiles.Select(x => x.Data.ToR1MapTile()).ToArray();
            var newTileSize = outLev.MapInfo.Blocks.Length * 6;

            // Update pointers
            outLev.ObjectsPointer += newTileSize - prevTileSize;
            outLev.NormalBlockTexturesPointer += newTileSize - prevTileSize;

            // TODO: Set tileset from .pcx file
            //outLev.TileTextureData = null;

            // Get the file tables
            var outputDesNames = FileFactory.Read<WorldFile>(outputContext, rdManager.GetWorldFilePath(outputSettings)).DESFileNames;
            var outputEtaNames = FileFactory.Read<WorldFile>(outputContext, rdManager.GetWorldFilePath(outputSettings)).ETAFileNames;

            // Set event data
            outLev.ObjData.ObjectsCount = (ushort)inputLev.EventData.Count;
            outLev.ObjData.ObjLinkingTable = inputObjManager.LinkTable;
            outLev.ObjData.Objects = inputLev.EventData.Cast<Unity_Object_R1>().Select(x =>
            {
                var e = x.EventData;

                var newDesIndex = (uint)outputDesNames.FindItemIndex(y => y == inputObjManager.DES[x.DESIndex].Name);
                var newEtaIndex = (uint)outputEtaNames.FindItemIndex(y => y == inputObjManager.ETA[x.ETAIndex].Name);

                // Set DES and ETA indexes
                e.PCPacked_SpritesIndex = newDesIndex;
                e.PCPacked_ImageBufferIndex = newDesIndex;
                e.PCPacked_AnimationsIndex = newDesIndex;
                e.PCPacked_ETAIndex = newEtaIndex;

                // Set image and animation descriptor counts
                e.SpritesCount = (ushort)outputObjManager.DES[x.DESIndex].Data.Graphics.Sprites.Count;
                e.AnimationsCount = (byte)outputObjManager.DES[x.DESIndex].Data.Graphics.Animations.Count;

                return e;
            }).ToArray();

            // Set event commands
            outLev.ObjData.ObjCommands = inputLev.EventData.Cast<Unity_Object_R1>().Select(x =>
            {
                // Get the commands in the compiled format
                var cmds = ObjCommandCompiler.Compile(x.EventData.Commands);

                // Remove commands which only contain the invalid command
                if (cmds.Commands.Commands.Length == 1)
                    cmds = new ObjCommandCompiler.CompiledObjCommandData(new ObjCommands()
                    {
                        Commands = new Command[0]
                    }, new ushort[0]);

                // Create a command object
                return new ObjCommandsData
                {
                    Commands = cmds.Commands,
                    LabelOffsetTable = cmds.LabelOffsets
                };
            }).ToArray();

            // TODO: Get data from .ini file
            // Set profile define data
            outLev.ProfileDefine = new ProfileDefine
            {
                LevelName = "Test Export",
                LevelAuthor = "RayCarrot",
                LevelDescription = "This is a test export map",
                Power_Fist = true,
                Power_Hang = true,
                Power_Run = true,
                Power_Seed = false,
                Power_Helico = true,
                Power_SuperHelico = false
            };

            // Write the changes to the file
            FileFactory.Write<LevelFile>(outputContext, rdManager.GetLevelFilePath(outputSettings));
        }
    }
}