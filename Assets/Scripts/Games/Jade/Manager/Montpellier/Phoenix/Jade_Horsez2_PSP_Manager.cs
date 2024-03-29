﻿namespace Ray1Map
{
    public class Jade_Horsez2_PSP_Manager : Jade_Montpellier_BaseManager {
        // Levels
        public override LevelInfo[] LevelInfos => new LevelInfo[] {
            new LevelInfo(0x0000520F, "ROOT/EngineDatas/06 Levels/_BASE/_BASE_CROSS_JUMP", "_BASE_CROSS_JUMP.wol"),
            new LevelInfo(0x00001909, "ROOT/EngineDatas/06 Levels/_BASE/_BASE_GFX", "_BASE_GFX.wol"),
            new LevelInfo(0x00004B3B, "ROOT/EngineDatas/06 Levels/_BASE/_BASE_GFX_MINIGAME", "_BASE_GFX_MINIGAME.wol"),
            new LevelInfo(0x00000EB3, "ROOT/EngineDatas/06 Levels/_BASE/_BASE_OUTSIDE", "_BASE_OUTSIDE.wol"),
            new LevelInfo(0x00003458, "ROOT/EngineDatas/06 Levels/_PREFABS_/_ANIMALS", "_ANIMALS.wol"),
            new LevelInfo(0x00004C3E, "ROOT/EngineDatas/06 Levels/_PREFABS_/_DOORS", "_DOORS.wol"),
            new LevelInfo(0x000022BD, "ROOT/EngineDatas/06 Levels/_PREFABS_/_OBSTACLES/_PF_CROSS", "_PF_CROSS.wol"),
            new LevelInfo(0x000021A3, "ROOT/EngineDatas/06 Levels/_PREFABS_/_OBSTACLES/_PF_JUMP", "_PF_JUMP.wol"),
            new LevelInfo(0x000009D1, "ROOT/EngineDatas/06 Levels/_PREFABS_/_OBSTACLES_GRAPH/_PF_CROSS_GRAPH", "_PF_CROSS_GRAPH.wol"),
            new LevelInfo(0x00004D29, "ROOT/EngineDatas/06 Levels/GAME_MAPS/_COMPIL", "_COMPIL.wol"),
            new LevelInfo(0x000031F0, "ROOT/EngineDatas/06 Levels/GAME_MAPS/_DEMO/CROSS_DEMO", "CROSS_DEMO.wol"),
            new LevelInfo(0x00002BF3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/_DEMO/DEMO", "DEMO.wol"),
            new LevelInfo(0x0000A9CA, "ROOT/EngineDatas/06 Levels/GAME_MAPS/_DEMO/DEMO", "DEMO_FOAL.wol"),
            new LevelInfo(0x0000AD50, "ROOT/EngineDatas/06 Levels/GAME_MAPS/_DEMO/DEMO", "DEMO_HORSE.wol"),
            new LevelInfo(0x00004EAD, "ROOT/EngineDatas/06 Levels/GAME_MAPS/_DEMO/FX_DEMO", "FX_DEMO.wol"),
            new LevelInfo(0x00004E9E, "ROOT/EngineDatas/06 Levels/GAME_MAPS/_DEMO/SOUND_DEMO", "SOUND_DEMO.wol"),
            new LevelInfo(0x0000077B, "ROOT/EngineDatas/06 Levels/GAME_MAPS/_SHOW_DIAL", "_SHOW_DIAL.wol"),
            new LevelInfo(0x00002636, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/_SHOWROOM/ADV_110", "ADV_110.wol"),
            new LevelInfo(0x0000263B, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/_SHOWROOM/ADV_203", "ADV_203.wol"),
            new LevelInfo(0x00002645, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/_SHOWROOM/ADV_310", "ADV_310.wol"),
            new LevelInfo(0x0000264A, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/_SHOWROOM/ADV_410", "ADV_410.wol"),
            new LevelInfo(0x0000264F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/_SHOWROOM/ADV_606", "ADV_606.wol"),
            new LevelInfo(0x00000FA0, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/_SHOWROOM/ADV_607", "ADV_607.wol"),
            new LevelInfo(0x00002659, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/_SHOWROOM/ADV_612", "ADV_612.wol"),
            new LevelInfo(0x00002668, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/_SHOWROOM/ADV_701", "ADV_701.wol"),
            new LevelInfo(0x00002672, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/_SHOWROOM/ADV_711", "ADV_711.wol"),
            new LevelInfo(0x0000267C, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/_SHOWROOM/ADV_721", "ADV_721.wol"),
            new LevelInfo(0x00001EAA, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_100", "ADV_100.wol"),
            new LevelInfo(0x00004B8E, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_100", "ADV_100_RTC_05.wol"),
            new LevelInfo(0x00004BAE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_100_RTC_49", "ADV_100_RTC_49.wol"),
            new LevelInfo(0x0000404D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_102", "ADV_102.wol"),
            new LevelInfo(0x00001F9A, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_202", "ADV_202.wol"),
            new LevelInfo(0x00001EAB, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_300", "ADV_300.wol"),
            new LevelInfo(0x000041D0, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_300", "ADV_300_RTC_09_10_11.wol"),
            new LevelInfo(0x00001EAD, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_400", "ADV_400.wol"),
            new LevelInfo(0x00001EAE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_500", "ADV_500.wol"),
            new LevelInfo(0x00004059, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_502", "ADV_502.wol"),
            new LevelInfo(0x00001F97, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_503", "ADV_503.wol"),
            new LevelInfo(0x00005093, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_503", "ADV_503_LD.wol"),
            new LevelInfo(0x00001EAF, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_600", "ADV_600.wol"),
            new LevelInfo(0x00004F91, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_600", "ADV_600_LD.wol"),
            new LevelInfo(0x00001F96, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_604", "ADV_604.wol"),
            new LevelInfo(0x000040FD, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_610", "ADV_610.wol"),
            new LevelInfo(0x0000266D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_702", "ADV_702.wol"),
            new LevelInfo(0x00001FB7, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_705", "ADV_705.wol"),
            new LevelInfo(0x00001FB6, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_706", "ADV_706.wol"),
            new LevelInfo(0x00002677, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_712", "ADV_712.wol"),
            new LevelInfo(0x00001FB5, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_715", "ADV_715.wol"),
            new LevelInfo(0x000004FA, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_716", "ADV_716.wol"),
            new LevelInfo(0x00002681, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_722", "ADV_722.wol"),
            new LevelInfo(0x00001FB4, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_725", "ADV_725.wol"),
            new LevelInfo(0x00001FB3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_800", "ADV_800.wol"),
            new LevelInfo(0x00001FB2, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_801", "ADV_801.wol"),
            new LevelInfo(0x00001FB1, "ROOT/EngineDatas/06 Levels/GAME_MAPS/ADVENTURE/ADV_802", "ADV_802.wol"),
            new LevelInfo(0x00001FD7, "ROOT/EngineDatas/06 Levels/GAME_MAPS/CREDITS", "CREDITS.wol"),
            new LevelInfo(0x0000047C, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_000", "DEC_000.wol"),
            new LevelInfo(0x00000AE8, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_000", "DEC_000_NPC_AI.wol"),
            new LevelInfo(0x000029F3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_100_PLAIN", "DEC_100_LD.wol"),
            new LevelInfo(0x00005424, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_100_PLAIN", "DEC_100_OBSTACLES.wol"),
            new LevelInfo(0x00000DE3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_100_PLAIN", "DEC_100_PLAIN.wol"),
            new LevelInfo(0x00003048, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_110_RIDING_SCHOOL_PLAIN", "DEC_110_RIDING_SCHOOL_PLAIN.wol"),
            new LevelInfo(0x00000EE9, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_200_EDIE", "DEC_200_EDIE.wol"),
            new LevelInfo(0x00000FE4, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_200_EDIE", "DEC_200_EDIE_ADV.wol"),
            new LevelInfo(0x0000E7AD, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_201_BOX_EDIE", "DEC_201_BOX_EDIE.wol"),
            new LevelInfo(0x00000349, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_300_WOOD", "DEC_300_WOOD.wol"),
            new LevelInfo(0x00003042, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_310_RIDING_SCHOOL_WOOD", "DEC_310_RIDING_SCHOOL_WOOD.wol"),
            new LevelInfo(0x00000E23, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_400_BEACH", "DEC_400_BEACH.wol"),
            new LevelInfo(0x00002D3D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_410_RIDING_SCHOOL_BEACH", "DEC_410_RIDING_SCHOOL_BEACH.wol"),
            new LevelInfo(0x00000C60, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_500_BANKER", "DEC_500_BANKER.wol"),
            new LevelInfo(0x00003F84, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_503_LABYRINTH", "DEC_503_LABYRINTH.wol"),
            new LevelInfo(0x000007D0, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_600_RANCH", "DEC_600_RANCH.wol"),
            new LevelInfo(0x00003479, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_604_STABLE", "DEC_604_STABLE.wol"),
            new LevelInfo(0x00003FB4, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_610_RANCH_MANEGE", "DEC_610_RANCH_MANEGE.wol"),
            new LevelInfo(0x00003EA1, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_701_JUMP_HANOVRE", "DEC_701_JUMP_HANOVRE.wol"),
            new LevelInfo(0x00001B26, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_702_CROSS_HANOVRE", "DEC_702_CROSS_HANOVRE.wol"),
            new LevelInfo(0x00004853, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_705_TICKET_OFFICE", "DEC_705_TICKET_OFFICE.wol"),
            new LevelInfo(0x0000410C, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_706_BOX_HANOVRE", "DEC_706_BOX_HANOVRE.wol"),
            new LevelInfo(0x00006702, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_711_JUMP_PHOENIX", "DEC_711_JUMP_PHOENIX.wol"),
            new LevelInfo(0x00001E43, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_712_CROSS_PHOENIX", "DEC_712_CROSS_PHOENIX.wol"),
            new LevelInfo(0x000038F4, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_715_BOX_PHOENIX", "DEC_715_BOX_PHOENIX.wol"),
            new LevelInfo(0x000004F9, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_716_BOX_FIRE", "DEC_716_BOX_FIRE.wol"),
            new LevelInfo(0x00003994, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_721_JUMP_PARIS", "DEC_721_JUMP_PARIS.wol"),
            new LevelInfo(0x00001970, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_722_CROSS_PARIS", "DEC_722_CROSS_PARIS.wol"),
            new LevelInfo(0x00005666, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_725_BOX_PARIS", "DEC_725_BOX_PARIS.wol"),
            new LevelInfo(0x000014C3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_800_MENTOR_OUT", "DEC_800_MENTOR_OUT.wol"),
            new LevelInfo(0x00007F15, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_801_MENTOR_IN_HOUSE", "DEC_801_IN_HOUSE.wol"),
            new LevelInfo(0x00004769, "ROOT/EngineDatas/06 Levels/GAME_MAPS/DECOR/DEC_802_MENTOR_IN_STABLE", "DEC_802_MENTOR_IN_STABLE.wol"),
            new LevelInfo(0x00000A93, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS/_GRAPH/CROSS_100_GRAPH", "CROSS_100_GRAPH.wol"),
            new LevelInfo(0x0000308B, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS/CROSS_01", "CROSS_01_100.wol"),
            new LevelInfo(0x00004B90, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS/CROSS_01", "CROSS_01_100_RTC_49.wol"),
            new LevelInfo(0x00004EC5, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS/CROSS_01_ACTIV", "CROSS_01_ACTIV.wol"),
            new LevelInfo(0x0000308C, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS/CROSS_02", "CROSS_02_100.wol"),
            new LevelInfo(0x00004EC6, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS/CROSS_02_ACTIV", "CROSS_02_ACTIV.wol"),
            new LevelInfo(0x0000308D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS/CROSS_03", "CROSS_03_300.wol"),
            new LevelInfo(0x0000308E, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS/CROSS_04", "CROSS_04_400.wol"),
            new LevelInfo(0x0000308F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS/CROSS_05", "CROSS_05_702.wol"),
            new LevelInfo(0x00003090, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS/CROSS_06", "CROSS_06_710.wol"),
            new LevelInfo(0x00001D6D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS/CROSS_07", "CROSS_07.wol"),
            new LevelInfo(0x00001969, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/CROSS/CROSS_09", "CROSS_09_722.wol"),
            new LevelInfo(0x000051A5, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_01", "JUMP_01_110.wol"),
            new LevelInfo(0x00002F76, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_02", "JUMP_02_410.wol"),
            new LevelInfo(0x00002F75, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_02", "JUMP_02_610.wol"),
            new LevelInfo(0x00002FA2, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_03", "JUMP_03_310.wol"),
            new LevelInfo(0x00002FA1, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_03", "JUMP_03_610.wol"),
            new LevelInfo(0x00002FA7, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_04", "JUMP_04_310.wol"),
            new LevelInfo(0x00002FA9, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_04", "JUMP_04_610.wol"),
            new LevelInfo(0x00004B34, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_05", "JUMP_05_410.wol"),
            new LevelInfo(0x00002FBD, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_05", "JUMP_05_610.wol"),
            new LevelInfo(0x00002FC3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_06", "JUMP_06_410.wol"),
            new LevelInfo(0x00002FC2, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_06", "JUMP_06_610.wol"),
            new LevelInfo(0x00002FC8, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_07", "JUMP_07_110.wol"),
            new LevelInfo(0x00002FC7, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_07", "JUMP_07_610.wol"),
            new LevelInfo(0x00002FCF, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_08", "JUMP_08_310.wol"),
            new LevelInfo(0x00002FD4, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_09", "JUMP_09_701.wol"),
            new LevelInfo(0x00002FD7, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_10", "JUMP_10_711.wol"),
            new LevelInfo(0x00002FDD, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_12", "JUMP_12_721.wol"),
            new LevelInfo(0x00002FE0, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_13", "JUMP_13_701.wol"),
            new LevelInfo(0x00002FE3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/JUMP/JUMP_14", "JUMP_14_110.wol"),
            new LevelInfo(0x000034D8, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/SPEED/SPEED_01", "SPEED_01.wol"),
            new LevelInfo(0x00002A51, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/SPEED/SPEED_02", "SPEED_02.wol"),
            new LevelInfo(0x000034D9, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/SPEED/SPEED_03", "SPEED_03.wol"),
            new LevelInfo(0x000034DA, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/SPEED/SPEED_04", "SPEED_04.wol"),
            new LevelInfo(0x000034DB, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/SPEED/SPEED_05", "SPEED_05.wol"),
            new LevelInfo(0x000034DC, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/SPEED/SPEED_06", "SPEED_06.wol"),
            new LevelInfo(0x000034DD, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/SPEED/SPEED_07", "SPEED_07.wol"),
            new LevelInfo(0x000041B4, "ROOT/EngineDatas/06 Levels/GAME_MAPS/EVENTS/SPEED/SPEED_08", "SPEED_08.wol"),
            new LevelInfo(0x000040A0, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_100", "GFX_100.wol"),
            new LevelInfo(0x000046C5, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_102", "GFX_102.wol"),
            new LevelInfo(0x00004B04, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_110", "GFX_110.wol"),
            new LevelInfo(0x00004824, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_201", "GFX_201.wol"),
            new LevelInfo(0x00003FEE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_202", "GFX_202.wol"),
            new LevelInfo(0x00000EC2, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_300", "GFX_300.wol"),
            new LevelInfo(0x000040ED, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_310", "GFX_310.wol"),
            new LevelInfo(0x00003F3F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_400", "GFX_400.wol"),
            new LevelInfo(0x00004103, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_410", "GFX_410.wol"),
            new LevelInfo(0x00004063, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_500", "GFX_500.wol"),
            new LevelInfo(0x000079F3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_503", "GFX_503.wol"),
            new LevelInfo(0x00003FFE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_600", "GFX_600.wol"),
            new LevelInfo(0x00004127, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_604", "GFX_604.wol"),
            new LevelInfo(0x000046A3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_606", "GFX_606.wol"),
            new LevelInfo(0x00004CEC, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_608", "GFX_608.wol"),
            new LevelInfo(0x0000412E, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_610", "GFX_610.wol"),
            new LevelInfo(0x00004D05, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_611", "GFX_611.wol"),
            new LevelInfo(0x00004D06, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_617", "GFX_617.wol"),
            new LevelInfo(0x00004BAB, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_701", "GFX_701.wol"),
            new LevelInfo(0x00001068, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_702", "GFX_702.wol"),
            new LevelInfo(0x0000466A, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_705", "GFX_705.wol"),
            new LevelInfo(0x0000416D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_706", "GFX_706.wol"),
            new LevelInfo(0x00003F76, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_711", "GFX_711.wol"),
            new LevelInfo(0x00003F3C, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_712", "GFX_712.wol"),
            new LevelInfo(0x0000473B, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_715", "GFX_715.wol"),
            new LevelInfo(0x00004920, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_716", "GFX_716.wol"),
            new LevelInfo(0x0000415A, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_721", "GFX_721.wol"),
            new LevelInfo(0x00004139, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_722", "GFX_722.wol"),
            new LevelInfo(0x000046E3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_725", "GFX_725.wol"),
            new LevelInfo(0x0000408A, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_800", "GFX_800.wol"),
            new LevelInfo(0x00007F16, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_801", "GFX_801.wol"),
            new LevelInfo(0x000042E8, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GFX/GFX_802", "GFX_802.wol"),
            new LevelInfo(0x00000E71, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_BOX", "GUI_BOX.wol"),
            new LevelInfo(0x00004731, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_CARES", "GUI_CARES.wol"),
            new LevelInfo(0x00000F35, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_COMPETITION", "GUI_COMPETITION.wol"),
            new LevelInfo(0x000044EB, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_COVERING", "GUI_COVERING.wol"),
            new LevelInfo(0x0000086F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_HERO", "GUI_HUD_HERO.wol"),
            new LevelInfo(0x00008AEF, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_HORSE", "GUI_HUD_HORSE.wol"),
            new LevelInfo(0x00002A25, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_BoxWash", "GUI_HUD_MG_BoxWash.wol"),
            new LevelInfo(0x00002A26, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_Brush", "GUI_HUD_MG_Brush.wol"),
            new LevelInfo(0x00000FBB, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_Chick", "GUI_HUD_MG_Chick.wol"),
            new LevelInfo(0x00002A27, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_Circle", "GUI_HUD_MG_Circle.wol"),
            new LevelInfo(0x000026B7, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_Friendship", "GUI_HUD_MG_Friendship.wol"),
            new LevelInfo(0x000029F9, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_HorseWash", "GUI_HUD_MG_HorseWash.wol"),
            new LevelInfo(0x00002A28, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_Picking", "GUI_HUD_MG_Picking.wol"),
            new LevelInfo(0x00000FBC, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_HUD_MG_Sheep", "GUI_HUD_MG_Sheep.wol"),
            new LevelInfo(0x00000B64, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_LANGUAGE", "GUI_LANGUAGE.wol"),
            new LevelInfo(0x0000089A, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_MAIN", "GUI_MAIN.wol"),
            new LevelInfo(0x00000A42, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_MESSAGE_BOX", "GUI_MESSAGE_BOX.wol"),
            new LevelInfo(0x00000B69, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_NAMING", "GUI_NAMING.wol"),
            new LevelInfo(0x00000B7F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_PAUSE", "GUI_PAUSE.wol"),
            new LevelInfo(0x000046D3, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_TRAINING", "GUI_TRAINING.wol"),
            new LevelInfo(0x000055BB, "ROOT/EngineDatas/06 Levels/GAME_MAPS/GUI/GUI_TRASH", "GUI_TRASH.wol"),
            new LevelInfo(0x0000313F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_201_SHEEP", "MINI_201_SHEEP.wol"),
            new LevelInfo(0x00000191, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_601_FRIENDSHIP", "MINI_601.wol"),
            new LevelInfo(0x0000DF04, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_601_FRIENDSHIP", "MINI_601_FRIENDSHIP.wol"),
            new LevelInfo(0x0000B4AE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_601_FRIENDSHIP", "MINI_601_GRANT_STABLE.wol"),
            new LevelInfo(0x0000B4B0, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_601_FRIENDSHIP", "MINI_601_LABYRINTH.wol"),
            new LevelInfo(0x0000B4AF, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_601_FRIENDSHIP", "MINI_601_PHOENIX.wol"),
            new LevelInfo(0x00002F78, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_602_CHICK", "MINI_602_CHICK.wol"),
            new LevelInfo(0x0000299E, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_603_CIRCLE", "MINI_603_CIRCLE.wol"),
            new LevelInfo(0x0000055A, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_604_YOYO", "MINI_604_YOYO.wol"),
            new LevelInfo(0x00000187, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_606_PICKING", "MINI_606_PICKING.wol"),
            new LevelInfo(0x00000236, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_607_VEG_GARDEN", "MINI_607_VEG_GARDEN.wol"),
            new LevelInfo(0x0000BD2D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_611_FOAL_BRUSH", "MINI_611_FOAL_BRUSH.wol"),
            new LevelInfo(0x000073AE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_611_HORSE_BRUSH", "MINI_611_HORSE_BRUSH.wol"),
            new LevelInfo(0x0000C0E8, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_617_FOAL_WASH", "MINI_617_FOAL_WASH.wol"),
            new LevelInfo(0x00002D6F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_617_WASH", "MINI_617_WASH.wol"),
            new LevelInfo(0x0000343B, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_627_BOX_WASH", "MINI_627_BOX_WASH.wol"),
            new LevelInfo(0x00009AB2, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_701_DOG", "MINI_701_DOG.wol"),
            new LevelInfo(0x00000670, "ROOT/EngineDatas/06 Levels/GAME_MAPS/MINIGAMES/MINI_715_BOX_IN_FIRE", "MINI_715_BOX_IN_FIRE.wol"),
            new LevelInfo(0x00009617, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_100_05", "RTC_100_05.wol"),
            new LevelInfo(0x000098DE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_100_49", "RTC_100_49.wol"),
            new LevelInfo(0x000098F9, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_100_51", "RTC_100_51.wol"),
            new LevelInfo(0x00009895, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_100_55", "RTC_100_55.wol"),
            new LevelInfo(0x0000989D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_102_56", "RTC_102_56.wol"),
            new LevelInfo(0x000098AA, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_102_57", "RTC_102_57.wol"),
            new LevelInfo(0x000098B7, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_102_58", "RTC_102_58.wol"),
            new LevelInfo(0x0000985C, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_201_53", "RTC_201_53.wol"),
            new LevelInfo(0x00009876, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_201_54", "RTC_201_54.wol"),
            new LevelInfo(0x000095F5, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_202_02", "RTC_202_02.wol"),
            new LevelInfo(0x00009583, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_202_03", "RTC_202_03.wol"),
            new LevelInfo(0x00009883, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_202_46", "RTC_202_46.wol"),
            new LevelInfo(0x000098C4, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_202_47", "RTC_202_47.wol"),
            new LevelInfo(0x000098D1, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_202_48", "RTC_202_48.wol"),
            new LevelInfo(0x000098EC, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_202_50", "RTC_202_50.wol"),
            new LevelInfo(0x00009625, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_300_06", "RTC_300_06.wol"),
            new LevelInfo(0x0000963F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_300_08", "RTC_300_08.wol"),
            new LevelInfo(0x0000964C, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_300_09", "RTC_300_09.wol"),
            new LevelInfo(0x00009659, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_300_10", "RTC_300_10.wol"),
            new LevelInfo(0x00009666, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_300_11", "RTC_300_11.wol"),
            new LevelInfo(0x00009808, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_500_39", "RTC_500_39.wol"),
            new LevelInfo(0x00009815, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_500_40", "RTC_500_40.wol"),
            new LevelInfo(0x00009822, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_503_41", "RTC_503_41.wol"),
            new LevelInfo(0x0000982F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_503_42", "RTC_503_42.wol"),
            new LevelInfo(0x000095DE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_600_01", "RTC_600_01.wol"),
            new LevelInfo(0x00009602, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_600_04", "RTC_600_04.wol"),
            new LevelInfo(0x00009680, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_600_12", "RTC_600_12.wol"),
            new LevelInfo(0x00009673, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_600_13", "RTC_600_13.wol"),
            new LevelInfo(0x000096AE, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_600_16", "RTC_600_16.wol"),
            new LevelInfo(0x000097FB, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_600_38", "RTC_600_38.wol"),
            new LevelInfo(0x0000983C, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_600_43", "RTC_600_43.wol"),
            new LevelInfo(0x00009730, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_701_26", "RTC_701_26.wol"),
            new LevelInfo(0x000096BB, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_702_17", "RTC_702_17.wol"),
            new LevelInfo(0x000096C8, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_705_18", "RTC_705_18.wol"),
            new LevelInfo(0x000096D5, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_705_19", "RTC_705_19.wol"),
            new LevelInfo(0x000096E2, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_705_20", "RTC_705_20.wol"),
            new LevelInfo(0x0000973D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_712_27", "RTC_712_27.wol"),
            new LevelInfo(0x0000974C, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_715_23", "RTC_715_23.wol"),
            new LevelInfo(0x00009759, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_715_28", "RTC_715_28.wol"),
            new LevelInfo(0x00009766, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_715_29", "RTC_715_29.wol"),
            new LevelInfo(0x00009773, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_715_30", "RTC_715_30.wol"),
            new LevelInfo(0x00009780, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_715_31", "RTC_715_31.wol"),
            new LevelInfo(0x0000F9CA, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_716_61", "RTC_716_61.wol"),
            new LevelInfo(0x0000978F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_722_32", "RTC_722_32.wol"),
            new LevelInfo(0x000097B8, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_725_33", "RTC_725_33.wol"),
            new LevelInfo(0x000097C5, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_725_34", "RTC_725_34.wol"),
            new LevelInfo(0x000097D2, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_725_35", "RTC_725_35.wol"),
            new LevelInfo(0x000097DF, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_725_36", "RTC_725_36.wol"),
            new LevelInfo(0x000097EC, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_725_37", "RTC_725_37.wol"),
            new LevelInfo(0x0000979C, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_725_52", "RTC_725_52.wol"),
            new LevelInfo(0x000097AB, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_725_59", "RTC_725_59.wol"),
            new LevelInfo(0x00009632, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_801_07", "RTC_801_07.wol"),
            new LevelInfo(0x0000968D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_802_14", "RTC_802_14.wol"),
            new LevelInfo(0x0000969A, "ROOT/EngineDatas/06 Levels/GAME_MAPS/RTC/RTC_802_15", "RTC_802_15.wol"),
            new LevelInfo(0x00003E96, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_100", "SND_100.wol"),
            new LevelInfo(0x00003E97, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_202", "SND_202.wol"),
            new LevelInfo(0x00003E29, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_300", "SND_300.wol"),
            new LevelInfo(0x00003E6C, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_400", "SND_400.wol"),
            new LevelInfo(0x00003E66, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_500", "SND_500.wol"),
            new LevelInfo(0x00004D53, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_503", "SND_503.wol"),
            new LevelInfo(0x00003E91, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_600", "SND_600.wol"),
            new LevelInfo(0x00003E92, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_604", "SND_604.wol"),
            new LevelInfo(0x00004518, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_610", "SND_610.wol"),
            new LevelInfo(0x0000451C, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_702", "SND_702.wol"),
            new LevelInfo(0x00004409, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_705", "SND_705.wol"),
            new LevelInfo(0x00005193, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_706", "SND_706.wol"),
            new LevelInfo(0x00004526, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_712", "SND_712.wol"),
            new LevelInfo(0x0000453D, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_722", "SND_722.wol"),
            new LevelInfo(0x00005158, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_725", "SND_725.wol"),
            new LevelInfo(0x00004D58, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_800", "SND_800.wol"),
            new LevelInfo(0x0000435E, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_802", "SND_802.wol"),
            new LevelInfo(0x00004B5F, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_EVENTS", "SND_EVENTS_610.wol"),
            new LevelInfo(0x000045EF, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_MINI_201", "SND_MINI_201_SHEEP.wol"),
            new LevelInfo(0x000045F8, "ROOT/EngineDatas/06 Levels/GAME_MAPS/SOUND/SND_MINI_201", "SND_MINI_602_CHICK.wol"),
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
