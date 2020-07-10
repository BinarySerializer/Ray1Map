using System.Linq;
using System.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public static class MapConverter
    {
        public static async Task MapperToRDAsync(GameSettings inputSettings, GameSettings outputSettings)
        {
            using (var inputContext = new Context(inputSettings))
            {
                using (var outputContext = new Context(outputSettings))
                {
                    // Create managers
                    var mapperManager = new PC_Mapper_Manager();
                    var rdManager = new PC_RD_Manager();

                    // Load files to context
                    await mapperManager.LoadFilesAsync(inputContext);
                    await rdManager.LoadFilesAsync(outputContext);

                    // Load the mapper level
                    var inputLev = (await mapperManager.LoadAsync(inputContext, false)).Level;
                    var inputMap = inputLev.Maps[0];

                    // Load the editor manager data for the output level
                    var outData = await rdManager.LoadAsync(outputContext, true);
                    
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
                    outLev.MapData.Tiles = inputMap.MapTiles.Select(x => x.Data).ToArray();
                    var newTileSize = outLev.MapData.Tiles.Length * 6;

                    // Update pointers
                    outLev.EventBlockPointer += newTileSize - prevTileSize;
                    outLev.TextureBlockPointer += newTileSize - prevTileSize;

                    // TODO: Set tilemap from .pcx file
                    //outLev.TileTextureData = null;

                    // Get the file tables
                    var desNames = FileFactory.Read<PC_WorldFile>(rdManager.GetWorldFilePath(outputSettings), outputContext).DESFileNames;
                    var etaNames = FileFactory.Read<PC_WorldFile>(rdManager.GetWorldFilePath(outputSettings), outputContext).ETAFileNames;

                    // Set event data
                    outLev.EventData.EventCount = (ushort)inputLev.EventData.Count;
                    outLev.EventData.EventLinkingTable = inputLev.EventData.Select(x => (ushort)x.LinkIndex).ToArray();
                    outLev.EventData.Events = inputLev.EventData.Select(x =>
                    {
                        var e = x.Data;

                        // Set DES and ETA indexes
                        e.PC_ImageDescriptorsIndex = (uint)desNames.FindItemIndex(y => y == x.DESKey);
                        e.PC_ImageBufferIndex = (uint)desNames.FindItemIndex(y => y == x.DESKey);
                        e.PC_AnimationDescriptorsIndex = (uint)desNames.FindItemIndex(y => y == x.DESKey);
                        e.PC_ETAIndex = (uint)etaNames.FindItemIndex(y => y == x.ETAKey);

                        // Set image and animation descriptor counts
                        e.ImageDescriptorCount = (ushort)outData.DES[x.DESKey].Sprites.Count;
                        e.AnimDescriptorCount = (byte)outData.DES[x.DESKey].Animations.Count;

                        return e;
                    }).ToArray();

                    // Set event commands
                    outLev.EventData.EventCommands = inputLev.EventData.Select(x =>
                    {
                        // Get the commands in the compiled format
                        var cmds = EventCommandCompiler.Compile(x.CommandCollection, x.CommandCollection.ToBytes(inputSettings));

                        // Remove commands which only contain the invalid command
                        if (cmds.Commands.Commands.Length == 1)
                            cmds = new EventCommandCompiler.CompiledEventCommandData(new Common_EventCommandCollection()
                            {
                                Commands = new Common_EventCommand[0]
                            }, new ushort[0]);

                        // Create a command object
                        return new PC_EventCommand
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
    }
}