using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_TileMapSection : BinarySerializable
    {
        public ushort Length { get; set; } // Set before serializing

        public GBAVV_TileMapCommand[] Commands { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Commands == null)
            {
                var cmds = new List<GBAVV_TileMapCommand>();
                var count = 0;
                var index = 0;

                while (count < Length)
                {
                    var cmd = s.SerializeObject<GBAVV_TileMapCommand>(default, name: $"{nameof(Commands)}[{index++}]");

                    cmds.Add(cmd);

                    count += cmd.Type == 2 || cmd.Type == 3 ? cmd.Length : 1;
                }

                Commands = cmds.ToArray();
            }
            else
            {
                s.SerializeObjectArray<GBAVV_TileMapCommand>(Commands, Commands.Length, name: nameof(Commands));
            }
        }
    }
}