using Cysharp.Threading.Tasks;
using Ray1Map;
using System.Collections.Generic;
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


		List<Camera> cameras = new List<Camera>();
		cameras.Add(Camera.main);
		if (LevelEditorData.Level?.IsometricData != null) {
			EditorCam ec = Controller.obj?.levelController?.editor?.cam;
			cameras.Add(ec.camera3D);
		}

		var lScreenshot = zzTransparencyCapture.CaptureScreenshot(width, height, isTransparent, camera: cameras.ToArray());
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

	public async UniTask<byte[]> CaptureFullLevel(bool isTransparent, RectInt? rect = null, bool is3DOnly = false, CameraPos? pos3D = null) {
		if (isTransparent) {
			Controller.obj.levelController.controllerTilemap.backgroundTint.gameObject.SetActive(false);
		}
		//Camera[] cams = Camera.allCameras.OrderBy(c => c.depth).ToArray();
		float width = LevelEditorData.MaxX - LevelEditorData.MinX;
		float height = LevelEditorData.MaxY - LevelEditorData.MinY;
		int cellSize = LevelEditorData.Level.CellSize;
		int screenshotWidth = Mathf.CeilToInt(width / cellSize);
		int screenshotHeight = Mathf.CeilToInt(height / cellSize);
		var cellSizeInUnits = cellSize / (float)LevelEditorData.Level.PixelsPerUnit;
		Dictionary<Camera, CameraSettings> camSettings = new Dictionary<Camera, CameraSettings>();
		EditorCam ec = Controller.obj?.levelController?.editor?.cam;
		bool? prevFreeCameraMode = null;
		ec.enabled = false;
		List<Camera> cameras = new List<Camera>();
		if (!is3DOnly) {
			// Add main camera
			{
				Camera cam = Camera.main;
				camSettings[cam] = CameraSettings.Current(cam);

				cam.transform.position = new Vector3(
					(LevelEditorData.MinX + (LevelEditorData.MaxX - LevelEditorData.MinX) / 2f) * cellSizeInUnits,
					-(LevelEditorData.MinY + (LevelEditorData.MaxY - LevelEditorData.MinY) / 2f) * cellSizeInUnits,
					cam.transform.position.z);
				cam.orthographicSize = (LevelEditorData.MaxY * cellSizeInUnits / 2f);
				cam.rect = new Rect(0, 0, 1, 1);
				cameras.Add(cam);
			}
			if (LevelEditorData.Level?.IsometricData != null) {
				// Add isometric camera
				Camera cam = ec.camera3D;

				camSettings[cam] = CameraSettings.Current(cam);

				// Update 3D camera
				float scl = 1f;
				Quaternion rot3D = LevelEditorData.Level.IsometricData.ViewAngle;
				cam.transform.rotation = rot3D;
				Vector3 v = rot3D * Vector3.back;
				float w = LevelEditorData.Level.IsometricData.TilesWidth * cellSizeInUnits;
				float h = (LevelEditorData.Level.IsometricData.TilesHeight) * cellSizeInUnits;
				float colYDisplacement = LevelEditorData.Level.IsometricData.CalculateYDisplacement();
				float colXDisplacement = LevelEditorData.Level.IsometricData.CalculateXDisplacement();

				var pos = new Vector3(
					(LevelEditorData.MinX + (LevelEditorData.MaxX - LevelEditorData.MinX) / 2f) * cellSizeInUnits,
					-(LevelEditorData.MinY + (LevelEditorData.MaxY - LevelEditorData.MinY) / 2f) * cellSizeInUnits,
					-10f);

				cam.transform.position = v * 300 + rot3D * ((pos -
					new Vector3((w - colXDisplacement) / 2f, -(h - colYDisplacement) / 2f, 0f)) / scl); // Move back 300 units
				cam.orthographicSize = Camera.main.orthographicSize / scl;
				cam.rect = new Rect(0, 0, 1, 1);
				cam.orthographic = true;
				cam.gameObject.SetActive(true);
				cameras.Add(cam);

				await UniTask.WaitForEndOfFrame();
				// Update all object positions & rotations according to this new camera pos
				var objects = Controller.obj.levelController.Objects;
				foreach (var obj in objects) {
					obj.UpdatePosition3D();
				}
				// Now disable the tilemap controller & culling mask
				prevFreeCameraMode = ec.FreeCameraMode;
				ec.FreeCameraMode = false;
				ec.UpdateCullingMask(ec.FreeCameraMode);
				if (Controller.obj?.levelController?.controllerTilemap != null) {
					Controller.obj.levelController.controllerTilemap.enabled = false;
					Controller.obj.levelController.controllerTilemap.UpdateLayersVisibility();
				}
			}
		} else {// Add isometric camera
			Camera cam = ec.camera3D;

			camSettings[cam] = CameraSettings.Current(cam);
			CameraSettings newSettings = new CameraSettings() {
				orthographic = true,
				active = true,
				rect = cam.rect,
				fov = cam.fieldOfView,
			};
			var bounds = LevelEditorData.Level.Bounds3D.Value;
			float orthoSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) / 2f; // half size
			newSettings.orthographicSize = orthoSize;

			var backDistance = Camera.main.farClipPlane * 0.5f;

			width = 1920 * 2f;
			height = 1080 * 2f;
			int i = 0;
			Quaternion rot;
			switch (pos3D) {
				case CameraPos.Front:
				case CameraPos.Left:
				case CameraPos.Back:
				case CameraPos.Right:
					i = (int)pos3D - (int)CameraPos.Front;
					rot = Quaternion.Euler(0, (90 * i), 0);
					newSettings.position = bounds.center - rot * Vector3.forward * backDistance;
					newSettings.rotation = rot;
					break;
				case CameraPos.IsometricFront:
				case CameraPos.IsometricLeft:
				case CameraPos.IsometricBack:
				case CameraPos.IsometricRight:
					i = (int)pos3D - (int)CameraPos.IsometricFront;
					var pitch = Mathf.Rad2Deg * Mathf.Atan(Mathf.Sin(Mathf.Deg2Rad * 45));
					rot = Quaternion.Euler(pitch, 45 + (90 * i), 0);
					newSettings.position = bounds.center - rot * Vector3.forward * backDistance;
					newSettings.rotation = rot;
					break;
				case CameraPos.Top:
				case CameraPos.Bottom:
					i = (int)pos3D - (int)CameraPos.Top;
					rot = Quaternion.Euler(90 - 180 * i, 0, 0);
					newSettings.position = bounds.center - rot * Vector3.forward * backDistance;
					newSettings.rotation = rot;
					break;
			}

			// Update 3D camera
			newSettings.Apply(cam);
			cameras.Add(cam);

			await UniTask.WaitForEndOfFrame();
			// Update all object positions & rotations according to this new camera pos
			var objects = Controller.obj.levelController.Objects;
			foreach (var obj in objects) {
				obj.UpdatePosition3D();
			}
			// Now disable the tilemap controller & culling mask
			prevFreeCameraMode = ec.FreeCameraMode;
			ec.FreeCameraMode = false;
			ec.UpdateCullingMask(ec.FreeCameraMode);
			if (Controller.obj?.levelController?.controllerTilemap != null) {
				Controller.obj.levelController.controllerTilemap.enabled = false;
				Controller.obj.levelController.controllerTilemap.UpdateLayersVisibility();
			}
		}
		await UniTask.WaitForEndOfFrame();
		byte[] screenshotBytes = null;
		var lScreenshot = zzTransparencyCapture.CaptureScreenshot(Mathf.CeilToInt(width) * cellSize, Mathf.CeilToInt(height) * cellSize, isTransparent, camera: cameras.ToArray());
		
		// Restore previous settings
		foreach (var cam in cameras) {
			camSettings[cam].Apply(cam);
		}
		if (prevFreeCameraMode.HasValue) {
			ec.FreeCameraMode = prevFreeCameraMode.Value;
			ec.UpdateCullingMask(ec.FreeCameraMode);
			if (Controller.obj?.levelController?.controllerTilemap != null) {
				Controller.obj.levelController.controllerTilemap.UpdateLayersVisibility();
				Controller.obj.levelController.controllerTilemap.enabled = true;
			}
		}
		ec.enabled = true;

		try 
        {
			if (rect != null)
				lScreenshot = lScreenshot.Crop(rect.Value, true);

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