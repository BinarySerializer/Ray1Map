namespace R1Engine
{
    /// <summary>
    /// Extension methods for <see cref="IR1_AnimationDescriptor"/>
    /// </summary>
    public static class AnimationDescriptorExtensions
    {
        /// <summary>
        /// Gets a common animation from the animation descriptor
        /// </summary>
        /// <param name="animationDescriptor">The animation descriptor</param>
        /// <returns>The common animation</returns>
        public static Unity_ObjAnimation ToCommonAnimation(this IR1_AnimationDescriptor animationDescriptor, int baseSpriteIndex = 0)
        {
            // Create the animation
            var animation = new Unity_ObjAnimation
            {
                Frames = new Unity_ObjAnimationFrame[animationDescriptor.FrameCount],
            };

            // The layer index
            var layer = 0;

            var layers = animationDescriptor.Layers;

            // Create each frame
            for (int i = 0; i < animationDescriptor.FrameCount; i++)
            {
                // Create the frame
                var frame = new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[animationDescriptor.LayersPerFrame]);

                if (layers != null) {

                    // Create each layer
                    for (var layerIndex = 0; layerIndex < animationDescriptor.LayersPerFrame; layerIndex++) {
                        var animationLayer = layers[layer];
                        layer++;

                        // Create the animation part
                        var part = new Unity_ObjAnimationPart {
                            ImageIndex = animationLayer.ImageIndex + baseSpriteIndex,
                            XPosition = animationLayer.XPosition,
                            YPosition = animationLayer.YPosition,
                            IsFlippedHorizontally = animationLayer.IsFlippedHorizontally,
                            IsFlippedVertically = animationLayer.IsFlippedVertically
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