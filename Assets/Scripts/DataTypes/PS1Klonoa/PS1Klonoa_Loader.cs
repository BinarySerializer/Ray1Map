using System;
using BinarySerializer.PS1;

namespace BinarySerializer.KlonoaDTP
{
    /// <summary>
    /// Handles loading data from the game BIN
    /// </summary>
    public class PS1Klonoa_Loader
    {
        #region Constructor

        protected PS1Klonoa_Loader(Context context)
        {
            Context = context;
            VRAM = new PS1_VRAM();
            SpriteFrames = new PS1Klonoa_ArchiveFile_Sprites[70];
        }

        #endregion

        #region Constants

        private const string Key = "KLONOA_LOADER";

        public const string FilePath_BIN = "FILE.BIN";
        public const string FilePath_IDX = "FILE.IDX";

        #endregion

        #region Public Properties

        // The currently loaded data
        public PS1_VRAM VRAM { get; }
        public PS1Klonoa_ArchiveFile_Sprites[] SpriteFrames { get; }
        public PS1Klonoa_ArchiveFile_BackgroundPack BackgroundPack { get; set; }
        public PS1Klonoa_File_OA05 OA05 { get; set; }
        public PS1Klonoa_ArchiveFile_LevelPack LevelPack { get; set; }

        // Properties
        public Context Context { get; }
        public int BINBlock { get; set; }
        public bool IsLevel => BINBlock >= 3;
        public bool IsBossFight => IsLevel && BINBlock % 3 == 2;

        #endregion

        #region Public Methods

        public void Load_BIN(PS1Klonoa_IDXEntry entry, int blockIndex)
        {
            // Null out previous data (this does not get kept between levels)
            BackgroundPack = null;
            OA05 = null;
            LevelPack = null;

            ProcessBINFiles(entry, (cmd, i) =>
            {
                // Load the file
                var binFile = Load_BINFile(cmd, blockIndex, i);

                switch (binFile)
                {
                    // Copy the TIM files data to VRAM
                    case PS1Klonoa_ArchiveFile_TIM file:

                        foreach (PS1_TIM tim in file.Files)
                            AddToVRAM(tim);

                        break;

                    // Save for later
                    case PS1Klonoa_File_OA05 file:
                        OA05 = file;
                        break;

                    // Save for later and copy the TIM files data to VRAM
                    case PS1Klonoa_ArchiveFile_BackgroundPack file:

                        BackgroundPack = file;

                        foreach (PS1_TIM tim in file.TIMFiles.Files)
                            AddToVRAM(tim);

                        break;

                    case PS1Klonoa_ArchiveFile_Unk0 file:
                        // TODO: Use file
                        break;

                    // The fixed sprites are always the last set of sprite frames
                    case PS1Klonoa_ArchiveFile_Sprites file:
                        SpriteFrames[69] = file;
                        break;

                    // Add the level sprite frames
                    case PS1Klonoa_ArchiveFile_LevelSpritePack file:

                        for (int j = 0; j < 69; j++)
                            SpriteFrames[j] = file.Sprites[j];

                        break;

                    // Save for later
                    case PS1Klonoa_ArchiveFile_LevelPack file:
                        LevelPack = file;
                        break;
                }
            });
        }

        public PS1Klonoa_BaseFile Load_BINFile(PS1Klonoa_IDXLoadCommand cmd, int blockIndex, int cmdIndex)
        {
            switch (cmd.FILE_Type)
            {
                case PS1Klonoa_IDXLoadCommand.FileType.Archive_TIM:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_TIM>(cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.OA05:
                    return Load_BINFile<PS1Klonoa_File_OA05>(cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.SEQ:
                    // TODO: Parse SEQ
                    return null;

                case PS1Klonoa_IDXLoadCommand.FileType.Code:
                    // Ignore compiled code
                    return null;

                case PS1Klonoa_IDXLoadCommand.FileType.Archive_BackgroundPack:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_BackgroundPack>(cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.Archive_Unk0:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_Unk0>(cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.FixedSprites:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_Sprites>(cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.Archive_SpritePack:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_LevelSpritePack>(cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.Archive_LevelPack:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_LevelPack>(cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.Archive_Unk4:
                    return Load_BINFile<PS1Klonoa_ArchiveFile_RawData>(cmd, blockIndex, cmdIndex);

                case PS1Klonoa_IDXLoadCommand.FileType.Unknown:
                default:
                    Context.Logger.LogWarning($"Unsupported file format for file at command {cmdIndex} parsed at 0x{cmd.FILE_FunctionPointer:X8}");
                    return null;
            }
        }

        public T Load_BINFile<T>(PS1Klonoa_IDXLoadCommand cmd, int blockIndex, int cmdIndex)
            where T : PS1Klonoa_BaseFile, new()
        {
            var s = Context.Deserializer;

            return s.SerializeObject<T>(null, x =>
            {
                x.Pre_FileSize = cmd.FILE_Length;
                x.Pre_IsCompressed = false;
            }, name: $"BIN_File_{blockIndex}_{cmdIndex}");
        }

        public void ProcessBINFiles(PS1Klonoa_IDXEntry entry, Action<PS1Klonoa_IDXLoadCommand, int> action)
        {
            var s = Context.Deserializer;
            var binFile = Context.GetFile(FilePath_BIN);

            // Enumerate every load command
            for (int cmdIndex = 0; cmdIndex < entry.LoadCommands.Length; cmdIndex++)
            {
                var cmd = entry.LoadCommands[cmdIndex];

                // Seek
                if (cmd.Type == 1)
                {
                    s.Goto(binFile.StartPointer + cmd.BIN_Offset);
                }
                // Read file
                else if (cmd.Type == 2)
                {
                    var p = s.CurrentPointer;

                    // Add a region for nicer pointer logging
                    binFile.AddRegion(p.FileOffset, cmd.FILE_Length, $"File_{cmdIndex}");

                    // Process the file
                    action(cmd, cmdIndex);

                    // Go to the end of the file for the next file
                    s.Goto(p + cmd.FILE_Length);
                }
            }
        }

        public void AddToVRAM(PS1_TIM tim)
        {
            // Add the palette if available
            if (tim.Clut != null)
                VRAM.AddPalette(tim.Clut.Palette, 0, 0, tim.Clut.XPos * 2, tim.Clut.YPos, tim.Clut.Width * 2, tim.Clut.Height);

            // Add the image data
            if (!(tim.XPos == 0 && tim.YPos == 0) && tim.Width != 0 && tim.Height != 0)
                VRAM.AddDataAt(0, 0, tim.XPos * 2, tim.YPos, tim.ImgData, tim.Width * 2, tim.Height);
        }

        #endregion

        #region Public Static Methods

        public static PS1Klonoa_Loader GetLoader(Context context) => context.GetStoredObject<PS1Klonoa_Loader>(Key);
        public static PS1Klonoa_Loader Create(Context context)
        {
            // Create the loader
            var loader = new PS1Klonoa_Loader(context);

            // Store in the context so it can be accessed
            context.StoreObject(Key, loader);

            return loader;
        }

        #endregion
    }
}