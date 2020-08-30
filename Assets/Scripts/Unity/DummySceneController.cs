using System;
using System.IO;
using System.Linq;
using R1Engine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummySceneController : MonoBehaviour
{
    static DummySceneController()
    {
        Data = EnumHelpers.GetValues<GameModeSelection>().Select(x => new
        {
            Mode = x,
            Manager = (IGameManager)Activator.CreateInstance(x.GetAttribute<GameModeAttribute>().ManagerType),
        }).
            Where(x => Directory.Exists(Settings.GameDirectories.TryGetItem(x.Mode))).
            SelectMany(x => x.Manager.GetLevels(new GameSettings(x.Mode, Settings.GameDirectories[x.Mode], 1, 1)).
                SelectMany(vol => vol.Worlds.SelectMany(world => world.Maps.Select(map => new SettingsData(x.Mode, vol.Name, world.Index, map))))).ToArray();
        Index = Data.FindItemIndex(x => x.GameModeSelection == Settings.SelectedGameMode && (x.Volume == null || x.Volume == Settings.EduVolume) && x.World == Settings.World && x.Level == Settings.Level);
        Debug.Log($"Screenshot enumeration from {Index} with {Data.Length - Index} items");
    }

    void Start()
    {
        Settings.SelectedGameMode = Data[Index].GameModeSelection;
        Settings.EduVolume = Data[Index].Volume;
        Settings.World = Data[Index].World;
        Settings.Level = Data[Index].Level;

        SceneManager.LoadScene("MapViewer");

        Index++;
        Debug.Log(Index);
    }

    private static SettingsData[] Data { get; }
    private static int Index { get; set; }

    private class SettingsData
    {
        public SettingsData(GameModeSelection gameModeSelection, string volume, int world, int level)
        {
            GameModeSelection = gameModeSelection;
            Volume = volume;
            World = world;
            Level = level;
        }

        public GameModeSelection GameModeSelection { get; }

        public string Volume { get; }

        public int World { get; }

        public int Level { get; }
    }
}
