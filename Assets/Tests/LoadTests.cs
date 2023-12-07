using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using BinarySerializer;
using Cysharp.Threading.Tasks;
using Ray1Map;
using UnityEngine;
using UnityEngine.TestTools;

public static class LoadTests
{
    public static void GenerateTestCode()
    {
        var str = new StringBuilder();

        foreach (EngineVersion engineVersion in EnumHelpers.GetValues<EngineVersion>())
        {
            str.AppendLine($"public class {engineVersion}");
            str.AppendLine("{");

            foreach (GameModeSelection gameMode in EnumHelpers.GetValues<GameModeSelection>().
                         Where(x => x.GetAttribute<GameModeAttribute>().EngineVersion == engineVersion))
            {
                str.AppendLine($"    [UnityTest]");
                str.AppendLine($"    public IEnumerator {gameMode}() => LoadTests.Load(GameModeSelection.{gameMode});");
                str.AppendLine();
            }

            str.AppendLine("}");
            str.AppendLine();
        }

        str.ToString().CopyToClipboard();
    }

    public static IEnumerator Load(GameModeSelection gameMode)
    {
        GenerateTestCode();

        return UniTask.ToCoroutine(async () =>
        {
            string folder = Settings.GameDirectories.TryGetItem(gameMode);

            if (folder == null || !Directory.Exists(folder))
            {
                Debug.Log($"Skipped loading {gameMode} due to its specified folder not being valid");
                return;
            }

            BaseGameManager manager = gameMode.GetManager();

            GameSettings settings = new GameSettings(gameMode, folder, 0, 0);

            GameInfo_Volume[] levels = manager.GetLevels(settings);

            // Default to the first available map
            settings.World = levels[0].Worlds[0].Index;
            settings.Level = levels[0].Worlds[0].Maps[0];

            using Context context = new Ray1MapContext(settings);

            await LevelEditorData.InitAsync(settings);

            await manager.LoadFilesAsync(context);
            await manager.LoadAsync(context);
        });
    }
}

public class R1_PS1
{
    [UnityTest]
    public IEnumerator RaymanPS1US() => LoadTests.Load(GameModeSelection.RaymanPS1US);

    [UnityTest]
    public IEnumerator RaymanPS1EU() => LoadTests.Load(GameModeSelection.RaymanPS1EU);

    [UnityTest]
    public IEnumerator RaymanPS1USDemo() => LoadTests.Load(GameModeSelection.RaymanPS1USDemo);

    [UnityTest]
    public IEnumerator RaymanPS1EUDemo() => LoadTests.Load(GameModeSelection.RaymanPS1EUDemo);

}

public class R2_PS1
{
    [UnityTest]
    public IEnumerator Rayman2PS1Demo() => LoadTests.Load(GameModeSelection.Rayman2PS1Demo);

}

public class R1_PS1_JP
{
    [UnityTest]
    public IEnumerator RaymanPS1Japan() => LoadTests.Load(GameModeSelection.RaymanPS1Japan);

}

public class R1_PS1_JPDemoVol3
{
    [UnityTest]
    public IEnumerator RaymanPS1DemoVol3Japan() => LoadTests.Load(GameModeSelection.RaymanPS1DemoVol3Japan);

}

public class R1_PS1_JPDemoVol6
{
    [UnityTest]
    public IEnumerator RaymanPS1DemoVol6Japan() => LoadTests.Load(GameModeSelection.RaymanPS1DemoVol6Japan);

}

public class R1_Saturn
{
    [UnityTest]
    public IEnumerator RaymanSaturnUS() => LoadTests.Load(GameModeSelection.RaymanSaturnUS);

    [UnityTest]
    public IEnumerator RaymanSaturnProto() => LoadTests.Load(GameModeSelection.RaymanSaturnProto);

    [UnityTest]
    public IEnumerator RaymanSaturnEU() => LoadTests.Load(GameModeSelection.RaymanSaturnEU);

    [UnityTest]
    public IEnumerator RaymanSaturnJP() => LoadTests.Load(GameModeSelection.RaymanSaturnJP);

    [UnityTest]
    public IEnumerator RaymanSaturnUSDemo() => LoadTests.Load(GameModeSelection.RaymanSaturnUSDemo);

    [UnityTest]
    public IEnumerator RaymanSaturnJPDemo() => LoadTests.Load(GameModeSelection.RaymanSaturnJPDemo);

}

public class R1_PC
{
    [UnityTest]
    public IEnumerator RaymanPC_1_00() => LoadTests.Load(GameModeSelection.RaymanPC_1_00);

    [UnityTest]
    public IEnumerator RaymanPC_1_10() => LoadTests.Load(GameModeSelection.RaymanPC_1_10);

    [UnityTest]
    public IEnumerator RaymanPC_1_12() => LoadTests.Load(GameModeSelection.RaymanPC_1_12);

    [UnityTest]
    public IEnumerator RaymanPC_1_20() => LoadTests.Load(GameModeSelection.RaymanPC_1_20);

    [UnityTest]
    public IEnumerator RaymanPC_1_21_JP() => LoadTests.Load(GameModeSelection.RaymanPC_1_21_JP);

    [UnityTest]
    public IEnumerator RaymanPC_1_21() => LoadTests.Load(GameModeSelection.RaymanPC_1_21);

    [UnityTest]
    public IEnumerator RaymanPC_Demo_1() => LoadTests.Load(GameModeSelection.RaymanPC_Demo_1);

    [UnityTest]
    public IEnumerator RaymanPC_Demo_2() => LoadTests.Load(GameModeSelection.RaymanPC_Demo_2);

}

public class R1_PocketPC
{
    [UnityTest]
    public IEnumerator RaymanPocketPC() => LoadTests.Load(GameModeSelection.RaymanPocketPC);

    [UnityTest]
    public IEnumerator RaymanClassicMobile() => LoadTests.Load(GameModeSelection.RaymanClassicMobile);

}

public class R1_PC_Kit
{
    [UnityTest]
    public IEnumerator RaymanGoldPCDemo() => LoadTests.Load(GameModeSelection.RaymanGoldPCDemo);

    [UnityTest]
    public IEnumerator RaymanDesignerPC() => LoadTests.Load(GameModeSelection.RaymanDesignerPC);

    [UnityTest]
    public IEnumerator MapperPC() => LoadTests.Load(GameModeSelection.MapperPC);

    [UnityTest]
    public IEnumerator RaymanByHisFansPC() => LoadTests.Load(GameModeSelection.RaymanByHisFansPC);

