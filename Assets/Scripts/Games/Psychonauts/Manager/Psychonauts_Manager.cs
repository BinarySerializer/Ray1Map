using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using PsychoPortal;
using PsychoPortal.Unity;
using UnityEngine;
using Context = BinarySerializer.Context;
using Debug = UnityEngine.Debug;
using Joint = PsychoPortal.Joint;
using Mesh = PsychoPortal.Mesh;
using UnityMesh = UnityEngine.Mesh;

namespace Ray1Map.Psychonauts
{
    public class Psychonauts_Manager : BaseGameManager
    {
        #region Manager

        public override GameInfo_Volume[] GetLevels(GameSettings settings)
        {
            Loader loader = new Loader(new PsychonautsSettings(GetVersion(settings)), settings.GameDirectory);

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
                Where(x => loader.FileManager.FileExists(loader.GetPackPackFilePath(x.Level))).
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

        private static IBinarySerializerLogger GetLogger() => Settings.Log ? new BinarySerializerLogger(Settings.LogFile) : null;
        private static PsychonautsVersion GetVersion(GameSettings settings) => settings.GameModeSelection switch
        {
            GameModeSelection.Psychonauts_Xbox_Proto_20041217 => PsychonautsVersion.Xbox_Proto_20041217,
            GameModeSelection.Psychonauts_PC_Digital => PsychonautsVersion.PC_Digital,
            GameModeSelection.Psychonauts_PS2 => PsychonautsVersion.PS2,
            _ => throw new Exception("Invalid game mode"),
        };

        #endregion

        #region Game Actions

        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new GameAction("Export All Level Textures", false, true, (_, output) => ExportAllLevelTextures(settings, output)),
                new GameAction("Export Current Level Textures", false, true, (_, output) => ExportCurrentLevelTextures(settings, output)),
                new GameAction("Export Current Level Model as OBJ", false, true, (_, output) => ExportCurrentLevelModelAsOBJ(settings, output)),
            };
        }

        public void ExportAllLevelTextures(GameSettings settings, string outputPath)
        {
            Loader loader = new Loader(new PsychonautsSettings(GetVersion(settings)), settings.GameDirectory);
            loader.UseNativeTextures = false;

            using IBinarySerializerLogger logger = GetLogger();

            loader.LoadCommonPackPack(logger);

            foreach (string lvl in Maps.Select(x => x.Name))
            {
                if (!loader.FileManager.FileExists(loader.GetPackPackFilePath(lvl)))
                    continue;

                loader.LoadLevelPackPack(lvl, logger);
                loader.TexturesManager.DumpTextures(outputPath);

                Debug.Log($"Exported {lvl}");
            }

            Debug.Log($"Finished exporting");
        }

        public void ExportCurrentLevelTextures(GameSettings settings, string outputPath)
        {
            string lvl = Maps[settings.Level].Name;

            Loader loader = new Loader(new PsychonautsSettings(GetVersion(settings)), settings.GameDirectory);
            loader.UseNativeTextures = false;

            using IBinarySerializerLogger logger = GetLogger();

            loader.LoadLevelPackPack(lvl, logger);

            loader.TexturesManager.DumpTextures(outputPath);

            Debug.Log($"Finished exporting");
        }

        public void ExportCurrentLevelModelAsOBJ(GameSettings settings, string outputPath)
        {
            string lvl = Maps[settings.Level].Name;

            Loader loader = new Loader(new PsychonautsSettings(GetVersion(settings)), settings.GameDirectory);
            loader.UseNativeTextures = false;

            using IBinarySerializerLogger logger = GetLogger();

            loader.LoadLevelPackPack(lvl, logger);

            var exp = new PsychonautsObjExporter();
            var textures = loader.TexturesManager.GetTextures(loader.LevelScene.TextureTranslationTable);

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

        #endregion

        #region Load

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            // Get the settings
            GameSettings r1Settings = context.GetR1Settings();
            string lvl = Maps[r1Settings.Level].Name;

            // Create a loader
            Loader loader = new Loader(new PsychonautsSettings(GetVersion(r1Settings)), r1Settings.GameDirectory);

            // Use a PsychoPortal logger
            using IBinarySerializerLogger logger = GetLogger();

            Controller.DetailedState = "Loading common packs";
            await Controller.WaitIfNecessary();

            // Load common data
            loader.LoadFilePackages(logger);
            loader.LoadCommonPackPack(logger);

            Controller.DetailedState = "Loading level packs";
            await Controller.WaitIfNecessary();

            // Load level data
            loader.LoadLevelPackPack(lvl, logger);

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
            };

