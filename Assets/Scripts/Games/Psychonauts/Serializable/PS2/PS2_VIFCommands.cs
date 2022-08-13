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

        public IEnumerable<PS2_GIF_Command> ParseCommands(Context context, string key, uint uvSetUVsCount)
        {
            // Create a parser
            var parser = new VIF_Parser() { IsVIF1 = true, };

            int mcIndex = 0;
            
            // Enumerate every command
            foreach (VIF_Command cmd in Commands)
            {
                if (parser.StartsNewMicroProgram(cmd))
                {
                    byte[] microProg = parser.GetCurrentBuffer();

                    if (microProg != null)
                    {
                        string mcKey = $"{key}_{mcIndex}";

                        try
                        {
                            var file = new StreamFile(context, mcKey, new MemoryStream(microProg), endianness: Endian.Little);
                            context.AddFile(file);

                            uint tops = parser.TOPS * 16;

                            BinaryDeserializer s = context.Deserializer;
                            s.Goto(file.StartPointer + tops);

                            var gifCmd = s.SerializeObject<PS2_GIF_Command>(default,
                                onPreSerialize: c => c.Pre_UVSetUVsCount = uvSetUVsCount, name: "GIFCommand");
                            
                            yield return gifCmd;
                        }
                        finally
                        {
                            context.RemoveFile(mcKey);
                        }

                        mcIndex++;
                    }
                }

                parser.ExecuteCommand(cmd, executeFull: true);
            }
        }
    }
}