    [UnityTest]
    public IEnumerator Rayman60LevelsPC() => LoadTests.Load(GameModeSelection.Rayman60LevelsPC);

}

public class R1_PC_Edu
{
    [UnityTest]
    public IEnumerator RaymanEducationalPC() => LoadTests.Load(GameModeSelection.RaymanEducationalPC);

    [UnityTest]
    public IEnumerator RaymanQuizPC() => LoadTests.Load(GameModeSelection.RaymanQuizPC);

}

public class R1_PS1_Edu
{
    [UnityTest]
    public IEnumerator RaymanEducationalPS1() => LoadTests.Load(GameModeSelection.RaymanEducationalPS1);

    [UnityTest]
    public IEnumerator RaymanQuizPS1() => LoadTests.Load(GameModeSelection.RaymanQuizPS1);

}

public class R1_GBA
{
    [UnityTest]
    public IEnumerator RaymanAdvanceGBAEU() => LoadTests.Load(GameModeSelection.RaymanAdvanceGBAEU);

    [UnityTest]
    public IEnumerator RaymanAdvanceGBAUS() => LoadTests.Load(GameModeSelection.RaymanAdvanceGBAUS);

    [UnityTest]
    public IEnumerator RaymanAdvanceGBAEUBeta() => LoadTests.Load(GameModeSelection.RaymanAdvanceGBAEUBeta);

}

public class R1_DSi
{
    [UnityTest]
    public IEnumerator RaymanDSi() => LoadTests.Load(GameModeSelection.RaymanDSi);

}

public class R1Jaguar
{
    [UnityTest]
    public IEnumerator RaymanJaguar() => LoadTests.Load(GameModeSelection.RaymanJaguar);

}

public class R1Jaguar_Proto
{
    [UnityTest]
    public IEnumerator RaymanJaguarPrototype() => LoadTests.Load(GameModeSelection.RaymanJaguarPrototype);

}

public class R1Jaguar_Demo
{
    [UnityTest]
    public IEnumerator RaymanJaguarDemo() => LoadTests.Load(GameModeSelection.RaymanJaguarDemo);

}

public class SNES
{
    [UnityTest]
    public IEnumerator RaymanSNES() => LoadTests.Load(GameModeSelection.RaymanSNES);

}

public class GBA_DonaldDuck
{
    [UnityTest]
    public IEnumerator DonaldDuckAdvanceGBAEU() => LoadTests.Load(GameModeSelection.DonaldDuckAdvanceGBAEU);

    [UnityTest]
    public IEnumerator DonaldDuckAdvanceGBAUS() => LoadTests.Load(GameModeSelection.DonaldDuckAdvanceGBAUS);

}

public class GBA_CrouchingTiger
{
    [UnityTest]
    public IEnumerator CrouchingTigerHiddenDragonGBAEU() => LoadTests.Load(GameModeSelection.CrouchingTigerHiddenDragonGBAEU);

    [UnityTest]
    public IEnumerator CrouchingTigerHiddenDragonGBAUS() => LoadTests.Load(GameModeSelection.CrouchingTigerHiddenDragonGBAUS);

    [UnityTest]
    public IEnumerator CrouchingTigerHiddenDragonGBAUSBeta() => LoadTests.Load(GameModeSelection.CrouchingTigerHiddenDragonGBAUSBeta);

}

public class GBA_R3_MadTrax
{
    [UnityTest]
    public IEnumerator Rayman3GBAMadTraxEU() => LoadTests.Load(GameModeSelection.Rayman3GBAMadTraxEU);

    [UnityTest]
    public IEnumerator Rayman3GBAMadTraxUS() => LoadTests.Load(GameModeSelection.Rayman3GBAMadTraxUS);

}

public class GBA_TomClancysRainbowSixRogueSpear
{
    [UnityTest]
    public IEnumerator TomClancysRainbowSixRogueSpearEU() => LoadTests.Load(GameModeSelection.TomClancysRainbowSixRogueSpearEU);

    [UnityTest]
    public IEnumerator TomClancysRainbowSixRogueSpearUS() => LoadTests.Load(GameModeSelection.TomClancysRainbowSixRogueSpearUS);

}

public class GBA_TheMummy
{
    [UnityTest]
    public IEnumerator TheMummyEU() => LoadTests.Load(GameModeSelection.TheMummyEU);

    [UnityTest]
    public IEnumerator TheMummyUS() => LoadTests.Load(GameModeSelection.TheMummyUS);

}

public class GBA_TombRaiderTheProphecy
{
    [UnityTest]
    public IEnumerator TombRaiderTheProphecyEU() => LoadTests.Load(GameModeSelection.TombRaiderTheProphecyEU);

    [UnityTest]
    public IEnumerator TombRaiderTheProphecyUS() => LoadTests.Load(GameModeSelection.TombRaiderTheProphecyUS);

}

public class GBA_BatmanVengeance
{
    [UnityTest]
    public IEnumerator BatmanVengeanceGBAEU() => LoadTests.Load(GameModeSelection.BatmanVengeanceGBAEU);

    [UnityTest]
    public IEnumerator BatmanVengeanceGBAUS() => LoadTests.Load(GameModeSelection.BatmanVengeanceGBAUS);

}

public class GBA_Sabrina
{
    [UnityTest]
    public IEnumerator SabrinaTheTeenageWitchPotionCommotionGBAEU() => LoadTests.Load(GameModeSelection.SabrinaTheTeenageWitchPotionCommotionGBAEU);

    [UnityTest]
    public IEnumerator SabrinaTheTeenageWitchPotionCommotionGBAUS() => LoadTests.Load(GameModeSelection.SabrinaTheTeenageWitchPotionCommotionGBAUS);

}

public class GBA_R3_Proto
{
    [UnityTest]
    public IEnumerator Rayman3GBAUSPrototype() => LoadTests.Load(GameModeSelection.Rayman3GBAUSPrototype);

}

public class GBA_R3
{
    [UnityTest]
    public IEnumerator Rayman3GBAEU() => LoadTests.Load(GameModeSelection.Rayman3GBAEU);

    [UnityTest]
    public IEnumerator Rayman3GBAUS() => LoadTests.Load(GameModeSelection.Rayman3GBAUS);

    [UnityTest]
    public IEnumerator Rayman3GBAEUBeta() => LoadTests.Load(GameModeSelection.Rayman3GBAEUBeta);

    [UnityTest]
    public IEnumerator Rayman3Digiblast() => LoadTests.Load(GameModeSelection.Rayman3Digiblast);

}