            // Create the object manager
            level.ObjManager = new Unity_ObjectManager(context);

            GameObject world = LoadLevel(loader, Controller.obj.levelController.editor.layerTiles.transform, lvl);

            // Temporarily scale everything down
            const float scale = 1 / 32f;

            // Need to invert the z-axis
            world.transform.localScale = new Vector3(scale, scale, -scale);

            level.Layers = new Unity_Layer[]
            {
                new Unity_Layer_GameObject(true)
                {
                    Name = "Map",
                    ShortName = "MAP",
                    Graphics = world,
                    DisableGraphicsWhenCollisionIsActive = true
                }
            };

            Controller.obj.levelController.editor.cam.camera3D.farClipPlane = 10000f;

            // Show entity positions
            foreach (DomainEntityInfo ei in loader.LevelScene.RootDomain.DomainEntityInfos)
            {
                var pos = ei.Position.ToInvVector3();

                level.EventData.Add(new Unity_Object_Dummy(null, Unity_ObjectType.Object, 
                    position: new Vector3(pos.x * scale, -pos.y * scale, pos.z * scale), 
                    name: $"Entity: {ei.Name}", 
                    debugText: ei.ScriptClass));
            }

            // Show trigger positions
            foreach (TriggerOBB t in loader.LevelScene.RootDomain.Meshes.SelectMany(x => x.Triggers))
            {
                var pos = t.Position.ToInvVector3();

                level.EventData.Add(new Unity_Object_Dummy(null, Unity_ObjectType.Trigger, 
                    position: new Vector3(pos.x * scale, -pos.y * scale,-pos.z * scale),  
                    name: $"Trigger: {t.Name}", 
                    debugText: t.Name));
            }

