using BinarySerializer;
using System.Collections.Generic;

namespace R1Engine
{
    public class PS1Klonoa_IDXLoadCommand : BinarySerializable
    {
        private const uint SectorSize = 2048;

        public int Type { get; set; }

        // Type 1
        public uint BIN_LBA { get; set; } // The LBA offset relative to the LBA of the BIN
        public uint BIN_Offset => BIN_LBA * SectorSize;
        public uint BIN_UnknownPointerValue { get; set; }
        public uint BIN_LengthValue { get; set; }
        public uint BIN_Length => BIN_LengthValue * SectorSize;

        // Type 2
        public uint FILE_Length { get; set; }
        public uint FILE_UnknownValue { get; set; }
        public uint FILE_FunctionPointer { get; set; }
        public FileType FILE_Type { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Type = s.Serialize<int>(Type, name: nameof(Type));

            if (Type == 1)
            {
                BIN_LBA = s.Serialize<uint>(BIN_LBA, name: nameof(BIN_LBA));
                s.Log($"{nameof(BIN_Offset)}: {BIN_Offset}");

                BIN_UnknownPointerValue = s.Serialize<uint>(BIN_UnknownPointerValue, name: nameof(BIN_UnknownPointerValue));
                s.Log($"BIN_UnknownPointer: 0x{BIN_UnknownPointerValue:X8}");

                BIN_LengthValue = s.Serialize<uint>(BIN_LengthValue, name: nameof(BIN_LengthValue));
                s.Log($"{nameof(BIN_Length)}: {BIN_Length}");
            }
            else
            {
                FILE_Length = s.Serialize<uint>(FILE_Length, name: nameof(FILE_Length));

                FILE_UnknownValue = s.Serialize<uint>(FILE_UnknownValue, name: nameof(FILE_UnknownValue));
                s.Log($"{nameof(FILE_UnknownValue)}: 0x{FILE_UnknownValue:X8}");

                FILE_FunctionPointer = s.Serialize<uint>(FILE_FunctionPointer, name: nameof(FILE_FunctionPointer));
                s.Log($"{nameof(FILE_FunctionPointer)}: 0x{FILE_FunctionPointer:X8}");

                if (FileTypes.ContainsKey(FILE_FunctionPointer))
                {
                    FILE_Type = FileTypes[FILE_FunctionPointer];
                    s.Log($"{nameof(FILE_Type)}: {FILE_Type}");
                }
            }
        }

        // The game parses the files using the supplied function pointer, so we can use that to determine the file type
        public static Dictionary<uint, FileType> FileTypes { get; } = new Dictionary<uint, FileType>()
        {
            [0x80016CF0] = FileType.Archive_TIM,
            [0x80111E80] = FileType.Archive_TIM,
            [0x8012311C] = FileType.Archive_TIM,
            [0x8001F638] = FileType.Archive_TIM,
            [0x80016F68] = FileType.Archive_TIM,

            [0x80034A88] = FileType.OA05,
            [0x80036ECC] = FileType.SEQ,
            [0x80034EB0] = FileType.SEQ,

            [0x8002304C] = FileType.Archive_BackgroundPack,

            [0x800264d8] = FileType.Archive_Unk0,
            [0x80073930] = FileType.FixedSprites,
            [0x800737F4] = FileType.Archive_SpritePack,
            [0x8001845C] = FileType.Archive_LevelPack,

            [0x8007825C] = FileType.Code,
            [0x80078274] = FileType.Code,
            [0x00000000] = FileType.Code, // Hopefully all of these are code and not that some are data referenced in memory

            [0x80122B08] = FileType.Archive_Unk4,
        };

        // These are just for clearer exports and don't match actual file extensions
        public static Dictionary<FileType, string> FileExtensions { get; } = new Dictionary<FileType, string>()
        {
            [FileType.Unknown] = ".BIN",
            [FileType.Archive_TIM] = ".TIM",
            [FileType.OA05] = ".OA05",
            [FileType.SEQ] = ".SEQ",
            [FileType.Archive_BackgroundPack] = ".BGPACK",
            [FileType.Archive_Unk0] = ".UNK0",
            [FileType.FixedSprites] = ".FIXSPRITES",
            [FileType.Archive_SpritePack] = ".SPRITEPACK",
            [FileType.Archive_LevelPack] = ".LEV",
            [FileType.Code] = ".CODE",
            [FileType.Archive_Unk4] = ".UNK4",
        };

        public static Dictionary<FileType, int> ArchiveDepths { get; } = new Dictionary<FileType, int>()
        {
            [FileType.Unknown] = 0,
            [FileType.Archive_TIM] = 1,
            [FileType.OA05] = 0,
            [FileType.SEQ] = 0,
            [FileType.Archive_BackgroundPack] = 2,
            [FileType.Archive_Unk0] = 1,
            [FileType.FixedSprites] = 1,
            [FileType.Archive_SpritePack] = 1,
            [FileType.Archive_LevelPack] = 1,
            [FileType.Code] = 0,
            [FileType.Archive_Unk4] = 2,
        };

        public enum FileType
        {
            Unknown,

            Archive_TIM, // Textures
            OA05, // Sounds
            SEQ, // Sound
            Archive_BackgroundPack, // Backgrounds
            Archive_Unk0,
            FixedSprites, // Fixed sprite descriptors
            Archive_SpritePack, // Sprites
            Archive_LevelPack, // Level data
            Code, // Compiled code
            Archive_Unk4,
        }
    }
}