public class GBA_R3_NGage
{
    [UnityTest]
    public IEnumerator Rayman3NGage() => LoadTests.Load(GameModeSelection.Rayman3NGage);

}

public class GBA_SplinterCell
{
    [UnityTest]
    public IEnumerator SplinterCellGBAEU() => LoadTests.Load(GameModeSelection.SplinterCellGBAEU);

    [UnityTest]
    public IEnumerator SplinterCellGBAUS() => LoadTests.Load(GameModeSelection.SplinterCellGBAUS);

    [UnityTest]
    public IEnumerator SplinterCellGBAEUBeta() => LoadTests.Load(GameModeSelection.SplinterCellGBAEUBeta);

}

public class GBA_SplinterCell_NGage
{
    [UnityTest]
    public IEnumerator SplinterCellNGage() => LoadTests.Load(GameModeSelection.SplinterCellNGage);

}

public class GBA_PrinceOfPersia
{
    [UnityTest]
    public IEnumerator PrinceOfPersiaGBAEU() => LoadTests.Load(GameModeSelection.PrinceOfPersiaGBAEU);

    [UnityTest]
    public IEnumerator PrinceOfPersiaGBAUS() => LoadTests.Load(GameModeSelection.PrinceOfPersiaGBAUS);

}

public class GBA_BatmanRiseOfSinTzu
{
    [UnityTest]
    public IEnumerator BatmanRiseOfSinTzuGBAUS() => LoadTests.Load(GameModeSelection.BatmanRiseOfSinTzuGBAUS);

}

public class GBA_SplinterCellPandoraTomorrow
{
    [UnityTest]
    public IEnumerator SplinterCellPandoraTomorrowGBAEU() => LoadTests.Load(GameModeSelection.SplinterCellPandoraTomorrowGBAEU);

    [UnityTest]
    public IEnumerator SplinterCellPandoraTomorrowGBAUS() => LoadTests.Load(GameModeSelection.SplinterCellPandoraTomorrowGBAUS);

}

public class GBA_StarWarsTrilogy
{
    [UnityTest]
    public IEnumerator StarWarsTrilogyApprenticeOfTheForceGBAEU() => LoadTests.Load(GameModeSelection.StarWarsTrilogyApprenticeOfTheForceGBAEU);

    [UnityTest]
    public IEnumerator StarWarsTrilogyApprenticeOfTheForceGBAUS() => LoadTests.Load(GameModeSelection.StarWarsTrilogyApprenticeOfTheForceGBAUS);

}

public class GBA_StarWarsEpisodeIII
{
    [UnityTest]
    public IEnumerator StarWarsEpisodeIIIGBAEU() => LoadTests.Load(GameModeSelection.StarWarsEpisodeIIIGBAEU);

    [UnityTest]
    public IEnumerator StarWarsEpisodeIIIGBAUS() => LoadTests.Load(GameModeSelection.StarWarsEpisodeIIIGBAUS);

}

public class GBA_KingKong
{
    [UnityTest]
    public IEnumerator KingKongGBAEU() => LoadTests.Load(GameModeSelection.KingKongGBAEU);

    [UnityTest]
    public IEnumerator KingKongGBAUS() => LoadTests.Load(GameModeSelection.KingKongGBAUS);

}

public class GBA_OpenSeason
{
    [UnityTest]
    public IEnumerator OpenSeasonGBAEU1() => LoadTests.Load(GameModeSelection.OpenSeasonGBAEU1);

    [UnityTest]
    public IEnumerator OpenSeasonGBAEU2() => LoadTests.Load(GameModeSelection.OpenSeasonGBAEU2);

    [UnityTest]
    public IEnumerator OpenSeasonGBAUS() => LoadTests.Load(GameModeSelection.OpenSeasonGBAUS);

}

public class GBA_TMNT
{
    [UnityTest]
    public IEnumerator TMNTGBAEU() => LoadTests.Load(GameModeSelection.TMNTGBAEU);

    [UnityTest]
    public IEnumerator TMNTGBAUS() => LoadTests.Load(GameModeSelection.TMNTGBAUS);

}

public class GBA_SurfsUp
{
    [UnityTest]
    public IEnumerator SurfsUpEU1() => LoadTests.Load(GameModeSelection.SurfsUpEU1);

    [UnityTest]
    public IEnumerator SurfsUpEU2() => LoadTests.Load(GameModeSelection.SurfsUpEU2);

    [UnityTest]
    public IEnumerator SurfsUpUS() => LoadTests.Load(GameModeSelection.SurfsUpUS);

}

public class GBARRR
{
    [UnityTest]
    public IEnumerator RaymanRavingRabbidsGBAEU() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsGBAEU);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsGBAUS() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsGBAUS);

}

public class GBAIsometric_Spyro1
{
    [UnityTest]
    public IEnumerator SpyroSeasonIceUS() => LoadTests.Load(GameModeSelection.SpyroSeasonIceUS);

}

public class GBAIsometric_Spyro2
{
    [UnityTest]
    public IEnumerator SpyroSeasonFlameEU() => LoadTests.Load(GameModeSelection.SpyroSeasonFlameEU);

    [UnityTest]
    public IEnumerator SpyroSeasonFlameUS() => LoadTests.Load(GameModeSelection.SpyroSeasonFlameUS);

}

public class GBAIsometric_Spyro3
{
    [UnityTest]
    public IEnumerator SpyroAdventureEU() => LoadTests.Load(GameModeSelection.SpyroAdventureEU);

    [UnityTest]
    public IEnumerator SpyroAdventureUS() => LoadTests.Load(GameModeSelection.SpyroAdventureUS);

}

public class GBAIsometric_Tron2
{
    [UnityTest]
    public IEnumerator Tron2KillerAppEU() => LoadTests.Load(GameModeSelection.Tron2KillerAppEU);

    [UnityTest]
    public IEnumerator Tron2KillerAppUS() => LoadTests.Load(GameModeSelection.Tron2KillerAppUS);

}

public class GBAIsometric_RHR
{
    [UnityTest]
    public IEnumerator RaymanHoodlumsRevengeEU() => LoadTests.Load(GameModeSelection.RaymanHoodlumsRevengeEU);

    [UnityTest]
    public IEnumerator RaymanHoodlumsRevengeUS() => LoadTests.Load(GameModeSelection.RaymanHoodlumsRevengeUS);

}

public class GBC_R1
{
    [UnityTest]
    public IEnumerator RaymanGBC() => LoadTests.Load(GameModeSelection.RaymanGBC);

