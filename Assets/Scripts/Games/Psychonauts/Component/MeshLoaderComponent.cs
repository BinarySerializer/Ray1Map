using System;
using System.IO;
using Cysharp.Threading.Tasks;
using PsychoPortal;
using UnityEditor;
using UnityEngine;

namespace Ray1Map.Psychonauts
{
    public class MeshLoaderComponent : MonoBehaviour
    {
        public Psychonauts_Manager Manager;
        public Ray1MapLoader Loader;

        public string MeshFilePath;
        public string ScreenshotMeshFilePaths;
        
        public bool PS2_CreateDummyColors;
        public bool PS2_IgnoreColorsForFlag19;
        public bool PS2_IgnoreColors;
        public bool PS2_IgnoreBlackColors;
        public bool PS2_RemoveFlag19;
        public int PS2_InvertNormalsForTexture = -1; // Set to 20 for CMBT level to fix the hallway

        private async UniTask DoMeshFragLoadAsync(Func<UniTask> task)
        {
            try
            {
                Psychonauts_Manager_PS2.PS2MeshFragFlags flags = Psychonauts_Manager_PS2.PS2MeshFragFlags.None;

                if (PS2_CreateDummyColors)
                    flags |= Psychonauts_Manager_PS2.PS2MeshFragFlags.CreateDummyColors;

                if (PS2_IgnoreColorsForFlag19)
                    flags |= Psychonauts_Manager_PS2.PS2MeshFragFlags.IgnoreColorsForFlag19;

                if (PS2_IgnoreColors)
                    flags |= Psychonauts_Manager_PS2.PS2MeshFragFlags.IgnoreColors;

                if (PS2_IgnoreBlackColors)
                    flags |= Psychonauts_Manager_PS2.PS2MeshFragFlags.IgnoreBlackColors;

                if (PS2_RemoveFlag19)
                    flags |= Psychonauts_Manager_PS2.PS2MeshFragFlags.RemoveFlag19;

                Loader.Context.StoreObject(Psychonauts_Manager_PS2.PS2MeshFragSettingsKey,
                    new Psychonauts_Manager_PS2.PS2MeshFragSettings()
                    {
                        Flags = flags,
                        InvertNormalsForTextures = new [] { PS2_InvertNormalsForTexture },
                    });

                await task();
            }
            finally
            {
                Loader.Context.RemoveStoredObject(Psychonauts_Manager_PS2.PS2MeshFragSettingsKey);
            }
        }

        public async UniTask LoadMeshAsync()
        {
            using (Loader)
            {
                Scene plb = Loader.FileManager.ReadFromFile<Scene>(MeshFilePath, logger: Loader.Logger);

                if (plb == null)
                {
                    Debug.LogWarning($"Could not find {MeshFilePath}");
                    return;
                }

                await DoMeshFragLoadAsync(() => Manager.LoadSceneAsync(Loader, plb, transform, MeshFilePath));
            }
        }

        public async UniTask ConvertPL2ToPLBAsync()
        {
            using (Loader)
            {
                if (Manager is not Psychonauts_Manager_PS2 ps2Manager)
                {
                    Debug.LogWarning("Can only perform this action using a PS2 manager");
                    return;
                }

                Scene plb = Loader.FileManager.ReadFromFile<Scene>(MeshFilePath, logger: Loader.Logger);

                if (plb == null)
                {
                    Debug.LogWarning($"Could not read {MeshFilePath}");
                    return;
                }

                string outputDir = EditorUtility.OpenFolderPanel("Select output directory", null, "");

                if (!Directory.Exists(outputDir))
                    return;

                string outputFile = Path.ChangeExtension(Path.Combine(outputDir, MeshFilePath), ".plb");
                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                await DoMeshFragLoadAsync(() =>
                {
                    ps2Manager.ExportPL2ToPLB(Loader, plb, new PsychonautsSettings(PsychonautsVersion.PC_Digital), 
                        outputFile, logger: Loader.Logger);
                    return UniTask.CompletedTask;
                });
            }
        }

        public async UniTask ScreenshotMeshesAsync()
        {
            string outputDir = EditorUtility.OpenFolderPanel("Select output directory", null, "");

            if (!Directory.Exists(outputDir))
                return;

            using (Loader)
            {
                TransparencyCaptureBehaviour tcb = Camera.main.GetComponent<TransparencyCaptureBehaviour>();

                foreach (string meshFilePath in ScreenshotMeshFilePaths.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Scene plb;

                    try
                    {
                        plb = Loader.FileManager.ReadFromFile<Scene>(meshFilePath, logger: Loader.Logger);

                        if (plb == null)
                        {
                            Debug.LogWarning($"Could not find {MeshFilePath}");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Could not read {MeshFilePath}. Exception: {ex.Message}{Environment.NewLine}{ex}");
                        continue;
                    }

                    // Load mesh scene
                    GameObject obj = await Manager.LoadSceneAsync(Loader, plb, transform, MeshFilePath);

                    // Re-calculate bounds
                    Bounds bounds = Manager.GetDimensions(plb.Yield());
                    Vector2 min = new(bounds.min.x, bounds.min.z);
                    Vector2 max = new(bounds.max.x, bounds.max.z);
                    LevelEditorData.Level.IsometricData.CalculateXDisplacement = () => 0;
                    LevelEditorData.Level.IsometricData.CalculateYDisplacement = () => -(max.y + min.y) * 2;
                    LevelEditorData.Level.Bounds3D = bounds;

                    // Save screenshots
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{meshFilePath}_Front.png"), 
                        await tcb.CaptureFullLevel(true, pos3D: CameraPos.Front));
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{meshFilePath}_Isometric.png"), 
                        await tcb.CaptureFullLevel(true, pos3D: CameraPos.IsometricLeft));
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{meshFilePath}_Top.png"), 
                        await tcb.CaptureFullLevel(true, pos3D: CameraPos.Top));

                    Destroy(obj);

                    // Unload textures
                    await Resources.UnloadUnusedAssets();
                }
            }
        }
    }
}