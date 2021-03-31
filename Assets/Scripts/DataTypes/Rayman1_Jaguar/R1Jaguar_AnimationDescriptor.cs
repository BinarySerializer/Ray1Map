using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for Rayman 1 (Jaguar)
    /// </summary>
    public class R1Jaguar_AnimationDescriptor : BinarySerializable, IR1_AnimationDescriptor
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
        public R1_AnimationLayer[] Layers { get; set; }

        /// <summary>
        /// The animation frames
        /// </summary>
        public R1_AnimationFrame[] Frames { get; set; }

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
        public Unity_ObjAnimation ToCommonAnimation(R1Jaguar_EventDefinition eventDefinition) {
            // Create the animation
            var animation = new Unity_ObjAnimation {
                Frames = new Unity_ObjAnimationFrame[FrameCount],
            };

            // The layer index
            var layer = 0;

            // Create each frame
            for (int i = 0; i < FrameCount; i++) {
                // Create the frame
                var frame = new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[LayersPerFrame]);

                if (Layers != null) {

                    // Create each layer
                    for (var layerIndex = 0; layerIndex < LayersPerFrame; layerIndex++) {
                        var animationLayer = Layers[layer];
                        layer++;

                        // Create the animation part
                        Unity_ObjAnimationPart part;
                        if (((eventDefinition.UShort_12 & 5) == 5) || eventDefinition.StructType == 31) {
                            part = new Unity_ObjAnimationPart {
                                ImageIndex = BitHelpers.ExtractBits(animationLayer.ImageIndex, 7, 0),
                                XPosition = animationLayer.XPosition,
                                YPosition = animationLayer.YPosition,
                                IsFlippedHorizontally = BitHelpers.ExtractBits(animationLayer.ImageIndex, 1, 7) != 0
                            };
                        } else {
                            part = new Unity_ObjAnimationPart {
                                ImageIndex = animationLayer.ImageIndex,
                                XPosition = BitHelpers.ExtractBits(animationLayer.XPosition, 7, 0),
                                YPosition = animationLayer.YPosition,
                                IsFlippedHorizontally = BitHelpers.ExtractBits(animationLayer.XPosition, 1, 7) != 0
                            };
                        }

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