    [UnityTest]
    public IEnumerator RaymanGBCJP() => LoadTests.Load(GameModeSelection.RaymanGBCJP);

    [UnityTest]
    public IEnumerator Rayman2GBCEU() => LoadTests.Load(GameModeSelection.Rayman2GBCEU);

    [UnityTest]
    public IEnumerator Rayman2GBCUS() => LoadTests.Load(GameModeSelection.Rayman2GBCUS);

    [UnityTest]
    public IEnumerator DonaldDuckGBCEU() => LoadTests.Load(GameModeSelection.DonaldDuckGBCEU);

    [UnityTest]
    public IEnumerator DonaldDuckGBCUS() => LoadTests.Load(GameModeSelection.DonaldDuckGBCUS);

    [UnityTest]
    public IEnumerator MowgliGBC() => LoadTests.Load(GameModeSelection.MowgliGBC);

}

public class GBC_R1_Palm
{
    [UnityTest]
    public IEnumerator RaymanGBCPalmOSColor() => LoadTests.Load(GameModeSelection.RaymanGBCPalmOSColor);

    [UnityTest]
    public IEnumerator RaymanGBCPalmOSGreyscale() => LoadTests.Load(GameModeSelection.RaymanGBCPalmOSGreyscale);

    [UnityTest]
    public IEnumerator RaymanGBCPalmOSColorDemo() => LoadTests.Load(GameModeSelection.RaymanGBCPalmOSColorDemo);

    [UnityTest]
    public IEnumerator RaymanGBCPalmOSGreyscaleDemo() => LoadTests.Load(GameModeSelection.RaymanGBCPalmOSGreyscaleDemo);

}

public class GBC_R1_PocketPC
{
    [UnityTest]
    public IEnumerator RaymanGBCPocketPC_Portrait() => LoadTests.Load(GameModeSelection.RaymanGBCPocketPC_Portrait);

    [UnityTest]
    public IEnumerator RaymanGBCPocketPC_Landscape() => LoadTests.Load(GameModeSelection.RaymanGBCPocketPC_Landscape);

    [UnityTest]
    public IEnumerator RaymanGBCPocketPC_LandscapeIPAQ() => LoadTests.Load(GameModeSelection.RaymanGBCPocketPC_LandscapeIPAQ);

    [UnityTest]
    public IEnumerator RaymanGBCPocketPC_PortraitDemo() => LoadTests.Load(GameModeSelection.RaymanGBCPocketPC_PortraitDemo);

    [UnityTest]
    public IEnumerator RaymanGBCPocketPC_LandscapeDemo() => LoadTests.Load(GameModeSelection.RaymanGBCPocketPC_LandscapeDemo);

    [UnityTest]
    public IEnumerator RaymanGBCPocketPC_LandscapeIPAQDemo() => LoadTests.Load(GameModeSelection.RaymanGBCPocketPC_LandscapeIPAQDemo);

}

public class GBAVV_Crash1
{
    [UnityTest]
    public IEnumerator Crash1GBAEU() => LoadTests.Load(GameModeSelection.Crash1GBAEU);

    [UnityTest]
    public IEnumerator Crash1GBAUS() => LoadTests.Load(GameModeSelection.Crash1GBAUS);

    [UnityTest]
    public IEnumerator Crash1GBAJP() => LoadTests.Load(GameModeSelection.Crash1GBAJP);

}

public class GBAVV_ThePowerpuffGirlsHimAndSeek
{
    [UnityTest]
    public IEnumerator ThePowerpuffGirlsHimAndSeekGBAEU() => LoadTests.Load(GameModeSelection.ThePowerpuffGirlsHimAndSeekGBAEU);

    [UnityTest]
    public IEnumerator ThePowerpuffGirlsHimAndSeekGBAUS() => LoadTests.Load(GameModeSelection.ThePowerpuffGirlsHimAndSeekGBAUS);

}

public class GBAVV_FroggerAdvance
{
    [UnityTest]
    public IEnumerator FroggerAdvanceGBAEU() => LoadTests.Load(GameModeSelection.FroggerAdvanceGBAEU);

    [UnityTest]
    public IEnumerator FroggerAdvanceGBAUS() => LoadTests.Load(GameModeSelection.FroggerAdvanceGBAUS);

}

public class GBAVV_SpongeBobRevengeOfTheFlyingDutchman
{
    [UnityTest]
    public IEnumerator GBAVV_SpongeBobRevengeOfTheFlyingDutchmanGBA() => LoadTests.Load(GameModeSelection.GBAVV_SpongeBobRevengeOfTheFlyingDutchmanGBA);

    [UnityTest]
    public IEnumerator GBAVV_SpongeBobRevengeOfTheFlyingDutchmanGBAUSBeta() => LoadTests.Load(GameModeSelection.GBAVV_SpongeBobRevengeOfTheFlyingDutchmanGBAUSBeta);

}

public class GBAVV_TheMuppetsOnWithTheShow
{
    [UnityTest]
    public IEnumerator MuppetsOnWithTheShowGBA() => LoadTests.Load(GameModeSelection.MuppetsOnWithTheShowGBA);

}

public class GBAVV_SpyMuppetsLicenseToCroak
{
    [UnityTest]
    public IEnumerator SpyMuppetsLicenseToCroakGBA() => LoadTests.Load(GameModeSelection.SpyMuppetsLicenseToCroakGBA);

}

public class GBAVV_Crash2
{
    [UnityTest]
    public IEnumerator Crash2GBAEU() => LoadTests.Load(GameModeSelection.Crash2GBAEU);

    [UnityTest]
    public IEnumerator Crash2GBAUS() => LoadTests.Load(GameModeSelection.Crash2GBAUS);

    [UnityTest]
    public IEnumerator Crash2GBAJP() => LoadTests.Load(GameModeSelection.Crash2GBAJP);

}

public class GBAVV_BruceLeeReturnOfTheLegend
{
    [UnityTest]
    public IEnumerator BruceLeeReturnOfTheLegendGBAEU() => LoadTests.Load(GameModeSelection.BruceLeeReturnOfTheLegendGBAEU);

    [UnityTest]
    public IEnumerator BruceLeeReturnOfTheLegendGBAUS() => LoadTests.Load(GameModeSelection.BruceLeeReturnOfTheLegendGBAUS);

}

public class GBAVV_X2WolverinesRevenge
{
    [UnityTest]
    public IEnumerator X2WolverinesRevengeGBA() => LoadTests.Load(GameModeSelection.X2WolverinesRevengeGBA);

}

