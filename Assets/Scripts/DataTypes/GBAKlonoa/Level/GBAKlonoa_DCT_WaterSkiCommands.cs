using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_DCT_WaterSkiCommands : BinarySerializable
    {
        public GBAKlonoa_DCT_WaterSkiCommand[] Commands { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Commands = s.SerializeObjectArrayUntil(Commands, x => x.CmdType == GBAKlonoa_DCT_WaterSkiCommand.WaterSkiCommandType.EndOfLevel, name: nameof(Commands));
        }
    }
}