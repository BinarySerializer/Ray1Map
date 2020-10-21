using Boo.Lang;
using System;
using UnityEngine;

namespace R1Engine {
    public class GAX2_MusicTrack : R1Serializable {
        public ushort Duration { get; set; }
        public GAX2_MusicCommand[] Commands { get; set; }
        public Pointer EndOffset { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            if (Commands == null) {
                List<GAX2_MusicCommand> cmds = new List<GAX2_MusicCommand>();
                bool isEndOfTrack = false;
                int curDuration = 0;
                while (!isEndOfTrack) {
                    GAX2_MusicCommand cmd = s.SerializeObject<GAX2_MusicCommand>(default, name: $"{nameof(Commands)}[{cmds.Count}]");
                    cmds.Add(cmd);
                    curDuration += cmd.Duration;
                    if (cmd.Command == GAX2_MusicCommand.Cmd.EmptyTrack || curDuration >= Duration) {
                        isEndOfTrack = true;
                        EndOffset = s.CurrentPointer;
                        s.Log($"GAX2 Track Duration: {curDuration} - Last Command: {cmd.Command}");
                    }
                }
                Commands = cmds.ToArray();
            }
        }
    }
}