public class GBAVV_FindingNemo
{
    [UnityTest]
    public IEnumerator FindingNemoGBA() => LoadTests.Load(GameModeSelection.FindingNemoGBA);

    [UnityTest]
    public IEnumerator FindingNemoGBAJP() => LoadTests.Load(GameModeSelection.FindingNemoGBAJP);

}

public class GBAVV_TheLionKing
{
    [UnityTest]
    public IEnumerator TheLionKing112GBAEU() => LoadTests.Load(GameModeSelection.TheLionKing112GBAEU);

    [UnityTest]
    public IEnumerator TheLionKing112GBAUS() => LoadTests.Load(GameModeSelection.TheLionKing112GBAUS);

}

public class GBAVV_BrotherBear
{
    [UnityTest]
    public IEnumerator BrotherBearGBAEU() => LoadTests.Load(GameModeSelection.BrotherBearGBAEU);

    [UnityTest]
    public IEnumerator BrotherBearGBAUS() => LoadTests.Load(GameModeSelection.BrotherBearGBAUS);

}

public class GBAVV_SpongeBobBattleForBikiniBottom
{
    [UnityTest]
    public IEnumerator SpongeBobBattleForBikiniBottomGBAEU() => LoadTests.Load(GameModeSelection.SpongeBobBattleForBikiniBottomGBAEU);

    [UnityTest]
    public IEnumerator SpongeBobBattleForBikiniBottomGBAUS() => LoadTests.Load(GameModeSelection.SpongeBobBattleForBikiniBottomGBAUS);

}

public class GBAVV_CrashNitroKart
{
    [UnityTest]
    public IEnumerator CrashNitroKartEU() => LoadTests.Load(GameModeSelection.CrashNitroKartEU);

    [UnityTest]
    public IEnumerator CrashNitroKartUS() => LoadTests.Load(GameModeSelection.CrashNitroKartUS);

    [UnityTest]
    public IEnumerator CrashNitroKartJP() => LoadTests.Load(GameModeSelection.CrashNitroKartJP);

}

public class GBAVV_CrashNitroKart_NGage
{
    [UnityTest]
    public IEnumerator CrashNitroKartNGage() => LoadTests.Load(GameModeSelection.CrashNitroKartNGage);

}

public class GBAVV_CrashFusion
{
    [UnityTest]
    public IEnumerator CrashFusionGBAEU() => LoadTests.Load(GameModeSelection.CrashFusionGBAEU);

    [UnityTest]
    public IEnumerator CrashFusionGBAUS() => LoadTests.Load(GameModeSelection.CrashFusionGBAUS);

    [UnityTest]
    public IEnumerator CrashFusionGBAJP() => LoadTests.Load(GameModeSelection.CrashFusionGBAJP);

}

public class GBAVV_SpyroFusion
{
    [UnityTest]
    public IEnumerator SpyroFusionGBAEU() => LoadTests.Load(GameModeSelection.SpyroFusionGBAEU);

    [UnityTest]
    public IEnumerator SpyroFusionGBAUS() => LoadTests.Load(GameModeSelection.SpyroFusionGBAUS);

    [UnityTest]
    public IEnumerator SpyroFusionGBAUS2() => LoadTests.Load(GameModeSelection.SpyroFusionGBAUS2);

    [UnityTest]
    public IEnumerator SpyroFusionGBAJP() => LoadTests.Load(GameModeSelection.SpyroFusionGBAJP);

}

public class GBAVV_SharkTale
{
    [UnityTest]
    public IEnumerator SharkTaleGBA() => LoadTests.Load(GameModeSelection.SharkTaleGBA);

    [UnityTest]
    public IEnumerator SharkTaleGBAJP() => LoadTests.Load(GameModeSelection.SharkTaleGBAJP);

}

public class GBAVV_ThatsSoRaven
{
    [UnityTest]
    public IEnumerator ThatsSoRavenGBA() => LoadTests.Load(GameModeSelection.ThatsSoRavenGBA);

}

public class GBAVV_Shrek2
{
    [UnityTest]
    public IEnumerator Shrek2GBA() => LoadTests.Load(GameModeSelection.Shrek2GBA);

}

public class GBAVV_Shrek2BegForMercy
{
    [UnityTest]
    public IEnumerator Shrek2BegForMercyGBA() => LoadTests.Load(GameModeSelection.Shrek2BegForMercyGBA);

}

public class GBAVV_KidsNextDoorOperationSODA
{
    [UnityTest]
    public IEnumerator KidsNextDoorOperationSODAGBAUS() => LoadTests.Load(GameModeSelection.KidsNextDoorOperationSODAGBAUS);

}

public class GBAVV_Madagascar
{
    [UnityTest]
    public IEnumerator MadagascarGBA() => LoadTests.Load(GameModeSelection.MadagascarGBA);

    [UnityTest]
    public IEnumerator MadagascarGBAJP() => LoadTests.Load(GameModeSelection.MadagascarGBAJP);

}

public class GBAVV_BatmanBegins
{
    [UnityTest]
    public IEnumerator BatmanBeginsGBA() => LoadTests.Load(GameModeSelection.BatmanBeginsGBA);

}

public class GBAVV_UltimateSpiderMan
{
    [UnityTest]
    public IEnumerator UltimateSpiderManGBAEU() => LoadTests.Load(GameModeSelection.UltimateSpiderManGBAEU);

    [UnityTest]
    public IEnumerator UltimateSpiderManGBAUS() => LoadTests.Load(GameModeSelection.UltimateSpiderManGBAUS);

}

public class GBAVV_MadagascarOperationPenguin
{
    [UnityTest]
    public IEnumerator MadagascarOperationPenguinGBA() => LoadTests.Load(GameModeSelection.MadagascarOperationPenguinGBA);

}

public class GBAVV_OverTheHedge
{
    [UnityTest]
    public IEnumerator OverTheHedgeGBAEU() => LoadTests.Load(GameModeSelection.OverTheHedgeGBAEU);

    [UnityTest]
    public IEnumerator OverTheHedgeGBAUS() => LoadTests.Load(GameModeSelection.OverTheHedgeGBAUS);

}

public class GBAVV_OverTheHedgeHammyGoesNuts
{
    [UnityTest]
    public IEnumerator OverTheHedgeHammyGoesNutsGBAEU() => LoadTests.Load(GameModeSelection.OverTheHedgeHammyGoesNutsGBAEU);

    [UnityTest]
    public IEnumerator OverTheHedgeHammyGoesNutsGBAUS() => LoadTests.Load(GameModeSelection.OverTheHedgeHammyGoesNutsGBAUS);

}

