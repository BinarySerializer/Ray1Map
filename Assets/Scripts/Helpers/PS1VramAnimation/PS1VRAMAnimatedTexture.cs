using System;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class PS1VRAMAnimatedTexture
    {
        public PS1VRAMAnimatedTexture(int width, int height, bool clear, Action<Texture2D> loadTextureAction, PS1VRAMAnimation[] animations)
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

            for (int i = 0; i < firstAnim.FramesLength; i++)
                Textures[i] = TextureHelpers.CreateTexture2D(width, height, clear: clear);

            if (firstAnim.PingPong)
            {
                var sourceIndex = firstAnim.FramesLength - 2;

                for (int i = firstAnim.FramesLength; i < Textures.Length; i++)
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