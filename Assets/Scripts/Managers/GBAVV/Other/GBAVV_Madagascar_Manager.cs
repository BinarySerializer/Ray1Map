using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public abstract class GBAVV_Madagascar_Manager : GBAVV_BaseManager
    {
        // Metadata
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, LevInfos.Length).ToArray()),
        });

        // Exports
        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_Madagascar>(GetROMFilePath, context, (s, x) => x.CurrentLevInfo = LevInfos[context.Settings.Level]);
        public override UniTask ExportCutscenesAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        // Load
        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM_Madagascar>(GetROMFilePath, context, (s, x) => x.CurrentLevInfo = LevInfos[context.Settings.Level]);
            
            //GenerateLevInfos(rom);

            return await LoadMap2DAsync(context, rom, rom.CurrentMap, false);
        }

        // Scripts
        public override Dictionary<int, GBAVV_ScriptCommand.CommandType> ScriptCommands => new Dictionary<int, GBAVV_ScriptCommand.CommandType>()
        {
            [1101] = GBAVV_ScriptCommand.CommandType.Name,
            [1106] = GBAVV_ScriptCommand.CommandType.Return,
            [0502] = GBAVV_ScriptCommand.CommandType.Dialog,
        };

        // Helpers
        public void GenerateLevInfos(GBAVV_ROM_Madagascar rom)
        {
            var str = new StringBuilder();

            for (int volumeIndex = 0; volumeIndex < rom.Volumes.Length; volumeIndex++)
            {
                var volume = rom.Volumes[volumeIndex];

                for (int levelIndex = 0; levelIndex < volume.LevelInfos.Length; levelIndex++)
                {
                    var level = volume.LevelInfos[levelIndex];

                    for (int mapIndex = 0; mapIndex < level.MapInfos.Length; mapIndex++)
                    {
                        str.AppendLine($"new LevInfo({volumeIndex}, {levelIndex}, {mapIndex}, \"{volume.VolumeName.DefaultString}\", \"{level.LevelName.DefaultString}\"),");
                    }
                }
            }


            str.ToString().CopyToClipboard();
        }

        // Levels
        public LevInfo[] LevInfos => Levels;
        public static LevInfo[] Levels => new LevInfo[]
        {
            new LevInfo(0, 0, 0, "Adventure", "Alex's Double Jump"),
            new LevInfo(0, 1, 0, "Adventure", "Marty's Run"),
            new LevInfo(0, 2, 0, "Adventure", "Melman's Hideout Dig"),
            new LevInfo(0, 3, 0, "Adventure", "Gloria's Swim"),
            new LevInfo(0, 4, 0, "Adventure", "Daily Grind"),
            new LevInfo(0, 4, 1, "Adventure", "Daily Grind"),
            new LevInfo(0, 4, 2, "Adventure", "Daily Grind"),
            new LevInfo(0, 5, 0, "Adventure", "Marty's Crawl"),
            new LevInfo(0, 6, 0, "Adventure", "Operation: Zoo Askew"),
            new LevInfo(0, 6, 1, "Adventure", "Operation: Zoo Askew"),
            new LevInfo(0, 6, 2, "Adventure", "Operation: Zoo Askew"),
            new LevInfo(0, 7, 0, "Adventure", "Subway Surfers"),
            new LevInfo(0, 7, 1, "Adventure", "Subway Surfers"),
            new LevInfo(0, 7, 2, "Adventure", "Subway Surfers"),
            new LevInfo(0, 8, 0, "Adventure", "Operation: Ship Sneak"),
            new LevInfo(0, 8, 1, "Adventure", "Operation: Ship Sneak"),
            new LevInfo(0, 8, 2, "Adventure", "Operation: Ship Sneak"),
            new LevInfo(0, 9, 0, "Adventure", "On the Beach"),
            new LevInfo(0, 9, 1, "Adventure", "On the Beach"),
            new LevInfo(0, 9, 2, "Adventure", "On the Beach"),
            new LevInfo(0, 10, 0, "Adventure", "Jungle Dub"),
            new LevInfo(0, 10, 1, "Adventure", "Jungle Dub"),
            new LevInfo(0, 10, 2, "Adventure", "Jungle Dub"),
            new LevInfo(0, 11, 0, "Adventure", "Lemur Rave"),
            new LevInfo(0, 12, 0, "Adventure", "Melman's Sneeze Jump"),
            new LevInfo(0, 13, 0, "Adventure", "Lemur Liberation"),
            new LevInfo(0, 13, 1, "Adventure", "Lemur Liberation"),
            new LevInfo(0, 13, 2, "Adventure", "Lemur Liberation"),
            new LevInfo(0, 14, 0, "Adventure", "Wild World"),
            new LevInfo(0, 14, 1, "Adventure", "Wild World"),
            new LevInfo(0, 14, 2, "Adventure", "Wild World"),
            new LevInfo(0, 15, 0, "Adventure", "Alex's Claws"),
            new LevInfo(0, 16, 0, "Adventure", "Alex's Escape"),
            new LevInfo(0, 16, 1, "Adventure", "Alex's Escape"),
            new LevInfo(0, 16, 2, "Adventure", "Alex's Escape"),
            new LevInfo(0, 17, 0, "Adventure", "Gloria's Dive"),
            new LevInfo(0, 18, 0, "Adventure", "Island Getaway"),
            new LevInfo(0, 18, 1, "Adventure", "Island Getaway"),
            new LevInfo(0, 18, 2, "Adventure", "Island Getaway"),
            new LevInfo(0, 19, 0, "Adventure", "Marty to the Rescue"),
            new LevInfo(0, 19, 1, "Adventure", "Marty to the Rescue"),
            new LevInfo(0, 19, 2, "Adventure", "Marty to the Rescue"),
            new LevInfo(0, 20, 0, "Adventure", "Alex's Claw Climb"),
            new LevInfo(0, 21, 0, "Adventure", "Foosa Lair"),
            new LevInfo(0, 21, 1, "Adventure", "Foosa Lair"),
            new LevInfo(0, 21, 2, "Adventure", "Foosa Lair"),
            new LevInfo(0, 22, 0, "Adventure", "Foosa Arena"),
            new LevInfo(1, 0, 0, "Bonus", "Marty's Crawl"),
            new LevInfo(1, 1, 0, "Bonus", "Operation: Zoo Askew"),
            new LevInfo(1, 1, 1, "Bonus", "Operation: Zoo Askew"),
            new LevInfo(1, 1, 2, "Bonus", "Operation: Zoo Askew"),
            new LevInfo(1, 2, 0, "Bonus", "Operation: Ship Sneak"),
            new LevInfo(1, 2, 1, "Bonus", "Operation: Ship Sneak"),
            new LevInfo(1, 2, 2, "Bonus", "Operation: Ship Sneak"),
            new LevInfo(1, 3, 0, "Bonus", "Lemur Rave"),
            new LevInfo(1, 4, 0, "Bonus", "Melman's Sneeze Jump"),
            new LevInfo(1, 5, 0, "Bonus", "Alex's Claws"),
            new LevInfo(1, 6, 0, "Bonus", "Gloria's Dive"),
            new LevInfo(1, 7, 0, "Bonus", "Alex's Claw Climb"),
        };

        public class LevInfo
        {
            public LevInfo(int volume, int level, int map, string volumeName, string levelName)
            {
                Volume = volume;
                Level = level;
                Map = map;
                VolumeName = volumeName;
                LevelName = levelName;
            }

            public int Volume { get; }
            public int Level { get; }
            public int Map { get; }
            public string VolumeName { get; }
            public string LevelName { get; }
            public string DisplayName => $"{VolumeName}: {LevelName} {Map}";
        }
    }
    public class GBAVV_MadagascarEU_Manager : GBAVV_Madagascar_Manager
    {
        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0816848C,
            0x08169DEC,
            0x0816A5E8,
            0x0816DF40,
            0x0816E280,
            0x0816E810,
            0x0816ED74,
            0x0816FB4C,
            0x08170F80,
            0x0817134C,
            0x081728F8,
            0x08173B20,
            0x0817463C,
            0x08174E2C,
            0x08176644,
            0x08176DFC,
            0x08178D9C,
            0x0817A088,
            0x0817B20C,
            0x0817B8DC,
            0x0817BE00,
            0x0817C1F0,
            0x08180B84,
            0x08181730,
            0x081817A0,
            0x08181810,
            0x08181880,
            0x081818F0,
            0x08181960,
            0x081819C8,
            0x08181A48,
            0x08181AC4,
            0x08183548,
            0x08184F84,
            0x08185340,
            0x08185650,
            0x08185900,
            0x08186844,
            0x0818A668,
            0x0818A9A0,
            0x08190270,
            0x081933B8,
            0x08194720,
            0x08195258,
            0x08195308,
            0x08199B90,
            0x0819AFCC,
            0x0819B970,
            0x0819BE38,
            0x0819C76C,
            0x0819D0A0,
            0x0819D440,
            0x0819E014,
            0x0819E730,
            0x0819F0B4,
            0x0819F4FC,
            0x081A011C,
            0x081A18E0,
            0x081A2BB0,
            0x081A3614,
            0x081A5DA8,
            0x081A913C,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08090A6C, // movie_license
            0x08090B04, // movie_credits
            0x08090BBC, // L05_Core1aPre
            0x08090DEC, // L05_Core1cPost
            0x08090E84, // L07_Stealth1aPre
            0x080910A4, // L08_Core2aPre
            0x080912C4, // L08_Core2cPost
            0x08091458, // L09_Stealth2aPre
            0x08091670, // L09_Stealth2cPost
            0x08091920, // L10_Core3aPre
            0x08091B40, // L11_Core4aPre
            0x08091CCC, // L12_RavePre
            0x08091EEC, // L15_Core6aPre
            0x0809207C, // L18_Core7aPre
            0x0809220C, // L21_Core8aPost
            0x08092398, // L25_BossPre
            0x0809260C, // L25_BossPost
            0x080928C8, // BL12_RavePre
            0x08092994, // movie_intro
            0x08092AF0, // waitForPagedText
            0x08092B60, // JpegPleaseWait
            0x08092BE4, // waitForFlcOrA
            0x080B45AC, // script_waitForInputOrTime
            0x080B51F0, // genericNPC
            0x080B5238, // activeNPC
            0x080B52B8, // penguinPointer
            0x080B5380, // fallDown
            0x080B53FC, // talkToVictim
            0x080B5458, // talkToVictimActive
            0x080B58D4, // invisibleCandySpawner
            0x080B5938, // genericGrass
            0x080B59B8, // genericBug
            0x080B5A40, // genericGrassIdle
            0x080B5A98, // genericGrassOpen
            0x080B5AE8, // genericGrassClose
            0x080B5B40, // genericBugPatrol
            0x080B5B9C, // genericBugWalkLeft
            0x080B5BF8, // genericBugWalkRight
            0x080B6174, // notFound
            0x080B64B4, // waitForPagedText
            0x080B651C, // missing
            0x080B6568, // L01_PC1_Intro
            0x080B65E4, // L01_PC1_IntroDS
            0x080B6660, // L01_PC1_Peng1
            0x080B66C4, // L01_PC1_Peng2
            0x080B674C, // L01_PC1_Peng3
            0x080B67A4, // penguinFailD
            0x080B67FC, // penguinFailDSub
            0x080B6854, // L01_PC1_Hint1
            0x080B68AC, // L01_PC1_Hint2
            0x080B6928, // L01_PC1_Hint2DS
            0x080B69A4, // L01_PC1_Hint3
            0x080B6A00, // SoftPlatformHintGBA
            0x080B6A5C, // SoftPlatformHintDS
            0x080B6AB4, // L02_PC2_Intro
            0x080B6B30, // L02_PC2_IntroDS
            0x080B6BAC, // L02_PC2_Hint
            0x080B6C28, // L02_PC2_HintDS
            0x080B6CA4, // L02_PC2_Outro
            0x080B6CFC, // L02_PC2_Fail
            0x080B6D54, // L03_PC3_Intro
            0x080B6DD0, // L03_PC3_Hint1
            0x080B6E28, // L03_PC3_Attack
            0x080B6EA8, // L03_PC3_AttackDS
            0x080B6F00, // L03_PC3_Outro
            0x080B6F58, // L04_PC4_Intro
            0x080B6FD4, // L04_PC4_Peng1
            0x080B7038, // L04_PC4_Peng2
            0x080B709C, // L04_PC4_Peng3
            0x080B70F4, // L04_PC4_Hint1
            0x080B7170, // L04_PC4_Hint1DS
            0x080B71F0, // L05_Core1_aIntro
            0x080B7294, // L05_Core1_aHint1
            0x080B72F0, // L05_Core1_aHint2
            0x080B7368, // SwitchGBA
            0x080B73BC, // SwitchDS
            0x080B7418, // L05_Core1_aHint2AGB
            0x080B749C, // L05_Core1_aBridgeHint
            0x080B74F8, // L05_Core1_aOutro
            0x080B7578, // L05_Core1_bIntro
            0x080B761C, // L05_Core1_bHint1
            0x080B7678, // L05_Core1_bHint2
            0x080B76F8, // L05_Core1_bOutro
            0x080B7754, // L05_Core1_cIntro
            0x080B781C, // L05_Core1_cOutro
            0x080B7898, // L06_PC5_Intro
            0x080B7914, // L06_PC5_Outro
            0x080B7970, // L07_Stealth1_Intro
            0x080B79CC, // L07_Stealth1_Hint1
            0x080B7A28, // L07_Stealth1_Hint2
            0x080B7A84, // L07_Stealth1_Outro
            0x080B7AE0, // L08_Core2_aIntro
            0x080B7BA4, // L08_Core2_aHint
            0x080B7C24, // L08_Core2_cOutro
            0x080B7CC8, // L09_Stealth2_Intro
            0x080B7D24, // L09_Stealth2_Hint
            0x080B7D80, // L09_Stealth2_Outro
            0x080B7E00, // L10_Core3_aIntro
            0x080B7E80, // L10_Core3_cOutro
            0x080B7F00, // L11_Core4_aIntro
            0x080B7F80, // L11_Core4_cOutro
            0x080B7FD8, // L13_PC6_Intro
            0x080B8054, // L13_PC6_IntroDS
            0x080B80D0, // L13_PC6_Fail
            0x080B812C, // L14_Core5_aIntro
            0x080B81F4, // L14_Core5_cOutro
            0x080B8250, // L15_Core6_aIntro
            0x080B82D0, // L15_Core6_cOutro
            0x080B8328, // L17_PC7_Intro
            0x080B83A4, // L17_PC7_IntroDS
            0x080B8420, // L17_PC7_Outro
            0x080B849C, // L17_PC7_Fail
            0x080B84F8, // L18_Core7_aIntro
            0x080B8554, // L18_Core7_cOutro
            0x080B85AC, // L20_PC7_Intro
            0x080B8628, // L20_PC7_Outro
            0x080B8684, // L21_Core8_aIntro
            0x080B8728, // L21_Core8_cOutro
            0x080B8784, // L22_Core9_aIntro
            0x080B8804, // L22_Core9_cOutro
            0x080B885C, // L23_PC9_Intro
            0x080B88DC, // L24_Core10_aIntro
            0x080B8938, // L24_Core10_cIntro
            0x080B89DC, // L24_Core10_bIntro
            0x080B8A38, // L24_Core10_cOutro
            0x080B8A90, // BonusLevelIntro
            0x080B8AEC, // BonusLevelIntroSub
            0x080B8B4C, // stealth
            0x080B8C3C, // stealthFailure
            0x080B8C98, // time
            0x080B8D88, // timeFailure
            0x080B8DE4, // waitForPagedTextSub
            0x080B9FB8, // lemurCollectable
            0x080BA020, // penguinCollectable
            0x080C0E70, // powerupHealthLarge
            0x080C0F18, // powerupHealthSmall
            0x080C0FD4, // fallDown
            0x080C121C, // goRight
            0x080C127C, // goRightWithSFX
            0x080C12E0, // goDown
            0x080C1348, // dropDown
            0x080C1C44, // goLeft
            0x080C1CA4, // goLeftWithSFX
            0x080C1D08, // goUp
            0x080C1D60, // climbUp
            0x080C1DF0, // groundBounce
            0x080C1EE0, // groundPatrolOneHPScript
            0x080C1F7C, // groundPatrolTwoHPScript
            0x080C2020, // groundBounce
            0x080C2110, // groundPatrolWithAttackScript
            0x080C21B4, // groundInvulnerablePatrolScript
            0x080C2254, // groundNoTerrainCollisonPatrolScript
            0x080C22F0, // airHBounce
            0x080C23E0, // airHOneHPPatrolScript
            0x080C2478, // airHBounce
            0x080C2568, // airHTwoHPDropperPatrolScript
            0x080C260C, // airHBounce
            0x080C26FC, // airHOneHPDropperPatrolScript
            0x080C27A0, // airHInvulnerablePatrolScript
            0x080C2818, // aBounce
            0x080C2908, // spiderLinearPatrolScript
            0x080C2980, // setupDefaultAIParams
            0x080C2A0C, // setupStandardHitScripts
            0x080C2ACC, // flipped
            0x080C2BC8, // notflipped
            0x080C2CB8, // blizzock
            0x080C2DD0, // attackPlayerIfCloseBy
            0x080C2E50, // handleWallBounces
            0x080C2ECC, // toRight
            0x080C2FC4, // toLeft
            0x080C30E8, // toAttack
            0x080C31D8, // handleWallBouncesWithAttacks
            0x080C327C, // toAttack
            0x080C336C, // handlePigeonWallBouncesWithAttacks
            0x080C3420, // spiderHandleFloorCeilBounces
            0x080C34B0, // die
            0x080C35A8, // ouch
            0x080C3698, // AITakeOneDamage
            0x080C3718, // AITakeSlashDamage
            0x080C37BC, // AITakeKickDamage
            0x080C3848, // AITakeJumpingDamage
            0x080C38F8, // AIDieNow
            0x080C398C, // AIPlummetNow
            0x080C3DF4, // bridge
            0x080C3E44, // softGround
            0x080C3EA8, // wall
            0x080C3EEC, // treeBridge
            0x080C3F58, // bridgeCrumble
            0x080C3FEC, // floorCrumble
            0x080C40AC, // wallCrumble
            0x080C4100, // bridgeDown
            0x080C44A0, // idle
            0x080C4590, // bouncyMushroom
            0x080C45F0, // bouncyPost
            0x080C4644, // bouncyFlower
            0x080C469C, // bouncyFlowerSimple
            0x080C46F0, // bouncyMushroomBounce
            0x080C4734, // bouncyPostBounce
            0x080C4778, // bouncyFlowerBounce
            0x080C4AE0, // spike
            0x080C4B14, // bushes
            0x080C4B50, // bushSquish
        };
    }
    public class GBAVV_MadagascarUS_Manager : GBAVV_Madagascar_Manager
    {
        public override string[] Languages => new string[]
        {
            "English"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0816848C,
            0x08169DEC,
            0x0816A5E8,
            0x0816DF40,
            0x0816E280,
            0x0816E810,
            0x0816ED74,
            0x0816FB4C,
            0x08170F80,
            0x0817134C,
            0x081728F8,
            0x08173B20,
            0x0817463C,
            0x08174E2C,
            0x08176644,
            0x08176DFC,
            0x08178D9C,
            0x0817A088,
            0x0817B20C,
            0x0817B8DC,
            0x0817BE00,
            0x0817C1F0,
            0x08180B84,
            0x08181730,
            0x081817A0,
            0x08181810,
            0x08181880,
            0x081818F0,
            0x08181960,
            0x081819C8,
            0x08181A48,
            0x08181AC4,
            0x08183548,
            0x08184F84,
            0x08185340,
            0x08185650,
            0x08185900,
            0x08186844,
            0x0818A668,
            0x0818A9A0,
            0x08190270,
            0x081933B8,
            0x08194720,
            0x08195258,
            0x08195308,
            0x08199B90,
            0x0819AFCC,
            0x0819B970,
            0x0819BE38,
            0x0819C76C,
            0x0819D0A0,
            0x0819D440,
            0x0819E014,
            0x0819E730,
            0x0819F0B4,
            0x0819F4FC,
            0x081A011C,
            0x081A18E0,
            0x081A2BB0,
            0x081A3614,
            0x081A5DA8,
            0x081A913C,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08090A6C, // movie_license
            0x08090B04, // movie_credits
            0x08090BBC, // L05_Core1aPre
            0x08090DEC, // L05_Core1cPost
            0x08090E84, // L07_Stealth1aPre
            0x080910A4, // L08_Core2aPre
            0x080912C4, // L08_Core2cPost
            0x08091458, // L09_Stealth2aPre
            0x08091670, // L09_Stealth2cPost
            0x08091920, // L10_Core3aPre
            0x08091B40, // L11_Core4aPre
            0x08091CCC, // L12_RavePre
            0x08091EEC, // L15_Core6aPre
            0x0809207C, // L18_Core7aPre
            0x0809220C, // L21_Core8aPost
            0x08092398, // L25_BossPre
            0x0809260C, // L25_BossPost
            0x080928C8, // BL12_RavePre
            0x08092994, // movie_intro
            0x08092AF0, // waitForPagedText
            0x08092B60, // JpegPleaseWait
            0x08092BE4, // waitForFlcOrA
            0x080B45AC, // script_waitForInputOrTime
            0x080B51F0, // genericNPC
            0x080B5238, // activeNPC
            0x080B52B8, // penguinPointer
            0x080B5380, // fallDown
            0x080B53FC, // talkToVictim
            0x080B5458, // talkToVictimActive
            0x080B58D4, // invisibleCandySpawner
            0x080B5938, // genericGrass
            0x080B59B8, // genericBug
            0x080B5A40, // genericGrassIdle
            0x080B5A98, // genericGrassOpen
            0x080B5AE8, // genericGrassClose
            0x080B5B40, // genericBugPatrol
            0x080B5B9C, // genericBugWalkLeft
            0x080B5BF8, // genericBugWalkRight
            0x080B6174, // notFound
            0x080B64B4, // waitForPagedText
            0x080B651C, // missing
            0x080B6568, // L01_PC1_Intro
            0x080B65E4, // L01_PC1_IntroDS
            0x080B6660, // L01_PC1_Peng1
            0x080B66C4, // L01_PC1_Peng2
            0x080B674C, // L01_PC1_Peng3
            0x080B67A4, // penguinFailD
            0x080B67FC, // penguinFailDSub
            0x080B6854, // L01_PC1_Hint1
            0x080B68AC, // L01_PC1_Hint2
            0x080B6928, // L01_PC1_Hint2DS
            0x080B69A4, // L01_PC1_Hint3
            0x080B6A00, // SoftPlatformHintGBA
            0x080B6A5C, // SoftPlatformHintDS
            0x080B6AB4, // L02_PC2_Intro
            0x080B6B30, // L02_PC2_IntroDS
            0x080B6BAC, // L02_PC2_Hint
            0x080B6C28, // L02_PC2_HintDS
            0x080B6CA4, // L02_PC2_Outro
            0x080B6CFC, // L02_PC2_Fail
            0x080B6D54, // L03_PC3_Intro
            0x080B6DD0, // L03_PC3_Hint1
            0x080B6E28, // L03_PC3_Attack
            0x080B6EA8, // L03_PC3_AttackDS
            0x080B6F00, // L03_PC3_Outro
            0x080B6F58, // L04_PC4_Intro
            0x080B6FD4, // L04_PC4_Peng1
            0x080B7038, // L04_PC4_Peng2
            0x080B709C, // L04_PC4_Peng3
            0x080B70F4, // L04_PC4_Hint1
            0x080B7170, // L04_PC4_Hint1DS
            0x080B71F0, // L05_Core1_aIntro
            0x080B7294, // L05_Core1_aHint1
            0x080B72F0, // L05_Core1_aHint2
            0x080B7368, // SwitchGBA
            0x080B73BC, // SwitchDS
            0x080B7418, // L05_Core1_aHint2AGB
            0x080B749C, // L05_Core1_aBridgeHint
            0x080B74F8, // L05_Core1_aOutro
            0x080B7578, // L05_Core1_bIntro
            0x080B761C, // L05_Core1_bHint1
            0x080B7678, // L05_Core1_bHint2
            0x080B76F8, // L05_Core1_bOutro
            0x080B7754, // L05_Core1_cIntro
            0x080B781C, // L05_Core1_cOutro
            0x080B7898, // L06_PC5_Intro
            0x080B7914, // L06_PC5_Outro
            0x080B7970, // L07_Stealth1_Intro
            0x080B79CC, // L07_Stealth1_Hint1
            0x080B7A28, // L07_Stealth1_Hint2
            0x080B7A84, // L07_Stealth1_Outro
            0x080B7AE0, // L08_Core2_aIntro
            0x080B7BA4, // L08_Core2_aHint
            0x080B7C24, // L08_Core2_cOutro
            0x080B7CC8, // L09_Stealth2_Intro
            0x080B7D24, // L09_Stealth2_Hint
            0x080B7D80, // L09_Stealth2_Outro
            0x080B7E00, // L10_Core3_aIntro
            0x080B7E80, // L10_Core3_cOutro
            0x080B7F00, // L11_Core4_aIntro
            0x080B7F80, // L11_Core4_cOutro
            0x080B7FD8, // L13_PC6_Intro
            0x080B8054, // L13_PC6_IntroDS
            0x080B80D0, // L13_PC6_Fail
            0x080B812C, // L14_Core5_aIntro
            0x080B81F4, // L14_Core5_cOutro
            0x080B8250, // L15_Core6_aIntro
            0x080B82D0, // L15_Core6_cOutro
            0x080B8328, // L17_PC7_Intro
            0x080B83A4, // L17_PC7_IntroDS
            0x080B8420, // L17_PC7_Outro
            0x080B849C, // L17_PC7_Fail
            0x080B84F8, // L18_Core7_aIntro
            0x080B8554, // L18_Core7_cOutro
            0x080B85AC, // L20_PC7_Intro
            0x080B8628, // L20_PC7_Outro
            0x080B8684, // L21_Core8_aIntro
            0x080B8728, // L21_Core8_cOutro
            0x080B8784, // L22_Core9_aIntro
            0x080B8804, // L22_Core9_cOutro
            0x080B885C, // L23_PC9_Intro
            0x080B88DC, // L24_Core10_aIntro
            0x080B8938, // L24_Core10_cIntro
            0x080B89DC, // L24_Core10_bIntro
            0x080B8A38, // L24_Core10_cOutro
            0x080B8A90, // BonusLevelIntro
            0x080B8AEC, // BonusLevelIntroSub
            0x080B8B4C, // stealth
            0x080B8C3C, // stealthFailure
            0x080B8C98, // time
            0x080B8D88, // timeFailure
            0x080B8DE4, // waitForPagedTextSub
            0x080B9FB8, // lemurCollectable
            0x080BA020, // penguinCollectable
            0x080C0E70, // powerupHealthLarge
            0x080C0F18, // powerupHealthSmall
            0x080C0FD4, // fallDown
            0x080C121C, // goRight
            0x080C127C, // goRightWithSFX
            0x080C12E0, // goDown
            0x080C1348, // dropDown
            0x080C1C44, // goLeft
            0x080C1CA4, // goLeftWithSFX
            0x080C1D08, // goUp
            0x080C1D60, // climbUp
            0x080C1DF0, // groundBounce
            0x080C1EE0, // groundPatrolOneHPScript
            0x080C1F7C, // groundPatrolTwoHPScript
            0x080C2020, // groundBounce
            0x080C2110, // groundPatrolWithAttackScript
            0x080C21B4, // groundInvulnerablePatrolScript
            0x080C2254, // groundNoTerrainCollisonPatrolScript
            0x080C22F0, // airHBounce
            0x080C23E0, // airHOneHPPatrolScript
            0x080C2478, // airHBounce
            0x080C2568, // airHTwoHPDropperPatrolScript
            0x080C260C, // airHBounce
            0x080C26FC, // airHOneHPDropperPatrolScript
            0x080C27A0, // airHInvulnerablePatrolScript
            0x080C2818, // aBounce
            0x080C2908, // spiderLinearPatrolScript
            0x080C2980, // setupDefaultAIParams
            0x080C2A0C, // setupStandardHitScripts
            0x080C2ACC, // flipped
            0x080C2BC8, // notflipped
            0x080C2CB8, // blizzock
            0x080C2DD0, // attackPlayerIfCloseBy
            0x080C2E50, // handleWallBounces
            0x080C2ECC, // toRight
            0x080C2FC4, // toLeft
            0x080C30E8, // toAttack
            0x080C31D8, // handleWallBouncesWithAttacks
            0x080C327C, // toAttack
            0x080C336C, // handlePigeonWallBouncesWithAttacks
            0x080C3420, // spiderHandleFloorCeilBounces
            0x080C34B0, // die
            0x080C35A8, // ouch
            0x080C3698, // AITakeOneDamage
            0x080C3718, // AITakeSlashDamage
            0x080C37BC, // AITakeKickDamage
            0x080C3848, // AITakeJumpingDamage
            0x080C38F8, // AIDieNow
            0x080C398C, // AIPlummetNow
            0x080C3DF4, // bridge
            0x080C3E44, // softGround
            0x080C3EA8, // wall
            0x080C3EEC, // treeBridge
            0x080C3F58, // bridgeCrumble
            0x080C3FEC, // floorCrumble
            0x080C40AC, // wallCrumble
            0x080C4100, // bridgeDown
            0x080C44A0, // idle
            0x080C4590, // bouncyMushroom
            0x080C45F0, // bouncyPost
            0x080C4644, // bouncyFlower
            0x080C469C, // bouncyFlowerSimple
            0x080C46F0, // bouncyMushroomBounce
            0x080C4734, // bouncyPostBounce
            0x080C4778, // bouncyFlowerBounce
            0x080C4AE0, // spike
            0x080C4B14, // bushes
            0x080C4B50, // bushSquish
        };
    }
    public class GBAVV_MadagascarJP_Manager : GBAVV_Madagascar_Manager
    {
        public override string[] Languages => new string[]
        {
            "Japanese"
        };

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x08167A44,
            0x081693A4,
            0x08169C94,
            0x0816D5EC,
            0x0816D92C,
            0x0816DEBC,
            0x0816E420,
            0x0816F1F8,
            0x0817062C,
            0x08170740,
            0x08170B0C,
            0x08170C60,
            0x08170D18,
            0x08170DD0,
            0x0817237C,
            0x08172444,
            0x0817366C,
            0x08174188,
            0x08174978,
            0x08175CE4,
            0x0817649C,
            0x08178230,
            0x0817951C,
            0x0817A6A0,
            0x0817AD70,
            0x0817B294,
            0x0817B684,
            0x08180018,
            0x08180BC4,
            0x08180C34,
            0x08180CA4,
            0x08180D14,
            0x08180D84,
            0x08180DF4,
            0x08180E5C,
            0x08180EDC,
            0x08180F58,
            0x081829DC,
            0x08184418,
            0x081847D4,
            0x08184AE4,
            0x08184D94,
            0x08185CD8,
            0x08189AFC,
            0x08189E34,
            0x0818F704,
            0x08193498,
            0x08194800,
            0x08195338,
            0x081953E8,
            0x08199C70,
            0x0819B0AC,
            0x0819BA50,
            0x0819BF18,
            0x0819D164,
            0x0819D504,
            0x0819E0D8,
            0x0819E7F4,
            0x0819F178,
            0x0819F5C0,
            0x081A01E0,
            0x081A13EC,
            0x081A26BC,
            0x081A3120,
            0x081A58B4,
            0x081A8C48,
        };

        public override uint[] ScriptPointers => new uint[]
        {
            0x08091524, // movie_license
            0x080915EC, // movie_credits
            0x080916A4, // L05_Core1aPre
            0x080918D4, // L05_Core1cPost
            0x0809196C, // L07_Stealth1aPre
            0x08091B8C, // L08_Core2aPre
            0x08091DAC, // L08_Core2cPost
            0x08091F40, // L09_Stealth2aPre
            0x08092158, // L09_Stealth2cPost
            0x08092408, // L10_Core3aPre
            0x08092628, // L11_Core4aPre
            0x080927B4, // L12_RavePre
            0x080929D4, // L15_Core6aPre
            0x08092B64, // L18_Core7aPre
            0x08092CF4, // L21_Core8aPost
            0x08092E80, // L25_BossPre
            0x080930F4, // L25_BossPost
            0x080933B0, // BL12_RavePre
            0x0809347C, // movie_intro
            0x080935D8, // waitForPagedText
            0x08093648, // JpegPleaseWait
            0x080936CC, // waitForFlcOrA
            0x080B5094, // script_waitForInputOrTime
            0x080B5CD8, // genericNPC
            0x080B5D20, // activeNPC
            0x080B5DA0, // penguinPointer
            0x080B5E68, // fallDown
            0x080B5EE4, // talkToVictim
            0x080B5F40, // talkToVictimActive
            0x080B67F4, // invisibleCandySpawner
            0x080B6858, // genericGrass
            0x080B68B8, // genericBush
            0x080B694C, // supriseWindow
            0x080B6A1C, // genericWindow
            0x080B6ADC, // genericBug
            0x080B6B54, // invulnerableBugPatrol
            0x080B6BA4, // genericGrassIdle
            0x080B6BE8, // genericGrassOpen
            0x080B6C38, // genericGrassClose
            0x080B6C78, // genericBushIdle
            0x080B6CBC, // genericBushTouched
            0x080B6D0C, // genericBugWalkLeft
            0x080B6D68, // genericBugWalkRight
            0x080B6DD8, // genericBugPatrol
            0x080B7354, // notFound
            0x080B7694, // waitForPagedText
            0x080B76FC, // missing
            0x080B7748, // L01_PC1_Intro
            0x080B77C4, // L01_PC1_IntroDS
            0x080B7840, // L01_PC1_Peng1
            0x080B78A4, // L01_PC1_Peng2
            0x080B792C, // L01_PC1_Peng3
            0x080B7984, // penguinFailD
            0x080B79DC, // penguinFailDSub
            0x080B7A34, // L01_PC1_Hint1
            0x080B7A8C, // L01_PC1_Hint2
            0x080B7B08, // L01_PC1_Hint2DS
            0x080B7B84, // L01_PC1_Hint3
            0x080B7BE0, // SoftPlatformHintGBA
            0x080B7C3C, // SoftPlatformHintDS
            0x080B7C94, // L02_PC2_Intro
            0x080B7D10, // L02_PC2_IntroDS
            0x080B7D8C, // L02_PC2_Hint
            0x080B7E08, // L02_PC2_HintDS
            0x080B7E84, // L02_PC2_Outro
            0x080B7EDC, // L02_PC2_Fail
            0x080B7F34, // L03_PC3_Intro
            0x080B7FB0, // L03_PC3_Hint1
            0x080B8008, // L03_PC3_Attack
            0x080B8088, // L03_PC3_AttackDS
            0x080B80E0, // L03_PC3_Outro
            0x080B8138, // L04_PC4_Intro
            0x080B81B4, // L04_PC4_Peng1
            0x080B8218, // L04_PC4_Peng2
            0x080B827C, // L04_PC4_Peng3
            0x080B82D4, // L04_PC4_Hint1
            0x080B8350, // L04_PC4_Hint1DS
            0x080B83D0, // L05_Core1_aIntro
            0x080B8474, // L05_Core1_aHint1
            0x080B84D0, // L05_Core1_aHint2
            0x080B8548, // SwitchGBA
            0x080B859C, // SwitchDS
            0x080B85F8, // L05_Core1_aHint2AGB
            0x080B867C, // L05_Core1_aBridgeHint
            0x080B86D8, // L05_Core1_aOutro
            0x080B8758, // L05_Core1_bIntro
            0x080B87FC, // L05_Core1_bHint1
            0x080B8858, // L05_Core1_bHint2
            0x080B88D8, // L05_Core1_bOutro
            0x080B8934, // L05_Core1_cIntro
            0x080B89FC, // L05_Core1_cOutro
            0x080B8A78, // L06_PC5_Intro
            0x080B8AF4, // L06_PC5_Outro
            0x080B8B50, // L07_Stealth1_Intro
            0x080B8BAC, // L07_Stealth1_Hint1
            0x080B8C08, // L07_Stealth1_Hint2
            0x080B8C64, // L07_Stealth1_Outro
            0x080B8CC0, // L08_Core2_aIntro
            0x080B8D84, // L08_Core2_aHint
            0x080B8E04, // L08_Core2_cOutro
            0x080B8EA8, // L09_Stealth2_Intro
            0x080B8F04, // L09_Stealth2_Hint
            0x080B8F60, // L09_Stealth2_Outro
            0x080B8FE0, // L10_Core3_aIntro
            0x080B9060, // L10_Core3_cOutro
            0x080B90E0, // L11_Core4_aIntro
            0x080B9160, // L11_Core4_cOutro
            0x080B91B8, // L13_PC6_Intro
            0x080B9234, // L13_PC6_IntroDS
            0x080B92B0, // L13_PC6_Fail
            0x080B930C, // L14_Core5_aIntro
            0x080B93D4, // L14_Core5_cOutro
            0x080B9430, // L15_Core6_aIntro
            0x080B94B0, // L15_Core6_cOutro
            0x080B9508, // L17_PC7_Intro
            0x080B9584, // L17_PC7_IntroDS
            0x080B9600, // L17_PC7_Outro
            0x080B967C, // L17_PC7_Fail
            0x080B96D8, // L18_Core7_aIntro
            0x080B9734, // L18_Core7_cOutro
            0x080B978C, // L20_PC7_Intro
            0x080B9808, // L20_PC7_Outro
            0x080B9864, // L21_Core8_aIntro
            0x080B9908, // L21_Core8_cOutro
            0x080B9964, // L22_Core9_aIntro
            0x080B99E4, // L22_Core9_cOutro
            0x080B9A3C, // L23_PC9_Intro
            0x080B9ABC, // L24_Core10_aIntro
            0x080B9B18, // L24_Core10_cIntro
            0x080B9BBC, // L24_Core10_bIntro
            0x080B9C18, // L24_Core10_cOutro
            0x080B9C70, // BonusLevelIntro
            0x080B9CCC, // BonusLevelIntroSub
            0x080B9D2C, // stealth
            0x080B9E1C, // stealthFailure
            0x080B9E78, // time
            0x080B9F68, // timeFailure
            0x080B9FC4, // waitForPagedTextSub
            0x080BB198, // lemurCollectable
            0x080BB200, // penguinCollectable
            0x080C02C0, // powerupHealthLarge
            0x080C0368, // powerupHealthSmall
            0x080C0424, // fallDown
            0x080C06DC, // goRight
            0x080C073C, // goRightWithSFX
            0x080C07A0, // goDown
            0x080C0808, // dropDown
            0x080C1104, // goLeft
            0x080C1164, // goLeftWithSFX
            0x080C11C8, // goUp
            0x080C1220, // climbUp
            0x080C12B0, // groundBounce
            0x080C13A0, // groundPatrolOneHPScript
            0x080C143C, // groundPatrolTwoHPScript
            0x080C14E0, // groundBounce
            0x080C15D0, // groundPatrolWithAttackScript
            0x080C1674, // groundInvulnerablePatrolScript
            0x080C1714, // groundNoTerrainCollisonPatrolScript
            0x080C17B0, // airHBounce
            0x080C18A0, // airHOneHPPatrolScript
            0x080C1938, // airHBounce
            0x080C1A28, // airHTwoHPDropperPatrolScript
            0x080C1ACC, // airHBounce
            0x080C1BBC, // airHOneHPDropperPatrolScript
            0x080C1C60, // airHInvulnerablePatrolScript
            0x080C1CD8, // aBounce
            0x080C1DC8, // spiderLinearPatrolScript
            0x080C1E40, // setupDefaultAIParams
            0x080C1ECC, // setupStandardHitScripts
            0x080C1F8C, // flipped
            0x080C2088, // notflipped
            0x080C2178, // blizzock
            0x080C2290, // attackPlayerIfCloseBy
            0x080C2310, // handleWallBounces
            0x080C238C, // toRight
            0x080C2484, // toLeft
            0x080C25A8, // toAttack
            0x080C2698, // handleWallBouncesWithAttacks
            0x080C273C, // toAttack
            0x080C282C, // handlePigeonWallBouncesWithAttacks
            0x080C28E0, // spiderHandleFloorCeilBounces
            0x080C2970, // die
            0x080C2A68, // ouch
            0x080C2B58, // AITakeOneDamage
            0x080C2BD8, // AITakeSlashDamage
            0x080C2C7C, // AITakeKickDamage
            0x080C2D08, // AITakeJumpingDamage
            0x080C2DB8, // AIDieNow
            0x080C2E4C, // AIPlummetNow
            0x080C32B4, // bridge
            0x080C3304, // softGround
            0x080C3368, // wall
            0x080C33AC, // treeBridge
            0x080C3418, // bridgeCrumble
            0x080C34AC, // floorCrumble
            0x080C356C, // wallCrumble
            0x080C35C0, // bridgeDown
            0x080C3960, // idle
            0x080C3A50, // bouncyMushroom
            0x080C3AB0, // bouncyPost
            0x080C3B04, // bouncyFlower
            0x080C3B5C, // bouncyFlowerSimple
            0x080C3BB0, // bouncyMushroomBounce
            0x080C3BF4, // bouncyPostBounce
            0x080C3C38, // bouncyFlowerBounce
            0x080C4040, // spike
            0x080C4074, // bushes
            0x080C40CC, // rollModeBushes
            0x080C4108, // bushSquish
        };
    }
}