using System.Collections.Generic;

namespace R1Engine
{
    public abstract class GBAVV_OverTheHedge_Manager : GBAVV_Volume_BaseManager
    {
        // Metadata
        public override int VolumesCount => 6;

        // Scripts
        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [1201] = GBAVV_ScriptCommand.CommandType.Name,
            [1206] = GBAVV_ScriptCommand.CommandType.Return,
            [1402] = GBAVV_ScriptCommand.CommandType.Dialog,
        };

        // Levels
        public override LevInfo[] LevInfos => Levels;
        public static LevInfo[] Levels => new LevInfo[]
        {
            new LevInfo(0, 0, 0, "", "logos", ""),
            new LevInfo(0, 1, 0, "", "slot select", ""),
            new LevInfo(0, 2, 0, "", "main", ""),
            new LevInfo(0, 3, 0, "", "shop", ""),
            new LevInfo(1, 0, 0, "", "C1L1_forest", ""),
            new LevInfo(1, 1, 0, "", "C2L1_forest", ""),
            new LevInfo(1, 2, 0, "", "C4L1_forest", ""),
            new LevInfo(1, 3, 0, "", "C6L1_forest", ""),
            new LevInfo(1, 4, 0, "", "D_forest01", ""),
            new LevInfo(1, 5, 0, "", "D_forest02", ""),
            new LevInfo(1, 6, 0, "", "D_forest03", ""),
            new LevInfo(1, 7, 0, "", "D_forest04", ""),
            new LevInfo(1, 8, 0, "", "D_forest05", ""),
            new LevInfo(1, 9, 0, "", "D_forest06", ""),
            new LevInfo(1, 10, 0, "", "D_forest07", ""),
            new LevInfo(1, 11, 0, "", "D_forest08", ""),
            new LevInfo(2, 0, 0, "", "C1L2_sidewalk", ""),
            new LevInfo(2, 1, 0, "", "C2L3_sidewalk", ""),
            new LevInfo(2, 2, 0, "", "C6L2_sidewalk", ""),
            new LevInfo(2, 3, 0, "", "D_sidewalk01", ""),
            new LevInfo(2, 4, 0, "", "D_sidewalk02", ""),
            new LevInfo(2, 5, 0, "", "D_sidewalk03", ""),
            new LevInfo(2, 6, 0, "", "D_sidewalk04", ""),
            new LevInfo(2, 7, 0, "", "D_sidewalk05", ""),
            new LevInfo(2, 8, 0, "", "D_sidewalk06", ""),
            new LevInfo(2, 9, 0, "", "D_sidewalk07", ""),
            new LevInfo(2, 10, 0, "", "D_sidewalk08", ""),
            new LevInfo(3, 0, 0, "", "C1L3_bckyrdAI", ""),
            new LevInfo(3, 1, 0, "", "C2L2_bckyrdAI", ""),
            new LevInfo(3, 2, 0, "", "C4L2_bckyrdAI", ""),
            new LevInfo(3, 3, 0, "", "C4L4_bckyrdT", ""),
            new LevInfo(3, 4, 0, "", "D_bckyrdAI01", ""),
            new LevInfo(3, 5, 0, "", "D_bckyrdAI02", ""),
            new LevInfo(3, 6, 0, "", "D_bckyrdAI03", ""),
            new LevInfo(3, 7, 0, "", "D_bckyrdAI04", ""),
            new LevInfo(3, 8, 0, "", "D_bckyrdT05", ""),
            new LevInfo(3, 9, 0, "", "D_bckyrdT06", ""),
            new LevInfo(3, 10, 0, "", "D_bckyrdT07", ""),
            new LevInfo(3, 11, 0, "", "D_bckyrdT08", ""),
            new LevInfo(4, 0, 0, "", "C1L4a_houseL", ""),
            new LevInfo(4, 1, 0, "", "C1L4b_houseK", ""),
            new LevInfo(4, 2, 0, "", "C7L2a_houseK", ""),
            new LevInfo(4, 3, 0, "", "C7L2b_houseB", ""),
            new LevInfo(4, 4, 0, "", "D_houseB01", ""),
            new LevInfo(4, 5, 0, "", "D_houseB02", ""),
            new LevInfo(4, 6, 0, "", "D_houseB03", ""),
            new LevInfo(4, 7, 0, "", "D_houseB04", ""),
            new LevInfo(4, 8, 0, "", "D_houseB05", ""),
            new LevInfo(4, 9, 0, "", "D_houseB06", ""),
            new LevInfo(4, 10, 0, "", "D_houseK01", ""),
            new LevInfo(4, 11, 0, "", "D_houseK02", ""),
            new LevInfo(4, 12, 0, "", "D_houseK03", ""),
            new LevInfo(4, 13, 0, "", "D_houseK04", ""),
            new LevInfo(4, 14, 0, "", "D_houseK05", ""),
            new LevInfo(4, 15, 0, "", "D_houseK06", ""),
            new LevInfo(4, 16, 0, "", "D_houseL01", ""),
            new LevInfo(4, 17, 0, "", "D_houseL02", ""),
            new LevInfo(4, 18, 0, "", "D_houseL03", ""),
            new LevInfo(4, 19, 0, "", "D_houseL04", ""),
            new LevInfo(4, 20, 0, "", "D_houseL05", ""),
            new LevInfo(4, 21, 0, "", "D_houseL06", ""),
            new LevInfo(5, 0, 0, "", "C2L4_sidewalkH", ""),
            new LevInfo(5, 1, 0, "", "C4L3_sidewalkO", ""),
            new LevInfo(5, 2, 0, "", "C6L3_backyardF", ""),
            new LevInfo(5, 3, 0, "", "C6L4_backyardN", ""),
            new LevInfo(5, 4, 0, "", "C7L1_backyardTS", ""),
            new LevInfo(5, 5, 0, "", "C7L4_sidewalkV", ""),
            new LevInfo(5, 6, 0, "", "C7L5_backyardTH", ""),
        };
    }
    public class GBAVV_OverTheHedgeEU_Manager : GBAVV_OverTheHedge_Manager
    {
        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x081C0AC0,
            0x081C7294,
            0x081CB28C,
            0x081CF4D4,
            0x081D0A0C,
            0x081D579C,
            0x081D746C,
            0x081D9A94,
            0x081DC6D8,
            0x081DCBDC,
            0x081DF1C8,
            0x081E3E48,
            0x081E52A8,
            0x081E8474,
            0x081EC20C,
            0x081EF310,
            0x081EF8C8,
            0x081EFEE0,
            0x081F04E8,
            0x081F0B08,
            0x081F11A4,
            0x081F58A8,
            0x081FA184,
            0x0820DCF4,
            0x08212364,
            0x082202B4,
            0x08221030,
            0x0822267C,
            0x08225380,
            0x08227270,
            0x0822DA50,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08037124, // script_waitForInputOrTime
            0x08063D6C, // pacingChildScript
            0x08063DEC, // setDeletePending
            0x08063EDC, // checkpointStartScript
            0x08063F48, // pacingChildRun
            0x08063FFC, // pacingChildSetup
            0x080640E8, // turnN
            0x080641D8, // changeDirectionN
            0x08064238, // turnNE
            0x08064328, // changeDirectionNE
            0x08064388, // turnNW
            0x08064478, // changeDirectionNW
            0x080644D8, // turnE
            0x080645C8, // changeDirectionE
            0x08064628, // turnS
            0x08064718, // changeDirectionS
            0x08064778, // turnSE
            0x08064868, // changeDirectionSE
            0x080648C8, // turnSW
            0x080649B8, // changeDirectionSW
            0x08064A18, // turnW
            0x08064B08, // changeDirectionW
            0x08064B5C, // pacingChildTrip
            0x08064BF8, // reset
            0x08064CE8, // pacingChildReset
            0x08064EBC, // arrowFountainScript
            0x08064EFC, // spikesScript
            0x08064F40, // arrowFountainRun
            0x08064F8C, // arrowFountainSetup
            0x08064FF8, // spikesRun
            0x0806503C, // spikesSetup
            0x080650B0, // arrowFountainShoot
            0x0806516C, // spikesActive
            0x080657C0, // linger
            0x080658E0, // setDeletePending
            0x080659D0, // maceScript
            0x08065A90, // RJVerneGolfBallScript
            0x08065B6C, // linger
            0x08065C5C, // stinkScript
            0x08065D08, // hazardSpawnerScript
            0x08065D9C, // timedHazardSpawnerScript
            0x08065EF4, // scrollingHazardScript
            0x08065FC8, // scrollingBBQScript
            0x08066100, // returnToPlayer
            0x080661F0, // boomerangScript
            0x0806630C, // reverseDirectionScript
            0x08066410, // golfBallDeathScript
            0x080664D0, // easy
            0x080665D4, // easy
            0x080666D8, // easy
            0x080667C8, // spawnStaticHazard
            0x080668C4, // spawnCandy
            0x080669B4, // spawnTriggeredHazard
            0x08066A34, // waitBetweenHazards
            0x08066D30, // waitForPagedText
            0x080BCF24, // notFound
            0x080BD40C, // waitForPagedText
            0x080BD474, // missing
            0x080BD4B8, // C0L0
            0x080BD50C, // ID_C1L0_01
            0x080BD788, // ID_C1L0_02
            0x080BD86C, // ID_C1L0_03
            0x080BD98C, // ID_C1L0_04
            0x080BDA10, // ID_C1L0_05
            0x080BDA64, // ID_C1L0_06
            0x080BDBF0, // ID_C1L0_07
            0x080BDC98, // ID_C1L0_08
            0x080BDD28, // ID_C1L0_09
            0x080BDD7C, // ID_C1L1_01
            0x080BDDD0, // ID_C1L1_02
            0x080BDE24, // ID_C1L1_03
            0x080BDE78, // ID_C1L1_04
            0x080BDECC, // ID_C1L1_98
            0x080BDF20, // ID_C1L2_01
            0x080BDF74, // ID_C1L2_98
            0x080BDFC8, // ID_C1L3_01
            0x080BE01C, // ID_C1L3_97
            0x080BE070, // ID_C1L3_02
            0x080BE0C4, // ID_C1L3_98
            0x080BE118, // ID_C1L4_01
            0x080BE16C, // ID_C1L5_01
            0x080BE1C0, // ID_C1L5_97
            0x080BE214, // ID_C1L5_98
            0x080BE268, // ID_C2L0_01
            0x080BE418, // ID_C2L0_02
            0x080BE49C, // ID_C2L0_03
            0x080BE52C, // ID_C2L0_04
            0x080BE5BC, // ID_C2L0_05
            0x080BE610, // ID_C2L0_06
            0x080BE664, // ID_C2L0_07
            0x080BE700, // ID_C2L0_08
            0x080BE754, // ID_C2L0_09
            0x080BE7A8, // ID_C2L1_01
            0x080BE7FC, // ID_C2L2_01
            0x080BE850, // ID_C2L3_01
            0x080BE8A4, // ID_C2L3_98
            0x080BE9F4, // ID_C2L4_01
            0x080BEA48, // ID_C2L4_98
            0x080BEACC, // ID_C3L0_01
            0x080BECD0, // ID_C3L0_02
            0x080BED48, // ID_C3L0_03
            0x080BEE38, // ID_C3L0_04
            0x080BEEBC, // ID_C3L0_05
            0x080BEF4C, // ID_C3L0_06
            0x080BF00C, // ID_C3L0_07
            0x080BF0A8, // ID_C3L0_08
            0x080BF18C, // ID_C3L0_09
            0x080BF234, // ID_C4L0_01
            0x080BF3CC, // ID_C4L0_02
            0x080BF450, // ID_C4L0_03
            0x080BF4D4, // ID_C4L0_04
            0x080BF528, // ID_C4L0_05
            0x080BF5A0, // ID_C4L0_06
            0x080BF618, // ID_C4L0_07
            0x080BF6E4, // ID_C4L0_08
            0x080BF768, // ID_C4L0_09
            0x080BF888, // ID_C4L1_01
            0x080BF8DC, // ID_C4L2_01
            0x080BF930, // ID_C4L3_01
            0x080BFA68, // ID_C4L3_02
            0x080BFAC8, // ID_C4L3_03
            0x080BFB28, // ID_C4L3_98
            0x080BFB7C, // ID_C4L4_01
            0x080BFBD0, // ID_C4L4_98
            0x080BFC24, // ID_C5L0_01
            0x080BFDE0, // ID_C5L0_02
            0x080BFE64, // ID_C5L0_03
            0x080BFEE8, // ID_C5L0_04
            0x080BFF78, // ID_C5L0_05
            0x080BFFFC, // ID_C5L0_06
            0x080C00B0, // ID_C5L0_07
            0x080C0188, // ID_C5L0_08
            0x080C020C, // ID_C5L0_09
            0x080C02C0, // ID_C6L0_01
            0x080C04C4, // ID_C6L0_03
            0x080C0518, // ID_C6L0_04
            0x080C056C, // ID_C6L0_05
            0x080C05C0, // ID_C6L0_06
            0x080C0614, // ID_C6L0_07
            0x080C06B0, // ID_C6L0_08
            0x080C0704, // ID_C6L0_09
            0x080C0758, // ID_C6L1_01
            0x080C07B8, // ID_C6L2_01
            0x080C0818, // ID_C6L3_01
            0x080C092C, // ID_C6L3_98
            0x080C0AD0, // ID_C6L4_01
            0x080C0B54, // ID_C7L0_01
            0x080C0D28, // ID_C7L0_02
            0x080C0DAC, // ID_C7L0_03
            0x080C0E30, // ID_C7L0_04
            0x080C0EB4, // ID_C7L0_05
            0x080C0F08, // ID_C7L0_06
            0x080C0F5C, // ID_C7L0_07
            0x080C0FF8, // ID_C7L0_08
            0x080C104C, // ID_C7L0_09
            0x080C1100, // ID_C7L1_01
            0x080C1154, // ID_C7L1_98
            0x080C1208, // ID_C7L2_01
            0x080C125C, // ID_C7L2_98
            0x080C12B0, // ID_C7L3_01
            0x080C1304, // ID_C7L3_98
            0x080C13C4, // ID_C7L4_01
            0x080C13F4, // ID_C7L5_01
            0x080C1478, // ID_C7L5_98
            0x080C14FC, // ID_C7L6_01
            0x080C155C, // ID_C7L6_98
            0x080C15B0, // ID_C8L0_02
            0x080C1634, // ID_C8L0_03
            0x080C16B8, // ID_C8L0_04
            0x080C170C, // ID_C8L0_05
            0x080C1760, // ID_C8L0_06
            0x080C17B4, // ID_C8L0_07
            0x080C1850, // ID_C8L0_08
            0x080C18A4, // ID_C8L0_09
            0x080C18F8, // ID_C8L0_10
            0x080C1978, // C8L0
            0x080C19CC, // ID_C0L0_02
            0x080C1A64, // lamp
            0x080C1AB4, // sink
            0x080C1B04, // stove
            0x080C1B54, // blender
            0x080C1BA4, // alarm
            0x080C1BF8, // microwave
            0x080C1C48, // radio
            0x080C1C98, // tree
            0x080C1CE4, // tv
            0x080C1D30, // gba
            0x080C1D80, // othCart
            0x080C1DD4, // penguinCart
            0x080C1E28, // boomerang
            0x080C1E7C, // golfclubs
            0x080C1ED0, // attackRJ
            0x080C1F24, // attackVerne
            0x080C1F78, // healthRJ
            0x080C1FCC, // healthVerne
            0x080C2020, // enduranceRJ
            0x080C2078, // enduranceVerne
            0x080C20D0, // couldntAfford
            0x080CB104, // movie_license
            0x080CB190, // movie_credits
            0x080CB244, // CS_C0L0_00
            0x080CBA78, // CS_C4L3_99
            0x080CBD48, // CS_C6L4_00
            0x080CC018, // CS_C6L4_99
            0x080CC234, // CS_C7L5_00
            0x080CC708, // CS_C7L5_99
            0x080CC918, // CS_C7L6_00
            0x080CCABC, // CS_C7L6_99
            0x080CCE1C, // waitForPagedText
            0x080CCE8C, // JpegPleaseWait
        };
    }
    public class GBAVV_OverTheHedgeUS_Manager : GBAVV_OverTheHedge_Manager
    {
        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x081C0AC0,
            0x081C7294,
            0x081CB28C,
            0x081CF4D4,
            0x081D0A0C,
            0x081D579C,
            0x081D746C,
            0x081D9A94,
            0x081DC6D8,
            0x081DCBDC,
            0x081DF1C8,
            0x081E3E48,
            0x081E52A8,
            0x081E8474,
            0x081EC20C,
            0x081EF310,
            0x081EF8C8,
            0x081EFEE0,
            0x081F04E8,
            0x081F0B08,
            0x081F11A4,
            0x081F58A8,
            0x081FA184,
            0x0820DCF4,
            0x08212364,
            0x082202B4,
            0x08221030,
            0x0822267C,
            0x08225380,
            0x08227270,
            0x0822DA50,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08037124, // script_waitForInputOrTime
            0x08063D6C, // pacingChildScript
            0x08063DEC, // setDeletePending
            0x08063EDC, // checkpointStartScript
            0x08063F48, // pacingChildRun
            0x08063FFC, // pacingChildSetup
            0x080640E8, // turnN
            0x080641D8, // changeDirectionN
            0x08064238, // turnNE
            0x08064328, // changeDirectionNE
            0x08064388, // turnNW
            0x08064478, // changeDirectionNW
            0x080644D8, // turnE
            0x080645C8, // changeDirectionE
            0x08064628, // turnS
            0x08064718, // changeDirectionS
            0x08064778, // turnSE
            0x08064868, // changeDirectionSE
            0x080648C8, // turnSW
            0x080649B8, // changeDirectionSW
            0x08064A18, // turnW
            0x08064B08, // changeDirectionW
            0x08064B5C, // pacingChildTrip
            0x08064BF8, // reset
            0x08064CE8, // pacingChildReset
            0x08064EBC, // arrowFountainScript
            0x08064EFC, // spikesScript
            0x08064F40, // arrowFountainRun
            0x08064F8C, // arrowFountainSetup
            0x08064FF8, // spikesRun
            0x0806503C, // spikesSetup
            0x080650B0, // arrowFountainShoot
            0x0806516C, // spikesActive
            0x080657C0, // linger
            0x080658E0, // setDeletePending
            0x080659D0, // maceScript
            0x08065A90, // RJVerneGolfBallScript
            0x08065B6C, // linger
            0x08065C5C, // stinkScript
            0x08065D08, // hazardSpawnerScript
            0x08065D9C, // timedHazardSpawnerScript
            0x08065EF4, // scrollingHazardScript
            0x08065FC8, // scrollingBBQScript
            0x08066100, // returnToPlayer
            0x080661F0, // boomerangScript
            0x0806630C, // reverseDirectionScript
            0x08066410, // golfBallDeathScript
            0x080664D0, // easy
            0x080665D4, // easy
            0x080666D8, // easy
            0x080667C8, // spawnStaticHazard
            0x080668C4, // spawnCandy
            0x080669B4, // spawnTriggeredHazard
            0x08066A34, // waitBetweenHazards
            0x08066D30, // waitForPagedText
            0x080BCF24, // notFound
            0x080BD40C, // waitForPagedText
            0x080BD474, // missing
            0x080BD4B8, // C0L0
            0x080BD50C, // ID_C1L0_01
            0x080BD788, // ID_C1L0_02
            0x080BD86C, // ID_C1L0_03
            0x080BD98C, // ID_C1L0_04
            0x080BDA10, // ID_C1L0_05
            0x080BDA64, // ID_C1L0_06
            0x080BDBF0, // ID_C1L0_07
            0x080BDC98, // ID_C1L0_08
            0x080BDD28, // ID_C1L0_09
            0x080BDD7C, // ID_C1L1_01
            0x080BDDD0, // ID_C1L1_02
            0x080BDE24, // ID_C1L1_03
            0x080BDE78, // ID_C1L1_04
            0x080BDECC, // ID_C1L1_98
            0x080BDF20, // ID_C1L2_01
            0x080BDF74, // ID_C1L2_98
            0x080BDFC8, // ID_C1L3_01
            0x080BE01C, // ID_C1L3_97
            0x080BE070, // ID_C1L3_02
            0x080BE0C4, // ID_C1L3_98
            0x080BE118, // ID_C1L4_01
            0x080BE16C, // ID_C1L5_01
            0x080BE1C0, // ID_C1L5_97
            0x080BE214, // ID_C1L5_98
            0x080BE268, // ID_C2L0_01
            0x080BE418, // ID_C2L0_02
            0x080BE49C, // ID_C2L0_03
            0x080BE52C, // ID_C2L0_04
            0x080BE5BC, // ID_C2L0_05
            0x080BE610, // ID_C2L0_06
            0x080BE664, // ID_C2L0_07
            0x080BE700, // ID_C2L0_08
            0x080BE754, // ID_C2L0_09
            0x080BE7A8, // ID_C2L1_01
            0x080BE7FC, // ID_C2L2_01
            0x080BE850, // ID_C2L3_01
            0x080BE8A4, // ID_C2L3_98
            0x080BE9F4, // ID_C2L4_01
            0x080BEA48, // ID_C2L4_98
            0x080BEACC, // ID_C3L0_01
            0x080BECD0, // ID_C3L0_02
            0x080BED48, // ID_C3L0_03
            0x080BEE38, // ID_C3L0_04
            0x080BEEBC, // ID_C3L0_05
            0x080BEF4C, // ID_C3L0_06
            0x080BF00C, // ID_C3L0_07
            0x080BF0A8, // ID_C3L0_08
            0x080BF18C, // ID_C3L0_09
            0x080BF234, // ID_C4L0_01
            0x080BF3CC, // ID_C4L0_02
            0x080BF450, // ID_C4L0_03
            0x080BF4D4, // ID_C4L0_04
            0x080BF528, // ID_C4L0_05
            0x080BF5A0, // ID_C4L0_06
            0x080BF618, // ID_C4L0_07
            0x080BF6E4, // ID_C4L0_08
            0x080BF768, // ID_C4L0_09
            0x080BF888, // ID_C4L1_01
            0x080BF8DC, // ID_C4L2_01
            0x080BF930, // ID_C4L3_01
            0x080BFA68, // ID_C4L3_02
            0x080BFAC8, // ID_C4L3_03
            0x080BFB28, // ID_C4L3_98
            0x080BFB7C, // ID_C4L4_01
            0x080BFBD0, // ID_C4L4_98
            0x080BFC24, // ID_C5L0_01
            0x080BFDE0, // ID_C5L0_02
            0x080BFE64, // ID_C5L0_03
            0x080BFEE8, // ID_C5L0_04
            0x080BFF78, // ID_C5L0_05
            0x080BFFFC, // ID_C5L0_06
            0x080C00B0, // ID_C5L0_07
            0x080C0188, // ID_C5L0_08
            0x080C020C, // ID_C5L0_09
            0x080C02C0, // ID_C6L0_01
            0x080C04C4, // ID_C6L0_03
            0x080C0518, // ID_C6L0_04
            0x080C056C, // ID_C6L0_05
            0x080C05C0, // ID_C6L0_06
            0x080C0614, // ID_C6L0_07
            0x080C06B0, // ID_C6L0_08
            0x080C0704, // ID_C6L0_09
            0x080C0758, // ID_C6L1_01
            0x080C07B8, // ID_C6L2_01
            0x080C0818, // ID_C6L3_01
            0x080C092C, // ID_C6L3_98
            0x080C0AD0, // ID_C6L4_01
            0x080C0B54, // ID_C7L0_01
            0x080C0D28, // ID_C7L0_02
            0x080C0DAC, // ID_C7L0_03
            0x080C0E30, // ID_C7L0_04
            0x080C0EB4, // ID_C7L0_05
            0x080C0F08, // ID_C7L0_06
            0x080C0F5C, // ID_C7L0_07
            0x080C0FF8, // ID_C7L0_08
            0x080C104C, // ID_C7L0_09
            0x080C1100, // ID_C7L1_01
            0x080C1154, // ID_C7L1_98
            0x080C1208, // ID_C7L2_01
            0x080C125C, // ID_C7L2_98
            0x080C12B0, // ID_C7L3_01
            0x080C1304, // ID_C7L3_98
            0x080C13C4, // ID_C7L4_01
            0x080C13F4, // ID_C7L5_01
            0x080C1478, // ID_C7L5_98
            0x080C14FC, // ID_C7L6_01
            0x080C155C, // ID_C7L6_98
            0x080C15B0, // ID_C8L0_02
            0x080C1634, // ID_C8L0_03
            0x080C16B8, // ID_C8L0_04
            0x080C170C, // ID_C8L0_05
            0x080C1760, // ID_C8L0_06
            0x080C17B4, // ID_C8L0_07
            0x080C1850, // ID_C8L0_08
            0x080C18A4, // ID_C8L0_09
            0x080C18F8, // ID_C8L0_10
            0x080C1978, // C8L0
            0x080C19CC, // ID_C0L0_02
            0x080C1A64, // lamp
            0x080C1AB4, // sink
            0x080C1B04, // stove
            0x080C1B54, // blender
            0x080C1BA4, // alarm
            0x080C1BF8, // microwave
            0x080C1C48, // radio
            0x080C1C98, // tree
            0x080C1CE4, // tv
            0x080C1D30, // gba
            0x080C1D80, // othCart
            0x080C1DD4, // penguinCart
            0x080C1E28, // boomerang
            0x080C1E7C, // golfclubs
            0x080C1ED0, // attackRJ
            0x080C1F24, // attackVerne
            0x080C1F78, // healthRJ
            0x080C1FCC, // healthVerne
            0x080C2020, // enduranceRJ
            0x080C2078, // enduranceVerne
            0x080C20D0, // couldntAfford
            0x080CB104, // movie_license
            0x080CB190, // movie_credits
            0x080CB244, // CS_C0L0_00
            0x080CBA78, // CS_C4L3_99
            0x080CBD48, // CS_C6L4_00
            0x080CC018, // CS_C6L4_99
            0x080CC234, // CS_C7L5_00
            0x080CC708, // CS_C7L5_99
            0x080CC918, // CS_C7L6_00
            0x080CCABC, // CS_C7L6_99
            0x080CCE1C, // waitForPagedText
            0x080CCE8C, // JpegPleaseWait
        };
    }
}