using System.Collections.Generic;

namespace R1Engine
{
    public abstract class GBAVV_SharkTale_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 42;
        public override bool HasAssignedObjTypeGraphics => true;

        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [0401] = GBAVV_ScriptCommand.CommandType.Name,
            [0402] = GBAVV_ScriptCommand.CommandType.Script,
            [0406] = GBAVV_ScriptCommand.CommandType.Return,
            [0602] = GBAVV_ScriptCommand.CommandType.Dialog,
        };

        public override ObjTypeInit[] ObjTypeInitInfos => new ObjTypeInit[]
        {
            new ObjTypeInit(-1, -1, null), // 0
            new ObjTypeInit(-1, -1, null), // 1
            new ObjTypeInit(-1, -1, null), // 2
            new ObjTypeInit(-1, -1, null), // 3
            new ObjTypeInit(-1, -1, null), // 4
            new ObjTypeInit(-1, -1, null), // 5
            new ObjTypeInit(9, 2, null), // 6
            new ObjTypeInit(9, 13, null), // 7
            new ObjTypeInit(-1, -1, null), // 8
            new ObjTypeInit(-1, -1, null), // 9
            new ObjTypeInit(5, 29, "dialogNPC"), // 10
            new ObjTypeInit(5, 1, "crazyJoe"), // 11
            new ObjTypeInit(5, 27, "tightFollow"), // 12
            new ObjTypeInit(5, 5, "shortieTagger"), // 13
            new ObjTypeInit(5, 11, "shortieTagger"), // 14
            new ObjTypeInit(5, 17, "shortieTagger"), // 15
            new ObjTypeInit(5, 34, "lennyFollow"), // 16
            new ObjTypeInit(5, 35, "lennyFollow"), // 17
            new ObjTypeInit(5, 0, "dialogNPC"), // 18
            new ObjTypeInit(5, 0, "swimFast"), // 19
            new ObjTypeInit(5, 4, "dialogNPC"), // 20
            new ObjTypeInit(7, 76, "dancerStart"), // 21
            new ObjTypeInit(5, 27, "swimFast"), // 22
            new ObjTypeInit(5, 36, "dialogNPC"), // 23
            new ObjTypeInit(5, 6, "shortieSwimmer"), // 24
            new ObjTypeInit(5, 18, "shortieSwimmer"), // 25
            new ObjTypeInit(5, 12, "shortieSwimmer"), // 26
            new ObjTypeInit(5, 1, "crazyJoeImplode"), // 27
            new ObjTypeInit(6, 0, "bananaScript"), // 28
            new ObjTypeInit(6, 16, "healthSmall"), // 29
            new ObjTypeInit(6, 15, "healthMedium"), // 30
            new ObjTypeInit(6, 14, "healthLarge"), // 31
            new ObjTypeInit(6, 14, "healthFull"), // 32
            new ObjTypeInit(6, 18, "urchin"), // 33
            new ObjTypeInit(6, 19, "urchinTriple"), // 34
            new ObjTypeInit(6, 21, "sonarBomb"), // 35
            new ObjTypeInit(6, 20, "inkBomb"), // 36
            new ObjTypeInit(6, 17, "swarm"), // 37
            new ObjTypeInit(6, 22, "extraTime"), // 38
            new ObjTypeInit(6, 27, "extraPoints200"), // 39
            new ObjTypeInit(6, 26, "extraPoints500"), // 40
            new ObjTypeInit(6, 23, "extraPoints"), // 41
            new ObjTypeInit(6, 24, "speedUp"), // 42
            new ObjTypeInit(6, 25, "timeFreeze"), // 43
            new ObjTypeInit(6, 38, "disguise"), // 44
            new ObjTypeInit(6, 36, "disguise"), // 45
            new ObjTypeInit(6, 37, "disguise"), // 46
            new ObjTypeInit(6, 35, "disguise"), // 47
            new ObjTypeInit(-1, -1, null), // 48
            new ObjTypeInit(-1, -1, null), // 49
            new ObjTypeInit(-1, -1, null), // 50
            new ObjTypeInit(-1, -1, null), // 51
            new ObjTypeInit(-1, -1, null), // 52
            new ObjTypeInit(6, 28, "promptMessenger"), // 53
            new ObjTypeInit(-1, -1, null), // 54
            new ObjTypeInit(-1, -1, null), // 55
            new ObjTypeInit(-1, -1, null), // 56
            new ObjTypeInit(0, 47, "promptKilla"), // 57
            new ObjTypeInit(6, 33, "clampWait"), // 58
            new ObjTypeInit(6, 29, "anchorWait"), // 59
            new ObjTypeInit(6, 33, "clampBottom"), // 60
            new ObjTypeInit(6, 34, "clampBottom"), // 61
            new ObjTypeInit(6, 34, "clampWait"), // 62
            new ObjTypeInit(4, 2, "passivePatrol"), // 63
            new ObjTypeInit(4, 10, "crabBouncerWalker"), // 64
            new ObjTypeInit(5, 30, "dolphinCop"), // 65
            new ObjTypeInit(4, 30, "thugFishSeek"), // 66
            new ObjTypeInit(4, 34, "thugFishSeek"), // 67
            new ObjTypeInit(4, 38, "alertPatroller"), // 68
            new ObjTypeInit(4, 43, "shooterDude"), // 69
            new ObjTypeInit(4, 52, "ambusherDude"), // 70
            new ObjTypeInit(4, 10, "crabBouncerStationary"), // 71
            new ObjTypeInit(9, 5, null), // 72
            new ObjTypeInit(3, 5, null), // 73
            new ObjTypeInit(3, 6, null), // 74
            new ObjTypeInit(3, 4, null), // 75
            new ObjTypeInit(-1, -1, null), // 76
            new ObjTypeInit(9, 9, "racetrackEnemyScript"), // 77
            new ObjTypeInit(9, 18, "racetrackEnemyScript"), // 78
            new ObjTypeInit(9, 22, "racetrackEnemyScript"), // 79
            new ObjTypeInit(9, 5, "hurdleScript"), // 80
            new ObjTypeInit(-1, -1, null), // 81
            new ObjTypeInit(-1, -1, null), // 82
            new ObjTypeInit(-1, -1, null), // 83
            new ObjTypeInit(9, 9, "racetrackEnemyFasterScript"), // 84
            new ObjTypeInit(9, 18, "racetrackEnemyFasterScript"), // 85
            new ObjTypeInit(9, 22, "racetrackEnemyFasterScript"), // 86
            new ObjTypeInit(4, 15, "steamVent"), // 87
            new ObjTypeInit(4, 16, "octoWipeScript"), // 88
            new ObjTypeInit(4, 54, "washCrabScript"), // 89
            new ObjTypeInit(4, 21, "octoWipeScript"), // 90
            new ObjTypeInit(4, 22, "octoWipeScript"), // 91
            new ObjTypeInit(4, 20, "octoWipeScript"), // 92
            new ObjTypeInit(4, 19, "octoWipeScript"), // 93
            new ObjTypeInit(4, 14, null), // 94
            new ObjTypeInit(4, 25, "steamVent"), // 95
            new ObjTypeInit(4, 25, "steamVent"), // 96
            new ObjTypeInit(4, 27, "slaveIdle"), // 97
            new ObjTypeInit(4, 5, "masterScript"), // 98
            new ObjTypeInit(4, 5, "coralScript"), // 99
            new ObjTypeInit(4, 55, "coralScript"), // 100
            new ObjTypeInit(4, 1, "steamVent"), // 101
            new ObjTypeInit(6, 6, "destructible1"), // 102
            new ObjTypeInit(6, 9, "destructible1"), // 103
            new ObjTypeInit(6, 12, "destructible2"), // 104
            new ObjTypeInit(6, 1, "destructibleCoral"), // 105
            new ObjTypeInit(-1, -1, null), // 106
            new ObjTypeInit(0, 52, "buttonADown"), // 107
            new ObjTypeInit(0, 48, "buttonBRight"), // 108
            new ObjTypeInit(0, 47, "buttonBDown"), // 109
            new ObjTypeInit(0, 50, "buttonLEFTRight"), // 110
            new ObjTypeInit(0, 50, "buttonLEFTDown"), // 111
            new ObjTypeInit(0, 51, "buttonRIGHTRight"), // 112
            new ObjTypeInit(0, 51, "buttonRIGHTDown"), // 113
            new ObjTypeInit(0, 47, "buttonUPRight"), // 114
            new ObjTypeInit(0, 47, "buttonUPDown"), // 115
            new ObjTypeInit(-1, -1, null), // 116
            new ObjTypeInit(0, 49, "buttonDOWNRight"), // 117
            new ObjTypeInit(8, 13, "lolaProjPhone"), // 118
            new ObjTypeInit(8, 10, "lolaProj"), // 119
            new ObjTypeInit(8, 8, null), // 120
            new ObjTypeInit(2, 3, "ernieStart"), // 121
            new ObjTypeInit(2, 3, "bernieStart"), // 122
            new ObjTypeInit(2, 11, "lolaStart"), // 123
            new ObjTypeInit(2, 15, "lolaNet"), // 124
            new ObjTypeInit(2, 14, "lolaRope"), // 125
            new ObjTypeInit(2, 1, "frankieWait"), // 126
            new ObjTypeInit(2, 8, "donLinoStart"), // 127
        };
    }
    public class GBAVV_SharkTaleEUUS_Manager : GBAVV_SharkTale_Manager
    {
        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0811761C,
            0x0811AF2C,
            0x0811B04C,
            0x0811E5F4,
            0x0811F078,
            0x08128F40,
            0x08130AFC,
            0x08134AAC,
            0x08149B3C,
            0x0814B35C,
            0x0814DED8,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x080256D4, // script_waitForInputOrTime
            0x0802651C, // bananaScript
            0x08026624, // disguise
            0x08026898, // lolaDie
            0x08026940, // "hit"
            0x08026A30, // lolaTakeHit
            0x08026A94, // lolaNetAttack
            0x08026B04, // lolaNetShake
            0x08026B64, // lolaNet
            0x08026BD4, // lolaRopeBreak
            0x08026C1C, // lolaRopeHit
            0x08026C84, // lolaRope
            0x08026CF0, // lolaProjDeflect
            0x08026D9C, // lolaProj
            0x08026E5C, // lolaProjPhoneDeflect
            0x08026EE4, // lolaProjPhone
            0x08026FE0, // lolaAttack
            0x08027094, // lolaDown
            0x080270CC, // lolaUp
            0x08027140, // "attack"
            0x08027230, // lolaStart
            0x080272D8, // anchorDrop
            0x08027394, // anchorWait
            0x08027400, // clampDrop
            0x0802745C, // clampWait
            0x080274D0, // frankieStop
            0x0802753C, // frankieDie
            0x080275F0, // "attack!"
            0x080276FC, // "wait"
            0x080277EC, // "swimmin"
            0x080278DC, // frankieSwim
            0x08027954, // frankieWait
            0x080279C0, // clampBottom
            0x080279EC, // donLinoStop
            0x08027A7C, // donLinoDie
            0x08027B48, // "speed"
            0x08027C50, // "speed"
            0x08027D74, // "updown"
            0x08027E78, // "attack!"
            0x08027F68, // "wait"
            0x08028058, // "main"
            0x08028148, // donLinoStart
            0x080281D8, // doAMove
            0x0802824C, // doBMove
            0x08028364, // promptTookAHit
            0x08028458, // promptMessenger
            0x080284AC, // ernieHigh
            0x08028500, // ernieLow
            0x08028554, // ernieSwipe
            0x080285B8, // ernieStart
            0x0802860C, // bernieSwipe
            0x08028670, // bernieStart
            0x080286C4, // dancerAMove
            0x080286F4, // dancerBMove
            0x08028728, // dancerLeftMove
            0x0802875C, // dancerRightMove
            0x08028790, // dancerUpMove
            0x080287C4, // dancerDownMove
            0x08028824, // "main"
            0x08028914, // dancerStart
            0x080297A8, // waitForPagedText
            0x08029824, // missing
            0x0802986C, // notFound
            0x080298B8, // level_1_1_intro
            0x080299D0, // level_1_2_intro
            0x08029A68, // level_1_2_hottip
            0x08029AEC, // text
            0x08029BDC, // level_1_4_intro
            0x08029C34, // level_2_1_intro
            0x08029C8C, // level_2_2_intro
            0x08029CE4, // level_2_3_intro
            0x08029D3C, // level_2_4_intro
            0x08029DA0, // dialog
            0x08029E90, // level_3_1a_intro
            0x08029EDC, // level_3_1_intro
            0x08029F38, // level_3_2_intro1
            0x08029F90, // level_3_2_intro
            0x0802A000, // dialog
            0x0802A0F0, // level_3_2_outro
            0x0802A13C, // level_3_3_outro
            0x0802A1DC, // level_4_1_intro
            0x0802A234, // level_4_2_intro
            0x0802A290, // dialog
            0x0802A380, // level_4_3a_intro
            0x0802A3CC, // level_4_3_intro
            0x0802A470, // level_4_3b_intro
            0x0802A4CC, // dialog
            0x0802A5BC, // level_4_4c_outro
            0x0802A60C, // level_1_4_defeat
            0x0802A668, // level_2_2_defeat
            0x0802A6C4, // level_3_4_defeat
            0x0802A71C, // foundLennyPaint
            0x0802A768, // foundLennyTie
            0x0802A7BC, // foundLennyRubberband
            0x0802A80C, // foundLennyToolbelt
            0x0802A854, // crazyJoe51
            0x0802A8A8, // crazyJoe58
            0x0802A8FC, // crazyJoe60
            0x0802A950, // crazyJoe61
            0x0802A9A4, // crazyJoe62
            0x0802A9F8, // crazyJoe63
            0x0802AA4C, // crazyJoe64
            0x0802AAA4, // dolphinCop52
            0x0802AAF8, // shortie1_53
            0x0802AB70, // shortie2_54
            0x0802ABE8, // shortie3_55
            0x0802AC84, // shortie1_56
            0x0802ACD8, // shortie2_57
            0x0802AD98, // shortie3_59
            0x0802AE30, // angie65
            0x08045698, // passivePatrolGoUp
            0x08045708, // passivePatrolGoDown
            0x08045778, // passivePatrolGoLeft
            0x080457EC, // passivePatrolGoRight
            0x0804586C, // passivePatrolStunned
            0x080458E0, // passivePatrolDie
            0x080459F4, // passivePatrolDieIfVisible
            0x08045A50, // passivePatrol
            0x08045B0C, // ""
            0x08045C08, // ""
            0x08045D10, // AIDie
            0x08045EE4, // hit
            0x08045FD4, // AITakeHit
            0x0804605C, // AITakeExplosion
            0x080460E8, // "attack!"
            0x080461D8, // "patrol"
            0x080462C8, // crabBouncerStationary
            0x08046364, // crabGoLeft
            0x080463AC, // crabGoRight
            0x0804640C, // goleft
            0x08046504, // goright
            0x080465F4, // crabMove
            0x0804666C, // crabAttack
            0x08046720, // crabTryAttack
            0x0804679C, // patrol
            0x0804688C, // crabBouncerWalker
            0x0804694C, // "faceLeft"
            0x08046A48, // "faceRight"
            0x08046B7C, // "pause"
            0x08046C6C, // returnToBase
            0x08046D7C, // "attack"
            0x08046E6C, // thugFishTrackPlayer
            0x08046F44, // jellyfishTrackPlayer
            0x08047028, // "farFromHome?"
            0x08047118, // "seek or return"
            0x08047208, // thugFishSeek
            0x080472F0, // "farFromHome?"
            0x080473E0, // "seek or return"
            0x080474D0, // jellyfishSeek
            0x080475CC, // patrolHorizontalTurnLeft
            0x08047668, // patrolHorizontalTurnRight
            0x080476EC, // sharkAttack
            0x080477FC, // "movin"
            0x08047928, // "sharkORdolphin?"
            0x08047A18, // "main"
            0x08047B08, // alertPatroller
            0x08047B44, // dolphinCop
            0x08047BE4, // shooterAttack
            0x08047D28, // shooterDude
            0x08047DF8, // ambusherDead
            0x08047E7C, // "attack"
            0x08047F6C, // ambusherDude
            0x08048000, // patrolVerticalUp
            0x08048070, // patrolVerticalDown
            0x080480D4, // octoWipeScript
            0x08048150, // washCrabScript
            0x080481D4, // racetrackJump
            0x08048274, // racetrackEnd
            0x080482E0, // "rand1"
            0x080483F0, // "rand2"
            0x080484E0, // "run"
            0x080485D0, // racersStart
            0x0804862C, // "rand1"
            0x08048734, // "rand2"
            0x08048824, // "run"
            0x08048914, // racersFasterStart
            0x08048978, // racetrackEnemyScript
            0x080489F0, // racetrackEnemyFasterScript
            0x08048A5C, // promptKilla
            0x08048A98, // promptDie
            0x08048B1C, // buttonARight
            0x08048B80, // buttonBRight
            0x08048BE4, // buttonLEFTRight
            0x08048C4C, // buttonRIGHTRight
            0x08048CB0, // buttonUPRight
            0x08048D14, // buttonDOWNRight
            0x08048D74, // buttonADown
            0x08048DD4, // buttonBDown
            0x08048E38, // buttonLEFTDown
            0x08048E9C, // buttonRIGHTDown
            0x08048F00, // buttonUPDown
            0x08048F64, // buttonDOWNDown
            0x0804AB64, // waitForFLCOrA
            0x0804ABE4, // movie_intro
            0x0804AD50, // movie_license
            0x0804ADD4, // movie_credits
            0x0804B074, // waitForPagedText
            0x0804B0F0, // CS_1_3a
            0x0804B20C, // CS_1_4a
            0x0804B280, // CS_1_4b
            0x0804B378, // CS_2_1a
            0x0804B404, // CS_2_2a
            0x0804B4A8, // CS_2_2b
            0x0804B5A0, // CS_2_3a
            0x0804B638, // CS_2_3b
            0x0804B724, // CS_2_4a
            0x0804B87C, // CS_2_4b
            0x0804B9EC, // CS_3_1a
            0x0804BA64, // CS_3_1aa
            0x0804BAE4, // CS_3_1b
            0x0804BB70, // CS_3_2a
            0x0804BBFC, // CS_3_2b
            0x0804BCB8, // CS_3_3a
            0x0804BD68, // CS_3_4a
            0x0804BEFC, // CS_3_4b
            0x0804BFC4, // CS_4_1a
            0x0804C068, // CS_4_1b
            0x0804C178, // CS_4_2a
            0x0804C324, // CS_4_2b
            0x0804C398, // CS_4_4a
            0x0804C538, // CS_4_4b
            0x0804C814, // CS_winRace
            0x0804C880, // CS_winShark
            0x0804C8EC, // CS_winDance
            0x0804C958, // CS_winWash
            0x0804CA1C, // talkToVictim
            0x0804CA7C, // dialogNPC
            0x0804CAD8, // followPlayer
            0x0804CB5C, // followPlayerTighter
            0x0804CBD8, // lenny
            0x0804CCC8, // tightFollow
            0x0804CD18, // lenny
            0x0804CE08, // lennyFollow
            0x0804CE84, // talkAndSwim
            0x0804CFA8, // "spray1"
            0x0804D0A4, // "spray2"
            0x0804D194, // "tag"
            0x0804D284, // shortieTagger
            0x0804D2EC, // talkAndDisappear
            0x0804D398, // "laugh"
            0x0804D488, // crazyJoe
            0x0804D4EC, // crazyJoeImplode
            0x0804D588, // swimFast
            0x0804D604, // shortieSwimmer
            0x0804ED64, // powerup
            0x0804EDDC, // healthSmall
            0x0804EE28, // healthMedium
            0x0804EE70, // healthLarge
            0x0804EEB8, // healthFull
            0x0804EEFC, // urchin
            0x0804EF48, // urchinTriple
            0x0804EF90, // sonarBomb
            0x0804EFD4, // inkBomb
            0x0804F018, // swarm
            0x0804F060, // extraTime
            0x0804F0A8, // extraPoints
            0x0804F0F4, // extraPoints200
            0x0804F140, // extraPoints500
            0x0804F184, // speedUp
            0x0804F1CC, // timeFreeze
            0x0804F970, // sonarBombProjectile
            0x0804FA68, // inkBombProjectile
            0x080501FC, // "doit"
            0x080502EC, // slaveAttack
            0x08050358, // slaveIdle
            0x080503EC, // masterScript
            0x08050470, // steamVent
            0x0805050C, // hurdleScript
            0x080505A0, // coralDestroyed
            0x08050608, // coralScript
            0x08050698, // destroyed
            0x08050778, // "takehit"
            0x08050868, // takeHit
            0x08050914, // destructible1
            0x080509AC, // destructibleCoral
            0x08050A34, // destructible2
        };

        public override uint ObjTypesPointer => 0x08004a68;
        public override int ObjTypesCount => 128;
    }
    public class GBAVV_SharkTaleJP_Manager : GBAVV_SharkTale_Manager
    {
        public override string[] Languages => new string[]
        {
            "Japanese"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0811585C,
            0x08115A04,
            0x08119314,
            0x0811C8BC,
            0x0811D340,
            0x08127208,
            0x0812EDC4,
            0x08132D74,
            0x08147E04,
            0x08149624,
            0x0814C1A0,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08025680, // script_waitForInputOrTime
            0x080264B8, // bananaScript
            0x080265C0, // disguise
            0x08026834, // lolaDie
            0x080268DC, // "hit"
            0x080269CC, // lolaTakeHit
            0x08026A30, // lolaNetAttack
            0x08026AA0, // lolaNetShake
            0x08026B00, // lolaNet
            0x08026B70, // lolaRopeBreak
            0x08026BB8, // lolaRopeHit
            0x08026C20, // lolaRope
            0x08026C8C, // lolaProjDeflect
            0x08026D38, // lolaProj
            0x08026DF8, // lolaProjPhoneDeflect
            0x08026E80, // lolaProjPhone
            0x08026F7C, // lolaAttack
            0x08027030, // lolaDown
            0x08027068, // lolaUp
            0x080270DC, // "attack"
            0x080271CC, // lolaStart
            0x08027274, // anchorDrop
            0x08027330, // anchorWait
            0x0802739C, // clampDrop
            0x080273F8, // clampWait
            0x0802746C, // frankieStop
            0x080274D8, // frankieDie
            0x0802758C, // "attack!"
            0x08027698, // "wait"
            0x08027788, // "swimmin"
            0x08027878, // frankieSwim
            0x080278F0, // frankieWait
            0x0802795C, // clampBottom
            0x08027988, // donLinoStop
            0x08027A18, // donLinoDie
            0x08027AE4, // "speed"
            0x08027BEC, // "speed"
            0x08027D10, // "updown"
            0x08027E14, // "attack!"
            0x08027F04, // "wait"
            0x08027FF4, // "main"
            0x080280E4, // donLinoStart
            0x08028174, // doAMove
            0x080281E8, // doBMove
            0x08028300, // promptTookAHit
            0x080283F4, // promptMessenger
            0x08028448, // ernieHigh
            0x0802849C, // ernieLow
            0x080284F0, // ernieSwipe
            0x08028554, // ernieStart
            0x080285A8, // bernieSwipe
            0x0802860C, // bernieStart
            0x08028660, // dancerAMove
            0x08028690, // dancerBMove
            0x080286C4, // dancerLeftMove
            0x080286F8, // dancerRightMove
            0x0802872C, // dancerUpMove
            0x08028760, // dancerDownMove
            0x080287C0, // "main"
            0x080288B0, // dancerStart
            0x08029744, // waitForPagedText
            0x080297C0, // missing
            0x08029808, // notFound
            0x08029854, // level_1_1_intro
            0x0802996C, // level_1_2_intro
            0x08029A04, // level_1_2_hottip
            0x08029A88, // text
            0x08029B78, // level_1_4_intro
            0x08029BD0, // level_2_1_intro
            0x08029C28, // level_2_2_intro
            0x08029C80, // level_2_3_intro
            0x08029CD8, // level_2_4_intro
            0x08029D3C, // dialog
            0x08029E2C, // level_3_1a_intro
            0x08029E78, // level_3_1_intro
            0x08029ED4, // level_3_2_intro1
            0x08029F2C, // level_3_2_intro
            0x08029F9C, // dialog
            0x0802A08C, // level_3_2_outro
            0x0802A0D8, // level_3_3_outro
            0x0802A178, // level_4_1_intro
            0x0802A1D0, // level_4_2_intro
            0x0802A22C, // dialog
            0x0802A31C, // level_4_3a_intro
            0x0802A368, // level_4_3_intro
            0x0802A40C, // level_4_3b_intro
            0x0802A468, // dialog
            0x0802A558, // level_4_4c_outro
            0x0802A5A8, // level_1_4_defeat
            0x0802A604, // level_2_2_defeat
            0x0802A660, // level_3_4_defeat
            0x0802A6B8, // foundLennyPaint
            0x0802A704, // foundLennyTie
            0x0802A758, // foundLennyRubberband
            0x0802A7A8, // foundLennyToolbelt
            0x0802A7F0, // crazyJoe51
            0x0802A844, // crazyJoe58
            0x0802A898, // crazyJoe60
            0x0802A8EC, // crazyJoe61
            0x0802A940, // crazyJoe62
            0x0802A994, // crazyJoe63
            0x0802A9E8, // crazyJoe64
            0x0802AA40, // dolphinCop52
            0x0802AA94, // shortie1_53
            0x0802AB0C, // shortie2_54
            0x0802AB84, // shortie3_55
            0x0802AC20, // shortie1_56
            0x0802AC74, // shortie2_57
            0x0802AD34, // shortie3_59
            0x0802ADCC, // angie65
            0x080438A8, // passivePatrolGoUp
            0x08043918, // passivePatrolGoDown
            0x08043988, // passivePatrolGoLeft
            0x080439FC, // passivePatrolGoRight
            0x08043A7C, // passivePatrolStunned
            0x08043AF0, // passivePatrolDie
            0x08043C04, // passivePatrolDieIfVisible
            0x08043C60, // passivePatrol
            0x08043D1C, // ""
            0x08043E18, // ""
            0x08043F20, // AIDie
            0x080440F4, // hit
            0x080441E4, // AITakeHit
            0x0804426C, // AITakeExplosion
            0x080442F8, // "attack!"
            0x080443E8, // "patrol"
            0x080444D8, // crabBouncerStationary
            0x08044574, // crabGoLeft
            0x080445BC, // crabGoRight
            0x0804461C, // goleft
            0x08044714, // goright
            0x08044804, // crabMove
            0x0804487C, // crabAttack
            0x08044930, // crabTryAttack
            0x080449AC, // patrol
            0x08044A9C, // crabBouncerWalker
            0x08044B5C, // "faceLeft"
            0x08044C58, // "faceRight"
            0x08044D8C, // "pause"
            0x08044E7C, // returnToBase
            0x08044F8C, // "attack"
            0x0804507C, // thugFishTrackPlayer
            0x08045154, // jellyfishTrackPlayer
            0x08045238, // "farFromHome?"
            0x08045328, // "seek or return"
            0x08045418, // thugFishSeek
            0x08045500, // "farFromHome?"
            0x080455F0, // "seek or return"
            0x080456E0, // jellyfishSeek
            0x080457DC, // patrolHorizontalTurnLeft
            0x08045878, // patrolHorizontalTurnRight
            0x080458FC, // sharkAttack
            0x08045A0C, // "movin"
            0x08045B38, // "sharkORdolphin?"
            0x08045C28, // "main"
            0x08045D18, // alertPatroller
            0x08045D54, // dolphinCop
            0x08045DF4, // shooterAttack
            0x08045F38, // shooterDude
            0x08046008, // ambusherDead
            0x0804608C, // "attack"
            0x0804617C, // ambusherDude
            0x08046210, // patrolVerticalUp
            0x08046280, // patrolVerticalDown
            0x080462E4, // octoWipeScript
            0x08046360, // washCrabScript
            0x080463E4, // racetrackJump
            0x08046484, // racetrackEnd
            0x080464F0, // "rand1"
            0x08046600, // "rand2"
            0x080466F0, // "run"
            0x080467E0, // racersStart
            0x0804683C, // "rand1"
            0x08046944, // "rand2"
            0x08046A34, // "run"
            0x08046B24, // racersFasterStart
            0x08046B88, // racetrackEnemyScript
            0x08046C00, // racetrackEnemyFasterScript
            0x08046C6C, // promptKilla
            0x08046CA8, // promptDie
            0x08046D2C, // buttonARight
            0x08046D90, // buttonBRight
            0x08046DF4, // buttonLEFTRight
            0x08046E5C, // buttonRIGHTRight
            0x08046EC0, // buttonUPRight
            0x08046F24, // buttonDOWNRight
            0x08046F84, // buttonADown
            0x08046FE4, // buttonBDown
            0x08047048, // buttonLEFTDown
            0x080470AC, // buttonRIGHTDown
            0x08047110, // buttonUPDown
            0x08047174, // buttonDOWNDown
            0x08048D74, // waitForFLCOrA
            0x08048DF4, // movie_intro
            0x08048F90, // movie_license
            0x08049014, // movie_credits
            0x080492B4, // waitForPagedText
            0x08049330, // CS_1_3a
            0x0804944C, // CS_1_4a
            0x080494C0, // CS_1_4b
            0x080495B8, // CS_2_1a
            0x08049644, // CS_2_2a
            0x080496E8, // CS_2_2b
            0x080497E0, // CS_2_3a
            0x08049878, // CS_2_3b
            0x08049964, // CS_2_4a
            0x08049ABC, // CS_2_4b
            0x08049C2C, // CS_3_1a
            0x08049CA4, // CS_3_1aa
            0x08049D24, // CS_3_1b
            0x08049DB0, // CS_3_2a
            0x08049E3C, // CS_3_2b
            0x08049EF8, // CS_3_3a
            0x08049FA8, // CS_3_4a
            0x0804A13C, // CS_3_4b
            0x0804A204, // CS_4_1a
            0x0804A2A8, // CS_4_1b
            0x0804A3B8, // CS_4_2a
            0x0804A564, // CS_4_2b
            0x0804A5D8, // CS_4_4a
            0x0804A778, // CS_4_4b
            0x0804AA54, // CS_winRace
            0x0804AAC0, // CS_winShark
            0x0804AB2C, // CS_winDance
            0x0804AB98, // CS_winWash
            0x0804AC5C, // talkToVictim
            0x0804ACBC, // dialogNPC
            0x0804AD18, // followPlayer
            0x0804AD9C, // followPlayerTighter
            0x0804AE18, // lenny
            0x0804AF08, // tightFollow
            0x0804AF58, // lenny
            0x0804B048, // lennyFollow
            0x0804B0C4, // talkAndSwim
            0x0804B1E8, // "spray1"
            0x0804B2E4, // "spray2"
            0x0804B3D4, // "tag"
            0x0804B4C4, // shortieTagger
            0x0804B52C, // talkAndDisappear
            0x0804B5D8, // "laugh"
            0x0804B6C8, // crazyJoe
            0x0804B72C, // crazyJoeImplode
            0x0804B7C8, // swimFast
            0x0804B844, // shortieSwimmer
            0x0804CFA4, // powerup
            0x0804D01C, // healthSmall
            0x0804D068, // healthMedium
            0x0804D0B0, // healthLarge
            0x0804D0F8, // healthFull
            0x0804D13C, // urchin
            0x0804D188, // urchinTriple
            0x0804D1D0, // sonarBomb
            0x0804D214, // inkBomb
            0x0804D258, // swarm
            0x0804D2A0, // extraTime
            0x0804D2E8, // extraPoints
            0x0804D334, // extraPoints200
            0x0804D380, // extraPoints500
            0x0804D3C4, // speedUp
            0x0804D40C, // timeFreeze
            0x0804DBB0, // sonarBombProjectile
            0x0804DCA8, // inkBombProjectile
            0x0804E43C, // "doit"
            0x0804E52C, // slaveAttack
            0x0804E598, // slaveIdle
            0x0804E62C, // masterScript
            0x0804E6B0, // steamVent
            0x0804E74C, // hurdleScript
            0x0804E7E0, // coralDestroyed
            0x0804E848, // coralScript
            0x0804E8D8, // destroyed
            0x0804E9B8, // "takehit"
            0x0804EAA8, // takeHit
            0x0804EB54, // destructible1
            0x0804EBEC, // destructibleCoral
            0x0804EC74, // destructible2
        };
    }
}