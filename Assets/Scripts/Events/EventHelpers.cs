using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// Helper methods for events
    /// </summary>
    public static class EventHelpers
    {
        // TODO: Remove?
        /// <summary>
        /// Parses a command
        /// </summary>
        /// <param name="cmds">The command bytes</param>
        /// <param name="labelOffsets">The label offsets</param>
        /// <returns>The parsed command</returns>
        public static string[] ParseCommands(byte[] cmds, ushort[] labelOffsets)
        {
            // Create the output
            var output = new List<string>();

            // Handle every command byte
            for (int i = 0; i < cmds.Length;)
            {
                var command = cmds[i++];
                byte ReadArg() => cmds[i++];
                sbyte ReadSArg() => (sbyte)cmds[i++];

                // Handle the commands
                switch (command)
                {
                    case 0:
                        output.Add($"Go left {ReadSArg()}");
                        break;
                    
                    case 1:
                        output.Add($"Go right {ReadSArg()}");
                        break;
                    
                    case 2:
                        output.Add($"Go wait {ReadSArg()}");
                        break;
                    
                    case 3:
                        output.Add($"Go up {ReadSArg()}");
                        break;

                    case 4:
                        output.Add($"Go down {ReadSArg()}");
                        break;

                    case 5:
                        output.Add($"Set SubEtat to {ReadArg()}");
                        break;

                    case 6:
                        output.Add($"Skip {ReadArg()} commands");
                        break;

                    case 7:
                        output.Add($"Add {ReadArg()}");
                        break;

                    case 8:
                        output.Add($"Set Etat to {ReadArg()}");
                        break;

                    case 9:
                        output.Add($"Prepare loop {ReadArg()}");
                        break;

                    case 10:
                        output.Add($"Do loop");
                        break;

                    case 11:
                        output.Add($"Set label {ReadArg()}");
                        break;

                    case 12:
                        output.Add($"Go to label {ReadArg()}");
                        break;

                    case 13:
                        output.Add($"Enter sub-function with label {ReadArg()}");
                        break;

                    case 14:
                        output.Add($"Exit sub-function");
                        break;

                    case 15:
                        output.Add($"Go to label {ReadArg()} if flag is true");
                        break;

                    case 16:
                        output.Add($"Go to label {ReadArg()} if flag is false");
                        break;

                    case 17:
                        var arg1 = ReadArg();

                        output.Add(arg1 <= 4 ? $"Test {arg1} {ReadArg()}" : $"Test {arg1}");

                        break;

                    case 18:
                        output.Add($"Set test {ReadArg()}");
                        break;

                    case 19:
                        output.Add($"Wait {ReadArg()} seconds");
                        break;

                    case 20:
                        output.Add($"Move for {ReadArg()} seconds, speedX {ReadSArg()} and speedY {ReadSArg()}");
                        break;

                    case 21:
                        output.Add($"Set X to {ReadArg().ToString() + ReadArg().ToString()}");
                        break;

                    case 22:
                        output.Add($"Set Y to {ReadArg().ToString() + ReadArg().ToString()}");
                        break;

                    case 23:
                        output.Add($"Skip to offset {labelOffsets[ReadArg()]}");
                        break;

                    case 24:
                        output.Add($"Go to offset {labelOffsets[ReadArg()]}");
                        break;

                    case 25:
                        output.Add($"Enter sub-function with label {labelOffsets[ReadArg()]}");
                        break;

                    case 26:
                        output.Add($"Go to offset {labelOffsets[ReadArg()]} if flag is true");
                        break;

                    case 27:
                        output.Add($"Go to offset {labelOffsets[ReadArg()]} if flag is false");
                        break;

                    case 28:
                        output.Add($"Skip to offset {labelOffsets[ReadArg()]} if flag is true");
                        break;

                    case 29:
                        output.Add($"Skip to offset {labelOffsets[ReadArg()]} if flag is false");
                        break;

                    // GO_NOP perhaps?
                    case 30:
                        output.Add($"Self handled {command}: {ReadSArg()}");
                        break;

                    case 31:
                        output.Add($"Skip {ReadArg()} commands if flag is true");
                        break;

                    case 32:
                        output.Add($"Skip {ReadArg()} commands if flag is false");
                        break;

                    case 33:
                        output.Add($"End ({ReadArg()})");
                        break;
                }
            }

            // Return the output
            return output.ToArray();
        }
    }
}