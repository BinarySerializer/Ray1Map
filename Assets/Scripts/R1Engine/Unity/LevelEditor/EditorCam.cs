using UnityEngine;
using static UnityEngine.Input;

namespace R1Engine.Unity {
    public class EditorCam : MonoBehaviour {
        public bool pixelSnap;
        public float fov = 15;
        public float inertia = 0.05f;
        public float friction = 3;
        Vector3 vel;
        Vector3 mousePosPrev;
        Vector3 pos;

        void Start() {
            Camera.main.orthographicSize = fov;

            var startEvent = Controller.obj.levelController.currentLevel.Events.FindItem(x => (int)x.type == 99 || (int)x.type == 124);

            if (startEvent != null)
                pos = startEvent.pos;
        }

        void Update() {
            if (GetMouseButton(1)) {
                vel = 0.8f * Vector3.Lerp(vel, Vector3.ClampMagnitude(mousePosPrev - mousePosition, 50) * fov,
                    inertia <= 0 ? 1 : Time.deltaTime *  1f / inertia);
            }

            fov = Mathf.Clamp(fov - 0.25f * mouseScrollDelta.y * fov, 3.75f, 50);

            vel /= 1f + (1f * friction) * Time.deltaTime;
            pos += vel * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, 0, Controller.obj.levelController.currentLevel.Width);
            pos.y = Mathf.Clamp(pos.y, -Controller.obj.levelController.currentLevel.Height, 0);
            if (pixelSnap) transform.position = PxlVec.SnapVec(pos);
            else transform.position = pos;
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, fov, Time.deltaTime * 8);
            mousePosPrev = mousePosition;
        }
    }
}
