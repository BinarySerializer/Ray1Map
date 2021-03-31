using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_RLERow : BinarySerializable
    {
        public Pointer BaseOffset { get; set; } // Set before serializing
        public int Width { get; set; } // Set before serializing

        public ushort CommandsOffset { get; set; } // 1 == Empty, 2 == Repeat, otherwise offset
        public ushort ImageDataOffset { get; set; }

        // Serialized from offsets
        public GBAVV_NitroKart_NGage_RLECommand[] Commands { get; set; }
        public byte[] ImgageData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            CommandsOffset = s.Serialize<ushort>(CommandsOffset, name: nameof(CommandsOffset));
            ImageDataOffset = s.Serialize<ushort>(ImageDataOffset, name: nameof(ImageDataOffset));

            if (CommandsOffset > 2)
            {
                var count = 0;
                Commands = s.DoAt(BaseOffset + CommandsOffset, () => s.SerializeObjectArrayUntil<GBAVV_NitroKart_NGage_RLECommand>(Commands, x => (count += x.Count) >= Width, includeLastObj: true, name: nameof(Commands)));

                ImgageData = s.DoAt(BaseOffset + ImageDataOffset, () => s.SerializeArray<byte>(ImgageData, Commands.Sum(x =>
                {
                    if (x.Type == 0)
                        return x.Count;
                    else if (x.Type == 2)
                        return 1;
                    else
                        return 0;
                }), name: nameof(ImgageData)));
            }
            else if (CommandsOffset == 2)
            {
                ImgageData = s.DoAt(BaseOffset + ImageDataOffset, () => s.SerializeArray<byte>(ImgageData, 1, name: nameof(ImgageData)));
            }
        }
    }
}