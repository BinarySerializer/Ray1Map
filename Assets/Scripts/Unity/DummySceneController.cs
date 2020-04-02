using R1Engine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummySceneController : MonoBehaviour
{
    void Start()
    {
        var manager = Settings.GetGameManager;
        var world = manager.GetLevels(Settings.GetGameSettings);

        Settings.Level++;
        if (Settings.Level > world[(int)Settings.GetGameSettings.World].Value.Length) {
            Settings.Level = 1;
            Settings.GetGameSettings.World++;
        }

        SceneManager.LoadScene("MapViewer");
    }
}
