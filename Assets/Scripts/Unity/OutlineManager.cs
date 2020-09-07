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
	public Unity_ObjBehaviour Active { get; set; }
	public Unity_ObjBehaviour Highlight { get; set; }

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
		// Update selection square lines
		if (Active != null) {
			if(!outlineActive.gameObject.activeSelf) outlineActive.gameObject.SetActive(true);
			outlineActive.SetPosition(0, new Vector2(Active.midpoint.x - Active.boxCollider.size.x / 2f, Active.midpoint.y - Active.boxCollider.size.y / 2f));
			outlineActive.SetPosition(1, new Vector2(Active.midpoint.x + Active.boxCollider.size.x / 2f, Active.midpoint.y - Active.boxCollider.size.y / 2f));
			outlineActive.SetPosition(2, new Vector2(Active.midpoint.x + Active.boxCollider.size.x / 2f, Active.midpoint.y + Active.boxCollider.size.y / 2f));
			outlineActive.SetPosition(3, new Vector2(Active.midpoint.x - Active.boxCollider.size.x / 2f, Active.midpoint.y + Active.boxCollider.size.y / 2f));
			//outlineActive.SetPosition(4, new Vector2(Active.midpoint.x - Active.boxCollider.size.x / 2f, Active.midpoint.y - Active.boxCollider.size.y / 2f));
		} else {
			if (outlineActive.gameObject.activeSelf) outlineActive.gameObject.SetActive(false);
		}
		if (Highlight != null && Highlight != Active) {
			if (!outlineHighlight.gameObject.activeSelf) outlineHighlight.gameObject.SetActive(true);
			outlineHighlight.SetPosition(0, new Vector2(Highlight.midpoint.x - Highlight.boxCollider.size.x / 2f, Highlight.midpoint.y - Highlight.boxCollider.size.y / 2f));
			outlineHighlight.SetPosition(1, new Vector2(Highlight.midpoint.x + Highlight.boxCollider.size.x / 2f, Highlight.midpoint.y - Highlight.boxCollider.size.y / 2f));
			outlineHighlight.SetPosition(2, new Vector2(Highlight.midpoint.x + Highlight.boxCollider.size.x / 2f, Highlight.midpoint.y + Highlight.boxCollider.size.y / 2f));
			outlineHighlight.SetPosition(3, new Vector2(Highlight.midpoint.x - Highlight.boxCollider.size.x / 2f, Highlight.midpoint.y + Highlight.boxCollider.size.y / 2f));
			//outlineHighlight.SetPosition(4, new Vector2(Highlight.midpoint.x - Highlight.boxCollider.size.x / 2f, Highlight.midpoint.y - Highlight.boxCollider.size.y / 2f));
		} else {
			if (outlineHighlight.gameObject.activeSelf) outlineHighlight.gameObject.SetActive(false);
		}
    }
}