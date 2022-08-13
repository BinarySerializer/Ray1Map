using System.Collections.Generic;
using System.IO;
using BinarySerializer;
using BinarySerializer.PS2;

namespace Ray1Map.Psychonauts
{
    public class PS2_VIFCommands : BinarySerializable
    {
        public VIF_Command[] Commands { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var parser = new VIF_Parser() { IsVIF1 = true };
            Commands = s.SerializeObjectArrayUntil<VIF_Command>(Commands, _ => s.CurrentFileOffset >= s.CurrentLength, 
                onPreSerialize: (c,_) => c.Pre_Parser = parser, name: nameof(Commands));
        }

        public IEnumerable<PS2_GIF_Command> ParseCommands(Context context, string key, uint uvChannelsCount)
        {
            // Create a parser
            var parser = new VIF_Parser() { IsVIF1 = true, };

            int mcIndex = 0;

            PS2_GIF_Command ExecuteMicroProgram() {
                parser.HasPendingChanges = false;
                byte[] microProg = parser.GetCurrentBuffer();

                if (microProg != null) {
                    string mcKey = $"{key}_{mcIndex}";
                    mcIndex++;

                    try {
                        var file = new StreamFile(context, mcKey, new MemoryStream(microProg), endianness: Endian.Little);
                        context.AddFile(file);

                        uint tops = parser.TOPS * 16;

                        BinaryDeserializer s = context.Deserializer;
                        s.Goto(file.StartPointer + tops);

                        var gifCmd = s.SerializeObject<PS2_GIF_Command>(default,
                            onPreSerialize: c => c.Pre_UVChannelsCount = uvChannelsCount, name: "GIFCommand");

                        return gifCmd;
                    } finally {
                        context.RemoveFile(mcKey);
                    }
                }
                return null;
            }
            
            // Enumerate every command
            foreach (VIF_Command cmd in Commands)
            {
                if (parser.StartsNewMicroProgram(cmd))
                {
                    if (parser.HasPendingChanges) {
                        PS2_GIF_Command command = ExecuteMicroProgram();
                        if (command != null) yield return command;
                    }
                }

                parser.ExecuteCommand(cmd, executeFull: true);
            }
            if (parser.HasPendingChanges) {
                PS2_GIF_Command command = ExecuteMicroProgram();
                if (command != null) yield return command;
            }
        }
    }
}