public class GBAVV_SpiderMan3
{
    [UnityTest]
    public IEnumerator SpiderMan3GBAEU() => LoadTests.Load(GameModeSelection.SpiderMan3GBAEU);

    [UnityTest]
    public IEnumerator SpiderMan3GBAUS() => LoadTests.Load(GameModeSelection.SpiderMan3GBAUS);

}

public class GBAVV_ShrekTheThird
{
    [UnityTest]
    public IEnumerator GBAVV_ShrekTheThirdGBAEU() => LoadTests.Load(GameModeSelection.GBAVV_ShrekTheThirdGBAEU);

    [UnityTest]
    public IEnumerator GBAVV_ShrekTheThirdGBAUS() => LoadTests.Load(GameModeSelection.GBAVV_ShrekTheThirdGBAUS);

}

public class Gameloft_RRR
{
    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_128x128_s40v2() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_128x128_s40v2);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_128x128_CZ() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_128x128_CZ);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_128x128_Motorola() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_128x128_Motorola);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_128x160_s40v2a() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_128x160_s40v2a);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_128x160_SamsungX660() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_128x160_SamsungX660);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_128x160_SamsungE360() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_128x160_SamsungE360);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_128x160_SonyEricsson() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_128x160_SonyEricsson);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_176x208_s60v1() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_176x208_s60v1);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_176x208_s60v2() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_176x208_s60v2);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_208x208_s40v3() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_208x208_s40v3);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_240x320_s40v3a() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_240x320_s40v3a);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_240x320_SamsungF400() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_240x320_SamsungF400);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_240x320_SamsungD900() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_240x320_SamsungD900);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsMobile_240x320_SamsungF480V() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsMobile_240x320_SamsungF480V);

}

public class Gameloft_RK
{
    [UnityTest]
    public IEnumerator RaymanKartMobile_128x128() => LoadTests.Load(GameModeSelection.RaymanKartMobile_128x128);

    [UnityTest]
    public IEnumerator RaymanKartMobile_128x128_s40v2() => LoadTests.Load(GameModeSelection.RaymanKartMobile_128x128_s40v2);

    [UnityTest]
    public IEnumerator RaymanKartMobile_128x160_s40v2a_N6101() => LoadTests.Load(GameModeSelection.RaymanKartMobile_128x160_s40v2a_N6101);

    [UnityTest]
    public IEnumerator RaymanKartMobile_128x160_s40v2a_N6151() => LoadTests.Load(GameModeSelection.RaymanKartMobile_128x160_s40v2a_N6151);

    [UnityTest]
    public IEnumerator RaymanKartMobile_128x160_K500i() => LoadTests.Load(GameModeSelection.RaymanKartMobile_128x160_K500i);

    [UnityTest]
    public IEnumerator RaymanKartMobile_176x208_s60v1_N3650() => LoadTests.Load(GameModeSelection.RaymanKartMobile_176x208_s60v1_N3650);

    [UnityTest]
    public IEnumerator RaymanKartMobile_176x208_s60v1_N7650() => LoadTests.Load(GameModeSelection.RaymanKartMobile_176x208_s60v1_N7650);

    [UnityTest]
    public IEnumerator RaymanKartMobile_176x208_s60v2_N70() => LoadTests.Load(GameModeSelection.RaymanKartMobile_176x208_s60v2_N70);

    [UnityTest]
    public IEnumerator RaymanKartMobile_208x208_s40v3() => LoadTests.Load(GameModeSelection.RaymanKartMobile_208x208_s40v3);

    [UnityTest]
    public IEnumerator RaymanKartMobile_176x220_K700i() => LoadTests.Load(GameModeSelection.RaymanKartMobile_176x220_K700i);

    [UnityTest]
    public IEnumerator RaymanKartMobile_176x220_SamsungZV60() => LoadTests.Load(GameModeSelection.RaymanKartMobile_176x220_SamsungZV60);

    [UnityTest]
    public IEnumerator RaymanKartMobile_176x220_SamsungZV10() => LoadTests.Load(GameModeSelection.RaymanKartMobile_176x220_SamsungZV10);

    [UnityTest]
    public IEnumerator RaymanKartMobile_176x220_RAZRV3() => LoadTests.Load(GameModeSelection.RaymanKartMobile_176x220_RAZRV3);

    [UnityTest]
    public IEnumerator RaymanKartMobile_176x220_U8110() => LoadTests.Load(GameModeSelection.RaymanKartMobile_176x220_U8110);

    [UnityTest]
    public IEnumerator RaymanKartMobile_176x220_KG800() => LoadTests.Load(GameModeSelection.RaymanKartMobile_176x220_KG800);

    [UnityTest]
    public IEnumerator RaymanKartMobile_240x320_s40v3a() => LoadTests.Load(GameModeSelection.RaymanKartMobile_240x320_s40v3a);

    [UnityTest]
    public IEnumerator RaymanKartMobile_240x320_s40v5() => LoadTests.Load(GameModeSelection.RaymanKartMobile_240x320_s40v5);

    [UnityTest]
    public IEnumerator RaymanKartMobile_240x320_W910i() => LoadTests.Load(GameModeSelection.RaymanKartMobile_240x320_W910i);

    [UnityTest]
    public IEnumerator RaymanKartMobile_240x320_C905() => LoadTests.Load(GameModeSelection.RaymanKartMobile_240x320_C905);

    [UnityTest]
    public IEnumerator RaymanKartMobile_240x320_SamsungF400() => LoadTests.Load(GameModeSelection.RaymanKartMobile_240x320_SamsungF400);

    [UnityTest]
    public IEnumerator RaymanKartMobile_240x320_SamsungF480() => LoadTests.Load(GameModeSelection.RaymanKartMobile_240x320_SamsungF480);

    [UnityTest]
    public IEnumerator RaymanKartMobile_320x240_KS360() => LoadTests.Load(GameModeSelection.RaymanKartMobile_320x240_KS360);

    [UnityTest]
    public IEnumerator RaymanKartMobile_320x240_Broken() => LoadTests.Load(GameModeSelection.RaymanKartMobile_320x240_Broken);

}

public class Jade_Engine
{
}

public class Jade_Montpellier
{
}

public class Jade_BGE
{
    [UnityTest]
    public IEnumerator BeyondGoodAndEvilPC() => LoadTests.Load(GameModeSelection.BeyondGoodAndEvilPC);

