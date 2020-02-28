using UnityEngine;

namespace R1Engine.Unity {
    public class EventBehaviour : MonoBehaviour {
        public static GameObject resource { get {
                if (_resource == null) _resource = Resources.Load<GameObject>("Event");
                return _resource;
            } }
    static GameObject _resource;


        static Transform root;
        public Event ev;
        public Transform icon;

        void Start() {
            if (ev == null)
                ev = new Event();
            if (root == null) root = GameObject.Find("Events").transform;
            name = $"{root.childCount} | {ev.behaviour}";
            transform.parent = root;
            transform.position = ev.pos + Vector3.forward * 5;
        }
    }
}