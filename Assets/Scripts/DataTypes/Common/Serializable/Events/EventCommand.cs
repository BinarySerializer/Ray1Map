namespace R1Engine
{
    // Since Mapper levels are raw non-compiled levels it doesn't use label offsets. Any command utilizing these is replaced by its counterpart which doesn't. Then when the level is compiled it is "optimized" by having the label offsets be calculated and used instead.

    /// <summary>
    /// The event commands
    /// </summary>
    public enum EventCommand : byte
    {
        GO_LEFT = 0,
        GO_RIGHT = 1,

        GO_WAIT = 2,

        GO_UP = 3,
        GO_DOWN = 4,

        /// <summary>
        /// Set SubEtat to {arg1}
        /// </summary>
        GO_SUBSTATE = 5,

        /// <summary>
        /// Skips {arg1} bytes
        /// </summary>
        GO_SKIP = 6,

        GO_ADD = 7,

        /// <summary>
        /// Set Etat to {arg1}
        /// </summary>
        GO_STATE = 8,
        
        // 1 arg
        GO_PREPARELOOP = 9,

        // 0 args
        GO_DOLOOP = 10,
        
        /// <summary>
        /// Sets label {arg1}
        /// </summary>
        GO_LABEL = 11,

        /// <summary>
        /// Skips to {arg1}
        /// </summary>
        GO_GOTO = 12,

        /// <summary>
        /// Saves the current position and starts executing from {arg1} until GO_RETURN
        /// </summary>
        GO_GOSUB = 13,

        /// <summary>
        /// Returns from a GOSUB command execution
        /// </summary>
        GO_RETURN = 14,

        /// <summary>
        /// Skips to {arg1} if true
        /// </summary>
        GO_BRANCHTRUE = 15,

        /// <summary>
        /// Skips to {arg1} if false
        /// </summary>
        GO_BRANCHFALSE = 16,
        
        // 1 arg and one more if {arg1} <= 4
        GO_TEST = 17,

        // 1 arg
        GO_SETTEST = 18,
        
        /// <summary>
        /// Waits {arg1} seconds
        /// </summary>
        GO_WAITSTATE = 19,
        
        /// <summary>
        /// Moves for {arg1} seconds with speed X {arg2} and Y {arg3}
        /// </summary>
        GO_SPEED = 20,
        
        /// <summary>
        /// Sets the X position to {arg1}{arg2}
        /// </summary>
        GO_X = 21,

        /// <summary>
        /// Sets the Y position to {arg1}{arg2}
        /// </summary>
        GO_Y = 22,

        /// <summary>
        /// Skips to LabelOffsets[{arg1}]
        /// </summary>
        RESERVED_GO_SKIP = 23,

        /// <summary>
        /// Same as RESERVED_GO_SKIP
        /// </summary>
        RESERVED_GO_GOTO = 24,

        /// <summary>
        /// Saves the current position and starts executing from LabelOffsets[{arg1}] until GO_RETURN
        /// </summary>
        RESERVED_GO_GOSUB = 25,

        /// <summary>
        /// Skips to LabelOffsets[{arg1}] if true
        /// </summary>
        RESERVED_GO_GOTOT = 26,

        /// <summary>
        /// Skips to LabelOffsets[{arg1}] if false
        /// </summary>
        RESERVED_GO_GOTOF = 27,

        /// <summary>
        /// Same as RESERVED_GO_GOTOT
        /// </summary>
        RESERVED_GO_SKIPT = 28,

        /// <summary>
        /// Same as RESERVED_GO_GOTOF
        /// </summary>
        RESERVED_GO_SKIPF = 29,

        GO_NOP = 30,

        /// <summary>
        /// Skips {arg1} bytes if true
        /// </summary>
        GO_SKIPTRUE = 31,

        /// <summary>
        /// Skips {arg1} bytes if false
        /// </summary>
        GO_SKIPFALSE = 32,

        /// <summary>
        /// Terminates the commands and loops back, {arg1} is always 0xFF
        /// </summary>
        INVALID_CMD = 33
    }
}