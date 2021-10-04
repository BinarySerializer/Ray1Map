using R1Engine;
using UnityEngine;

public class OutlineManager : MonoBehaviour {
	public LineRenderer outlineActive;
	public LineRenderer outlineHighlight;
	public Color highlightColor;
	public Color selectColor;
	public Color activeColor;
	public float fadeInSpeed = 10f;
	public float fadeOutSpeed = 3f;
	public bool selecting = false;
	private float curLerpActive = 0f;
	private float curLerpHighlight = 0f;
	public Unity_SpriteObjBehaviour Active { get; set; }
	public Unity_SpriteObjBehaviour Highlight { get; set; }

	void Update() {
		// Update highlight color
		if (Highlight == null || (Highlight == Active) || !selecting) {
			curLerpHighlight = Mathf.Clamp01(curLerpHighlight - Time.deltaTime * fadeOutSpeed);
		} else {
			curLerpHighlight = Mathf.Clamp01(curLerpHighlight + Time.deltaTime * fadeInSpeed);
		}
		Color curColor = Color.Lerp(highlightColor, this.selectColor, curLerpHighlight);
		outlineHighlight.startColor = curColor;
		outlineHighlight.endColor = curColor;

		// Update selection color
		if (Active == null || (Highlight != Active) || !selecting) {
			curLerpActive = Mathf.Clamp01(curLerpActive - Time.deltaTime * fadeOutSpeed);
		} else {
			curLerpActive = Mathf.Clamp01(curLerpActive + Time.deltaTime * fadeInSpeed);
		}
		curColor = Color.Lerp(activeColor, this.selectColor, curLerpActive);
		outlineActive.startColor = curColor;
		outlineActive.endColor = curColor;
    }

	public void TransferHighlightToActiveColor() {
		if (Highlight != null) {
			curLerpActive = curLerpHighlight;
			Color curColor = Color.Lerp(activeColor, this.selectColor, curLerpActive);
			outlineActive.startColor = curColor;
			outlineActive.endColor = curColor;
		}
	}

    private void LateUpdate() {
		var ec = Controller.obj?.levelEventController?.editor?.cam;
		bool mouselook = ec != null && (ec.MouseLookEnabled || ec.MouseLookRMBEnabled);
		// Update selection square lines
		if (Active != null) {
			if(!outlineActive.gameObject.activeSelf) outlineActive.gameObject.SetActive(true);
			if (Active.boxCollider != null) {
				var size = Active.boxCollider.size;
				//outlineActive.gameObject.layer = LayerMask.NameToLayer("Default");
				SetPositions(outlineActive, Active.midpoint, size);
			} else if (Active.boxCollider3D != null) {
				Camera mainCam = Camera.main;
				Camera cam3D = Controller.obj?.levelEventController?.editor?.cam?.camera3D ?? mainCam;
				Vector2 objPos = Convert3DTo2DPoint(cam3D, mainCam, Active.midpoint);
				var size = Active.boxCollider3D.size;
				if (!cam3D.orthographic) size = ScaleSize(cam3D, mainCam, size, Active.midpoint);
				//outlineActive.gameObject.layer = LayerMask.NameToLayer("3D Links");
				SetPositions(outlineActive, objPos, size);
			}
			//outlineActive.SetPosition(4, new Vector2(Active.midpoint.x - Active.boxCollider.size.x / 2f, Active.midpoint.y - Active.boxCollider.size.y / 2f));
		} else {
			if (outlineActive.gameObject.activeSelf) outlineActive.gameObject.SetActive(false);
		}
		if (Highlight != null && Highlight != Active && !mouselook) {
			if (!outlineHighlight.gameObject.activeSelf) outlineHighlight.gameObject.SetActive(true);
			if (Highlight.boxCollider != null) {
				var size = Highlight.boxCollider.size;
				//outlineHighlight.gameObject.layer = LayerMask.NameToLayer("Default");
				SetPositions(outlineHighlight, Highlight.midpoint, size);
			} else if (Highlight.boxCollider3D != null) {
				Camera mainCam = Camera.main;
				Camera cam3D = Controller.obj?.levelEventController?.editor?.cam?.camera3D ?? mainCam;
				Vector2 objPos = Convert3DTo2DPoint(cam3D, mainCam, Highlight.midpoint);
				var size = Highlight.boxCollider3D.size;
				if(!cam3D.orthographic) size = ScaleSize(cam3D, mainCam, size, Highlight.midpoint);
				//outlineHighlight.gameObject.layer = LayerMask.NameToLayer("3D Links");
				SetPositions(outlineHighlight, objPos, size);
			}
			//outlineHighlight.SetPosition(4, new Vector2(Highlight.midpoint.x - Highlight.boxCollider.size.x / 2f, Highlight.midpoint.y - Highlight.boxCollider.size.y / 2f));
		} else {
			if (outlineHighlight.gameObject.activeSelf) outlineHighlight.gameObject.SetActive(false);
		}
    }

	private void SetPositions(LineRenderer line, Vector3 center, Vector3 size) {
		line.SetPosition(0, new Vector2(center.x - size.x / 2f, center.y - size.y / 2f));
		line.SetPosition(1, new Vector2(center.x + size.x / 2f, center.y - size.y / 2f));
		line.SetPosition(2, new Vector2(center.x + size.x / 2f, center.y + size.y / 2f));
		line.SetPosition(3, new Vector2(center.x - size.x / 2f, center.y + size.y / 2f));
	}
	private Vector3 ScaleSize(Camera cam3D, Camera mainCam, Vector3 size, Vector3 objWorldPos) {
		float angle = cam3D.fieldOfView / 2f;
		Vector3 pos = cam3D.WorldToScreenPoint(objWorldPos);
		float dist = pos.z;
		if(dist <= 0) return Vector3.zero;
		float orthoSizeAtDistance = dist * Mathf.Tan(angle * Mathf.Deg2Rad);
		if(orthoSizeAtDistance <= 0f) return Vector3.zero;
		float mainCamOrthoSize = mainCam.orthographicSize;
		float sizeCorr = mainCamOrthoSize / orthoSizeAtDistance;
		return size * sizeCorr;

	}

	private Vector2 Convert3DTo2DPoint(Camera cam3D, Camera mainCam, Vector3 point) {
		return mainCam.ScreenToWorldPoint(cam3D.WorldToScreenPoint(point));
	}
}