﻿using Cysharp.Threading.Tasks;
using PsychoPortal;
using PsychoPortal.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Context = BinarySerializer.Context;
using Debug = UnityEngine.Debug;
using Light = PsychoPortal.Light;
using Mesh = PsychoPortal.Mesh;
using UnityMesh = UnityEngine.Mesh;

namespace Ray1Map.Psychonauts
{
    public class Psychonauts_Manager : BaseGameManager
    {
        #region Manager

        public override GameInfo_Volume[] GetLevels(GameSettings settings)
        {
            using Ray1MapLoader loader = CreateLoader(settings, createLogger: false);

            if (loader.Version == PsychonautsVersion.PS2)
                loader.LoadFilePackages();

            return GameInfo_Volume.SingleVolume(Maps.
                Select((x, i) =>
                {
                    string world = x.Name.Substring(0, 2);

                    return new
                    {
                        Level = x.Name,
                        World = world,
                        WorldIndex = LevelNames.FindItemIndex(l => l.Name == world),
                        DisplayName = x.DisplayName,
                        Index = i
                    };
                }).
                Where(x => loader.FileManager.FileExists(new FileRef(loader.GetPackPackFilePath(x.Level), FileLocation.FileSystem))).
                GroupBy(x => x.World).
                Select(x => new GameInfo_World(
                    index: x.First().WorldIndex,
                    worldName: LevelNames[x.First().WorldIndex].DisplayName,
                    maps: x.Select(m => m.Index).ToArray(),
                    mapNames: x.Select(m => $"{m.Level} - {m.DisplayName}").ToArray())).
                ToArray());
        }

        private static (string Name, string DisplayName)[] LevelNames { get; } = 
        {
            ("ST", "Start"),
            ("CA", "Whispering Rock Summer Camp"),
            ("BB", "Coach Oleander's Basic Braining"),
            ("NI", "Nightmare in the Brain Tumbler"),
            ("SA", "Sasha's Shooting Gallery"),
            ("MI", "Milla's Dance Party"),
            ("LL", "Lair of the Lungfish"),
            ("LO", "Lungfishopolis"),
            ("AS", "Thorney Towers Home for the Disturbed"),
            ("MM", "The Milkman Conspiracy"),
            ("TH", "Gloria's Theater"),
            ("WW", "Waterloo World"),
            ("BV", "Black Velvetopia"),
            ("MC", "Meat Circus"),
        };

        private static (string Name, string DisplayName)[] Maps { get; } =
        {
            ("STMU", "Main Menu"),

            ("CARE", "Reception Area and Wilderness"),
            ("CARE_NIGHT", "Reception Area and Wilderness (Night)"),
            ("CAMA", "Campgrounds Main"),
            ("CAMA_NIGHT", "Campgrounds Main (Night)"),
            ("CAKC", "Kids' Cabins"),
            ("CAKC_NIGHT", "Kids' Cabins (Night)"),
            ("CABH", "Boathouse and Beach"),
            ("CABH_NIGHT", "Boathouse and Beach (Night)"),
            ("CALI", "Lodge"),
            ("CALI_NIGHT", "Lodge (Night)"),
            ("CAGP", "GPC and Wilderness"),
            ("CAGP_NIGHT", "GPC and Wilderness (Night)"),
            ("CAJA", "Ford's Sanctuary"),
            ("CASA", "Sasha's Underground Lab"),
            ("CABU", "Bunkhouse File Select UI"),

            ("BBA1", "Obstacle Course 1"),
            ("BBA2", "Obstacle Course 2"),
            ("BBLT", "Obstacle Course Finale"),

            ("NIMP", "The Woods"),
            ("NIBA", "The Braintank"),

            ("SACU", "Sasha's Shooting Gallery"),

            ("MIFL", "The Lounge"),
            ("MIMM", "The Race"),
            ("MILL", "The Party"),

            ("LLLL", "Lair of the Lungfish"),

            ("LOMA", "Lungfishopolis"),
            ("LOCB", "Kochamara"),

            ("ASGR", "Grounds"),
            ("ASCO", "Lower Floors"),
            ("ASUP", "Upper Floors"),
            ("ASLB", "The Lab of Dr. Lobato"),
            ("ASRU", "Ruins"),

            ("MMI1", "The Neighborhood"),
            ("MMI2", "Book Repository"),
            ("MMDM", "The Den Mother"),

            ("THMS", "The Stage"),
            ("THCW", "The Catwalks"),
            ("THFB", "Confrontation"),

            ("WWMA", "Waterloo World"),

            ("BVES", "Edgar's Sancuary"),
            ("BVRB", "Running Against the Bull"),
            ("BVWT", "Tiger"),
            ("BVWE", "Eagle"),
            ("BVWD", "Dragon"),
            ("BVWC", "Cobra"),
            ("BVMA", "Matador's Arena"),

            ("MCTC", "Tent City"),
            ("MCBB", "The Butcher"),
        };

