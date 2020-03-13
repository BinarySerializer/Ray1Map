using UnityEngine;
using static UnityEngine.Input;

namespace R1Engine {
    public class EditorCam : MonoBehaviour {
        public bool pixelSnap;
        public float fov = 15;
        public float inertia = 0.05f;
        public float friction = 3;
        public float WASDScrollSpeed = 5;
        [HideInInspector] public float fricStart;
        [HideInInspector] public Vector3 pos;
        [HideInInspector] public Vector3 vel;
        Vector3 mousePosPrev;
        Editor editor;

        void Start() {
            Camera.main.orthographicSize = fov;
            fricStart = friction;
            editor = FindObjectOfType<Editor>();

            /*if (Controller.obj.levelController != null) {
                var startEvent = Controller.obj.levelController.currentLevel.Events.FindItem(x => x.EventInfoData?.Type == 99 || x.EventInfoData?.Type == 124);

                if (startEvent != null)
                    pos = new Vector3(startEvent.XPosition, startEvent.YPosition);
            }*/
        }

        void Update() {
            if (Controller.obj.levelController.currentLevel != null) {
                // Right click scrolling
                if (GetMouseButton(1)) {
                    vel = 0.8f * Vector3.Lerp(vel, Vector3.ClampMagnitude(mousePosPrev - mousePosition, 50) * fov,
                        inertia <= 0 ? 1 : Time.deltaTime * 1f / inertia);
                    friction = fricStart;
                }

                // Mouse wheel zooming
                fov = Mathf.Clamp(fov - 0.25f * mouseScrollDelta.y * fov, 3.75f, 50);


                // WASD scrolling
                bool scrollLeft = GetKey(KeyCode.LeftArrow) || GetKey(KeyCode.A);
                bool scrollRight = GetKey(KeyCode.RightArrow) || GetKey(KeyCode.D);
                bool scrollUp = GetKey(KeyCode.UpArrow) || GetKey(KeyCode.W);
                bool scrollDown = GetKey(KeyCode.DownArrow) || GetKey(KeyCode.S);
                bool scrolling = scrollLeft || scrollRight || scrollUp || scrollDown;
                if (scrolling || editor.scrolling) friction = 30;

                float scr = friction * Camera.main.orthographicSize * WASDScrollSpeed * Time.deltaTime;
                if (scrollLeft) vel.x -= scr;
                if (scrollRight) vel.x += scr;
                if (scrollUp) vel.y += scr;
                if (scrollDown) vel.y -= scr;


                // Stuff
                vel /= 1f + (1f * friction) * Time.deltaTime;
                pos += vel * Time.deltaTime;
                pos.x = Mathf.Clamp(pos.x, 0, Controller.obj.levelController.currentLevel.Width);
                pos.y = Mathf.Clamp(pos.y, -Controller.obj.levelController.currentLevel.Height, 0);
                pos.z = -10f;
                if (pixelSnap) {
                    transform.position = PxlVec.SnapVec(pos);
                }else {
                    transform.position = pos;
                }
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, fov, Time.deltaTime * 8);
                mousePosPrev = mousePosition;
            }
        }
    }
}
