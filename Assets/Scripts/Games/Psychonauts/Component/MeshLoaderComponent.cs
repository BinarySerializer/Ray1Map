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

        public async UniTask LoadMeshAsync()
        {
            using (Loader)
            {
                Scene plb = Loader.FileManager.ReadFromFile<Scene>(MeshFilePath, logger: Loader.Logger);

                if (plb == null)
                {
                    Debug.LogWarning($"Could not read {MeshFilePath}");
                    return;
                }

                await Manager.LoadSceneAsync(Loader, plb, transform, MeshFilePath);
            }
        }

        public void ConvertPL2ToPLB()
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

                ps2Manager.ExportPL2ToPLB(Loader, plb, new PsychonautsSettings(PsychonautsVersion.PC_Digital),
                    outputFile, logger: Loader.Logger);
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
                    Scene plb = Loader.FileManager.ReadFromFile<Scene>(meshFilePath, logger: Loader.Logger);

                    if (plb == null)
                    {
                        Debug.LogWarning($"Could not read {MeshFilePath}");
                        continue;
                    }

                    GameObject obj = await Manager.LoadSceneAsync(Loader, plb, transform, MeshFilePath);

                    Bounds bounds = Manager.GetDimensions(plb);
                    Vector2 min = new Vector2(bounds.min.x, bounds.min.z);
                    Vector2 max = new Vector2(bounds.max.x, bounds.max.z);
                    LevelEditorData.Level.IsometricData.CalculateXDisplacement = () => 0;
                    LevelEditorData.Level.IsometricData.CalculateYDisplacement = () => -(max.y + min.y) * 2;
                    LevelEditorData.Level.Bounds3D = bounds;

                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{meshFilePath}_Front.png"), 
                        await tcb.CaptureFullLevel(true, pos3D: CameraPos.Front));
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{meshFilePath}_Isometric.png"), 
                        await tcb.CaptureFullLevel(true, pos3D: CameraPos.IsometricLeft));
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{meshFilePath}_Top.png"), 
                        await tcb.CaptureFullLevel(true, pos3D: CameraPos.Top));

                    Destroy(obj);
                }
            }
        }
    }
}