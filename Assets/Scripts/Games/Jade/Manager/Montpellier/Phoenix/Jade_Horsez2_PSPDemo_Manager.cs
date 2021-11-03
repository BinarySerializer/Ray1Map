namespace Ray1Map
{
    public class Jade_Horsez2_PSPDemo_Manager : Jade_Montpellier_BaseManager {
        // Levels
        public override LevelInfo[] LevelInfos => new LevelInfo[] {
            new LevelInfo(0x00001909, "ROOT/EngineDatas/06 Levels/_BASE/_BASE_GFX", "_BASE_GFX.wol"),
            new LevelInfo(0x00004B3B, "ROOT/EngineDatas/06 Levels/_BASE/_BASE_GFX_MINIGAME", "_BASE_GFX_MINIGAME.wol"),
            new LevelInfo(0x00000EB3, "ROOT/EngineDatas/06 Levels/_BASE/_BASE_OUTSIDE", "_BASE_OUTSIDE.wol"),
            new LevelInfo(0x00003458, "ROOT/EngineDatas/06 Levels/_PREFABS_/_ANIMALS", "_ANIMALS.wol"),
            new LevelInfo(0x00004C3E, "ROOT/EngineDatas/06 Levels/_PREFABS_/_DOORS", "_DOORS.wol"),
            new LevelInfo(0x00001EAA, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_100", "ADV_100.wol"),
            new LevelInfo(0x00001EAF, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_600", "ADV_600.wol"),
            new LevelInfo(0x00001F96, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_604", "ADV_604.wol"),
            new LevelInfo(0x000040FD, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_610", "ADV_610.wol"),
            new LevelInfo(0x00001FD7, "ROOT/EngineDatas/06 Levels/GAME_MAPS/CREDITS", "CREDITS.wol"),
            new LevelInfo(0x00000DE3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_100_PLAIN", "DEC_100_PLAIN.wol"),
            new LevelInfo(0x000007D0, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_600_RANCH", "DEC_600_RANCH.wol"),
            new LevelInfo(0x00003479, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_604_STABLE", "DEC_604_STABLE.wol"),
            new LevelInfo(0x00003FB4, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_610_RANCH_MANEGE", "DEC_610_RANCH_MANEGE.wol"),
            new LevelInfo(0x000040A0, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_100", "GFX_100.wol"),
            new LevelInfo(0x00003FFE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_600", "GFX_600.wol"),
            new LevelInfo(0x00004127, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_604", "GFX_604.wol"),
            new LevelInfo(0x000046A3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_606", "GFX_606.wol"),
            new LevelInfo(0x00004D05, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_611", "GFX_611.wol"),
            new LevelInfo(0x00004D06, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_617", "GFX_617.wol"),
            new LevelInfo(0x00000E71, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_BOX", "GUI_BOX.wol"),
            new LevelInfo(0x00004731, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_CARES", "GUI_CARES.wol"),
            new LevelInfo(0x000044EB, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_COVERING", "GUI_COVERING.wol"),
            new LevelInfo(0x0000086F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_HERO", "GUI_HUD_HERO.wol"),
            new LevelInfo(0x00008AEF, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_HORSE", "GUI_HUD_HORSE.wol"),
            new LevelInfo(0x00002A25, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_BoxWash", "GUI_HUD_MG_BoxWash.wol"),
            new LevelInfo(0x00002A26, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_Brush", "GUI_HUD_MG_Brush.wol"),
            new LevelInfo(0x000026B7, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_Friendship", "GUI_HUD_MG_Friendship.wol"),
            new LevelInfo(0x000029F9, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_HorseWash", "GUI_HUD_MG_HorseWash.wol"),
            new LevelInfo(0x00002A28, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_Picking", "GUI_HUD_MG_Picking.wol"),
            new LevelInfo(0x00000B64, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_LANGUAGE", "GUI_LANGUAGE.wol"),
            new LevelInfo(0x0000089A, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_MAIN", "GUI_MAIN.wol"),
            new LevelInfo(0x00000A42, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_MESSAGE_BOX", "GUI_MESSAGE_BOX.wol"),
            new LevelInfo(0x00000B69, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_NAMING", "GUI_NAMING.wol"),
            new LevelInfo(0x00000B7F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_PAUSE", "GUI_PAUSE.wol"),
            new LevelInfo(0x00000191, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_601_FRIENDSHIP", "MINI_601.wol"),
            new LevelInfo(0x0000DF04, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_601_FRIENDSHIP", "MINI_601_FRIENDSHIP.wol"),
            new LevelInfo(0x00000187, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_606_PICKING", "MINI_606_PICKING.wol"),
            new LevelInfo(0x000073AE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_611_HORSE_BRUSH", "MINI_611_HORSE_BRUSH.wol"),
            new LevelInfo(0x00002D6F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_617_WASH", "MINI_617_WASH.wol"),
            new LevelInfo(0x0000343B, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_627_BOX_WASH", "MINI_627_BOX_WASH.wol"),
            new LevelInfo(0x000095DE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_600_01", "RTC_600_01.wol"),
            new LevelInfo(0x00003E96, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_100", "SND_100.wol"),
            new LevelInfo(0x00003E91, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_600", "SND_600.wol"),
            new LevelInfo(0x00003E92, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_604", "SND_604.wol"),
            new LevelInfo(0x00004518, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_610", "SND_610.wol"),
            new LevelInfo(0x00004B5F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_EVENTS", "SND_EVENTS_610.wol"),
            new LevelInfo(0x00004B48, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_MINI_BREAKIN", "SND_MINI_BREAKIN.wol"),
            new LevelInfo(0x00004B41, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_MINI_WASH", "SND_606_PICKING.wol"),
            new LevelInfo(0x00004D4B, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_MUSIC_ADVENTURE", "SND_MUSIC_ADVENTURE.wol"),
            new LevelInfo(0x00004D12, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_MUSIC_MAIN", "SND_MUSIC_MAIN.wol"),
            new LevelInfo(0x00004D71, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_MUSIC_MENUS", "SND_MUSIC_MENUS.wol"),

        };

		public override string[] BFFiles => new string[] {
            "HORSEZ_clean.bf"
        };
	}
}
