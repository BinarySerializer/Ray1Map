namespace R1Engine
{
    // Since Mapper levels are raw non-compiled levels it doesn't use label offsets. Any command utilizing these is replaced by its counterpart which doesn't. Then when the level is compiled it is "optimized" by having the label offsets be calculated and used instead.

    /// <summary>
    /// The event commands
    /// </summary>
    public enum EventCommand : byte
    {
        GO_LEFT = 0x0,
        GO_RIGHT = 0x1,

        GO_WAIT = 0x2,

        GO_UP = 0x3,
        GO_DOWN = 0x4,

        /// <summary>
        /// Set SubEtat to {arg1}
        /// </summary>
        GO_SUBSTATE = 0x5,

        /// <summary>
        /// Skips {arg1} bytes
        /// </summary>
        GO_SKIP = 0x6,

        GO_ADD = 0x7,

        /// <summary>
        /// Set Etat to {arg1}
        /// </summary>
        GO_STATE = 0x8,
        
        // 1 arg
        GO_PREPARELOOP = 0x9,

        // 0 args
        GO_DOLOOP = 0xA,
        
        /// <summary>
        /// Sets label {arg1}
        /// </summary>
        GO_LABEL = 0xB,

        /// <summary>
        /// Skips to {arg1}
        /// </summary>
        GO_GOTO = 0xC,

        /// <summary>
        /// Saves the current position and starts executing from {arg1} until GO_RETURN
        /// </summary>
        GO_GOSUB = 0xD,

        /// <summary>
        /// Returns from a GOSUB command execution
        /// </summary>
        GO_RETURN = 0xE,

        /// <summary>
        /// Skips to {arg1} if true
        /// </summary>
        GO_BRANCHTRUE = 0xF,

        /// <summary>
        /// Skips to {arg1} if false
        /// </summary>
        GO_BRANCHFALSE = 0x10,
        
        // 1 arg and one more if {arg1} <= 4
        GO_TEST = 0x11,

        // 1 arg
        GO_SETTEST = 0x12,
        
        /// <summary>
        /// Waits {arg1} seconds
        /// </summary>
        GO_WAITSTATE = 0x13,
        
        /// <summary>
        /// Moves for {arg1} seconds with speed X {arg2} and Y {arg3}
        /// </summary>
        GO_SPEED = 0x14,
        
        /// <summary>
        /// Sets the X position to {arg1}{arg2}
        /// </summary>
        GO_X = 0x15,

        /// <summary>
        /// Sets the Y position to {arg1}{arg2}
        /// </summary>
        GO_Y = 0x16,

        /// <summary>
        /// Skips to LabelOffsets[{arg1}]
        /// </summary>
        RESERVED_GO_SKIP = 0x17,

        /// <summary>
        /// Same as RESERVED_GO_SKIP
        /// </summary>
        RESERVED_GO_GOTO = 0x18,

        /// <summary>
        /// Saves the current position and starts executing from LabelOffsets[{arg1}] until GO_RETURN
        /// </summary>
        RESERVED_GO_GOSUB = 0x19,

        /// <summary>
        /// Skips to LabelOffsets[{arg1}] if true
        /// </summary>
        RESERVED_GO_GOTOT = 0x1A,

        /// <summary>
        /// Skips to LabelOffsets[{arg1}] if false
        /// </summary>
        RESERVED_GO_GOTOF = 0x1B,

        /// <summary>
        /// Same as RESERVED_GO_GOTOT
        /// </summary>
        RESERVED_GO_SKIPT = 0x1C,

        /// <summary>
        /// Same as RESERVED_GO_GOTOF
        /// </summary>
        RESERVED_GO_SKIPF = 0x1D,

        GO_NOP = 0x1E,

        /// <summary>
        /// Skips {arg1} bytes if true
        /// </summary>
        GO_SKIPTRUE = 0x1F,

        /// <summary>
        /// Skips {arg1} bytes if false
        /// </summary>
        GO_SKIPFALSE = 0x20,

        /// <summary>
        /// Terminates the commands and loops back, {arg1} is always 0xFF
        /// </summary>
        INVALID_CMD = 0x21,

        // Used in the PS1 demos
        INVALID_CMD_DEMO = 0x42
    }
}