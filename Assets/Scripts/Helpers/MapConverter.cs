using System.Linq;
using BinarySerializer;
using BinarySerializer.Ray1;
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
            var outLev = FileFactory.Read<PC_LevFile>(rdManager.GetLevelFilePath(outputSettings), outputContext);

            // TODO: Set background data

            // Set map data
            outLev.MapData.Width = inputMap.Width;
            outLev.MapData.Height = inputMap.Height;
            //outLev.MapData.ColorPalettes = new RGB666Color[][]
            //{
            //    FileFactory.Read<PCX>(mapperManager.GetPCXFilePath(inputSettings), inputContext).VGAPalette.Select(x => new RGB666Color(x.Red, x.Green, x.Blue)).ToArray()
            //};

            // Set tiles while keeping track of the size changes
            var prevTileSize = outLev.MapData.Tiles.Length * 6;
            outLev.MapData.Tiles = inputMap.MapTiles.Select(x => x.Data.ToR1MapTile()).ToArray();
            var newTileSize = outLev.MapData.Tiles.Length * 6;

            // Update pointers
            outLev.ObjDataBlockPointer += newTileSize - prevTileSize;
            outLev.TextureBlockPointer += newTileSize - prevTileSize;

            // TODO: Set tileset from .pcx file
            //outLev.TileTextureData = null;

            // Get the file tables
            var outputDesNames = FileFactory.Read<PC_WorldFile>(rdManager.GetWorldFilePath(outputSettings), outputContext).DESFileNames;
            var outputEtaNames = FileFactory.Read<PC_WorldFile>(rdManager.GetWorldFilePath(outputSettings), outputContext).ETAFileNames;

            // Set event data
            outLev.ObjData.ObjCount = (ushort)inputLev.EventData.Count;
            outLev.ObjData.ObjLinkingTable = inputObjManager.LinkTable;
            outLev.ObjData.Objects = inputLev.EventData.Cast<Unity_Object_R1>().Select(x =>
            {
                var e = x.EventData;

                var newDesIndex = (uint)outputDesNames.FindItemIndex(y => y == inputObjManager.DES[x.DESIndex].Name);
                var newEtaIndex = (uint)outputEtaNames.FindItemIndex(y => y == inputObjManager.ETA[x.ETAIndex].Name);

                // Set DES and ETA indexes
                e.PC_SpritesIndex = newDesIndex;
                e.PC_ImageBufferIndex = newDesIndex;
                e.PC_AnimationsIndex = newDesIndex;
                e.PC_ETAIndex = newEtaIndex;

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
                    cmds = new ObjCommandCompiler.CompiledObjCommandData(new CommandCollection()
                    {
                        Commands = new Command[0]
                    }, new ushort[0]);

                // Create a command object
                return new PC_CommandCollection
                {
                    CommandLength = (ushort)cmds.Commands.Commands.Select(y => y.Length).Sum(),
                    Commands = cmds.Commands,
                    LabelOffsetCount = (ushort)cmds.LabelOffsets.Length,
                    LabelOffsetTable = cmds.LabelOffsets
                };
            }).ToArray();

            // TODO: Get data from .ini file
            // Set profile define data
            outLev.ProfileDefine = new PC_ProfileDefine
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
            FileFactory.Write<PC_LevFile>(rdManager.GetLevelFilePath(outputSettings), outputContext);
        }
    }
}