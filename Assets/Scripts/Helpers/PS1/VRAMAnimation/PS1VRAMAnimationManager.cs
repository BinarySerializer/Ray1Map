using System.Collections.Generic;
using System.Linq;
using BinarySerializer.PlayStation.PS1;
using Cysharp.Threading.Tasks;

namespace Ray1Map
{
    public class PS1VRAMAnimationManager
    {
        public PS1VRAMAnimationManager()
        {
            AnimatedTextures = new Dictionary<int, HashSet<PS1VRAMAnimatedTexture>>();
            MultiKeyAnimatedTextures = new Dictionary<int[], HashSet<PS1VRAMAnimatedTexture>>();
        }

        public Dictionary<int, HashSet<PS1VRAMAnimatedTexture>> AnimatedTextures { get; }
        public Dictionary<int[], HashSet<PS1VRAMAnimatedTexture>> MultiKeyAnimatedTextures { get; }

        public void AddAnimatedTexture(PS1VRAMAnimatedTexture tex)
        {
            if (!tex.HasMultipleKeys)
            {
                var key = tex.Animations.First().Key;

                if (!AnimatedTextures.ContainsKey(key))
                    AnimatedTextures.Add(key, new HashSet<PS1VRAMAnimatedTexture>());

                AnimatedTextures[key].Add(tex);
            }
            else
            {
                var key = tex.Animations.Select(x => x.Key).Distinct().OrderBy(x => x).ToArray();

                var matchingKey = MultiKeyAnimatedTextures.FirstOrDefault(x => x.Value.First().Speed == tex.Speed && x.Key.SequenceEqual(key)).Key;

                if (matchingKey == null)
                {
                    MultiKeyAnimatedTextures.Add(key, new HashSet<PS1VRAMAnimatedTexture>());
                    matchingKey = key;
                }

                MultiKeyAnimatedTextures[matchingKey].Add(tex);
            }
        }

        public async UniTask LoadTexturesAsync(VRAM vram)
        {
            var index = 0;
            var count = AnimatedTextures.Count + MultiKeyAnimatedTextures.Count;

            // Load every group. Animations are grouped based on length, speed and if they loop back.
            foreach (var textures in AnimatedTextures.Values)
            {
                int length = textures.First().Animations.First().FramesLength;

                // Load every frame
                for (int frameIndex = 0; frameIndex < length; frameIndex++)
                {
                    Controller.DetailedState = $"Loading animated textures {index + 1}/{count} (frame {frameIndex + 1}/{length})";
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

                // If there are multi-key animated textures we need to revert back to frame 0 as the same VRAM region might be accessed again
                if (MultiKeyAnimatedTextures.Any())
                {
                    foreach (var anim in textures.SelectMany(x => x.Animations).Distinct())
                    {
                        var region = anim.UsesSingleRegion ? anim.Region : anim.Regions[0];
                        vram.AddDataAt(0, 0, region.x, region.y, anim.GetFrame(0), region.width, region.height);
                    }
                }

                index++;
            }

            // Load multi-key animations
            foreach (var textures in MultiKeyAnimatedTextures.Values)
            {
                int length = textures.First().Textures.Length;
                var speed = textures.First().Speed;

                // Load every frame
                for (int frameIndex = 0; frameIndex < length; frameIndex++)
                {
                    Controller.DetailedState = $"Loading multi-animated textures {index + 1}/{count} (frame {frameIndex + 1}/{length})";
                    await Controller.WaitIfNecessary();

                    // Update the VRAM for this frame
                    foreach (var anim in textures.SelectMany(x => x.Animations).Distinct())
                    {
                        var speedDiff = anim.Speed / speed;
                        
                        var f = frameIndex;

                        f /= speedDiff;
                        f %= anim.ActualLength;

                        if (anim.PingPong && f >= anim.FramesLength)
                            f = anim.FramesLength - f + 2;

                        //if (anim.PingPong)
                        //    Debug.Log($"Frame: {frameIndex} | ActualLength: {anim.ActualLength} | Length: {anim.FramesLength} | F: {f}");

                        var region = anim.UsesSingleRegion ? anim.Region : anim.Regions[f];
                        vram.AddDataAt(0, 0, region.x, region.y, anim.GetFrame(f), region.width, region.height);
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