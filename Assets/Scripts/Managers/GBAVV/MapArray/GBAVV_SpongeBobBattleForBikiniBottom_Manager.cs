using System.Collections.Generic;

namespace R1Engine
{
    public abstract class GBAVV_SpongeBobBattleForBikiniBottom_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 112;

        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [0007] = GBAVV_ScriptCommand.CommandType.Name,
            [0012] = GBAVV_ScriptCommand.CommandType.Return,
        };
    }
    public class GBAVV_SpongeBobBattleForBikiniBottomEU_Manager : GBAVV_SpongeBobBattleForBikiniBottom_Manager
    {
        public override string[] Languages => new string[]
        {
            "English",
            "French",
            "German",
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x08053C3C,
            0x08061CBC,
            0x08064130,
            0x08065C54,
            0x0806D9E4,
            0x080726B8,
            0x08075084,
            0x0807BCF0,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x080311EC, // script_license
            0x0803122C, // script_intro
            0x0803126C, // script_credits
            0x080312B0, // script_creditsEnd
            0x08039DC8, // script_waitForInputOrTime
            0x0803C82C, // script_startLevel
            0x08051A84, // movie_credits
            0x08052190, // SCRIPT_pagedTextLoop
            0x080526E4, // script_danger1
            0x08052740, // script_danger2
            0x087F2CB8, // script_cutscene
            0x087F2CE8, // script_noCutscene
        };
    }
    public class GBAVV_SpongeBobBattleForBikiniBottomUS_Manager : GBAVV_SpongeBobBattleForBikiniBottom_Manager
    {
        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0804DFAC,
            0x0805C02C,
            0x0805E4A0,
            0x0805FFC4,
            0x08067D54,
            0x0806CA28,
            0x0806F3F4,
            0x08076060,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08030FFC, // script_license
            0x08031048, // script_intro
            0x08031088, // script_credits
            0x080310CC, // script_creditsEnd
            0x08039BF0, // script_waitForInputOrTime
            0x0803C640, // script_startLevel
            0x0804BD70, // movie_title
            0x0804BE08, // movie_credits
            0x0804C500, // SCRIPT_pagedTextLoop
            0x0804CA54, // script_danger1
            0x0804CAB0, // script_danger2
            0x087F7F1C, // script_cutscene
            0x087F7F4C, // script_noCutscene
        };
    }
}