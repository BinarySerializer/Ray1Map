using Cysharp.Threading.Tasks;
using R1Engine.Jade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Jade_PetzHorseClub_Wii_Manager : Jade_Montpellier_BaseManager {
		// Levels
		public override LevelInfo[] LevelInfos => new LevelInfo[] {
			new LevelInfo(0x00000B64, "ROOT/EngineDatas/06 Levels/GAME_MAPS/_GUI/GUI_LANGUAGE", "GUI_LANGUAGE.wol"),
			new LevelInfo(0x0000089A, "ROOT/EngineDatas/06 Levels/GAME_MAPS/_GUI/GUI_MAIN", "GUI_MAIN.wol"),
			new LevelInfo(0x0000755E, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_501_DV_PLATEAU", "ADV_501_DV_PLATEAU.wol"),
			new LevelInfo(0x0000755F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_502_DV_PLAIN", "ADV_502_DV_PLAIN.wol"),
			new LevelInfo(0x00007560, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_503_DV_MOUNTAIN", "ADV_503_DV_MOUNTAIN.wol"),
			new LevelInfo(0x00007561, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_504_DV_SEQUOIA", "ADV_504_DV_SEQUOIA.wol"),
			new LevelInfo(0x00007562, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_505_DV_FEUILLUS", "ADV_505_DV_FEUILLUS.wol"),
			new LevelInfo(0x00007563, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_600_RANCH", "ADV_600_RANCH.wol"),
			new LevelInfo(0x0000A7B9, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_600_RANCH", "ADV_600_RANCH_RTC.wol"),
			new LevelInfo(0x00000197, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_700_STADIUM", "ADV_700_STADIUM.wol"),
			new LevelInfo(0x00001FD7, "ROOT/EngineDatas/06 Levels/GAME_MAPS/CREDITS", "CREDITS.wol"),
			new LevelInfo(0x0000A00D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS_COMPETITION", "CROSS_COMPETITION.wol"),
			new LevelInfo(0x0000A00F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP_COMPETITION", "JUMP_COMPETITION.wol"),
			new LevelInfo(0x0000A012, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/SPEED_SELECTION", "SPEED_SELECTION.wol"),
			new LevelInfo(0x0000180E, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_630_CARE", "MINI_630_CARE.wol"),
			new LevelInfo(0x00001BAE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_BONUS_MODE_MANEGE", "MINI_BONUS_MODE_MANEGE.wol"),
			new LevelInfo(0x00001962, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_BONUS_MODE_STADIUM", "MINI_BONUS_MODE_STADIUM.wol"),
		};

		public override string[] BFFiles => new string[] {
			"horsez_clean.bf"
		};
	}
}
