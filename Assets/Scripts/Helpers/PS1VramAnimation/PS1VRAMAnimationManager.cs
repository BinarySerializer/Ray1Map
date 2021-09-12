using System.Collections.Generic;
using System.Linq;
using BinarySerializer.PS1;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    public class PS1VRAMAnimationManager
    {
        public PS1VRAMAnimationManager()
        {
            AnimatedTextures = new Dictionary<int, HashSet<PS1VRAMAnimatedTexture>>();
        }

        public Dictionary<int, HashSet<PS1VRAMAnimatedTexture>> AnimatedTextures { get; }

        public void AddAnimatedTexture(PS1VRAMAnimatedTexture tex)
        {
            var key = tex.Animations.First().Key;

            if (!AnimatedTextures.ContainsKey(key))
                AnimatedTextures.Add(key, new HashSet<PS1VRAMAnimatedTexture>());

            AnimatedTextures[key].Add(tex);
        }

        public async UniTask LoadTexturesAsync(PS1_VRAM vram)
        {
            var index = 0;

            // Load every group. Animations are grouped based on length, speed and if they loop back.
            foreach (var textures in AnimatedTextures.Values)
            {
                int length = textures.First().Animations.First().FramesLength;

                // Load every frame
                for (int frameIndex = 0; frameIndex < length; frameIndex++)
                {
                    Controller.DetailedState = $"Loading animated textures {index + 1}/{AnimatedTextures.Count} (frame {frameIndex + 1}/{length})";
                    await Controller.WaitIfNecessary();

                    // Update the VRAM for this frame
                    foreach (var anim in textures.SelectMany(x => x.Animations).Distinct())
                    {
                        var region = anim.UsesSingleRegion ? anim.Region : anim.Regions[frameIndex];
                        vram.AddDataAt(0, 0, region.x, region.y, anim.GetFrame(frameIndex), region.width, region.height);
                    }

                    // Load every texture
                    foreach (var tex in textures)
                        tex.LoadTextureAction(tex.Textures[frameIndex]);
                }

                index++;
            }
        }
    }
}