        protected static Ray1MapLoader CreateLoader(GameSettings settings, bool createLogger = true) =>
            new(new PsychonautsSettings(GetVersion(settings)), settings.GameDirectory, createLogger ? GetLogger() : null);
        protected static Ray1MapLoader CreateLoader(Context context, Unity_Level level) =>
            new(new PsychonautsSettings(GetVersion(context.GetR1Settings())), context.BasePath, context, level, GetLogger());
        protected static IBinarySerializerLogger GetLogger() => Settings.Log ? new BinarySerializerLogger(Settings.PsychoPortalLogFile) : null;
        protected static PsychonautsVersion GetVersion(GameSettings settings) => GetVersion(settings.GameModeSelection);
        protected static PsychonautsVersion GetVersion(GameModeSelection gameMode) => gameMode switch
        {
            GameModeSelection.Psychonauts_Xbox_Proto_20041217 => PsychonautsVersion.Xbox_Proto_20041217,
            GameModeSelection.Psychonauts_PC_Digital => PsychonautsVersion.PC_Digital,
            GameModeSelection.Psychonauts_PS2_EU => PsychonautsVersion.PS2,
            GameModeSelection.Psychonauts_PS2_US => PsychonautsVersion.PS2,
            GameModeSelection.Psychonauts_PS2_US_Demo => PsychonautsVersion.PS2,
            _ => throw new Exception("Invalid game mode"),
        };

        private static readonly float _scale = 1 / 32f;
        private static readonly Vector3 _scaleVector = new(_scale, _scale, -_scale); // Need to invert the z-axis

        #endregion

