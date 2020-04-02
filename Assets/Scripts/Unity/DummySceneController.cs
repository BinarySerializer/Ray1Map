using R1Engine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummySceneController : MonoBehaviour
{
    void Start()
    {
        var gameModes = new[]
        {
            GameModeSelection.RaymanPC,
            //GameModeSelection.RaymanDesignerPC,
            //GameModeSelection.RaymanByHisFansPC,
            //GameModeSelection.Rayman60LevelsPC,
            //GameModeSelection.RaymanPS1US,
            //GameModeSelection.RaymanPS1Japan,
        };

        // Enumerate the modes
        foreach (var gameMode in gameModes)
        {
            // Set the mode
            Settings.SelectedGameMode = gameMode;

            // Get the manager
            var manager = Settings.GetGameManager;

            // Enumerate every world
            foreach (var world in manager.GetLevels(Settings.GetGameSettings))
            {
                // Set the world
                Settings.World = world.Key;

                // Enumerate every level
                foreach (var lvl in world.Value)
                {
                    // Set the level
                    Settings.Level = lvl;

                    // Go back to mapviewer with the new settings
                    SceneManager.LoadScene("MapViewer");
                }
            }
        }
    }
}
