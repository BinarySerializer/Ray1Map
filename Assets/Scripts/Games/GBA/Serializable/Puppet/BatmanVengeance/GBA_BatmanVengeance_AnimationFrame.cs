using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBA
{
    public class GBA_BatmanVengeance_AnimationFrame : BinarySerializable {
        // Set in onPreSerialize
        public GBA_BatmanVengeance_Puppet Puppet { get; set; }

        public GBA_BatmanVengeance_AnimationCommand[] Commands { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            // Serialize commands
            if (Commands == null) {
                var commands = new List<GBA_BatmanVengeance_AnimationCommand>();

                var p = s.CurrentPointer;
                var index = 0;

                try {
                    GBA_BatmanVengeance_AnimationCommand lastCommand = null;
                    while ((lastCommand == null
                        || !lastCommand.IsTerminator)
                        && s.CurrentPointer.AbsoluteOffset < Puppet.BlockEndPointer.AbsoluteOffset) {
                        lastCommand = s.SerializeObject<GBA_BatmanVengeance_AnimationCommand>(default, onPreSerialize: c => c.Puppet = Puppet, name: $"{nameof(Commands)}[{index}]");
                        commands.Add(lastCommand);
                        index++;
                    }
                    if (s.CurrentPointer.AbsoluteOffset > Puppet.BlockEndPointer.AbsoluteOffset) {
                        throw new Exception($"Command overflow at {Offset}!");
                    }
                    if (!commands.Last().IsTerminator)
                        throw new Exception($"Frame did not end with terminator command!");
                } catch (Exception ex) {
                    s.Log($"Error parsing command: {ex.Message}");
                    Debug.LogWarning($"Error parsing command at {Offset}: {ex.Message}");
                }

                Commands = commands.ToArray();
            } else {
                Commands = s.SerializeObjectArray<GBA_BatmanVengeance_AnimationCommand>(Commands, Commands.Length, name: nameof(Commands));
            }
        }
    }
}