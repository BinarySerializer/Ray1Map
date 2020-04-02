using System.Linq;
using R1Engine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummySceneController : MonoBehaviour
{
    void Start()
    {
        var manager = Settings.GetGameManager;
        var world = manager.GetLevels(Settings.GetGameSettings).Where(x => x.Key == Settings.World).SelectMany(x => x.Value).ToArray();

        var levelIndex = world.FindItemIndex(x => x == Settings.Level);

        SceneManager.LoadScene("MapViewer");

        if ((levelIndex + 1) == world.Length)
        {
            Settings.World++;
            Settings.Level = manager.GetLevels(Settings.GetGameSettings).Where(x => x.Key == Settings.World).SelectMany(x => x.Value).First();
        }
        else
        {
            Settings.Level = world[levelIndex + 1];
        }
    }
}