    [UnityTest]
    public IEnumerator BeyondGoodAndEvilPCDemo() => LoadTests.Load(GameModeSelection.BeyondGoodAndEvilPCDemo);

    [UnityTest]
    public IEnumerator BeyondGoodAndEvilGC() => LoadTests.Load(GameModeSelection.BeyondGoodAndEvilGC);

    [UnityTest]
    public IEnumerator BeyondGoodAndEvilPS2_20030805() => LoadTests.Load(GameModeSelection.BeyondGoodAndEvilPS2_20030805);

    [UnityTest]
    public IEnumerator BeyondGoodAndEvilPS2_20030814() => LoadTests.Load(GameModeSelection.BeyondGoodAndEvilPS2_20030814);

    [UnityTest]
    public IEnumerator BeyondGoodAndEvilPS2() => LoadTests.Load(GameModeSelection.BeyondGoodAndEvilPS2);

    [UnityTest]
    public IEnumerator BeyondGoodAndEvilXbox() => LoadTests.Load(GameModeSelection.BeyondGoodAndEvilXbox);

}

public class Jade_BGE_HD
{
    [UnityTest]
    public IEnumerator BeyondGoodAndEvilPS3() => LoadTests.Load(GameModeSelection.BeyondGoodAndEvilPS3);

    [UnityTest]
    public IEnumerator BeyondGoodAndEvilXbox360() => LoadTests.Load(GameModeSelection.BeyondGoodAndEvilXbox360);

}

public class Jade_KingKong
{
    [UnityTest]
    public IEnumerator KingKongPC() => LoadTests.Load(GameModeSelection.KingKongPC);

    [UnityTest]
    public IEnumerator KingKongGC() => LoadTests.Load(GameModeSelection.KingKongGC);

    [UnityTest]
    public IEnumerator KingKongPS2() => LoadTests.Load(GameModeSelection.KingKongPS2);

    [UnityTest]
    public IEnumerator KingKongXbox() => LoadTests.Load(GameModeSelection.KingKongXbox);

    [UnityTest]
    public IEnumerator KingKongXbox_20050728() => LoadTests.Load(GameModeSelection.KingKongXbox_20050728);

    [UnityTest]
    public IEnumerator KingKongPSP() => LoadTests.Load(GameModeSelection.KingKongPSP);

}

public class Jade_KingKong_Xenon
{
    [UnityTest]
    public IEnumerator KingKongPCGamersEdition() => LoadTests.Load(GameModeSelection.KingKongPCGamersEdition);

    [UnityTest]
    public IEnumerator KingKongXbox360() => LoadTests.Load(GameModeSelection.KingKongXbox360);

    [UnityTest]
    public IEnumerator KingKongXbox360_20050926() => LoadTests.Load(GameModeSelection.KingKongXbox360_20050926);

}

public class Jade_RRR
{
    [UnityTest]
    public IEnumerator RaymanRavingRabbidsPC() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsPC);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsPCDemo() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsPCDemo);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsPCUnbinarized() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsPCUnbinarized);

	[UnityTest]
	public IEnumerator RaymanRavingRabbidsPCPrototype() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsPCPrototype);

	[UnityTest]
    public IEnumerator RaymanRavingRabbidsWii() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsWii);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsWiiJP() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsWiiJP);

    [UnityTest]
    public IEnumerator RaymanRavingRabbidsXbox360() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsXbox360);

	[UnityTest]
	public IEnumerator RaymanRavingRabbidsXbox360_20070213() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsXbox360_20070213);

	[UnityTest]
    public IEnumerator RaymanRavingRabbidsPS2() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsPS2);

}

public class Jade_Phoenix
{
}

public class Jade_Horsez2
{
    [UnityTest]
    public IEnumerator Horsez2PS2() => LoadTests.Load(GameModeSelection.Horsez2PS2);

    [UnityTest]
    public IEnumerator Horsez2PSP() => LoadTests.Load(GameModeSelection.Horsez2PSP);

    [UnityTest]
    public IEnumerator Horsez2PSPDemo() => LoadTests.Load(GameModeSelection.Horsez2PSPDemo);

    [UnityTest]
    public IEnumerator Horsez2Wii() => LoadTests.Load(GameModeSelection.Horsez2Wii);

    [UnityTest]
    public IEnumerator Horsez2PC() => LoadTests.Load(GameModeSelection.Horsez2PC);

    [UnityTest]
    public IEnumerator Horsez2PCHD() => LoadTests.Load(GameModeSelection.Horsez2PCHD);

}

public class Jade_PetzHorseClub
{
    [UnityTest]
    public IEnumerator PetzHorseClubWii() => LoadTests.Load(GameModeSelection.PetzHorseClubWii);

    [UnityTest]
    public IEnumerator PetzHorseClubPC() => LoadTests.Load(GameModeSelection.PetzHorseClubPC);

    [UnityTest]
    public IEnumerator PetzHorseClubPCHD() => LoadTests.Load(GameModeSelection.PetzHorseClubPCHD);

}

public class Jade_MovieGames
{
    [UnityTest]
    public IEnumerator MovieGamesWii() => LoadTests.Load(GameModeSelection.MovieGamesWii);

}

public class Jade_RRR2
{
    [UnityTest]
    public IEnumerator RaymanRavingRabbids2PC() => LoadTests.Load(GameModeSelection.RaymanRavingRabbids2PC);

    [UnityTest]
    public IEnumerator RaymanRavingRabbids2Wii() => LoadTests.Load(GameModeSelection.RaymanRavingRabbids2Wii);

	[UnityTest]
	public IEnumerator RaymanRavingRabbids2Wii_20070901() => LoadTests.Load(GameModeSelection.RaymanRavingRabbids2Wii_20070901);

}

public class Jade_Montreal
{
}

public class Jade_PoP_SoT_20030723
{
	[UnityTest]
	public IEnumerator PrinceOfPersiaTheSandsOfTimePS2_20030710() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheSandsOfTimePS2_20030710);

	[UnityTest]
    public IEnumerator PrinceOfPersiaTheSandsOfTimePS2_20030723() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheSandsOfTimePS2_20030723);

    [UnityTest]
    public IEnumerator PrinceOfPersiaTheSandsOfTimeXbox_20030723() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheSandsOfTimeXbox_20030723);

}

public class Jade_PoP_SoT_20030819
{
    [UnityTest]
    public IEnumerator PrinceOfPersiaTheSandsOfTimePS2_20030819() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheSandsOfTimePS2_20030819);

}

