using R1Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedTextureComponent : MonoBehaviour
{
    public Material material;
    public Texture2D[] animatedTextures;
    public float animatedTextureSpeed = 1f; // In frames, for 60FPS
    public float currentAnimatedTexture;
    public string textureName = "_MainTex";
    public float scrollU;
    public float scrollV;
    private float currentU;
    private float currentV;

    // Update is called once per frame
    void Update()
    {
        if(Controller.LoadState != Controller.State.Finished) return;
        if(!Settings.AnimateTiles) return;
        if (animatedTextures != null && animatedTextureSpeed != 0) {
            int curTex = Mathf.FloorToInt(currentAnimatedTexture);
            currentAnimatedTexture += Time.deltaTime * (LevelEditorData.FramesPerSecond / animatedTextureSpeed);
            if(currentAnimatedTexture >= animatedTextures.Length) currentAnimatedTexture = 0;
            int newTex = Mathf.FloorToInt(currentAnimatedTexture);
            if (newTex != curTex) {
                material.SetTexture(textureName, animatedTextures[newTex]);
            }
        }
        if (scrollU != 0) {
            currentU += Time.deltaTime * scrollU;
            currentU %= 1f;
        }
        if (scrollV != 0) {
            currentV += Time.deltaTime * scrollV;
            currentV %= 1f;
        }
        if (scrollU != 0 || scrollV != 0) {
            material.SetTextureOffset(textureName, new Vector2(currentU, currentV));
        }
    }
}
