namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_AnimationDescriptor : R1Serializable, IAnimationDescriptor
    {
        /// <summary>
        /// The number of layers to use per frame
        /// </summary>
        public byte LayersPerFrame { get; set; }

        /// <summary>
        /// The number of frames in the animation
        /// </summary>
        public byte FrameCount { get; set; }

        /// <summary>
        /// The animation layers
        /// </summary>
        public Common_AnimationLayer[] Layers { get; set; }

        /// <summary>
        /// The animation frames
        /// </summary>
        public Common_AnimationFrame[] Frames { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // TODO: Are there frames anywhere?

            // Serialize data
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            s.Serialize<byte>(default, name: "Padding");
            LayersPerFrame = s.Serialize<byte>(LayersPerFrame, name: nameof(LayersPerFrame));
            s.Serialize<byte>(default, name: "Padding");

            // Serialize data from pointers
            Layers = s.SerializeObjectArray(Layers, LayersPerFrame * FrameCount, name: nameof(Layers));
        }

        /// <summary>
        /// Gets a common animation from the animation descriptor
        /// </summary>
        /// <param name="animationDescriptor">The animation descriptor</param>
        /// <returns>The common animation</returns>
        public Common_Animation ToCommonAnimation(Jaguar_R1_EventDefinition eventDefinition) {
            // Create the animation
            var animation = new Common_Animation {
                Frames = new Common_AnimFrame[FrameCount],
            };

            // The layer index
            var layer = 0;

            // Create each frame
            for (int i = 0; i < FrameCount; i++) {
                // Create the frame
                var frame = new Common_AnimFrame() {
                    FrameData = Frames?[i],
                    Layers = new Common_AnimationPart[LayersPerFrame]
                };
                if (Frames?[i] == null) {
                    frame.FrameData = new Common_AnimationFrame();
                }
                if (Layers != null) {

                    // Create each layer
                    for (var layerIndex = 0; layerIndex < LayersPerFrame; layerIndex++) {
                        var animationLayer = Layers[layer];
                        layer++;

                        // Create the animation part
                        Common_AnimationPart part;
                        if (eventDefinition.UShort_12 == 5) {
                            part = new Common_AnimationPart {
                                ImageIndex = BitHelpers.ExtractBits(animationLayer.ImageIndex, 7, 0),
                                XPosition = animationLayer.XPosition,
                                YPosition = animationLayer.YPosition,
                                IsFlippedHorizontally = BitHelpers.ExtractBits(animationLayer.ImageIndex, 1, 7) != 0
                            };
                        } else {
                            part = new Common_AnimationPart {
                                ImageIndex = animationLayer.ImageIndex,
                                XPosition = BitHelpers.ExtractBits(animationLayer.XPosition, 7, 0),
                                YPosition = animationLayer.YPosition,
                                IsFlippedHorizontally = BitHelpers.ExtractBits(animationLayer.XPosition, 1, 7) != 0
                            };
                        }

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