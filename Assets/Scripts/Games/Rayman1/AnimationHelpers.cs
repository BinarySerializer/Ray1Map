using BinarySerializer.Ray1;

namespace Ray1Map.Rayman1
{
    /// <summary>
    /// Extension methods for <see cref="IAnimation"/>
    /// </summary>
    public static class AnimationHelpers
    {
        public static Unity_ObjAnimation ToCommonAnimation(
            AnimationLayer[] layers,
            int layersCount, 
            int framesCount, 
            int baseSpriteIndex = 0)
        {
            // Create the animation
            var animation = new Unity_ObjAnimation
            {
                Frames = new Unity_ObjAnimationFrame[framesCount],
            };

            // The layer index
            var layer = 0;

            // Create each frame
            for (int i = 0; i < framesCount; i++)
            {
                // Create the frame
                var frame = new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[layersCount]);

                if (layers != null) {

                    // Create each layer
                    for (var layerIndex = 0; layerIndex < layersCount; layerIndex++) {
                        var animationLayer = layers[layer];
                        layer++;

                        // Create the animation part
                        var part = new Unity_ObjAnimationPart {
                            ImageIndex = animationLayer.SpriteIndex + baseSpriteIndex,
                            XPosition = animationLayer.XPosition,
                            YPosition = animationLayer.YPosition,
                            IsFlippedHorizontally = animationLayer.FlipX,
                            IsFlippedVertically = animationLayer.FlipY
                        };

                        // Add the part
                        frame.SpriteLayers[layerIndex] = part;
                    }
                }
                // Set the frame
                animation.Frames[i] = frame;
            }

            return animation;
        }
    }
}