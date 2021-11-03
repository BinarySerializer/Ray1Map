using System.Collections.Generic;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Shrek2_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 30;

        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0831DDA4,
            0x08328294,
            0x0832A618,
            0x0832EB28,
            0x0832F380,
            0x0832FED0,
            0x08334304,
            0x08334890,
            0x08337308,
            0x0833CE78,
            0x0833D680,
            0x08342D40,
            0x08344BC0,
            0x0834AB54,
            0x0834F248,
            0x08355B60,
        };

        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [0501] = GBAVV_ScriptCommand.CommandType.Name,
            [0502] = GBAVV_ScriptCommand.CommandType.Script,
            [0506] = GBAVV_ScriptCommand.CommandType.Return,
            [0702] = GBAVV_ScriptCommand.CommandType.Dialog,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08025000, // genericNPC
            0x08025078, // leprechaunOneShot
            0x08025164, // fallDown
            0x080251E0, // talkToVictim
            0x08025410, // script_waitForInputOrTime
            0x0802558C, // notFound
            0x080258DC, // waitForPagedText
            0x08025944, // missing
            0x0802598C, // b1C1Intro
            0x08025A70, // b1C1Outro
            0x08025AC4, // b1C2Intro
            0x08025B18, // b1C2Outro
            0x08025BD8, // b1C3Intro
            0x08025CE0, // b1C3Outro
            0x08025D7C, // b1C4Intro
            0x08025E3C, // b1C4Outro
            0x08025EB4, // b1C5Intro
            0x08025F98, // b1C5Outro
            0x08025FEC, // b2C1Intro
            0x08026088, // b2C1Outro
            0x08026148, // b2C2Intro
            0x080261C0, // b2C2Outro
            0x08026238, // b2C3Intro
            0x080262D4, // b2C3Outro
            0x0802634C, // b2C4Intro
            0x080263C4, // b2C4Outro
            0x08026484, // b2C5Intro
            0x08026520, // b2C5Outro
            0x080265BC, // b3C1Intro
            0x0802667C, // b3C1Outro
            0x080266D0, // b3C2Intro
            0x0802676C, // b3C2Outro
            0x08026808, // b3C3Intro
            0x080268EC, // b3C3Outro
            0x08026964, // b3C4Intro
            0x08026A24, // b3C4Outro
            0x08026A78, // b3C5Intro
            0x08026B14, // b3C5Outro
            0x08026BB0, // b4C1Intro
            0x08026C70, // b4C1Outro
            0x08026D30, // b4C2Intro
            0x08026DCC, // b4C2Outro
            0x08026E8C, // b4C3Intro
            0x08026F28, // b4C3Outro
            0x08026FA0, // b4C4Intro
            0x08027018, // b4C4Outro
            0x080270B4, // b4C5Intro
            0x0802712C, // b4C5Outro
            0x08027180, // b5C1Intro
            0x0802721C, // b5C1Outro
            0x08027270, // b5C2Intro
            0x080272E8, // b5C2Outro
            0x08027360, // b5C3Intro
            0x080273FC, // b5C3Outro
            0x08027498, // b5C4Intro
            0x08027558, // b5C4Outro
            0x080275AC, // b5C5Intro
            0x08027648, // b5C5Outro
            0x080276C0, // fionaOgre11
            0x08027714, // bMouse11
            0x08027788, // pig11
            0x08027844, // pino12
            0x080278DC, // gbMan12
            0x08027930, // bMouse12
            0x080279EC, // pig212
            0x08027A60, // pig312
            0x08027AB4, // fionaOgre13
            0x08027B08, // mirror13
            0x08027B80, // fionaOgre14
            0x08027BD4, // levelText14
            0x08027C70, // mirror14
            0x08027CE8, // mirror22
            0x08027D84, // donkey31
            0x08027DD8, // donkey33
            0x08027E2C, // shrekOgre31
            0x08027EA4, // shrekOgre34
            0x08027EF4, // puss41
            0x08027F44, // steed41
            0x08027F94, // steed42
            0x0802800C, // levelText42
            0x08028134, // pino43
            0x080281A8, // pigA43
            0x0802821C, // pigB43
            0x08028290, // pigC43
            0x0802832C, // bMouse43
            0x08028380, // mirror44
            0x080283F8, // levelHelp42
            0x0802844C, // shrekH45
            0x0802849C, // steed53
            0x080284EC, // FGM5401
            0x0802853C, // FGM5402
            0x080285A0, // blahblah
            0x0802869C, // leprechaunShrek
            0x080286F0, // blahblah
            0x080287EC, // leprechaunDonkeyTrain
            0x0802883C, // blahblah
            0x0802892C, // leprechaunDonkey
            0x08028978, // blahblah
            0x08028A74, // leprechaunPuss
            0x08028AC4, // blahblah
            0x08028BC0, // leprechaunGinger
            0x08028C10, // blahblah
            0x08028D0C, // leprechaunShrekH
            0x0804A49C, // pussFireballDown
            0x0804A530, // impossiblePlatform
            0x0804A5BC, // fallingMouse
            0x0804A63C, // targetFairy
            0x0804A690, // targetPumpkin
            0x0804A6F8, // angryArrowElf
            0x0804A77C, // pussFireballUp
            0x0804A7DC, // pussFireball
            0x0804A860, // impossiblePlatformBounce
            0x0804A8CC, // right
            0x0804A9C4, // left
            0x0804AAD4, // fallingMouseBounce
            0x0804AB94, // movingTarget
            0x0804ABEC, // targetFairyHit
            0x0804AC24, // targetPumpkinHit
            0x0804AC9C, // angryArrowElfHitArrow
            0x0804AD20, // angryArrowElfHitCane
            0x0804ADFC, // shootPeriodically
            0x0804B59C, // waitForFLCOrA
            0x0804B638, // waitForPagedText
            0x0804C924, // powerupKeyDrop
            0x0804C97C, // powerupFloatDrop
            0x0804C9D4, // powerupFreezeDrop
            0x0804CA34, // powerupBombDrop
            0x0804CA98, // powerupShieldDrop
            0x0804CAF8, // powerupInvincibilityDrop
            0x0804CB54, // powerupInvisibilityDrop
            0x0804CC14, // fallUp
            0x0804CCA4, // fallDown
            0x0804CD78, // powerupKeyGrab
            0x0804CDFC, // powerupKey
            0x0804CE48, // powerupFloatGrab
            0x0804CED0, // powerupFloat
            0x0804CF1C, // powerupFreezeGrab
            0x0804CFB0, // powerupFreeze
            0x0804CFF8, // powerupBombGrab
            0x0804D084, // powerupBomb
            0x0804D0DC, // powerupShieldGrab
            0x0804D164, // powerupShield
            0x0804D1B8, // powerupInvincibilityGrab
            0x0804D248, // powerupInvincibility
            0x0804D298, // powerupInvisibilityGrab
            0x0804D324, // powerupInvisibility
            0x0804D37C, // powerupHealthLarge
            0x0804D42C, // powerupHealthSmall
            0x0804D4DC, // powerupHealthSuper
            0x0804D598, // powerupBombExplode
            0x0804DA24, // goRight
            0x0804DA88, // goDown
            0x0804DB98, // goLeft
            0x0804DBFC, // goUp
            0x0804DC84, // pumpkinPatrol
            0x0804DD74, // pumpkinScript
            0x0804DE28, // flyingPumpkinScriptH
            0x0804DEC4, // flyingPumpkinScriptV
            0x0804DF64, // heavyArmorKnight
            0x0804E040, // peasantPatrol
            0x0804E13C, // mainLoop
            0x0804E22C, // normalPeasant
            0x0804E2D8, // pumpkinPatrol
            0x0804E3C8, // mainLoop
            0x0804E4B8, // normalKnight
            0x0804E584, // mainLoop
            0x0804E680, // mainLoop
            0x0804E770, // crossbowElfScript
            0x0804E838, // mainLoop
            0x0804E928, // hazmatPatrol
            0x0804EA18, // mainLoop
            0x0804EB08, // hazmatElfScript
            0x0804EBE0, // spawn
            0x0804ECD0, // spawn
            0x0804EDC0, // pumpkinPatrol
            0x0804EEB0, // mainLoop
            0x0804EFA0, // peasantWithPumpkins
            0x0804EFE8, // spawn
            0x0804F0D8, // pumpkinPatrol
            0x0804F1C8, // mainLoop
            0x0804F2B8, // peasantWithVPumpkins
            0x0804F300, // spawn
            0x0804F3F0, // pumpkinPatrol
            0x0804F4E0, // mainLoop
            0x0804F5D0, // peasantWithHPumpkins
            0x0804F614, // spike
            0x0804F650, // waterSpike
            0x0804F68C, // mainLoop
            0x0804F77C, // angryHazmatElfScript
            0x0804F834, // mainLoop
            0x0804F924, // angryPeasant
            0x0804F9DC, // mainLoop
            0x0804FACC, // angryPeasantWithPumpkins
            0x0804FB18, // spawn
            0x0804FC08, // mainLoop
            0x0804FCF8, // angryPeasantWithVPumpkins
            0x0804FD44, // spawn
            0x0804FE34, // mainLoop
            0x0804FF24, // angryPeasantWithHPumpkins
            0x0804FF6C, // mainLoop
            0x0805005C, // angryCrossbowElfScript
            0x080500F0, // followNearestPlayer
            0x0805014C, // faceNearestPlayer
            0x080501D0, // handleWallBounces
            0x0805028C, // setupStandardHitScripts
            0x08050338, // gotoAttack
            0x0805043C, // gotoAttack
            0x0805052C, // patrolScript
            0x080505BC, // setupDefaultAIParams
            0x0805063C, // setupStaticAIExceptions
            0x080506DC, // knightDieIfVisible
            0x08050754, // shootArrow
            0x080507D8, // gotoAttack
            0x080508C8, // elfPatrolScript
            0x08050954, // shootSlugRay
            0x080509BC, // gotoAttack
            0x08050AAC, // hazmatPatrolScript
            0x08050B38, // throwPumpkin
            0x08050BC4, // throwVPumpkin
            0x08050C50, // throwHPumpkin
            0x08050CBC, // peasantSpawnerCommon
            0x08050D6C, // freeze
            0x08050E80, // pause
            0x08050F90, // die
            0x08051088, // ouch
            0x08051178, // AITakeButtDamage
            0x0805129C, // AITakeShrekOrDonkeyDamage
            0x08051360, // die
            0x08051450, // AITakePIBDamage
            0x08051518, // boom
            0x08051608, // AITakeGBDamage
            0x080516C8, // AIDieNow
            0x0805175C, // AIDieIfVisible
            0x080517D8, // setupUnawareHits
            0x08051878, // knightDieNow
            0x080518F8, // AITakeUnawareDamage
            0x08051A44, // fallingBananaScript
            0x08051AD8, // bananaScript
            0x08051B88, // fallDown
            0x08051C74, // crumblingStone
            0x08051CEC, // crumblingBrick
            0x08051D70, // breakableStoneScript
            0x08051DF8, // brickWall
            0x08051E70, // rockWall
            0x08051EF0, // doorLocked
            0x08051F5C, // pressureDoor
            0x08051FE8, // drawBridgeLever
            0x08052078, // crystalBall
            0x08052108, // drawBridge
            0x08052198, // acceptHit
            0x080521DC, // crumblingStoneCrumble
            0x08052260, // crumblingBrickCrumble
            0x080522EC, // DontTakeShrekDamage
            0x08052344, // brickWallBreak
            0x080523C0, // doorLockedOpen
            0x08052434, // pressureDoorOpen
            0x080524B8, // drawBridgeLeverOperate
            0x08052550, // crystalBallBreak
            0x080525E4, // drawBridgeUp
            0x0805263C, // drawBridgeDown
            0x08052714, // genericGrabbable
            0x080527F0, // idle
            0x080528E0, // bouncyMushroom
            0x08052994, // idle
            0x08052A84, // bouncyPlatform
            0x08052B38, // idle
            0x08052C28, // bouncyBranch
            0x08052C7C, // idle
            0x08052D6C, // bouncyBalloon
            0x08052DC0, // idle
            0x08052EB0, // bouncyBubble
            0x08052F00, // idle
            0x08052FF0, // bouncyPost
            0x08053078, // reactToKick
            0x080530DC, // reactToPickup
            0x0805310C, // reactToDrop
            0x08053140, // reactToSlugRay
            0x08053170, // bouncyMushroomBounce
            0x080531B8, // bouncyPlatformBounce
            0x080531FC, // bouncyBranchBounce
            0x08053240, // bouncyBalloonBounce
            0x08053284, // bouncyBubbleBounce
            0x080532C8, // bouncyPostBounce
            0x0805330C, // goRightMilk
            0x08053420, // spawnPumpkin
            0x08053510, // spawnerHatScript
            0x080535A4, // spawnPumpkin
            0x08053694, // minibossHatScript
            0x080536CC, // goLeftMilk
            0x08053768, // spawnMilkBomb
            0x08053858, // milkSpawnerScript
            0x080538DC, // spawnerDoorScript
            0x08053974, // killme
            0x080539D4, // killmeVisible
            0x08053A18, // spawnerHatFreeze
            0x08053A90, // spawnPumpkin
            0x08053B38, // spawnFallingCoin
            0x08053BC4, // waitRandom
            0x08053C54, // childKilled
        };
    }
}