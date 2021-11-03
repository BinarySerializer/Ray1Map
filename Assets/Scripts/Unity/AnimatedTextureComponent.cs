using Ray1Map;
using UnityEngine;

public class AnimatedTextureComponent : MonoBehaviour
{
    public Material material;
    public Texture2D[] animatedTextures;
    public AnimSpeed speed = new AnimSpeed_FrameDelay(1);
    public AnimLoopMode loopMode = AnimLoopMode.Repeat;
    public string textureName = "_MainTex";
    public float scrollU;
    public float scrollV;
    private float currentU;
    private float currentV;

    // Update is called once per frame
    void Update()
    {
        if (Controller.LoadState != Controller.State.Finished || !Settings.AnimateTiles)
            return;

        if (animatedTextures != null) 
        {
            if (speed.Update(animatedTextures.Length, loopMode))
                material.SetTexture(textureName, animatedTextures[speed.CurrentFrameInt]);
        }

        if (scrollU != 0) 
        {
            currentU += Time.deltaTime * scrollU;
            currentU %= 1f;
        }
        
        if (scrollV != 0) 
        {
            currentV += Time.deltaTime * scrollV;
            currentV %= 1f;
        }
     
        if (scrollU != 0 || scrollV != 0) 
            material.SetTextureOffset(textureName, new Vector2(currentU, currentV));
    }
}
