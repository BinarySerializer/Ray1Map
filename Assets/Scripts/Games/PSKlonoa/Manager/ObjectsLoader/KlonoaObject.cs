using System.Collections.Generic;
using BinarySerializer.PS1;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public abstract class KlonoaObject
    {
        protected KlonoaObject(KlonoaObjectsLoader objLoader)
        {
            ObjLoader = objLoader;
            GameObjects = new List<GameObject>();
        }

        // Global
        public KlonoaObjectsLoader ObjLoader { get; }
        public PS1_VRAM VRAM => ObjLoader.Loader.VRAM;
        public float Scale => ObjLoader.Scale;

        // Object
        public List<GameObject> GameObjects { get; }
        public bool IsAnimated { get; protected set; }

        public abstract void LoadAnimations();
        public abstract void LoadObject();
    }
}