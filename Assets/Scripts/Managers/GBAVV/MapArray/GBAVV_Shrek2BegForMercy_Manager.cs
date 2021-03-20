using System.Collections.Generic;

namespace R1Engine
{
    public class GBAVV_Shrek2BegForMercy_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 25;

        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x083566E4,
            0x08375DD8,
            0x083785FC,
            0x0837CCD4,
            0x0837D538,
            0x0837E40C,
            0x08382840,
            0x08385A14,
            0x0838B584,
            0x0838CB7C,
            0x08392308,
            0x08394218,
            0x0839F7AC,
            0x083A442C,
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
            0x0808BFD0, // genericNPC
            0x0808C034, // leprechaunOneShot
            0x0808C0E4, // fallDown
            0x0808C160, // talkToVictim
            0x0808C3CC, // script_waitForInputOrTime
            0x0808D45C, // notFound
            0x0808D67C, // waitForPagedText
            0x0808D6E4, // missing
            0x0808D72C, // A01Intro
            0x0808D780, // A02Intro
            0x0808D7D4, // A03Intro
            0x0808D828, // A04Intro
            0x0808D87C, // A05Intro
            0x0808D8D0, // B01Intro
            0x0808D948, // B02Intro
            0x0808D99C, // B03Intro
            0x0808DA14, // C01Intro
            0x0808DA68, // C02Intro
            0x0808DABC, // C03Intro
            0x0808DB10, // C04Intro
            0x0808DB64, // C05Intro
            0x0808DBB8, // D01Intro
            0x0808DC0C, // D04Intro
            0x0808DC60, // DbossIntro
            0x0808DD48, // dialogAMinigame
            0x0808DDA0, // dialogBMinigame
            0x0808DDF8, // dialogCMinigame
            0x0808DE50, // dialogDMinigame
            0x0808DEA0, // TreeA01
            0x0808DEF4, // MouseB01
            0x0808DF68, // TombB01
            0x0808DFE0, // SkeletonB01
            0x0808E07C, // SwordB01
            0x0808E118, // ShrekC01
            0x0808E1B0, // TreeD01
            0x0808E270, // StatueD01
            0x0808E30C, // StatueD02
            0x0808E35C, // MM001
            0x0808E3D0, // MM002
            0x0808E420, // MM003
            0x0808E470, // MM004
            0x0808E4E4, // MM005
            0x0808E534, // MM006
            0x0808E584, // MM007
            0x0808E5D4, // MM008
            0x0808E624, // MM009
            0x0808E674, // MM010
            0x0808E6C4, // MM011
            0x0808E714, // MM012
            0x0808E764, // MM013
            0x0808E7B4, // MM014
            0x0808E804, // MM015
            0x0808E854, // MM016
            0x0808E8A4, // MM017
            0x0808E8F4, // MM018
            0x0808E944, // MM019
            0x0808E994, // MM020
            0x0808E9E4, // MM021
            0x0808EA34, // MM022
            0x0808EA84, // MM023
            0x080AEED4, // pussFireballDown
            0x080AF398, // impossiblePlatform
            0x080AF424, // fallingMouse
            0x080AF4B4, // pussFireballUp
            0x080AF514, // shrekBouncerScript
            0x080AF58C, // impossiblePlatformBounce
            0x080AF5F8, // right
            0x080AF6F0, // left
            0x080AF800, // fallingMouseBounce
            0x080AF8A8, // shrekBounce
            0x080AF90C, // shrekDiveAttack
            0x080AF964, // movie_license
            0x080AFDE4, // movie_intro
            0x080AFFDC, // waitForPagedText
            0x080B0060, // waitForFLCOrA
            0x080B19C8, // powerupKey
            0x080B1AA4, // powerupFloat
            0x080B1B38, // powerupFreeze
            0x080B1BD8, // powerupBombItem
            0x080B1C14, // powerupBomb
            0x080B1C90, // powerupShield
            0x080B1D44, // powerupInvincibility
            0x080B1DD0, // powerupInvisibility
            0x080B1E70, // powerupHealthLarge
            0x080B1F20, // powerupHealthSmall
            0x080B1FD0, // powerupHealthSuper
            0x080B2098, // fallDown
            0x080B211C, // powerupGrab
            0x080B214C, // powerupDrop
            0x080B21B4, // powerupBombExplode
            0x080B27FC, // goRight
            0x080B2854, // goDown
            0x080B28B4, // spiderGoDown
            0x080B3F8C, // goLeft
            0x080B3FE4, // goUp
            0x080B405C, // heavyArmorKnight
            0x080B40E4, // pumpkinScript
            0x080B4168, // flyingPumpkinScriptH
            0x080B41EC, // flyingPumpkinScriptV
            0x080B4298, // minigamePumpkinScript
            0x080B43BC, // mainLoop
            0x080B44B8, // mainLoop
            0x080B45A8, // crossbowElfScript
            0x080B463C, // attack
            0x080B4750, // pumpkinPatrol
            0x080B4840, // mainLoop
            0x080B4930, // peasantWithVPumpkins
            0x080B4990, // attack
            0x080B4A80, // pumpkinPatrol
            0x080B4B70, // mainLoop
            0x080B4C60, // peasantWithHPumpkins
            0x080B4CC4, // attack
            0x080B4DB4, // pumpkinPatrol
            0x080B4EA4, // mainLoop
            0x080B4F94, // peasantWithPumpkins
            0x080B5004, // mainLoop
            0x080B50F4, // angryPeasantWithPumpkins
            0x080B516C, // spawn
            0x080B525C, // mainLoop
            0x080B534C, // angryPeasantWithVPumpkins
            0x080B53BC, // spawn
            0x080B54AC, // mainLoop
            0x080B559C, // angryPeasantWithHPumpkins
            0x080B5610, // mainLoop
            0x080B5700, // hazmatPatrol
            0x080B57F0, // mainLoop
            0x080B58E0, // hazmatElfScript
            0x080B594C, // mainLoop
            0x080B5A3C, // angryHazmatElfScript
            0x080B5AA8, // mainLoop
            0x080B5B98, // angryCrossbowElfScript
            0x080B5C1C, // pumpkinPatrol
            0x080B5D0C, // chaserScript
            0x080B5DAC, // mainLoop
            0x080B5E9C, // spikedCeilingScript
            0x080B5F2C, // sweeperMainLoop
            0x080B601C, // spikedSweeperScript
            0x080B6084, // spiderGoUp
            0x080B6124, // spiderScript
            0x080B6200, // jumpToward
            0x080B62FC, // jumpAway
            0x080B63EC, // mainLoop
            0x080B6508, // frogScript
            0x080B65CC, // appleScript
            0x080B6674, // arrowSpawnerScript
            0x080B6740, // setupDefaultAIParams
            0x080B67AC, // setupStaticAIExceptions
            0x080B6868, // visibleBlock
            0x080B6958, // beginAwe
            0x080B69B4, // endAwe
            0x080B69F0, // childKilled
            0x080B6A68, // setupStandardHitScripts
            0x080B6B14, // faceNearestPlayer
            0x080B6B98, // handleWallBounces
            0x080B6C00, // knightDieIfVisible
            0x080B6C48, // peasantSpawnerCommon
            0x080B6CD0, // die
            0x080B6DC8, // ouch
            0x080B6EB8, // killMinigamePumpkin
            0x080B6F8C, // shootArrow
            0x080B7028, // shootArrowDiagonal
            0x080B70B8, // gotoAttack
            0x080B71A8, // elfPatrolScript
            0x080B7220, // gotoAttack
            0x080B7310, // patrolScript
            0x080B739C, // throwVPumpkin
            0x080B7428, // throwHPumpkin
            0x080B74B4, // throwPumpkin
            0x080B7540, // shootSlugRay
            0x080B75A8, // hazmatPatrolScript
            0x080B7608, // pickUpItem
            0x080B7640, // spikedCeilingFall
            0x080B76E0, // spikedCeilingRise
            0x080B7754, // spiderDeathScript
            0x080B7808, // AIDieNow
            0x080B789C, // die
            0x080B798C, // AITakePIBDamage
            0x080B7A6C, // boom
            0x080B7B64, // pause
            0x080B7C54, // die
            0x080B7D44, // AITakeGBDamage
            0x080B7E24, // AITakeButtDamage
            0x080B7F60, // AITakeShrekOrDonkeyDamage
            0x080B8088, // setupUnawareHits
            0x080B8128, // knightDieNow
            0x080B81A8, // AITakeUnawareDamage
            0x080B82A8, // AIDieIfVisible
            0x080B8B00, // crumblingStone
            0x080B8B78, // crumblingBrick
            0x080B8BF0, // wall
            0x080B8C64, // doorLocked
            0x080B8CD4, // skeletonDoorLocked
            0x080B8D40, // pressureDoor
            0x080B8DD4, // drawBridgeLever
            0x080B8E78, // crystalBall
            0x080B8F14, // drawBridge
            0x080B8FB0, // cuteResponse
            0x080B8FE0, // acceptHit
            0x080B901C, // crumbleScript
            0x080B90C0, // DontTakeShrekDamage
            0x080B9118, // doorLockedOpen
            0x080B919C, // skeletonDoorLockedOpen
            0x080B9234, // pressureDoorOpen
            0x080B92B8, // drawBridgeLeverOperate
            0x080B935C, // crystalBallBreak
            0x080B93FC, // drawBridgeUp
            0x080B9454, // drawBridgeDown
            0x080B94A8, // deleteMe
            0x080B9AF4, // genericGrabbable
            0x080B9BD0, // idle
            0x080B9CC0, // bouncyMushroom
            0x080B9D74, // bouncyPlatform
            0x080B9E24, // bouncyPost
            0x080B9EAC, // reactToKick
            0x080B9F10, // reactToPickup
            0x080B9F40, // reactToDrop
            0x080B9F74, // reactToSlugRay
            0x080B9FA4, // bouncyMushroomBounce
            0x080B9FEC, // bouncyPlatformBounce
            0x080BA030, // bouncyPostBounce
            0x080BA074, // goRightMilk
            0x080BA4E0, // spawnPumpkin
            0x080BA5D0, // spawnerHatScript
            0x080BA65C, // goLeftMilk
            0x080BA6F8, // spawnMilkBomb
            0x080BA7E8, // milkSpawnerScript
            0x080BA870, // spawnerDoorScript
            0x080BA9A0, // spawnLeftBlock
            0x080BAAF0, // spawnRightBlock
            0x080BABF4, // spawnNotLeftBlock
            0x080BACE4, // beautifulStatueScript
            0x080BAE24, // batCaveScript
            0x080BAF08, // arrowSpawnerScript
            0x080BAFDC, // killme
            0x080BB028, // killmeVisible
            0x080BB064, // childKilled
            0x080BB090, // spawnerHatFreeze
            0x080BB730, // spawnPumpkin
            0x080BB820, // minibossHatScript
            0x080BB858, // spike
            0x080BB894, // waterSpike
            0x080BB8D8, // waitRandom
            0x080BB998, // spawnFallingCoin
            0x080BB9F4, // spawnPumpkin
        };
    }
}