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
            OrderBy(m => m.Mode == Settings.SelectedGameMode ? -1 : 0).
            SelectMany(x => x.Manager.GetLevels(new GameSettings(x.Mode, Settings.GameDirectories[x.Mode])
                {
                    EduVolume = x.Manager.GetEduVolumes(new GameSettings(x.Mode, Settings.GameDirectories[x.Mode])).Contains(Settings.EduVolume) ? Settings.EduVolume : null
                }).
                SelectMany(y => y.Value.Select(z => new SettingsData(x.Mode, y.Key, z)))).
            ToArray();
        Index = 0;
    }

    void Start()
    {
        Settings.SelectedGameMode = Data[Index].GameModeSelection;
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
        public SettingsData(GameModeSelection gameModeSelection, World world, int level)
        {
            GameModeSelection = gameModeSelection;
            World = world;
            Level = level;
        }

        public GameModeSelection GameModeSelection { get; }

        public World World { get; }

        public int Level { get; }
    }
}
