using Cysharp.Threading.Tasks;
using R1Engine;
using System.Collections;
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
		int width = LevelEditorData.MaxWidth;
		int height = LevelEditorData.MaxHeight;
		int cellSize = LevelEditorData.Level.CellSize;
		int screenshotWidth = Mathf.CeilToInt(width / (float)cellSize);
		int screenshotHeight = Mathf.CeilToInt(height / (float)cellSize);
		Camera cam = Controller.obj.levelController.renderCamera;
		var cellSizeInUnits = cellSize / (float)LevelEditorData.Level.PixelsPerUnit;
		cam.transform.position = new Vector3((LevelEditorData.MaxWidth) * cellSizeInUnits / 2f, -(LevelEditorData.MaxHeight) * cellSizeInUnits / 2f, cam.transform.position.z);
		cam.orthographicSize = (LevelEditorData.MaxHeight * cellSizeInUnits / 2f);
		cam.rect = new Rect(0, 0, 1, 1);
		await UniTask.WaitForEndOfFrame();
		byte[] screenshotBytes = null;
		var lScreenshot = zzTransparencyCapture.CaptureScreenshot(width * cellSize, height * cellSize, isTransparent, camera: cam);
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
}