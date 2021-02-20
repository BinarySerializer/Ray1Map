using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBAVV_Script : R1Serializable
    {
        public GBAVV_ScriptCommand[] Commands { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Commands == null)
            {
                var cmds = new List<GBAVV_ScriptCommand>();
                var index = 0;

                do
                {
                    cmds.Add(s.SerializeObject<GBAVV_ScriptCommand>(default, name: $"{nameof(Commands)}[{index++}]"));
                } while (cmds.Last().Type != GBAVV_ScriptCommand.CommandType.Terminator && index < 100); // TODO: Remove less than 100 check - used to avoid overflow

                Commands = cmds.ToArray();
            }
            else
            {
                Commands = s.SerializeObjectArray<GBAVV_ScriptCommand>(Commands, Commands.Length, name: nameof(Commands));
            }
        }
    }
}