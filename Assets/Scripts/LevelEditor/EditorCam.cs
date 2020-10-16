using UnityEngine;
using UnityEngine.EventSystems;

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
        Vector3 lastMousePosition;
        Vector3? panStart;
        bool panning = false;
        LevelEditorBehaviour editor;

        public LevelTilemapController levelTilemapController;

        void Start() {
            Camera.main.orthographicSize = fov;
            fricStart = friction;
            editor = FindObjectOfType<LevelEditorBehaviour>();
        }

        void Update() {

            if (Settings.LoadFromMemory && Controller.obj.levelEventController.hasLoaded && Settings.FollowRaymanInMemoryMode)
            {
                var rayman = LevelEditorData.Level.Rayman;

                if (rayman != null)
                    pos = new Vector3(rayman.XPosition / (float)LevelEditorData.Level.PixelsPerUnit, -(rayman.YPosition / (float)LevelEditorData.Level.PixelsPerUnit));
            }


            Vector3 mouseDeltaOrtho = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;


            if (LevelEditorData.Level != null) {
                // MMB scroling
                if (Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftControl)) {
                    if (!panStart.HasValue) {
                        panStart = Input.mousePosition;
                    }
                    Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

                    if (panning) {

                        float xFactor = Camera.main.orthographicSize * 2.0f / Camera.main.pixelHeight;
                        float yFactor = Camera.main.orthographicSize * 2.0f / Camera.main.pixelHeight;

                        /*vel = 0.8f * Vector3.Lerp(vel, Vector3.ClampMagnitude(mousePosPrev - mousePosition, 50) * fov,
                            inertia <= 0 ? 1 : Time.deltaTime * 1f / inertia);*/
                        friction = fricStart;
                        pos += new Vector3(-mouseDeltaOrtho.x * xFactor, -mouseDeltaOrtho.y * yFactor);
                    } else if (Input.GetMouseButtonDown(1) && screenRect.Contains(Input.mousePosition)) { // Only start panning if within game window when you click
                        panning = true;
                    }
                } else {
                    panStart = null;
                    panning = false;
                }

                // Mouse wheel zooming
                if (!EventSystem.current.IsPointerOverGameObject())
                    fov = Mathf.Clamp(fov - 0.25f * Input.mouseScrollDelta.y * fov, 3.75f, 50);


                // WASD scrolling
                bool scrollLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
                bool scrollRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
                bool scrollUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
                bool scrollDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
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
                pos.x = Mathf.Clamp(pos.x, 0, levelTilemapController.camMaxX * levelTilemapController.CellSizeInUnits);
                pos.y = Mathf.Clamp(pos.y, -levelTilemapController.camMaxY * levelTilemapController.CellSizeInUnits, 0);

                pos.z = -10f;
                if (pixelSnap) {
                    transform.position = PxlVec.SnapVec(pos);
                }else {
                    transform.position = pos;
                }
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, fov, Time.deltaTime * 8);
            }
        }
    }
}
