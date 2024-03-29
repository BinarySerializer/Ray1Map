﻿using BinarySerializer.Klonoa.DTP;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public class KlonoaObjectsLoader
    {
        public KlonoaObjectsLoader(Loader loader, float scale, Unity_IsometricData isometricData, GameObject parentObject)
        {
            Loader = loader;
            Scale = scale;
            IsometricData = isometricData;
            ParentObject = parentObject;
            Anim_Manager = new PS1VRAMAnimationManager();

            BackgroundLayers = new List<KlonoaBackgroundLayer>();
            BackgroundClears = new List<BackgroundGameObjectData_Clear>();
            ScrollAnimations = new List<UVScrollAnimation_File>();
            Sprites = new List<GameObjectData_Sprites>();
            BGPaletteAnimations = new List<PS1VRAMAnimation>();
            PaletteAnimations = new List<PS1VRAMAnimation>();
            TextureAnimations = new List<PS1VRAMAnimation>();
            CameraAnimations = new List<CameraAnimations_File>();
        }

        // Properties
        public Loader Loader { get; }
        public float Scale { get; }
        public GameObject ParentObject { get; }
        public Unity_IsometricData IsometricData { get; }
        public PS1VRAMAnimationManager Anim_Manager { get; }

        // Loaded objects
        public KlonoaObject[] LoadedObjects { get; private set; }
        public bool GetIsAnimated => LoadedObjects?.Any(x => x.IsAnimated) ?? false;

        // Object assets
        public List<CameraAnimations_File> CameraAnimations { get; }
        public List<PS1VRAMAnimation> TextureAnimations { get; }
        public List<PS1VRAMAnimation> PaletteAnimations { get; }
        public List<PS1VRAMAnimation> BGPaletteAnimations { get; }
        public List<UVScrollAnimation_File> ScrollAnimations { get; }
        public List<GameObjectData_Sprites> Sprites { get; }
        public List<BackgroundGameObjectData_Clear> BackgroundClears { get; }
        public List<KlonoaBackgroundLayer> BackgroundLayers { get; }

        public IEnumerable<PS1VRAMAnimation> Anim_GetAnimationsFromRegion(RectInt textureRegion, RectInt palRegion) => 
            PaletteAnimations.Where(x => x.Overlaps(palRegion)).Concat(TextureAnimations.Where(x => x.Overlaps(textureRegion)));

        public IEnumerable<PS1VRAMAnimation> Anim_GetBGAnimationsFromRegion(RectInt palRegion) => 
            BGPaletteAnimations.Where(x => x.Overlaps(palRegion));

        public async UniTask LoadAsync(GameObjectDefinition[] gameObjects3D, BackgroundGameObject[] backgroundObjects)
        {
            var objects = new List<KlonoaObject>();

            // Add 3D and background objects
            objects.AddRange(gameObjects3D.Where(x => x.Data != null).Select(x => new KlonoaGameObject3D(this, x.Data)));

            if (backgroundObjects != null)
                objects.AddRange(backgroundObjects.Select(x => new KlonoaBackgroundObject(this, x)));

            // Load hard-coded objects (cutscenes and boss objects)
            BaseHardCodedObjectsLoader hardCodedObjectsLoader = Loader.Settings.GetHardCodedObjectsLoader(Loader);
            hardCodedObjectsLoader.LoadObjects();
            objects.AddRange(hardCodedObjectsLoader.GameObjects.Select(x => new KlonoaGameObject3D(this, x)));

            // Load animations
            for (var objIndex = 0; objIndex < objects.Count; objIndex++)
            {
                KlonoaObject obj = objects[objIndex];

                Controller.DetailedState = $"Loading game object animations {objIndex + 1}/{objects.Count}";
                await Controller.WaitIfNecessary();

                obj.LoadAnimations();
            }

            // Load objects
            for (var objIndex = 0; objIndex < objects.Count; objIndex++)
            {
                KlonoaObject obj = objects[objIndex];

                Controller.DetailedState = $"Loading game object animations {objIndex + 1}/{objects.Count}";
                await Controller.WaitIfNecessary();

                obj.LoadObject();

                // Set the parent object
                if (obj.GameObject != null)
                    obj.GameObject.transform.SetParent(ParentObject.transform);
            }

            LoadedObjects = objects.ToArray();
        }
    }
}