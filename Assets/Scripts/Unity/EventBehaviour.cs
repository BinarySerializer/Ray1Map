﻿using UnityEngine;

namespace R1Engine {
    public class EventBehaviour : MonoBehaviour {
        public static GameObject resource { get {
                if (_resource == null) _resource = Resources.Load<GameObject>("Event");
                return _resource;
            } }
    static GameObject _resource;


        static Transform root;
        public Unity_SpriteObjBehaviour ev;
        public Transform icon;

        void Start() {
            if (ev == null)
                ev = new Unity_SpriteObjBehaviour();
            if (root == null) root = GameObject.Find("Events").transform;
            name = $"{root.childCount} | {ev.ObjData.Name}";
            transform.parent = root;
            transform.position = new Vector3(ev.ObjData.XPosition, ev.ObjData.YPosition) + Vector3.forward * 5;
        }
    }
}