        #region Game Actions

        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new("Export Packaged Files", false, true, (_, output) => ExportPackagedFiles(settings, output)),
                new("Export All Level Textures", false, true, (_, output) => ExportAllLevelTextures(settings, output)),
                new("Export Current Level Textures", false, true, (_, output) => ExportCurrentLevelTextures(settings, output)),
                new("Export Current Level Model as OBJ", false, true, (_, output) => ExportCurrentLevelModelAsOBJ(settings, output)),
                new("Export Packed Mesh Files", false, true, (_, output) => ExportPackedMeshFiles(settings, output)),
            };
        }

        public void ExportPackagedFiles(GameSettings settings, string outputPath)
        {
            using Ray1MapLoader loader = CreateLoader(settings);

            loader.LoadFilePackages(loader.Logger);
            loader.FileManager.ExportPackagedFiles(outputPath);

            Debug.Log("Finished exporting");
        }

        public void ExportAllLevelTextures(GameSettings settings, string outputPath)
        {
            using Ray1MapLoader loader = CreateLoader(settings);
            loader.UseNativeTextures = false;

            if (loader.Settings.Version == PsychonautsVersion.PS2)
                loader.LoadFilePackages(loader.Logger);

            loader.LoadCommonPackPack(loader.Logger);

            foreach (string lvl in Maps.Select(x => x.Name))
            {
                if (!loader.FileManager.FileExists(new FileRef(loader.GetPackPackFilePath(lvl), FileLocation.FileSystem)))
                    continue;

                loader.LoadLevelPackPack(lvl, loader.Logger);
                loader.TexturesManager.DumpPackedTextures(outputPath);

                Debug.Log($"Exported {lvl}");
            }

            Debug.Log("Finished exporting");
        }

        public void ExportCurrentLevelTextures(GameSettings settings, string outputPath)
        {
            string lvl = Maps[settings.Level].Name;

            using Ray1MapLoader loader = CreateLoader(settings);
            loader.UseNativeTextures = false;

            if (loader.Settings.Version == PsychonautsVersion.PS2)
                loader.LoadFilePackages(loader.Logger);
            
            loader.LoadLevelPackPack(lvl, loader.Logger);

            loader.TexturesManager.DumpPackedTextures(outputPath);

            Debug.Log("Finished exporting");
        }

        public void ExportCurrentLevelModelAsOBJ(GameSettings settings, string outputPath)
        {
            string lvl = Maps[settings.Level].Name;

            using Ray1MapLoader loader = CreateLoader(settings);
            loader.UseNativeTextures = false;

            loader.LoadLevelPackPack(lvl, loader.Logger);

            var exp = new PsychonautsObjExporter();
            var textures = loader.TexturesManager.GetTextures(loader.LevelScene.TextureTranslationTable, loader.UseNativeTextures, loader.Logger);

            exportDomain(loader.LevelScene.RootDomain);

            void exportDomain(Domain domain)
            {
                foreach (Mesh mesh in domain.Meshes)
                    exportMesh(mesh);

                foreach (Domain child in domain.Children)
                    exportDomain(child);

                void exportMesh(Mesh mesh)
                {
                    exp.Export(outputPath, $"{domain.Name}_{mesh.Name}", mesh, textures);

                    foreach (Mesh child in mesh.Children)
                        exportMesh(child);
                }
            }

            Debug.Log($"Finished exporting");
        }

        public void ExportPackedMeshFiles(GameSettings settings, string outputPath)
        {
            using Ray1MapLoader loader = CreateLoader(settings);
            loader.UseNativeTextures = false;

            if (loader.Settings.Version == PsychonautsVersion.PS2)
                loader.LoadFilePackages(loader.Logger);

            HashSet<string> exportedFiles = new();

            loader.LoadCommonPackPack(loader.Logger);

            exportMeshPack(loader.CommonMeshPack);

            foreach (string lvl in Maps.Select(x => x.Name))
            {
                if (!loader.FileManager.FileExists(new FileRef(loader.GetPackPackFilePath(lvl), FileLocation.FileSystem)))
                    continue;

                loader.LoadLevelPackPack(lvl, loader.Logger);
                exportMeshPack(loader.LevelMeshPack);

                string ext = loader.Version == PsychonautsVersion.PS2 ? ".pl2" : ".plb";
                var filePath = Path.Combine(outputPath, $"levels\\{lvl}{ext}");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                Binary.WriteToFile(loader.LevelScene, filePath, loader.Settings, logger: loader.Logger);

                for (var i = 0; i < loader.ReferencedLevelScenes.Length; i++)
                {
                    Scene scene = loader.ReferencedLevelScenes[i];
                    string name = normalizePath(loader.LevelScene.RootDomain.RuntimeReferences[i]);

                    filePath = Path.Combine(outputPath, name);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    Binary.WriteToFile(scene, filePath, loader.Settings, logger: loader.Logger);
                }

                Debug.Log($"Exported {lvl}");
            }

            void exportMeshPack(MeshPack pack)
            {
                foreach (PackedScene meshFile in pack.MeshFiles)
                {
                    var filePath = Path.Combine(outputPath, meshFile.FileName);

                    if (exportedFiles.Contains(filePath))
                        continue;

                    exportedFiles.Add(filePath);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    Binary.WriteToFile(meshFile.Scene, filePath, loader.Settings, logger: loader.Logger);
                }
            }

            string normalizePath(string filePath) => new FileRef(filePath).GetNormalizedFilePathRoot();

            Debug.Log("Finished exporting");
        }

        #endregion

        #region Load

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            // Get the settings
            string lvl = Maps[context.GetR1Settings().Level].Name;

            // Create the level
            var level = new Unity_Level()
            {
                FramesPerSecond = 60,
                IsometricData = new Unity_IsometricData
                {
                    CollisionMapWidth = 0,
                    CollisionMapHeight = 0,
                    TilesWidth = 0,
                    TilesHeight = 0,
                    CollisionMap = null,
                    Scale = Vector3.one,
                    ViewAngle = Quaternion.Euler(90, 0, 0),
                    CalculateYDisplacement = () => 0,
                    CalculateXDisplacement = () => 0,
                    ObjectScale = Vector3.one * 1
                },
                PixelsPerUnit = 1,
                CellSize = 1,
                StartIn3D = true, // TODO: Disable the 2D view entirely
            };

            // Create a loader
            using Ray1MapLoader loader = CreateLoader(context, level);

            Controller.DetailedState = "Loading common packs";
            await Controller.WaitIfNecessary();

            // Load common data
            loader.LoadFilePackages(loader.Logger);
            loader.LoadCommonPackPack(loader.Logger);
            loader.LoadCommonAnimPack(loader.Logger);

            Controller.DetailedState = "Loading level packs";
            await Controller.WaitIfNecessary();

            // Load level data
            loader.LoadLevelPackPack(lvl, loader.Logger);
            loader.LoadLevelAnimPack(lvl, loader.Logger);

            Controller.DetailedState = "Loading animations";
            await Controller.WaitIfNecessary();

            // Load stub animations from animation packs
            loader.AnimationManager.LoadStubAnimations(loader.CommonAnimPack);
            loader.AnimationManager.LoadStubAnimations(loader.LevelAnimPack);

            // Load all remaining animations from master package
            foreach (FileRef fileRef in loader.FileManager.EnumerateFiles(FileLocation.Package))
            {
                if ((fileRef.FilePath.EndsWith(".jan") || fileRef.FilePath.EndsWith(".ja2")) && 
                    !loader.AnimationManager.HasLoadedAnimation(fileRef.FilePath))
                    loader.AnimationManager.LoadAnimation(fileRef, loader.FileManager, loader.Logger);
            }

            // Load animation components. This loads the blend animations which will help with validating which animations
            // can be played on which mesh.
            foreach (PsychonautsSkelAnim anim in loader.AnimationManager.Animations)
                anim.InitComponents(loader.FileManager, loader.Logger);

            // Fully load animations which can't be validated based on the header
            foreach (PsychonautsSkelAnim anim in loader.AnimationManager.Animations)
            {
                try
                {
                    // Older version don't have the joints count in the header
                    if (anim.Header.Version < 200)
                        anim.Init(loader.FileManager, loader.Logger);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to load old animation file {anim.FilePath} with version {anim.Header.Version}: {ex.Message}");
                }
            }

            Controller.DetailedState = "Loading meshes";
            await Controller.WaitIfNecessary();

            // Create the object manager
            level.ObjManager = new Unity_ObjectManager(context);

            GameObject world = await LoadLevelAsync(loader, Controller.obj.levelController.editor.layerTiles.transform, lvl);

            world.transform.localScale = _scaleVector;

            var bounds = GetDimensions(loader.ReferencedLevelScenes.Append(loader.LevelScene));
            Vector2 min = new Vector2(bounds.min.x, bounds.min.z);
            Vector2 max = new Vector2(bounds.max.x, bounds.max.z);
            level.IsometricData.CalculateXDisplacement = () => 0;
            level.IsometricData.CalculateYDisplacement = () => -(max.y + min.y) * 2;
            level.Bounds3D = bounds;
            //UnityEngine.Debug.Log($"{min} - {max}");


            level.Layers = new Unity_Layer[]
            {
                new Unity_Layer_GameObject(true)
                {
                    Name = "Map",
                    ShortName = "MAP",
                    Graphics = world,
                    DisableGraphicsWhenCollisionIsActive = true,
                    Dimensions = new UnityEngine.Rect(min, max - min),
                }
            };
            Controller.obj.levelController.editor.cam.camera3D.farClipPlane = 10000f;

            return level;
        }

        public Bounds GetDimensions(IEnumerable<Scene> scenes)
        {
            List<Bounds> dimensions = new();

            foreach (Scene scene in scenes)
                getDomainBounds(scene.RootDomain);

            void getDomainBounds(Domain domain) 
            {
                Vector3 min = convertVector(domain.Bounds.Min);
                Vector3 max = convertVector(domain.Bounds.Max);
                Vector3 center = Vector3.Lerp(min, max, 0.5f);
                Vector3 size = new Vector3(
                    max.x - min.x,
                    max.y - min.y,
                    min.z - max.z);
                
                dimensions.Add(new Bounds(center, size));
                
                foreach(var child in domain.Children)
                    getDomainBounds(child);
            }

            Vector3 convertVector(Vec3 v) => new Vector3(v.X * _scaleVector.x, v.Y * _scaleVector.y, v.Z * _scaleVector.z);

            Vector3 totalMin = new Vector3(
                dimensions.Min(d => d.min.x),
                dimensions.Min(d => d.min.y),
                dimensions.Min(d => d.min.z));
            Vector3 totalMax = new Vector3(
                dimensions.Max(d => d.max.x),
                dimensions.Max(d => d.max.y),
                dimensions.Max(d => d.max.z));

            return new Bounds(
                totalMin + (totalMax - totalMin) / 2f,
                totalMax - totalMin);
        }

        public async UniTask<GameObject> LoadLevelAsync(Ray1MapLoader loader, Transform parent, string levelName)
        {
            GameObject gaoParent = new GameObject(levelName);
            gaoParent.transform.SetParent(parent, false);
            gaoParent.transform.localScale = Vector3.one;
            gaoParent.transform.localRotation = Quaternion.identity;
            gaoParent.transform.localPosition = Vector3.zero;

            gaoParent.AddComponent<MeshLoaderComponent>(x =>
            {
                x.Manager = this;
                x.Loader = loader;
            });

            // Load models outside the map for now
            Vector3 plbPos = new(-60000, 30000, 20000);

            foreach (PackedScene meshFile in loader.CommonMeshPack.MeshFiles.Concat(loader.LevelMeshPack.MeshFiles))
            {
                GameObject obj = await LoadSceneAsync(loader, meshFile.Scene, gaoParent.transform, $"{meshFile.FileName} ({meshFile.Type})");
                obj.transform.position = plbPos;
                plbPos += new Vector3(meshFile.Scene.RootDomain.Bounds.Max.X - meshFile.Scene.RootDomain.Bounds.Min.X, 0, 0);
            }

            // Load the level scene
            string ext = loader.Settings.Version == PsychonautsVersion.PS2 ? ".pl2" : ".plb";
            await LoadSceneAsync(loader, loader.LevelScene, gaoParent.transform, $"levels\\{levelName}{ext}");

            // Load referenced level scenes
            for (var i = 0; i < loader.ReferencedLevelScenes.Length; i++)
            {
                Scene scene = loader.ReferencedLevelScenes[i];
                string name = loader.LevelScene.RootDomain.RuntimeReferences[i];

                await LoadSceneAsync(loader, scene, gaoParent.transform, name);
            }

            return gaoParent;
        }

        public async UniTask<GameObject> LoadSceneAsync(Ray1MapLoader loader, Scene scene, Transform parent, string name)
        {
            Controller.DetailedState = $"Loading scene{Environment.NewLine}{name}";
            await Controller.WaitIfNecessary();

            GameObject sceneObj = new GameObject($"Scene: {name}");
            sceneObj.transform.SetParent(parent, false);
            sceneObj.AddBinarySerializableData(loader.Settings, scene);

            sceneObj.transform.localScale = Vector3.one;
            sceneObj.transform.localRotation = Quaternion.identity;
            sceneObj.transform.localPosition = Vector3.zero;

            LoadedTexture[] loadedTextures = scene.TextureTranslationTable.
                Select(x => new LoadedTexture(loader.TexturesManager.GetTexture(x, loader.UseNativeTextures, loader.Logger), x)).
                ToArray();
            LoadDomain(loader, scene.RootDomain, sceneObj.transform, loadedTextures);

            return sceneObj;
        }

        public void LoadDomain(Ray1MapLoader loader, Domain domain, Transform parent, LoadedTexture[] textures)
        {
            GameObject domainObj = new GameObject($"Domain: {domain.Name}");
            domainObj.transform.SetParent(parent, false);
            domainObj.AddBinarySerializableData(loader.Settings, domain);

            domainObj.transform.localScale = Vector3.one;
            domainObj.transform.localRotation = Quaternion.identity;
            domainObj.transform.localPosition = Vector3.zero;

            // Load children
            foreach (Domain domainChild in domain.Children)
                LoadDomain(loader, domainChild, domainObj.transform, textures);

            // Load meshes
            foreach (Mesh mesh in domain.Meshes)
                LoadMesh(loader, mesh, domainObj.transform, textures);

            // Show entity positions
            foreach (DomainEntityInfo ei in domain.DomainEntityInfos)
            {
                loader.Level.EventData.Add(new Unity_Object_Dummy(null, Unity_ObjectType.Object,
                    position: ei.Position.ToInvVector3() * _scale,
                    name: $"Entity: {ei.Name}",
                    debugText: $"Class: {ei.ScriptClass}{Environment.NewLine}" +
                               $"EditVars: {ei.EditVars}{Environment.NewLine}"));
            }
        }

        public void LoadMesh(Ray1MapLoader loader, Mesh mesh, Transform parent, LoadedTexture[] textures)
        {
            GameObject meshObj = new GameObject($"Mesh: {mesh.Name}");
            meshObj.transform.SetParent(parent, false);
            meshObj.AddBinarySerializableData(loader.Settings, mesh);

            meshObj.transform.localScale = Vector3.one;
            meshObj.transform.localRotation = Quaternion.identity;
            meshObj.transform.localPosition = Vector3.zero;

            if (mesh.AnimAffectors.Length > 0)
                meshObj.AddComponent<MeshAnimationComponent>(x => x.Mesh = mesh);

            foreach (Mesh meshChild in mesh.Children)
                LoadMesh(loader, meshChild, meshObj.transform, textures);

            meshObj.transform.localPosition = mesh.Position.ToVector3();
            meshObj.transform.localRotation = mesh.Rotation.ToQuaternionRad();
            meshObj.transform.localScale = mesh.Scale.ToVector3();

            GameObject visualMeshObj = new GameObject("Visual");
            visualMeshObj.transform.SetParent(meshObj.transform, false);
            visualMeshObj.transform.localScale = Vector3.one;
            visualMeshObj.transform.localRotation = Quaternion.identity;
            visualMeshObj.transform.localPosition = Vector3.zero;

            var mapObjComp = meshObj.AddComponent<MapObjectComponent>();
            mapObjComp.MapObject = visualMeshObj;

            int skeletonsCount = mesh.Skeletons.Length;

            PsychonautsSkeleton[] skeletons = new PsychonautsSkeleton[skeletonsCount];
            Matrix4x4[][] bindPoses = new Matrix4x4[skeletonsCount][];

            for (int skelIndex = 0; skelIndex < skeletonsCount; skelIndex++)
            {
                Skeleton s = mesh.Skeletons[skelIndex];

                GameObject skeletonObj = new GameObject($"Skeleton: {s.Name}");
                skeletonObj.transform.SetParent(visualMeshObj.transform, false);
                skeletonObj.AddBinarySerializableData(loader.Settings, s);

                skeletonObj.transform.localPosition = Vector3.zero;
                skeletonObj.transform.localRotation = Quaternion.identity;
                skeletonObj.transform.localScale = Vector3.one;

                bindPoses[skelIndex] = new Matrix4x4[s.JointsCount];
                skeletons[skelIndex] = new PsychonautsSkeleton(s, skeletonObj.transform, loader.Settings);

                for (int i = 0; i < s.JointsCount; i++)
                    bindPoses[skelIndex][i] = skeletons[skelIndex].Joints[i].Transform.worldToLocalMatrix * skeletonObj.transform.localToWorldMatrix;
            }

            // Collision
            if (mesh.CollisionTree != null)
            {
                GameObject colObj = LoadCollisionTree(loader, mesh.CollisionTree, meshObj.transform);
                var colObjComp = meshObj.AddComponent<CollisionObjectComponent>();
                colObjComp.CollisionObject = colObj;
            }

            var psychoMeshFrags = new PsychonautsMeshFrag[mesh.MeshFrags.Length];

            for (var i = 0; i < mesh.MeshFrags.Length; i++)
            {
                MeshFrag meshFrag = mesh.MeshFrags[i];
                psychoMeshFrags[i] = LoadMeshFrag(loader, meshFrag, visualMeshObj.transform, i, textures, skeletons, bindPoses, out _);
            }

            var psychoMesh = new PsychonautsMesh(mesh, psychoMeshFrags, skeletons);

            if (skeletonsCount > 0)
            {
                visualMeshObj.AddComponent<PsychoPortal.Unity.SkeletonAnimationComponent>(x =>
                {
                    x.Mesh = psychoMesh;
                    x.AnimationManager = loader.AnimationManager;
                    x.FileManager = loader.FileManager;
                    x.Logger = loader.Logger;
                    x.Settings = loader.Settings;
                });
            }

            // Show trigger positions
            foreach (TriggerOBB t in mesh.Triggers)
            {
                var triggerObj = new Unity_Object_Psychonauts_Trigger(t, _scale);
                loader.Level.EventData.Add(triggerObj);
            }

            // Show light positions
            foreach (Light l in mesh.Lights)
            {
                var lightObj = new Unity_Object_Psychonauts_Light(l, _scale);
                loader.Level.EventData.Add(lightObj);
            }
        }

        public virtual PsychonautsMeshFrag LoadMeshFrag(Ray1MapLoader loader, MeshFrag meshFrag, Transform parent, int index, LoadedTexture[] textures, PsychonautsSkeleton[] skeletons, Matrix4x4[][] bindPoses, out GameObject meshFragObj)
        {
            meshFragObj = new GameObject(
                $"Frag: {index}, " +
                $"Blend Shapes: {meshFrag.BlendshapeData?.Streams.Length ?? 0}, " +
                $"Flags: {meshFrag.MaterialFlags}");
            meshFragObj.transform.SetParent(parent, false);
            meshFragObj.AddBinarySerializableData(loader.Settings, meshFrag);

            // For meshes with LOD any number greater than 1 will be a lower-res variant, usually without blend shapes
            if (meshFrag.DistantLOD > 1)
                meshFragObj.SetActive(false);

            UnityMesh unityMesh = new UnityMesh();

            // Set vertices and normals
            unityMesh.SetVertices(meshFrag);
            unityMesh.SetNormals(meshFrag);
            unityMesh.SetPolygons(meshFrag);
            unityMesh.SetVertexColors(meshFrag);

            MeshFilter mf = meshFragObj.AddComponent<MeshFilter>();
            meshFragObj.layer = LayerMask.NameToLayer("3D Collision");
            meshFragObj.transform.localScale = Vector3.one;
            meshFragObj.transform.localRotation = Quaternion.identity;
            meshFragObj.transform.localPosition = Vector3.zero;
            mf.sharedMesh = unityMesh;

            MeshFragBlendComponent blendComponent = null;

            if (meshFrag.BlendshapeData != null)
                blendComponent = meshFragObj.AddComponent<MeshFragBlendComponent>(x =>
                {
                    x.MeshFrag = meshFrag;
                    x.UnityMesh = unityMesh;
                });

            var isSelfIllum = meshFrag.MaterialFlags.HasFlag(MaterialFlags.Flag_29) || meshFrag.MaterialFlags.HasFlag(MaterialFlags.Flag_31);
            var isAlphaTest = meshFrag.MaterialFlags.HasFlag(MaterialFlags.BinaryAlpha);

            // EMeshFrag::HasAlphaFlags
            var tempAlphaFlags = meshFrag.MaterialFlags;
            if (tempAlphaFlags.HasFlag(MaterialFlags.BinaryAlpha)) {
                tempAlphaFlags &= ~MaterialFlags.Alpha;
            }
            if (tempAlphaFlags.HasFlag(MaterialFlags.AdditiveBlending)) {
                tempAlphaFlags &= ~(MaterialFlags.InvEdgeAlpha | MaterialFlags.EdgeAlpha);
            }
            bool hasTransparency =
                !meshFrag.MaterialFlags.HasFlag(MaterialFlags.DetailTexture)
                && !meshFrag.MaterialFlags.HasFlag(MaterialFlags.Flag_28)
                && (tempAlphaFlags.HasFlag(MaterialFlags.Alpha)
                || tempAlphaFlags.HasFlag(MaterialFlags.Flag_12)
                || tempAlphaFlags.HasFlag(MaterialFlags.EdgeAlpha)
                || tempAlphaFlags.HasFlag(MaterialFlags.InvEdgeAlpha)
                || tempAlphaFlags.HasFlag(MaterialFlags.Flag_15)
                || tempAlphaFlags.HasFlag(MaterialFlags.GlareOnly));


            Material matSrc;

            if (meshFrag.MaterialFlags.HasFlag(MaterialFlags.AdditiveBlending))
                matSrc = Controller.obj.levelController.controllerTilemap.MaterialPsychonautsAdditive;
            else if (meshFrag.MaterialFlags.HasFlag(MaterialFlags.Decal))
                matSrc = Controller.obj.levelController.controllerTilemap.MaterialPsychonautsDecal;
            else if(!isSelfIllum && (hasTransparency || meshFrag.MaterialColor.W != 1))
                matSrc = Controller.obj.levelController.controllerTilemap.MaterialPsychonautsTransparent;
            else
                matSrc = Controller.obj.levelController.controllerTilemap.MaterialPsychonautsAlphaTest;

            Material mat = new Material(matSrc);

            
            //mat.color = meshFrag.MaterialColor.ToVector4();
            mat.SetVector("_MaterialColor", meshFrag.MaterialColor.ToVector4());
            mat.SetFloat("_IsSelfIllumination", isSelfIllum ? 1 : 0);

            if (meshFrag.AnimInfo != null)
            {
                if (meshFrag.Pre_Version < 308)
                    Debug.LogWarning($"Mesh frag of version {meshFrag.Pre_Version} is animated");

                int[] skelOffsets = skeletons.Length > 1 ? new int[skeletons.Length] : null;

                if (skelOffsets != null)
                    for (int i = 1; i < skeletons.Length; i++)
                        skelOffsets[i] = skelOffsets[i - 1] + skeletons[i - 1].Joints.Length;

                int getBoneIndex(JointID id) => id.SkeletonIndex == 0 ? id.JointIndex : skelOffsets[id.SkeletonIndex] + id.JointIndex;

                BoneWeight[] weights = meshFrag.AnimInfo.OriginalSkinWeights.Select(x => new BoneWeight()
                {
                    boneIndex0 = getBoneIndex(meshFrag.AnimInfo.JointIDs[x.Joint1]),
                    weight0 = x.Weight.X,
                    boneIndex1 = getBoneIndex(meshFrag.AnimInfo.JointIDs[x.Joint2]),
                    weight1 = 1 - x.Weight.X,
                }).ToArray();
                unityMesh.boneWeights = weights;
                unityMesh.bindposes = bindPoses.SelectMany(x => x).ToArray();

                SkinnedMeshRenderer smr = meshFragObj.AddComponent<SkinnedMeshRenderer>();
                smr.sharedMaterial = mat;
                smr.sharedMesh = unityMesh;
                smr.bones = skeletons.SelectMany(x => x.Joints).Select(x => x.Transform).ToArray();
                //smr.rootBone = skeletons[0].Joints[0].Transform;
            }
            else
            {
                MeshRenderer mr = meshFragObj.AddComponent<MeshRenderer>();
                mr.sharedMaterial = mat;
            }

            LoadedTexture tex = null;

            int texStride = 0;
            Vector4 texturesInUse = new Vector4();

            for (int i = 0; i < Math.Min(meshFrag.TextureIndices.Length, 3); i++)
            {
                if (meshFrag.TextureIndices[i] != -1)
                {
                    tex = textures[meshFrag.TextureIndices[i]];

                    if (tex.Texture != null)
                    {
                        texturesInUse[i] = 1;
                        mat.SetTexture($"_Tex{i}", tex.Texture.GetTexture(tex.TextureRef.RepeatUV));

                        if (tex.Texture.IsAnimated)
                            meshFragObj.AddComponent<TextureAnimationComponent>(x => x.SetTexture(tex.Texture, tex.TextureRef, mat, textureName: $"_Tex{i}"));

                        if (meshFrag.TexCoordTransVel.X != 0 || meshFrag.TexCoordTransVel.Y != 0)
                        {
                            meshFragObj.AddComponent<TextureScrollComponent>(x =>
                            {
                                x.material = mat;
                                x.textureName = $"_Tex{i}";
                                x.scroll = meshFrag.TexCoordTransVel.ToVector2();
                            });
                        }

                        unityMesh.SetUVs(meshFrag, i, texStride);
                    }
                }

                texStride++;
            }

            if (meshFrag.MaterialFlags.HasFlag(MaterialFlags.Lightmap) && meshFrag.LightMapTextureIndices[0] != -1) 
            {
                tex = textures[meshFrag.LightMapTextureIndices[0]];

                if (tex.Texture != null)
                {
                    texturesInUse[3] = 1;
                    mat.SetTexture("_TexLightMap", tex.Texture.GetTexture(tex.TextureRef.RepeatUV));

                    if (tex.Texture.IsAnimated)
                        meshFragObj.AddComponent<TextureAnimationComponent>(x => x.SetTexture(tex.Texture, tex.TextureRef, mat, textureName: "_TexLightMap"));

                    unityMesh.SetUVs(meshFrag, 3, texStride);
                }
            }
            mat.SetVector("_TexturesInUse", texturesInUse);

            return new PsychonautsMeshFrag(meshFrag, unityMesh, blendComponent);
        }

        public GameObject LoadCollisionTree(Ray1MapLoader loader, CollisionTree col, Transform parent)
        {
            GameObject colObj = new GameObject($"Collision");
            colObj.transform.SetParent(parent, false);
            colObj.AddBinarySerializableData(loader.Settings, col);

            colObj.transform.localPosition = Vector3.zero;
            colObj.transform.localRotation = Quaternion.identity;
            colObj.transform.localScale = Vector3.one;

            // Create a separate mesh for each type of collision polygons
            foreach (var v in col.CollisionPolys.GroupBy(x => x.SurfaceFlags))
            {
                SurfaceFlags flags = v.Key;
                Vector3[] vertices = v.SelectMany(x => x.VertexIndices).Select(x => col.Vertices[x].ToVector3()).ToArray();
                int[] triIndices = Enumerable.Range(0, vertices.Length / 3).Select(x =>
                {
                    int off = x * 3;
                    // TODO: Show double-sided?
                    //return new int[] { off + 0, off + 1, off + 2, off + 0, off + 2, off + 1 };
                    return new int[] { off + 0, off + 2, off + 1 };
                }).SelectMany(x => x).ToArray();

                GameObject polyObj = new GameObject($"CollisionPoly ({flags})");
                polyObj.transform.SetParent(colObj.transform, false);

                polyObj.transform.localPosition = Vector3.zero;
                polyObj.transform.localRotation = Quaternion.identity;
                polyObj.transform.localScale = Vector3.one;

                UnityMesh unityMesh = new UnityMesh();

                // Set vertices
                unityMesh.SetVertices(vertices);
                unityMesh.SetTriangles(triIndices, 0);

                // TODO: Use better colors here
                Color color = new Color32(60, 120, 180, 255);

                for (int i = 0; i < 32; i++)
                {
                    if (((int)flags & (1 << i)) != 0)
                        color = Color.Lerp(color, new Color(i / 31f, i / 31f, i / 31f), 0.5f);
                }

                unityMesh.SetColors(Enumerable.Repeat(color, vertices.Length).ToArray());

                unityMesh.RecalculateNormals();

                MeshFilter mf = polyObj.AddComponent<MeshFilter>();
                polyObj.layer = LayerMask.NameToLayer("3D Collision");
                polyObj.transform.localScale = Vector3.one;
                polyObj.transform.localRotation = Quaternion.identity;
                polyObj.transform.localPosition = Vector3.zero;
                mf.sharedMesh = unityMesh;

                MeshRenderer mr = polyObj.AddComponent<MeshRenderer>();
                mr.sharedMaterial = Controller.obj.levelController.controllerTilemap.isometricCollisionMaterial;

                if (vertices.Distinct().Count() >= 3)
                {
                    // Add Collider GameObject
                    GameObject gaoc = new GameObject($"Poly Collider");
                    MeshCollider mc = gaoc.AddComponent<MeshCollider>();
                    mc.sharedMesh = unityMesh;
                    gaoc.layer = LayerMask.NameToLayer("3D Collision");
                    gaoc.transform.SetParent(colObj.transform);
                    gaoc.transform.localScale = Vector3.one;
                    gaoc.transform.localRotation = Quaternion.identity;
                    gaoc.transform.localPosition = Vector3.zero;
                    var col3D = gaoc.AddComponent<Unity_Collision3DBehaviour>();
                    col3D.Type = $"{flags}";
                }
            }

            return colObj;
        }

        #endregion

        #region Data Types

        public record LoadedTexture(PsychonautsTexture Texture, TextureReference TextureRef);

        #endregion
    }
}