using UnityEngine;
using UnityEditor;
using Ray1Map;

[CustomEditor(typeof(TransparencyCaptureBehaviour))]
public class TransparencyCaptureBehaviourEditor : Editor {

    public override async void OnInspectorGUI() {
        DrawDefaultInspector();
        TransparencyCaptureBehaviour pb = (TransparencyCaptureBehaviour)target;

        if (GUILayout.Button("Take screenshot")) {
			Resolution res = TransparencyCaptureBehaviour.GetCurrentResolution();
			System.DateTime dateTime = System.DateTime.Now;
			byte[] screenshotBytes = await pb.CaptureFullLevel(true);
			//byte[] screenshotBytes = await pb.Capture(res.width* 4, res.height * 4, true);
			Util.ByteArrayToFile("Screenshots/" + dateTime.ToString("yyyy_MM_dd HH_mm_ss") + ".png", screenshotBytes);

			if (LevelEditorData.Level?.Bounds3D != null) {
				for (int i = 0; i < (int)CameraPos.Initial; i++) {

					screenshotBytes = await pb.CaptureFullLevel(true, pos3D: (CameraPos)i);
					//byte[] screenshotBytes = await pb.Capture(res.width* 4, res.height * 4, true);
					Util.ByteArrayToFile($"Screenshots/{dateTime.ToString("yyyy_MM_dd HH_mm_ss")}__{(CameraPos)i}.png", screenshotBytes);
				}
			}
		}
    }

	

}