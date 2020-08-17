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
        public static Unity_ObjAnimation ToCommonAnimation(this IR1_AnimationDescriptor animationDescriptor)
        {
            // Create the animation
            var animation = new Unity_ObjAnimation
            {
                Frames = new Unity_ObjAnimationFrame[animationDescriptor.FrameCount],
            };

            // The layer index
            var layer = 0;

            // Create each frame
            for (int i = 0; i < animationDescriptor.FrameCount; i++)
            {
                // Create the frame
                var frame = new Unity_ObjAnimationFrame()
                {
                    FrameData = animationDescriptor.Frames?[i],
                    Layers = new Unity_ObjAnimationPart[animationDescriptor.LayersPerFrame]
                };
                if (animationDescriptor.Frames?[i] == null) {
                    frame.FrameData = new R1_AnimationFrame();
                }
                if (animationDescriptor.Layers != null) {

                    // Create each layer
                    for (var layerIndex = 0; layerIndex < animationDescriptor.LayersPerFrame; layerIndex++) {
                        var animationLayer = animationDescriptor.Layers[layer];
                        layer++;

                        // Create the animation part
                        var part = new Unity_ObjAnimationPart {
                            ImageIndex = animationLayer.ImageIndex,
                            XPosition = animationLayer.XPosition,
                            YPosition = animationLayer.YPosition,
                            IsFlippedHorizontally = animationLayer.IsFlippedHorizontally
                        };

                        // Add the part
                        frame.Layers[layerIndex] = part;
                    }
                }
                // Set the frame
                animation.Frames[i] = frame;
            }

            return animation;
        }
    }
}