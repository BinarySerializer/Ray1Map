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
        public Camera camera3D;
        public Camera camera2DOverlay;
        public Camera camera3DOverlay;
        public bool FreeLookMode { get; set; } = false;

        void Start() {
            Camera.main.orthographicSize = fov;
            fricStart = friction;
            editor = FindObjectOfType<LevelEditorBehaviour>();
        }

        void Update() {
            if (FreeLookMode) {
                UpdateFreeLook();
                return;
            }

            //Allow RMB panning only in certain modes, otherwise force wasd movement
            bool canPan = (editor.currentMode == LevelEditorBehaviour.EditMode.Events || editor.currentMode == LevelEditorBehaviour.EditMode.Links);

            if (Settings.LoadFromMemory && Controller.obj.levelEventController.hasLoaded && Settings.FollowRaymanInMemoryMode)
            {
                var rayman = LevelEditorData.Level.Rayman;

                if (rayman != null)
                    pos = new Vector3(rayman.XPosition / (float)LevelEditorData.Level.PixelsPerUnit, -(rayman.YPosition / (float)LevelEditorData.Level.PixelsPerUnit));
            }

            Vector3 mouseDeltaOrtho = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            if (LevelEditorData.Level != null) {
                // RMB scroling
                if (Input.GetMouseButton(1) && canPan) {
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
                camera2DOverlay.orthographicSize = Camera.main.orthographicSize;

                if (LevelEditorData.Level?.IsometricData != null) {
                    // Update 3D camera
                    float scl = 1f;
                    Quaternion rot3D = Quaternion.Euler(30f, -45, 0);
                    camera3D.transform.rotation = rot3D;
                    Vector3 v = rot3D * Vector3.back;
                    float w = LevelEditorData.Level.IsometricData.TilesWidth * levelTilemapController.CellSizeInUnits;
                    float h = (LevelEditorData.Level.IsometricData.TilesHeight) * levelTilemapController.CellSizeInUnits;
                    float colH = (LevelEditorData.Level.IsometricData.CollisionWidth + LevelEditorData.Level.IsometricData.CollisionHeight);
                    /*if (!camera3D.gameObject.activeSelf) {
                        Debug.Log(LevelEditorData.Level.IsometricData.TilesWidth
                            + "x" + LevelEditorData.Level.IsometricData.TilesHeight
                            + " - " + LevelEditorData.Level.IsometricData.CollisionWidth
                            + "x" + LevelEditorData.Level.IsometricData.CollisionHeight);
                    }*/
                    camera3D.orthographic = true;
                    camera3D.transform.position = v * 300 + rot3D * ((pos - new Vector3(w / 2f, colH / 2f - h / 2f, 0f)) / scl); // Move back 300 units
                    camera3D.orthographicSize = Camera.main.orthographicSize / scl;

                    // Activate
                    if (!camera3D.gameObject.activeSelf) camera3D.gameObject.SetActive(true);
                    camera3DOverlay.orthographicSize = camera3D.orthographicSize;
                    camera2DOverlay.cullingMask &= ~(1 << LayerMask.NameToLayer("3D Overlay"));


                    if (Input.GetKeyDown(KeyCode.F)) {
                        FreeLookMode = true;
                        cullingMask = Camera.main.cullingMask;
                        cullingMask2DOverlay = camera2DOverlay.cullingMask;
                    }
                }
            }
        }

        void UpdateFreeLook() {
            if (!targetDirection.HasValue) {
                // Set target direction to the camera's initial orientation.
                targetDirection = camera3D.transform.localRotation;
            }
            if (Input.GetKeyDown(KeyCode.F)) {
                FreeLookMode = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                camera3D.orthographic = true;
                Camera.main.cullingMask = cullingMask;
                camera2DOverlay.cullingMask = cullingMask2DOverlay;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                camera3D.orthographic = false;
                Camera.main.cullingMask = 0;
                camera2DOverlay.cullingMask = 0;
            }

            CalculateMouseLook(false, true);

            //movement
            CameraControlsPerspective();
            CameraControlsSpeed();
        }

        #region Camera Controls

        public float flySpeed = 20f;
        public float flySpeedShiftMultiplier = 2.0f;
        private float flySpeedFactor = 30f;
        private float _flySpeedShiftMultiplier = 1.0f;
        Vector2 _mouseAbsolute;
        Vector2 _smoothMouse;

        public Vector2 clampInDegrees = new Vector2(360, 180);
        public Vector2 sensitivity = new Vector2(2, 2);
        public Vector2 sensitivityRMB = new Vector2(1.6f, 1.6f);
        public Vector2 smoothing = new Vector2(3, 3);
        public Quaternion? targetDirection;
        public float lerpFactor = 1f;
        private int cullingMask;
        private int cullingMask2DOverlay;


        void CameraControlsPerspective() {
            Camera cam = camera3D;
            if (Input.GetAxis("Mouse ScrollWheel") != 0) {
                flySpeed = Mathf.Max(0, flySpeed + Time.deltaTime * Input.GetAxis("Mouse ScrollWheel") * 100f * flySpeedFactor);
            }
            if (Input.GetAxis("Vertical") != 0) {
                cam.transform.Translate(cam.transform.forward * flySpeed * Time.deltaTime * _flySpeedShiftMultiplier * Input.GetAxis("Vertical"), Space.World);
            }
            if (Input.GetAxis("Horizontal") != 0) {
                cam.transform.Translate(cam.transform.right * flySpeed * Time.deltaTime * _flySpeedShiftMultiplier * Input.GetAxis("Horizontal"), Space.World);
            }
            if (Input.GetAxis("HeightAndZoom") != 0) {
                cam.transform.Translate(Vector3.up * flySpeed * Time.deltaTime * _flySpeedShiftMultiplier * Input.GetAxis("HeightAndZoom"), Space.World);
            }
            if (Input.GetKey(KeyCode.Keypad8)) {
                cam.transform.Translate(Vector3.up * flySpeed * Time.deltaTime * 0.5f, Space.World);
            } else if (Input.GetKey(KeyCode.Keypad2)) {
                cam.transform.Translate(-Vector3.up * flySpeed * Time.deltaTime * 0.5f, Space.World);
            }
        }
        void CameraControlsSpeed() {
            if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus)) {
                flySpeed += Time.deltaTime * flySpeedFactor;
            } else if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus)) {
                flySpeed = Mathf.Max(0, flySpeed - Time.deltaTime * flySpeedFactor);
            }
        }
        #endregion

        void CalculateMouseLook(bool orthographic, bool smooth) {
            Camera cam = camera3D;
            // Allow the script to clamp based on a desired target value.
            Vector3 eulerTargetDir = targetDirection.Value.eulerAngles;
            /*bool eulerDirChanged = false;
            if (eulerTargetDir.z != 0) {
                if (Mathf.Abs(eulerTargetDir.z) < 0.04f) {
                    eulerTargetDir = new Vector3(eulerTargetDir.x, eulerTargetDir.y, 0f);
                } else {
                    eulerTargetDir = new Vector3(eulerTargetDir.x, eulerTargetDir.y, Mathf.Lerp(eulerTargetDir.z, 0f, Time.deltaTime * lerpFactor));
                }
                eulerDirChanged = true;
            } else if (eulerTargetDir.x != 0) {
                //eulerTargetDir = new Vector3(0f, eulerTargetDir.y, eulerTargetDir.z);
                if (Mathf.Abs(eulerTargetDir.x) < 0.04f) {
                    eulerTargetDir = new Vector3(0f, eulerTargetDir.y, eulerTargetDir.z);
                } else {
                    eulerTargetDir = new Vector3(Mathf.Lerp(eulerTargetDir.x, 0f, Time.deltaTime * lerpFactor), eulerTargetDir.y, eulerTargetDir.z);
                }
                eulerDirChanged = true;
            }*/
            if (eulerTargetDir.x != 0 || eulerTargetDir.z != 0) {
                if (Mathf.Abs(eulerTargetDir.x) < 0.04f && Mathf.Abs(eulerTargetDir.z) < 0.04f) {
                    targetDirection = Quaternion.Euler(0f, eulerTargetDir.y, 0f);
                } else {
                    targetDirection = Quaternion.Lerp(targetDirection.Value, Quaternion.Euler(0, eulerTargetDir.y, 0), Time.deltaTime * lerpFactor);
                }
            }
            var targetOrientation = targetDirection.Value;

            // Get raw mouse input for a cleaner reading on more sensitive mice.
            var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            // Scale input against the sensitivity setting and multiply that against the smoothing value.
            var sensitivity = smooth ? this.sensitivity : sensitivityRMB;
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

            if (smooth) {
                // Interpolate mouse movement over time to apply smoothing delta.
                _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
                _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);
                // Find the absolute mouse movement value from point zero.
                _mouseAbsolute += _smoothMouse;
            } else {
                _mouseAbsolute += mouseDelta;
            }

            // Clamp and apply the local x value first, so as not to be affected by world transforms.
            if (clampInDegrees.x < 360)
                _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

            // Then clamp and apply the global y value.
            if (clampInDegrees.y < 360)
                _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

            if (!orthographic) {
                var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
                cam.transform.localRotation = xRotation * targetOrientation;

                // If there's a character body that acts as a parent to the camera
                var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, cam.transform.InverseTransformDirection(Vector3.up));
                cam.transform.localRotation *= yRotation;
            }
        }
    }
}
