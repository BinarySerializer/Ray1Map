using System.Collections.Generic;

namespace R1Engine
{
    public class GBAVV_KidsNextDoorOperationSODA_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 46;

        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0804067C,
            0x08051860,
            0x08054B74,
            0x0805B098,
            0x08064ED4,
            0x0808BB6C,
        };

        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [0007] = GBAVV_ScriptCommand.CommandType.Name,
            [0012] = GBAVV_ScriptCommand.CommandType.Return,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08032CD4, // script_license
            0x08032D34, // script_intro
            0x080364C8, // script_waitForInputOrTime
            0x080367F4, // titleFX0
            0x08036910, // titleFX1
            0x0803696C, // titleFX2
            0x080369C8, // titleFX3
            0x08036A24, // titleFX4
            0x08036AA0, // titleFX5
            0x08036B3C, // titleFX6
            0x08038A8C, // script_startLevel
            0x0803E898, // movie_title02
            0x0803E908, // movie_credits
            0x0803E980, // movie_creditsGameEnd
            0x0803EE9C, // SCRIPT_pagedTextLoop
            0x087E6274, // script_cutscene
            0x087E62B4, // script_noCutscene
            0x087E62F8, // script_cutscene
        };
    }
}