public class Jade_PoP_SoT
{
    [UnityTest]
    public IEnumerator PrinceOfPersiaTheSandsOfTimePS2() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheSandsOfTimePS2);

    [UnityTest]
    public IEnumerator PrinceOfPersiaTheSandsOfTimeGC() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheSandsOfTimeGC);

    [UnityTest]
    public IEnumerator PrinceOfPersiaTheSandsOfTimePC() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheSandsOfTimePC);

	[UnityTest]
	public IEnumerator PrinceOfPersiaTheSandsOfTimePC_20031217() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheSandsOfTimePC_20031217);

	[UnityTest]
    public IEnumerator PrinceOfPersiaTheSandsOfTimePC_20040227() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheSandsOfTimePC_20040227);

    [UnityTest]
    public IEnumerator PrinceOfPersiaTheSandsOfTimeXbox() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheSandsOfTimeXbox);

    [UnityTest]
    public IEnumerator PrinceOfPersiaTheSandsOfTimePS3() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheSandsOfTimePS3);

}

public class Jade_PoP_WW
{
    [UnityTest]
    public IEnumerator PrinceOfPersiaWarriorWithinPC() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinPC);

    [UnityTest]
    public IEnumerator PrinceOfPersiaWarriorWithinPCDemo() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinPCDemo);

    [UnityTest]
    public IEnumerator PrinceOfPersiaWarriorWithinGC() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinGC);

    [UnityTest]
    public IEnumerator PrinceOfPersiaWarriorWithinPS2() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinPS2);

    [UnityTest]
    public IEnumerator PrinceOfPersiaWarriorWithinPS2_20040920() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinPS2_20040920);

    [UnityTest]
    public IEnumerator PrinceOfPersiaWarriorWithinPS2_20041024() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinPS2_20041024);

    [UnityTest]
    public IEnumerator PrinceOfPersiaWarriorWithinPSP() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinPSP);

    [UnityTest]
    public IEnumerator PrinceOfPersiaWarriorWithinXbox() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinXbox);

    [UnityTest]
    public IEnumerator PrinceOfPersiaWarriorWithinIOS() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinIOS);

	[UnityTest]
	public IEnumerator PrinceOfPersiaWarriorWithinIOSHD() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinIOSHD);

	[UnityTest]
	public IEnumerator PrinceOfPersiaWarriorWithinIOSDemo() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinIOSDemo);

	[UnityTest]
	public IEnumerator PrinceOfPersiaWarriorWithinIOS3GOS4Demo() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinIOS3GOS4Demo);

	[UnityTest]
    public IEnumerator PrinceOfPersiaWarriorWithinPS3() => LoadTests.Load(GameModeSelection.PrinceOfPersiaWarriorWithinPS3);

}

public class Jade_PoP_T2T_20051002
{
    [UnityTest]
    public IEnumerator PrinceOfPersiaTheTwoThronesXbox_20051002() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheTwoThronesXbox_20051002);

}

public class Jade_PoP_T2T
{
    [UnityTest]
    public IEnumerator PrinceOfPersiaTheTwoThronesPC() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheTwoThronesPC);

    [UnityTest]
    public IEnumerator PrinceOfPersiaTheTwoThronesGC() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheTwoThronesGC);

    [UnityTest]
    public IEnumerator PrinceOfPersiaTheTwoThronesWii() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheTwoThronesWii);

    [UnityTest]
    public IEnumerator PrinceOfPersiaTheTwoThronesPS2() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheTwoThronesPS2);

    [UnityTest]
    public IEnumerator PrinceOfPersiaTheTwoThronesPSP() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheTwoThronesPSP);

    [UnityTest]
    public IEnumerator PrinceOfPersiaTheTwoThronesXbox() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheTwoThronesXbox);

    [UnityTest]
    public IEnumerator PrinceOfPersiaTheTwoThronesPS3() => LoadTests.Load(GameModeSelection.PrinceOfPersiaTheTwoThronesPS3);

}

public class Jade_PoP_TFS
{
}

public class Jade_PhoenixRayman4
{
}

public class Jade_Horsez
{
    [UnityTest]
    public IEnumerator HorsezPS2() => LoadTests.Load(GameModeSelection.HorsezPS2);

}

public class Jade_MyWordCoach
{
    [UnityTest]
    public IEnumerator MyWordCoachWii() => LoadTests.Load(GameModeSelection.MyWordCoachWii);

    [UnityTest]
    public IEnumerator MyFrenchCoachWii() => LoadTests.Load(GameModeSelection.MyFrenchCoachWii);

    [UnityTest]
    public IEnumerator MySpanishCoachWii() => LoadTests.Load(GameModeSelection.MySpanishCoachWii);

}

public class Jade_CloudyWithAChanceOfMeatballs
{
}

public class Jade_NCIS
{
}

public class Jade_JustDance
{
}

public class Jade_Avatar
{
}

public class Jade_TMNT
{
}

public class Jade_RRRTVParty
{
    [UnityTest]
    public IEnumerator RaymanRavingRabbidsTVPartyWii() => LoadTests.Load(GameModeSelection.RaymanRavingRabbidsTVPartyWii);

}

public class Jade_Naruto
{
}

public class Jade_Naruto1RiseOfANinja
{
}

public class Jade_Naruto2TheBrokenBond
{
}

public class GEN
{
    [UnityTest]
    public IEnumerator RaymanEveilPC() => LoadTests.Load(GameModeSelection.RaymanEveilPC);

}

public class KlonoaGBA_EOD
{
    [UnityTest]
    public IEnumerator KlonoaEmpireOfDreamsGBAUS() => LoadTests.Load(GameModeSelection.KlonoaEmpireOfDreamsGBAUS);

}

public class KlonoaGBA_DCT
{
    [UnityTest]
    public IEnumerator KlonoaDreamChampTournamentGBAUS() => LoadTests.Load(GameModeSelection.KlonoaDreamChampTournamentGBAUS);

}

public class KlonoaHeroes
{
    [UnityTest]
    public IEnumerator KlonoaHeroesGBAJP() => LoadTests.Load(GameModeSelection.KlonoaHeroesGBAJP);

}

public class PSKlonoa_DTP
{
    [UnityTest]
    public IEnumerator KlonoaDoorToPhantomilePS1USPrototype_19970717() => LoadTests.Load(GameModeSelection.KlonoaDoorToPhantomilePS1USPrototype_19970717);

    [UnityTest]
    public IEnumerator KlonoaDoorToPhantomilePS1US() => LoadTests.Load(GameModeSelection.KlonoaDoorToPhantomilePS1US);

}