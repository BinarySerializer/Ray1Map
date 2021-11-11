using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ray1Map {
    public class EditorCam : MonoBehaviour {

        public bool pixelSnap;
        public float fov = 15;
        public float inertia = 0.05f;
        public float friction = 3;
        public float WASDScrollSpeed = 5;
        public float orthographicZPosition = -10f;

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
        public bool FreeCameraMode { get; set; } = false;
        private bool _freeCameraMode = false;

        public float minZoomOrthographic = 3.75f;
        public float maxZoomOrthographic = 50f;

        void Start() {
            Camera.main.orthographicSize = fov;
            fricStart = friction;
            editor = FindObjectOfType<LevelEditorBehaviour>();
        }

        void Update() {
            if (_freeCameraMode) {
                UpdateFreeLook();
            } else {
                UpdateOrthographic();
            }
        }

        private bool? storedCollisionSetting = null;
        public bool ToggleFreeCameraMode(bool freeCameraMode, bool setShowCollision = true) {
            bool wasFreeCameraMode = FreeCameraMode;
            bool wasTrackMoving = IsTrackMovingEnabled;
            FreeCameraMode = freeCameraMode;
            if (wasTrackMoving && wasFreeCameraMode != FreeCameraMode && FreeCameraMode == false) {
                StopMovingAlongTrack();
            }
            if (setShowCollision && wasFreeCameraMode != FreeCameraMode && LevelEditorData.Level?.IsometricData?.Collision != null) {
                if (freeCameraMode) { // On enable free camera mode
                    storedCollisionSetting = Settings.ShowCollision;
                    Settings.ShowCollision = true;
                    return true;
                } else { // On disable
                    if (storedCollisionSetting.HasValue) {
                        Settings.ShowCollision = storedCollisionSetting.Value;
                        storedCollisionSetting = null;
                        return true;
                    }
                }
            }
            if(wasTrackMoving && !IsTrackMovingEnabled) return true;
            return false;
        }

        public void InitCamera3D() {
            UpdateOrthographic_Camera3D();
        }

        private void UpdateOrthographic_Camera3D() {
            if (LevelEditorData.Level != null) {

                if (LevelEditorData.Level?.IsometricData != null) {
                    // Update 3D camera
                    float scl = 1f;
                    Quaternion rot3D = LevelEditorData.Level.IsometricData.ViewAngle;
                    camera3D.transform.rotation = rot3D;
                    Vector3 v = rot3D * Vector3.back;
                    float w = LevelEditorData.Level.IsometricData.TilesWidth * levelTilemapController.CellSizeInUnits;
                    float h = (LevelEditorData.Level.IsometricData.TilesHeight) * levelTilemapController.CellSizeInUnits;
                    float colYDisplacement = LevelEditorData.Level.IsometricData.CalculateYDisplacement();
                    float colXDisplacement = LevelEditorData.Level.IsometricData.CalculateXDisplacement();
                    /*if (!camera3D.gameObject.activeSelf) {
                        Debug.Log(LevelEditorData.Level.IsometricData.TilesWidth
                            + "x" + LevelEditorData.Level.IsometricData.TilesHeight
                            + " - " + LevelEditorData.Level.IsometricData.CollisionWidth
                            + "x" + LevelEditorData.Level.IsometricData.CollisionHeight);
                    }*/
                    camera3D.orthographic = true;
                    camera3D.transform.position = v * 300 + rot3D * ((pos -
                        new Vector3((w - colXDisplacement) / 2f, -(h - colYDisplacement) / 2f, 0f)) / scl); // Move back 300 units
                    camera3D.orthographicSize = Camera.main.orthographicSize / scl;

                    // Activate
                    if (!camera3D.gameObject.activeSelf) camera3D.gameObject.SetActive(true);
                    camera3DOverlay.orthographicSize = camera3D.orthographicSize;
                    camera3DOverlay.orthographic = true;
                    camera2DOverlay.cullingMask &= ~(1 << LayerMask.NameToLayer("3D Overlay"));


                    if (FreeCameraMode) {
                        StopLerp();
                        _freeCameraMode = true;
                        cullingMask = Camera.main.cullingMask;
                        cullingMask2DOverlay = camera2DOverlay.cullingMask;
                        UpdateCullingMask(_freeCameraMode);

                        if (LevelEditorData.Level.StartIn3D)
                        {
                            camera3D.transform.position = LevelEditorData.Level.StartPosition;
                            camera3D.transform.rotation = LevelEditorData.Level.StartRotation;
                        }
                    }
                }
            }
        }

        void UpdateOrthographic() {
            //Allow RMB panning only in certain modes, otherwise force wasd movement
            bool canPan = (editor.currentMode == LevelEditorBehaviour.EditMode.Events || editor.currentMode == LevelEditorBehaviour.EditMode.Links);

            if (Settings.LoadFromMemory && Controller.obj.levelEventController.hasLoaded && Settings.FollowRaymanInMemoryMode)
            {
                StopLerp();
                var rayman = LevelEditorData.Level.Rayman;

                if (rayman != null)
                    pos = new Vector3(rayman.XPosition / (float)LevelEditorData.Level.PixelsPerUnit, -(rayman.YPosition / (float)LevelEditorData.Level.PixelsPerUnit));
            }

            Vector3 mouseDeltaOrtho = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            if (LevelEditorData.Level != null) {
                // RMB scroling
                if (Input.GetMouseButton(1) && canPan) {
                    StopLerp();
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
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    if(Input.mouseScrollDelta.y != 0) StopLerp();
                    fov = Mathf.Clamp(fov - 0.25f * Input.mouseScrollDelta.y * fov, minZoomOrthographic, maxZoomOrthographic);
                }


                // WASD scrolling
                bool scrollLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
                bool scrollRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
                bool scrollUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
                bool scrollDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
                bool scrolling = scrollLeft || scrollRight || scrollUp || scrollDown;
                if (scrolling || editor.scrolling) {
                    StopLerp();
                    friction = 30;
                }

                float scr = friction * Camera.main.orthographicSize * WASDScrollSpeed * Time.deltaTime;
                if (scrollLeft) vel.x -= scr;
                if (scrollRight) vel.x += scr;
                if (scrollUp) vel.y += scr;
                if (scrollDown) vel.y -= scr;


                // Stuff
                vel /= 1f + (1f * friction) * Time.deltaTime;
                pos += vel * Time.deltaTime;

                // Lerp
                var cam = this;
                if (targetPos.HasValue) {
                    if (Vector3.Distance(pos, targetPos.Value) < 0.4f) {
                        targetPos = null;
                    } else {
                        pos = Vector3.Lerp(pos, targetPos.Value, Time.deltaTime * lerpFactor);
                    }
                }
                if (targetOrthoSize.HasValue) {
                    if (Mathf.Abs(targetOrthoSize.Value - cam.fov) < 0.04f) {
                        targetOrthoSize = null;
                    } else {
                        cam.fov = Mathf.Lerp(cam.fov, targetOrthoSize.Value, Time.deltaTime * lerpFactor);
                    }
                }

                // Limit position
                var bounds = levelTilemapController.CameraBounds;
                pos.x = Mathf.Clamp(pos.x, bounds.xMin, bounds.xMax);
                pos.y = Mathf.Clamp(pos.y, bounds.yMin, bounds.yMax);
                pos.z = orthographicZPosition;
                // Apply position
                if (pixelSnap) {
                    transform.position = PxlVec.SnapVec(pos);
                }else {
                    transform.position = pos;
                }
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, fov, Time.deltaTime * 8);
                camera2DOverlay.orthographicSize = Camera.main.orthographicSize;

                UpdateOrthographic_Camera3D();
            }
        }

        public void UpdateCullingMask(bool is3D) {
            if (is3D) {
                camera3D.cullingMask |= (1 << LayerMask.NameToLayer("Tiles"));
                camera3D.cullingMask |= (1 << LayerMask.NameToLayer("Collision"));
            } else {
                camera3D.cullingMask &= ~(1 << LayerMask.NameToLayer("Tiles"));
                camera3D.cullingMask &= ~(1 << LayerMask.NameToLayer("Collision"));
            }

        }

        void UpdateFreeLook() {
            MouseLookRMBEnabled = false;

            if (!FreeCameraMode) {
                _freeCameraMode = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                MouseLookEnabled = false;
                MouseLookRMBEnabled = false;
                camera3D.orthographic = true;
                camera3DOverlay.orthographic = true;
                Camera.main.cullingMask = cullingMask;
                camera2DOverlay.cullingMask = cullingMask2DOverlay;
                targetDirection = null;
                UpdateCullingMask(_freeCameraMode);
                return;
            }
            if (!targetDirection.HasValue) {
                // Set target direction to the camera's initial orientation.
                targetDirection = camera3D.transform.localRotation;
            }
            camera3D.orthographic = false;
            camera3DOverlay.orthographic = false;
            camera3DOverlay.fieldOfView = camera3D.fieldOfView;
            Camera.main.cullingMask = 0;
            //camera2DOverlay.cullingMask = 0;
            CheckShifted();

            if (IsTrackMovingEnabled) {
                _shifted = false;
                MouseLookEnabled = false;
                MouseLookRMBEnabled = false;
                StopLerp();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                //movement
                CameraControlsPerspective();
                CameraControlsSpeed();
                MoveAlongTrack();
            } else if (!MouseLookEnabled) {
                Camera cam = camera3D;
                if (targetPos.HasValue) {
                    if (Vector3.Distance(cam.transform.position, targetPos.Value) < 0.4f) {
                        targetPos = null;
                    } else {
                        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos.Value, Time.deltaTime * lerpFactor);
                    }
                }
                if (targetRot.HasValue) {
                    if (Mathf.Abs(Quaternion.Angle(cam.transform.rotation, targetRot.Value)) < 10) {
                        targetRot = null;
                    } else {
                        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetRot.Value, Time.deltaTime * lerpFactor);
                    }
                }

                // Right click: drag also works
                if (Input.GetMouseButton(1)) {
                    MouseLookRMBEnabled = true;
                    StopLerp();

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    CalculateMouseLook(false, false);

                    if (Input.GetKey(KeyCode.LeftShift)) {
                        _flySpeedShiftMultiplier = flySpeedShiftMultiplier;
                    } else {
                        _flySpeedShiftMultiplier = 1.0f;
                    }

                    CameraControlsPerspective();
                }

                if (Input.GetMouseButtonUp(1)) {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    _flySpeedShiftMultiplier = 1.0f;
                }
                CameraControlsSpeed();
            } else {
                StopLerp();
                //ensure these stay this way
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                CalculateMouseLook(false, true);

                //movement
                CameraControlsPerspective();
                CameraControlsSpeed();
            }
        }

        #region Camera Controls
        public bool MouseLookEnabled { get; private set; } = false;
        public bool MouseLookRMBEnabled { get; private set; } = false;
        private bool _shifted = false;
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
        private int cullingMask;
        private int cullingMask2DOverlay;


        void CameraControlsPerspective() {
            Camera cam = camera3D;
            if (Input.GetAxis("Mouse ScrollWheel") != 0) {
                flySpeed = Mathf.Max(0, flySpeed + Time.deltaTime * Input.GetAxis("Mouse ScrollWheel") * 100f * flySpeedFactor);
            }
            if (!IsTrackMovingEnabled) {
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
            } else {
                TrackDistance += flySpeed * Time.deltaTime * _flySpeedShiftMultiplier * Input.GetAxis("Vertical");
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

        public void CheckShifted() {
            if (Input.GetKeyUp(KeyCode.LeftShift) & _shifted)
                _shifted = false;

            if ((Input.GetKeyDown(KeyCode.LeftShift) & !_shifted && !Input.GetMouseButton(1)) |
                (Input.GetKeyDown(KeyCode.Escape) & MouseLookEnabled)) {
                _shifted = true;

                if (!MouseLookEnabled) {
                    MouseLookEnabled = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                } else {
                    if (Input.GetKeyDown(KeyCode.Escape))
                        _shifted = false;

                    MouseLookEnabled = false;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }


        // Lerp
        public float lerpFactor = 6f;
        public Quaternion? targetDirection;
        Vector3? targetPos = null;
        Quaternion? targetRot = null;
        float? targetOrthoSize = null;

        public void StopLerp() {
            targetPos = null;
            targetRot = null;
            targetOrthoSize = null;
        }


        public void JumpTo(GameObject gao, bool immediate = false) {
            Vector3? center = null, size = null;
            Unity_SpriteObjBehaviour obj = gao.GetComponent<Unity_SpriteObjBehaviour>();
            if (obj != null) {
                if (obj.ObjData is Unity_SpriteObject_3D && LevelEditorData.Level?.IsometricData != null) {
                    bool orthographic = !_freeCameraMode;
                    if (orthographic) {
                        Vector3 target = camera3D.transform.InverseTransformPoint(obj.midpoint);
                        center = transform.TransformPoint(new Vector3(target.x, target.y, orthographicZPosition));
                        //center = obj.midpoint;
                    } else {
                        center = obj.midpoint;
                    }
                    if (obj.boxCollider3D == null)
                        size = Vector3.one;
                    else
                        size = obj.boxCollider3D.size;
                } else {
                    center = obj.midpoint;
                    size = obj.boxCollider?.size ?? Vector3.one;
                }
            } else {
                center = gao.transform.position;
                size = gao.transform.lossyScale;
            }
            if (center.HasValue) {
                StopMovingAlongTrack();
                float objectSize = Mathf.Min(5f, Mathf.Max(2.25f,size.Value.x, size.Value.y, size.Value.z));
                bool orthographic = !_freeCameraMode;
                if (orthographic) {
                    var cam = this;
                    var targetSize = objectSize * 2f * 1.5f;
                    if (Mathf.Abs(fov - targetSize) > 5f) {
                        targetOrthoSize = Mathf.Lerp(fov, targetSize, 0.75f);
                        targetOrthoSize = Mathf.Clamp(targetOrthoSize.Value, minZoomOrthographic, maxZoomOrthographic);
                    }
                    //targetOrthoSize = objectSize * 2f * 1.5f;
                    Vector3 target = cam.transform.InverseTransformPoint(center.Value);
                    targetPos = cam.transform.TransformPoint(new Vector3(target.x, target.y, orthographicZPosition));
                    if (immediate) {
                        if (targetOrthoSize.HasValue) {
                            fov = targetOrthoSize.Value;
                            Camera.main.orthographicSize = fov;
                            targetOrthoSize = null;
                        }
                        if (targetPos.HasValue) {
                            pos = targetPos.Value;
                            // Apply position
                            if (pixelSnap) {
                                transform.position = PxlVec.SnapVec(pos);
                            } else {
                                transform.position = pos;
                            }
                            // Set to null
                            targetPos = null;
                        }
                    }
                } else {
                    var cam = camera3D;
                    float cameraDistance = 4.0f; // Constant factor
                    float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * cam.fieldOfView); // Visible height 1 meter in front
                    float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
                    distance += objectSize; // Estimated offset from the center to the outside of the object * 2
                    targetPos = center.Value + Vector3.Normalize(cam.transform.position - center.Value) * distance;
                    if (center.Value - transform.position != Vector3.zero) {
                        targetRot = Quaternion.LookRotation(center.Value - cam.transform.position, Vector3.up);
                    }
                    if (immediate) {
                        if (!MouseLookEnabled) {
                            if (targetPos.HasValue) cam.transform.position = targetPos.Value;
                            if (targetRot.HasValue) cam.transform.rotation = targetRot.Value;
                        }
                        targetPos = null;
                        targetRot = null;
                    }
                }
            }
        }

        public bool IsTrackMovingEnabled;
        public float TrackDistance { get; set; }


        public bool ToggleTrackMoving(bool trackMoving, bool setFreeCamera = true, bool setShowCollision = true) 
        {
            var manager = LevelEditorData.Level.SelectedTrackManager;

            // Make sure track data is available
            if (manager == null || !LevelEditorData.Level.CanMoveAlongTrack)
                return false ;

            bool wasTrackMoving = IsTrackMovingEnabled;
            bool wasFreeCameraMode = FreeCameraMode;
            IsTrackMovingEnabled = trackMoving;
            if (trackMoving && !wasTrackMoving) {
                TrackDistance = 0f;

                // Disable lerp
                targetOrthoSize = null;
                targetPos = null;
                targetRot = null;
                if (setFreeCamera) {
                    if (ToggleFreeCameraMode(true, setShowCollision: setShowCollision) || wasFreeCameraMode != FreeCameraMode) {
                        return true;
                    }
                }
            } else if (wasTrackMoving && !trackMoving) {
                // Disable track
                TrackDistance = 0f;
            }
            return false;
        }

        public void StopMovingAlongTrack()
        {
            // Disable track
            IsTrackMovingEnabled = false;
            TrackDistance = 0f;
        }
        protected void MoveAlongTrack()
        {
            if (!IsTrackMovingEnabled)
                return;
            
            var manager = LevelEditorData.Level.SelectedTrackManager;
            if (!manager.Loop) {
                TrackDistance = Mathf.Clamp(TrackDistance, 0f, manager.TrackLength);
            }
            camera3D.transform.position = manager.GetPointAtDistance(LevelEditorData.Level, TrackDistance);
            camera3D.transform.rotation = manager.GetRotationAtDistance(LevelEditorData.Level, TrackDistance);
        }
    }
}