            return level;
        }

        public GameObject LoadLevel(Loader loader, Transform parent, string level)
        {
            GameObject gaoParent = new GameObject(level);
            gaoParent.transform.SetParent(parent, false);
            gaoParent.transform.localScale = Vector3.one;
            gaoParent.transform.localRotation = Quaternion.identity;
            gaoParent.transform.localPosition = Vector3.zero;

            // Load the level scene
            LoadScene(loader.LevelScene, gaoParent.transform, loader.TexturesManager, "Level");
            
            // Load models outside the map for now
            Vector3 plbPos = new Vector3(-60000, 30000, 20000);

            foreach (PLB meshFile in loader.CommonMeshPack.MeshFiles.Concat(loader.LevelMeshPack.MeshFiles))
            {
                GameObject obj = LoadScene(meshFile.Scene, gaoParent.transform, loader.TexturesManager, meshFile.Name);
                obj.transform.position = plbPos;
                plbPos += new Vector3(meshFile.Scene.RootDomain.Bounds.Max.X - meshFile.Scene.RootDomain.Bounds.Min.X, 0, 0);
            }

            return gaoParent;
        }

        public GameObject LoadScene(Scene scene, Transform parent, TexturesManager texManager, string name)
        {
            GameObject sceneObj = new GameObject($"Scene: {name}");
            sceneObj.transform.SetParent(parent, false);
            sceneObj.transform.localScale = Vector3.one;
            sceneObj.transform.localRotation = Quaternion.identity;
            sceneObj.transform.localPosition = Vector3.zero;

            LoadDomain(scene.RootDomain, sceneObj.transform, texManager.GetTextures(scene.TextureTranslationTable));

            // Load referenced scenes
            if (scene.ReferencedScenes != null)
                foreach (Scene refScene in scene.ReferencedScenes)
                    LoadScene(refScene, sceneObj.transform, texManager, $"{name} References");

            return sceneObj;
        }

        public void LoadDomain(Domain domain, Transform parent, PsychonautsTexture[] textures)
        {
            GameObject domainObj = new GameObject($"Domain: {domain.Name}");
            domainObj.transform.SetParent(parent, false);
            domainObj.transform.localScale = Vector3.one;
            domainObj.transform.localRotation = Quaternion.identity;
            domainObj.transform.localPosition = Vector3.zero;

            // Load children
            foreach (Domain domainChild in domain.Children)
                LoadDomain(domainChild, domainObj.transform, textures);

            // Load meshes
            foreach (Mesh mesh in domain.Meshes)
                LoadMesh(mesh, domainObj.transform, textures);
        }

        public void LoadMesh(Mesh mesh, Transform parent, PsychonautsTexture[] textures)
        {
            GameObject meshObj = new GameObject($"Mesh: {mesh.Name}");
            meshObj.transform.SetParent(parent, false);
            meshObj.transform.localScale = Vector3.one;
            meshObj.transform.localRotation = Quaternion.identity;
            meshObj.transform.localPosition = Vector3.zero;

            foreach (Mesh meshChild in mesh.Children)
                LoadMesh(meshChild, meshObj.transform, textures);

            meshObj.transform.localPosition = mesh.Position.ToVector3();
            meshObj.transform.localRotation = mesh.Rotation.ToQuaternion();
            meshObj.transform.localScale = mesh.Scale.ToVector3();

            int skeletonsCount = mesh.Skeletons.Length;

            Transform[][] bones = new Transform[skeletonsCount][];
            Matrix4x4[][] bindPoses = new Matrix4x4[skeletonsCount][];

            for (int skelIndex = 0; skelIndex < skeletonsCount; skelIndex++)
            {
                Skeleton s = mesh.Skeletons[skelIndex];

                GameObject skeletonObj = new GameObject($"Skeleton: {s.Name}");
                skeletonObj.transform.SetParent(meshObj.transform, false);
                skeletonObj.transform.localPosition = Vector3.zero;
                skeletonObj.transform.localRotation = Quaternion.identity;
                skeletonObj.transform.localScale = Vector3.one;

                bones[skelIndex] = new Transform[s.JointsCount];
                bindPoses[skelIndex] = new Matrix4x4[bones[skelIndex].Length];

                addJoint(s.RootJoint, skeletonObj.transform);

                void addJoint(Joint j, Transform parentJoint)
                {
                    GameObject jointObj = new GameObject($"Joint_{j.ID.JointIndex}: {j.Name}");
                    jointObj.transform.SetParent(parentJoint);
                    bones[skelIndex][j.ID.JointIndex] = jointObj.transform;

                    jointObj.transform.localPosition = j.Position.ToVector3();

                    if (j.Rotation.ToVector3() == new Vector3(-1, -1, -1))
                        jointObj.transform.localRotation = Quaternion.identity;
                    else
                        jointObj.transform.localRotation = j.Rotation.ToQuaternion();
                    
                    jointObj.transform.localScale = Vector3.one;
                    bindPoses[skelIndex][j.ID.JointIndex] = jointObj.transform.worldToLocalMatrix * meshObj.transform.localToWorldMatrix;

                    foreach (Joint child in j.Children)
                        addJoint(child, jointObj.transform);
                }
            }

            for (var i = 0; i < mesh.MeshFrags.Length; i++)
            {
                MeshFrag meshFrag = mesh.MeshFrags[i];

                LoadMeshFrag(meshFrag, meshObj.transform, i, textures, bones, bindPoses);
            }
        }
        
        public void LoadMeshFrag(MeshFrag meshFrag, Transform parent, int index, PsychonautsTexture[] textures, Transform[][] bones, Matrix4x4[][] bindPoses)
        {
            GameObject meshFragObj = new GameObject(
                $"Frag: {index}, " +
                $"Blend Shapes: {meshFrag.BlendshapeData?.Streams.Length ?? 0}, " +
                $"VertexStreamBasis: {meshFrag.VertexStreamBasis?.Length ?? 0}, " +
                $"Textures: {meshFrag.TextureIndices.Length}, " +
                $"Flags: {meshFrag.MaterialFlags}");
            meshFragObj.transform.SetParent(parent, false);

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

            // Temporary code for visualizing the blend shapes
            if (meshFrag.BlendshapeData != null)
            {
                BlendTestComponent ba = meshFragObj.AddComponent<BlendTestComponent>();
                ba.mesh = unityMesh;
                ba.blendStreams = meshFrag.BlendshapeData.Streams;
                ba.vertices = meshFrag.Vertices.Select(x => x.Vertex.ToVector3()).ToArray();
                ba.speed = new AnimSpeed_SecondIncrease(1f);
            }

            Material matSrc;

            if (meshFrag.MaterialFlags.HasFlag(MaterialFlags.AdditiveBlending))
                matSrc = Controller.obj.levelController.controllerTilemap.unlitAdditiveMaterial;
            else
                matSrc = Controller.obj.levelController.controllerTilemap.unlitTransparentCutoutMaterial;

            Material mat = new Material(matSrc);

            mat.color = meshFrag.MaterialColor.ToVector4();

            if (meshFrag.AnimInfo != null)
            {
                // TODO: Right now this is hard-coded to only ever use the first skeleton. However there can be multiple, like for Raz.

                BoneWeight[] weights = meshFrag.AnimInfo.OriginalSkinWeights.Select(x => new BoneWeight()
                {
                    boneIndex0 = meshFrag.AnimInfo.JointIDs[x.Joint1].JointIndex,
                    weight0 = x.Weight.X,
                    boneIndex1 = meshFrag.AnimInfo.JointIDs[x.Joint2].JointIndex,
                    weight1 = 1 - x.Weight.X,
                }).ToArray();
                unityMesh.boneWeights = weights;
                unityMesh.bindposes = bindPoses[0];

                SkinnedMeshRenderer smr = meshFragObj.AddComponent<SkinnedMeshRenderer>();
                smr.sharedMaterial = mat;
                smr.sharedMesh = unityMesh;
                smr.bones = bones[0];
                smr.rootBone = bones[0][0];
            }
            else
            {
                MeshRenderer mr = meshFragObj.AddComponent<MeshRenderer>();
                mr.sharedMaterial = mat;
            }

            PsychonautsTexture tex = textures.ElementAtOrDefault((int?)meshFrag.TextureIndices?.FirstOrDefault() ?? -1);
            
            if (tex != null)
            {
                mat.mainTexture = tex.Texture;

                if (tex.IsAnimated)
                {
                    TextureAnimationComponent animTex = meshFragObj.AddComponent<TextureAnimationComponent>();
                    animTex.SetTexture(tex, mat);
                    //Debug.Log($"Texture {tex.GameTexture.FileName} is animated with {tex.AnimInfo.FramesCount} frames");
                }

                unityMesh.SetUVs(meshFrag, 0);
            }
        }

        #endregion
    }

    // TODO: Remove once animations have been implemented
    public class BlendTestComponent : ObjectAnimationComponent
    {
        public UnityMesh mesh;
        public BlendshapeStream[] blendStreams;
        public Vector3[] vertices;

        protected override void UpdateAnimation()
        {
            if (mesh == null || blendStreams == null)
                return;

            speed.Update(blendStreams.Length, loopMode);

            int frameInt = speed.CurrentFrameInt;

            int nextFrameIndex = frameInt + 1 * speed.Direction;

            if (nextFrameIndex >= blendStreams.Length)
            {
                switch (loopMode)
                {
                    case AnimLoopMode.Repeat:
                        nextFrameIndex = 0;
                        break;

                    case AnimLoopMode.PingPong:
                        nextFrameIndex = blendStreams.Length - 1;
                        break;
                }
            }
            else if (nextFrameIndex < 0)
            {
                switch (loopMode)
                {
                    case AnimLoopMode.PingPong:
                        nextFrameIndex = 1;
                        break;
                }
            }

            BlendshapeStream currentFrame = blendStreams[frameInt];
            BlendshapeStream nextFrame = blendStreams[nextFrameIndex];

            float lerpFactor = speed.CurrentFrame - frameInt;

            Vector3[] newVertices = Enumerable.Range(0, vertices.Length)
                .Select(x =>
                {
                    Vector3 current = x >= currentFrame.Vertices.Length
                        ? new Vector3()
                        : currentFrame.Vertices[x].Vertex.ToVector3() * currentFrame.Scale;
                    Vector3 next = x >= nextFrame.Vertices.Length
                        ? new Vector3()
                        : nextFrame.Vertices[x].Vertex.ToVector3() * nextFrame.Scale;

                    return Vector3.Lerp(current, next, lerpFactor) + vertices[x];
                }).
                ToArray();

            mesh.SetVertices(newVertices);
        }
    }
}