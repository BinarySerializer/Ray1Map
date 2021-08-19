using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.PS1;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    public class PS1VRAMAnimationManager
    {
        public PS1VRAMAnimationManager()
        {
            AnimatedTextures = new Dictionary<int, HashSet<AnimatedTexture>>();
        }

        public Dictionary<int, HashSet<AnimatedTexture>> AnimatedTextures { get; }

        public void AddAnimatedTexture(AnimatedTexture tex)
        {
            var key = tex.Animations.First().Key;

            if (!AnimatedTextures.ContainsKey(key))
                AnimatedTextures.Add(key, new HashSet<AnimatedTexture>());

            AnimatedTextures[key].Add(tex);
        }

        public async UniTask LoadTexturesAsync(PS1_VRAM vram)
        {
            var index = 0;

            // Load every group. Animations are grouped based on length, speed and if they loop back.
            foreach (var textures in AnimatedTextures.Values)
            {
                int length = textures.First().Animations.First().Frames.Length;

                // Load every frame
                for (int frameIndex = 0; frameIndex < length; frameIndex++)
                {
                    Controller.DetailedState = $"Loading animated textures {index + 1}/{AnimatedTextures.Count} (frame {frameIndex + 1}/{length})";
                    await Controller.WaitIfNecessary();

                    // Update the VRAM for this frame
                    foreach (var anim in textures.SelectMany(x => x.Animations).Distinct())
                    {
                        var region = anim.UsesSingleRegion ? anim.Region : anim.Regions[frameIndex];
                        vram.AddDataAt(0, 0, region.x, region.y, anim.Frames[frameIndex], region.width, region.height);
                    }

                    // Load every texture
                    foreach (var tex in textures)
                        tex.LoadTextureAction(tex.Textures[frameIndex]);
                }

                index++;
            }
        }

        public class AnimatedTexture
        {
            public AnimatedTexture(int width, int height, bool clear, Action<Texture2D> loadTextureAction, PS1VRAMAnimation[] animations)
            {
                if (!animations.Any())
                    throw new Exception("Animated texture must have at least one animation");

                var firstAnim = animations.First();

                if (animations.Any(x => x.Key != firstAnim.Key))
                    throw new Exception("Animated texture can't have animations with different keys");

                Textures = new Texture2D[firstAnim.ActualLength];
                LoadTextureAction = loadTextureAction;
                Animations = animations;
                Speed = firstAnim.Speed;

                for (int i = 0; i < firstAnim.Frames.Length; i++)
                    Textures[i] = TextureHelpers.CreateTexture2D(width, height, clear: clear);

                if (firstAnim.PingPong)
                {
                    var sourceIndex = firstAnim.Frames.Length - 1;

                    for (int i = firstAnim.Frames.Length; i < Textures.Length; i++)
                    {
                        Textures[i] = Textures[sourceIndex];
                        sourceIndex--;
                    }
                }
            }

            public Texture2D[] Textures { get; }
            public Action<Texture2D> LoadTextureAction { get; }
            public PS1VRAMAnimation[] Animations { get; }
            public int Speed { get; }
        }
    }
}