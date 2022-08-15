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
                string outputFile = Path.ChangeExtension(Path.Combine(outputDir, MeshFilePath), ".plb");
                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                ps2Manager.ExportPL2ToPLB(Loader, plb, new PsychonautsSettings(PsychonautsVersion.PC_Digital),
                    outputFile, logger: Loader.Logger);
            }
        }
    }
}