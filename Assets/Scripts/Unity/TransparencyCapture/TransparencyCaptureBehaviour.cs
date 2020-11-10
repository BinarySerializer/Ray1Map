using Cysharp.Threading.Tasks;
using R1Engine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TransparencyCaptureBehaviour : MonoBehaviour
{
    public async UniTask<byte[]> Capture(int width, int height, bool isTransparent) {
		if (isTransparent) {
			Controller.obj.levelController.controllerTilemap.backgroundTint.gameObject.SetActive(false);
		}
		await UniTask.WaitForEndOfFrame();
		//After Unity4,you have to do this function after WaitForEndOfFrame in Coroutine
		//Or you will get the error:"ReadPixels was called to read pixels from system frame buffer, while not inside drawing frame"
		//zzTransparencyCapture.captureScreenshot("capture.png");
		byte[] screenshotBytes = null;
		var lScreenshot = zzTransparencyCapture.CaptureScreenshot(width, height, isTransparent);
		try {
			screenshotBytes = lScreenshot.EncodeToPNG();
		} finally {
			Object.DestroyImmediate(lScreenshot);
		}
		if (isTransparent) {
			Controller.obj.levelController.controllerTilemap.backgroundTint.gameObject.SetActive(true);
		}
		Debug.Log("Screenshot saved.");
		return screenshotBytes;
	}

	public async UniTask<byte[]> CaptureFulllevel(bool isTransparent) {
		if (isTransparent) {
			Controller.obj.levelController.controllerTilemap.backgroundTint.gameObject.SetActive(false);
		}
		//Camera[] cams = Camera.allCameras.OrderBy(c => c.depth).ToArray();
		int width = LevelEditorData.MaxWidth;
		int height = LevelEditorData.MaxHeight;
		int cellSize = LevelEditorData.Level.CellSize;
		int screenshotWidth = Mathf.CeilToInt(width / (float)cellSize);
		int screenshotHeight = Mathf.CeilToInt(height / (float)cellSize);
		var cellSizeInUnits = cellSize / (float)LevelEditorData.Level.PixelsPerUnit;
		Dictionary<Camera, CameraSettings> camSettings = new Dictionary<Camera, CameraSettings>();
		EditorCam ec = Controller.obj?.levelController?.editor?.cam;
		ec.enabled = false;
		List<Camera> cameras = new List<Camera>();
		// Add main camera
		{
			Camera cam = Camera.main;
			camSettings[cam] = CameraSettings.Current(cam);

			cam.transform.position = new Vector3((LevelEditorData.MaxWidth) * cellSizeInUnits / 2f, -(LevelEditorData.MaxHeight) * cellSizeInUnits / 2f, cam.transform.position.z);
			cam.orthographicSize = (LevelEditorData.MaxHeight * cellSizeInUnits / 2f);
			cam.rect = new Rect(0, 0, 1, 1);
			cameras.Add(cam);
		}
		if (LevelEditorData.Level?.IsometricData != null) {
			// Add isometric camera
			Camera cam = ec.camera3D;

			camSettings[cam] = CameraSettings.Current(cam);

			// Update 3D camera
			float scl = 1f;
			Quaternion rot3D = Quaternion.Euler(30f, -45, 0);
			cam.transform.rotation = rot3D;
			Vector3 v = rot3D * Vector3.back;
			float w = LevelEditorData.Level.IsometricData.TilesWidth * cellSizeInUnits;
			float h = (LevelEditorData.Level.IsometricData.TilesHeight) * cellSizeInUnits;
			float colH = (LevelEditorData.Level.IsometricData.CollisionWidth + LevelEditorData.Level.IsometricData.CollisionHeight);

			var pos = new Vector3((LevelEditorData.MaxWidth) * cellSizeInUnits / 2f, -(LevelEditorData.MaxHeight) * cellSizeInUnits / 2f, cam.transform.position.z);
			cam.transform.position = v * 300 + rot3D * ((pos - new Vector3(w / 2f, colH / 2f - h / 2f, 0f)) / scl); // Move back 300 units
			cam.orthographicSize = Camera.main.orthographicSize / scl;
			cam.rect = new Rect(0, 0, 1, 1);
			cam.gameObject.SetActive(true);
			cameras.Add(cam);

			// Update all object positions & rotations according to this new camera pos
			var objects = Controller.obj.levelController.Objects;
			foreach (var obj in objects) {
				obj.UpdatePosition3D();
			}
		}
		await UniTask.WaitForEndOfFrame();
		byte[] screenshotBytes = null;
		var lScreenshot = zzTransparencyCapture.CaptureScreenshot(width * cellSize, height * cellSize, isTransparent, camera: cameras.ToArray());
		
		foreach (var cam in cameras) {
			camSettings[cam].Apply(cam);
		}
		ec.enabled = true;
		try {
			screenshotBytes = lScreenshot.EncodeToPNG();
		} finally {
			Object.DestroyImmediate(lScreenshot);
		}
		if (isTransparent) {
			Controller.obj.levelController.controllerTilemap.backgroundTint.gameObject.SetActive(true);
		}
		Debug.Log("Screenshot saved.");
		return screenshotBytes;
	}

    public static Resolution GetCurrentResolution()
    {   
        return new Resolution
        {
            width = Camera.main.pixelWidth,
            height = Camera.main.pixelHeight,
            refreshRate = Screen.currentResolution.refreshRate,
        };
    }

	public class CameraSettings {
		public Rect rect;
		public bool orthographic;
		public float orthographicSize;
		public float fov;
		public Vector3 position;
		public Quaternion rotation;
		public bool active;

		public static CameraSettings Current(Camera cam) {
			return new CameraSettings() {
				rect = cam.rect,
				orthographic = cam.orthographic,
				orthographicSize = cam.orthographicSize,
				fov = cam.fieldOfView,
				position = cam.transform.position,
				rotation = cam.transform.rotation,
				active = cam.gameObject.activeSelf
			};
		}
		public void Apply(Camera cam) {
			cam.rect = rect;
			cam.orthographic = orthographic;
			cam.orthographicSize = orthographicSize;
			cam.fieldOfView = fov;
			cam.transform.position = position;
			cam.transform.rotation = rotation;
			if(cam.gameObject.activeSelf != active) cam.gameObject.SetActive(active);
		}
	}
}