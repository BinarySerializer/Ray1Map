using System.Collections.Generic;

namespace R1Engine
{
    public class GBAVV_ThatsSoRaven_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 193;

        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x08289AB8,
            0x0828B540,
            0x0828EF84,
            0x082A3B4C,
            0x082A3D30,
            0x082A40D8,
            0x082A5564,
            0x082A91D0,
            0x082B2408,
            0x082B911C,
            0x082C3614,
        };

        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [0801] = GBAVV_ScriptCommand.CommandType.Name,
            [0802] = GBAVV_ScriptCommand.CommandType.Script,
            [0806] = GBAVV_ScriptCommand.CommandType.Return,
            [1002] = GBAVV_ScriptCommand.CommandType.Dialog,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x0802F8E4, // healthNPC
            0x0802F92C, // genericNPC
            0x0802F968, // endLevelNPC
            0x0802F9B0, // seleneNPC
            0x0802FA54, // enoughStart
            0x0802FB20, // notEnoughStart
            0x0802FBD4, // start
            0x0802FCB4, // enough
            0x0802FD88, // notEnough
            0x0802FE3C, // mid
            0x0802FEF0, // midOrEnd
            0x0802FFA4, // dory3NPC
            0x08030044, // enoughStart
            0x080300F8, // start
            0x080301AC, // enough
            0x08030260, // mid
            0x08030314, // midOrEnd
            0x080303C8, // dory6NPC
            0x08030484, // enoughStart
            0x08030540, // notEnoughStart
            0x080305F4, // start
            0x080306BC, // enough
            0x08030784, // notEnough
            0x08030838, // mid
            0x080308EC, // midOrEnd
            0x080309A0, // horatio3NPC
            0x08030A40, // enoughStart
            0x08030AF4, // start
            0x08030BA8, // enough
            0x08030C5C, // mid
            0x08030D10, // midOrEnd
            0x08030DC4, // horatio4NPC
            0x08030E64, // enoughStart
            0x08030F18, // start
            0x08030FCC, // enough
            0x08031080, // mid
            0x08031134, // midOrEnd
            0x080311E8, // horatio5NPC
            0x080312A4, // enoughStart
            0x08031360, // notEnoughStart
            0x08031414, // start
            0x080314DC, // enough
            0x080315A4, // notEnough
            0x08031658, // mid
            0x0803170C, // midOrEnd
            0x080317C0, // freddie2NPC
            0x08031860, // enoughStart
            0x08031914, // start
            0x080319C8, // enough
            0x08031A7C, // mid
            0x08031B30, // midOrEnd
            0x08031BE4, // freddie4NPC
            0x08031C9C, // enoughStart
            0x08031D58, // notEnoughStart
            0x08031E0C, // start
            0x08031ED4, // enough
            0x08031F9C, // notEnough
            0x08032050, // mid
            0x08032104, // midOrEnd
            0x080321B8, // chelseaMallNPC
            0x0803227C, // enoughStart
            0x08032338, // notEnoughStart
            0x080323EC, // start
            0x080324B4, // enough
            0x0803257C, // notEnough
            0x08032630, // mid
            0x080326E4, // midOrEnd
            0x08032798, // chelseaProtestNPC
            0x08032844, // enoughStart
            0x08032900, // notEnoughStart
            0x080329B4, // start
            0x08032A7C, // enough
            0x08032B30, // mid
            0x08032BE4, // midOrEnd
            0x08032C98, // eddieProtestNPC
            0x08032D40, // enoughStart
            0x08032DFC, // notEnoughStart
            0x08032EB0, // start
            0x08032F78, // enough
            0x0803302C, // mid
            0x080330E0, // midOrEnd
            0x08033194, // student4ProtestNPC
            0x0803323C, // enoughStart
            0x080332F8, // notEnoughStart
            0x080333AC, // start
            0x08033474, // enough
            0x08033528, // mid
            0x080335DC, // midOrEnd
            0x08033690, // student5ProtestNPC
            0x08033738, // enoughStart
            0x080337F4, // notEnoughStart
            0x080338A8, // start
            0x08033970, // enough
            0x08033A24, // mid
            0x08033AD8, // midOrEnd
            0x08033B8C, // student6ProtestNPC
            0x08033C48, // enoughStart
            0x08033D04, // notEnoughStart
            0x08033DB8, // start
            0x08033E80, // enough
            0x08033F34, // mid
            0x08033FE8, // midOrEnd
            0x0803409C, // student7ProtestNPC
            0x08034158, // enoughStart
            0x08034214, // notEnoughStart
            0x080342C8, // start
            0x08034390, // enough
            0x08034458, // notEnough
            0x0803450C, // mid
            0x080345C0, // midOrEnd
            0x08034674, // chelseaSupplyNPC
            0x0803472C, // enoughStart
            0x080347E8, // notEnoughStart
            0x0803489C, // start
            0x08034964, // enough
            0x08034A2C, // notEnough
            0x08034AE0, // mid
            0x08034B94, // midOrEnd
            0x08034C48, // coryTokenNPC
            0x08034CD0, // talkToVictim
            0x08034D2C, // talkToVictimHealth
            0x08034D8C, // talkAndEnd
            0x08034DF8, // talkAndChange
            0x08034EA8, // questOver
            0x080351CC, // script_waitForInputOrTime
            0x08035370, // notFound
            0x08035784, // waitForPagedText
            0x080357EC, // missing
            0x08035840, // SeleneDone
            0x080358A0, // IntroE1L1
            0x08035900, // EddieE1L1
            0x080359C0, // ChelseaE1L1
            0x08035AE4, // Teacher1E1L1
            0x08035B48, // Student1E1L1
            0x08035C0C, // Student2E1L1
            0x08035C70, // DoryStartE1L1
            0x08035CD0, // DoryMidE1L1
            0x08035D30, // DoryEndE1L1
            0x08035DC0, // VisionE1L1
            0x08035E24, // Student3E1L1
            0x08035E88, // Student4E1L1
            0x08035EE8, // IntroE1L2
            0x08035F48, // ChelseaE1L2
            0x08035FA8, // EddieE1L2
            0x08036038, // SeleneE1L2
            0x08036128, // FailE1L2
            0x08036188, // OutroE1L2
            0x080361F0, // FindGiftsStartE1L3
            0x080362B8, // FindGiftsMidE1L3
            0x08036320, // FindGiftsEndE1L3
            0x080363B8, // HoratioStartE1L3
            0x0803641C, // HoratioMidE1L3
            0x08036480, // HoratioEndE1L3
            0x080364E0, // EddieE1L3
            0x08036540, // OutroE1L4
            0x08036604, // IntroEndE1L4
            0x08036664, // ChelseaE1L4
            0x080366FC, // FreddieStartE1L4
            0x080367C0, // FreddieMidE1L4
            0x08036824, // FreddieEndE1L4
            0x08036890, // chelseaProtestEndE2L1
            0x08036958, // eddieProtestEndE2L1
            0x08036A28, // protestStudent4StartE2L1
            0x08036A94, // protestStudent4MidE2L1
            0x08036B00, // protestStudent4EndE2L1
            0x08036B70, // protestStudent5StartE2L1
            0x08036BDC, // protestStudent5MidE2L1
            0x08036C48, // protestStudent5EndE2L1
            0x08036CB8, // protestStudent6StartE2L1
            0x08036D24, // protestStudent6MidE2L1
            0x08036D90, // protestStudent6EndE2L1
            0x08036E00, // protestStudent7StartE2L1
            0x08036E6C, // protestStudent7MidE2L1
            0x08036ED8, // protestStudent7EndE2L1
            0x08036F38, // introE2L1
            0x08036FA0, // FreddieStartE2L1
            0x08037094, // FreddieMidE2L1
            0x080370F8, // FreddieEndE2L1
            0x08037158, // OutroE2L2
            0x08037218, // SeleneE2L2
            0x080372E0, // HoratioStartE2L2
            0x08037374, // HoratioMidE2L2
            0x080373D8, // HoratioEndE2L2
            0x0803746C, // OutroEndE2L3
            0x080374CC, // HoratioE2L3
            0x0803758C, // ChelseaE2L3
            0x0803761C, // EddieE2L3
            0x080376AC, // SeleneE2L3
            0x08037740, // OutroEndE2L4
            0x080377A4, // DoryStartE2L4
            0x08037894, // DoryMidE2L4
            0x080378F4, // DoryEndE2L4
            0x08037954, // ChelseaE2L4
            0x080379E8, // OutroEndE3L1
            0x08037AA8, // IntroE3L1
            0x08037B08, // ChelseaE3L1
            0x08037BD0, // HoratioStartE3L1
            0x08037C34, // HoratioMidE3L1
            0x08037C98, // HoratioEndE3L1
            0x08037CFC, // OutroEndE3L2
            0x08037D5C, // ChelseaE3L2
            0x08037DEC, // EddieE3L2
            0x08037E80, // DoryStartE3L2
            0x08037F70, // DoryMidE3L2
            0x08037FD0, // DoryEndE3L2
            0x0803803C, // FindSuppliesStartE3L3
            0x080380A4, // FindSuppliesMidE3L3
            0x0803810C, // FindSuppliesEndE3L3
            0x0803816C, // EddieE3L3
            0x080381FC, // SeleneE3L3
            0x08038290, // OutroEndE3L4
            0x08038320, // EddieE3L4
            0x080383B0, // ChelseaE3L4
            0x08038478, // FreddieStartE3L4
            0x080384DC, // FreddieMidE3L4
            0x08038540, // FreddieEndE3L4
            0x080385A4, // OutroEndE4L1
            0x08038604, // ChelseaE4L1
            0x08038694, // EddieE4L1
            0x0803875C, // FreddieStartE4L1
            0x080387C0, // FreddieMidE4L1
            0x08038824, // FreddieEndE4L1
            0x0803888C, // tokenTrialStartE4L2
            0x08038954, // tokenTrialMidE4L2
            0x080389BC, // tokenTrialEndE4L2
            0x08038A1C, // DevonE4L2
            0x08038ADC, // NadineE4L2
            0x08038B44, // HoratioStartE4L2
            0x08038BA8, // HoratioMidE4L2
            0x08038C0C, // HoratioEndE4L2
            0x08038C6C, // DevonE4L3
            0x08038CFC, // CoryE4L3
            0x08038D60, // DoryStartE4L3
            0x08038DC0, // DoryMidE4L3
            0x08038E20, // DoryEndE4L3
            0x08038E80, // DevonE4L4
            0x08038F10, // CoryE4L4
            0x08038FD0, // SeleneE4L4
            0x08044CC0, // movie_credits
            0x08044D84, // CS_unlockable1
            0x08044EA8, // CS_unlockable2
            0x08044FCC, // CS_unlockable3
            0x080450F0, // CS_unlockable4
            0x08045214, // CS_unlockable5
            0x08045338, // CS_unlockable6
            0x0804545C, // CS_unlockable7
            0x08045580, // CS_unlockable8
            0x080456A4, // CS_unlockable9
            0x080457C8, // CS_unlockable10
            0x080458EC, // CS_unlockable11
            0x08045A10, // CS_unlockable12
            0x08045B34, // CS_unlockable13
            0x08045C58, // CS_unlockable14
            0x08045D7C, // CS_unlockable15
            0x08045EA0, // CS_unlockable16
            0x08046F68, // waitForPagedText
            0x08047BCC, // CDScript
            0x08047C68, // keyScript
            0x08047D14, // hallpassScript
            0x08047DB4, // hamburgerScript
            0x08047E54, // homeworkScript
            0x08047EF8, // arcadeTokenScript
            0x08047F9C, // presentBoxScript
            0x08048044, // schoolSuppliesScript
            0x080480E4, // SoapTowelScript
            0x08048184, // perfumeScript
            0x08048228, // waterBottleScript
            0x080482BC, // _obsoleteScript
            0x08049690, // avCartLeft
            0x08049770, // avCartRight
            0x08049824, // avCartLoop
            0x080498D8, // avCart
            0x0804999C, // stealthPatrolLeftWait
            0x08049A50, // stealthPatrolLeft
            0x08049B18, // stealthPatrolLeftWait
            0x08049BCC, // stealthPatrolRight
            0x08049C80, // stealthPatrolLoop
            0x08049D34, // stealthPatrol
            0x08049DAC, // hazardTrashCan
            0x08049E20, // brokenVent
            0x08049E9C, // shooterCupCake
            0x08049F78, // shooterPizza
            0x0804A058, // dodgeBallShooter
            0x0804A134, // badGirlShooter
            0x0804A258, // custodianDown
            0x0804A33C, // custodianUp
            0x0804A3F0, // custodianLoop
            0x0804A4A4, // custodian
            0x0804A4EC, // lawlerAggroChaserSeek
            0x0804A534, // nadineAggroChaserSeek
            0x0804A5D8, // "farFromHome?"
            0x0804A68C, // "seek or return"
            0x0804A740, // chaserSeek
            0x0804A7F0, // homeIn
            0x0804A8A4, // hawkSeek
            0x0804A934, // "farFromHome?"
            0x0804A9E8, // "seek or return"
            0x0804AA9C, // stinkySeek
            0x0804AB80, // barracudaLeft
            0x0804AC50, // barracudaRight
            0x0804AD04, // barracudaLoop
            0x0804ADB8, // barracuda
            0x0804AE34, // "chasing"
            0x0804AEE8, // bugs
            0x0804AF48, // avCartHit
            0x0804AFD0, // die
            0x0804B084, // attackOrDie
            0x0804B0DC, // "wait"
            0x0804B190, // stealthIdle
            0x0804B1F0, // brokenVentMore
            0x0804B258, // shooterPerfumeHit
            0x0804B2A4, // custodianHitUp
            0x0804B318, // custodianHitDown
            0x0804B388, // chaserWaterHit
            0x0804B3E4, // chaserPerfumeHit
            0x0804B45C, // "faceLeft"
            0x0804B51C, // "faceRight"
            0x0804B600, // "pause"
            0x0804B6B4, // returnToBase
            0x0804B77C, // chaserTrackPlayer
            0x0804B844, // "farFromHome?"
            0x0804B8F8, // "seek or return"
            0x0804B9AC, // aggressiveChaserSeek
            0x0804BA4C, // hawkTrackPlayer
            0x0804BAD8, // barracudaWaterHit
            0x0804BB4C, // barracudaPerfumeHit
            0x0804BBC4, // "stay stunned"
            0x0804BC78, // stunned
            0x0804BCCC, // brokenVentGone
            0x0804BD5C, // "attack"
            0x0804BE10, // aggressiveChaserTrackPlayer
            0x0804BEE0, // brokenVentQuit
            0x0804CC7C, // brickWall
            0x0804CCD4, // bgObjectSprite
            0x0804CD3C, // minigameWin
            0x0804CD94, // cheese
            0x0804CDFC, // sink
            0x0804CE6C, // dialogTrigger
            0x0804CEFC, // brickWallBreak
            0x0804CF5C, // cheeseEat
        };
    }
}