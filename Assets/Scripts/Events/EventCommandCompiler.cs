using System;

namespace R1Engine
{
    /// <summary>
    /// Compiles and decompiles event commands
    /// </summary>
    public class EventCommandCompiler
    {
        /// <summary>
        /// Compiles the commands
        /// </summary>
        /// <param name="commands">The commands</param>
        /// <param name="commandBytes">The command bytes</param>
        /// <returns>The compiled the command data</returns>
        public CompiledEventCommandData Compile(Common_EventCommandCollection commands, byte[] commandBytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decompiles the commands
        /// </summary>
        /// <param name="commands">The commands</param>
        /// <param name="commandBytes">The command bytes</param>
        /// <returns>The decompiled commands</returns>
        public Common_EventCommandCollection Decompile(CompiledEventCommandData commands, byte[] commandBytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Data for compiled event commands
        /// </summary>
        public class CompiledEventCommandData
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="events">The event commands</param>
            /// <param name="labelOffsets">The label offsets</param>
            public CompiledEventCommandData(Common_EventCommandCollection events, ushort[] labelOffsets)
            {
                Events = events;
                LabelOffsets = labelOffsets;
            }

            /// <summary>
            /// The event commands
            /// </summary>
            public Common_EventCommandCollection Events { get; }

            /// <summary>
            /// The label offsets
            /// </summary>
            public ushort[] LabelOffsets { get; }
        }
    }
}