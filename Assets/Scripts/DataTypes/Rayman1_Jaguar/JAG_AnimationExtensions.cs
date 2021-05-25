using BinarySerializer;
using BinarySerializer.Ray1;

namespace R1Engine
{
    public static class JAG_AnimationExtensions
    {
        public static Unity_ObjAnimation ToCommonAnimation(this JAG_Animation anim, JAG_EventDefinition eventDefinition)
        {
            // Create the animation
            var animation = new Unity_ObjAnimation
            {
                Frames = new Unity_ObjAnimationFrame[anim.FrameCount],
            };

            // The layer index
            var layer = 0;

            // Create each frame
            for (int i = 0; i < anim.FrameCount; i++)
            {
                // Create the frame
                var frame = new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[anim.LayersPerFrame]);

                if (anim.Layers != null)
                {

                    // Create each layer
                    for (var layerIndex = 0; layerIndex < anim.LayersPerFrame; layerIndex++)
                    {
                        var animationLayer = anim.Layers[layer];
                        layer++;

                        // Create the animation part
                        Unity_ObjAnimationPart part;
                        if (((eventDefinition.UShort_12 & 5) == 5) || eventDefinition.StructType == 31)
                        {
                            part = new Unity_ObjAnimationPart
                            {
                                ImageIndex = BitHelpers.ExtractBits(animationLayer.SpriteIndex, 7, 0),
                                XPosition = animationLayer.XPosition,
                                YPosition = animationLayer.YPosition,
                                IsFlippedHorizontally = BitHelpers.ExtractBits(animationLayer.SpriteIndex, 1, 7) != 0
                            };
                        }
                        else
                        {
                            part = new Unity_ObjAnimationPart
                            {
                                ImageIndex = animationLayer.SpriteIndex,
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

        public static Unity_ObjAnimation ToCommonAnimation(this JAG_EventDefinition ev)
        {
            // Create the animation
            var animation = new Unity_ObjAnimation
            {
                Frames = new Unity_ObjAnimationFrame[ev.FrameCount],
            };

            // The layer index
            var layer = 0;
            int LayersPerFrame = ev.NumLayers > 0 ? ev.NumLayers : 1;

            // Create each frame
            for (int i = 0; i < ev.FrameCount; i++)
            {
                // Create the frame
                var frame = new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[LayersPerFrame]);

                if (ev.AnimationLayers != null)
                {
                    // Create each layer
                    for (var layerIndex = 0; layerIndex < LayersPerFrame; layerIndex++)
                    {
                        var animationLayer = ev.AnimationLayers[layer];
                        layer++;

                        // Create the animation part
                        Unity_ObjAnimationPart part;
                        if (ev.UShort_12 == 5 || ev.StructType == 31)
                        {
                            part = new Unity_ObjAnimationPart
                            {
                                ImageIndex = BitHelpers.ExtractBits(animationLayer.SpriteIndex, 7, 0),
                                XPosition = animationLayer.XPosition,
                                YPosition = animationLayer.YPosition,
                                IsFlippedHorizontally = BitHelpers.ExtractBits(animationLayer.SpriteIndex, 1, 7) != 0
                            };
                        }
                        else
                        {
                            part = new Unity_ObjAnimationPart
                            {
                                ImageIndex = animationLayer.SpriteIndex,
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

        public static Unity_ObjAnimation ToCommonAnimation(this JAG_EventComplexDataState state, JAG_EventDefinition eventDefinition)
        {
            // Create the animation
            var animation = new Unity_ObjAnimation
            {
                Frames = new Unity_ObjAnimationFrame[state.FramesCount],
            };

            // The layer index
            var layer = 0;

            var ignoreYBit = state.Context.GetR1Settings().EngineVersion == EngineVersion.R1Jaguar_Proto && state.AnimationPointer.AbsoluteOffset == 0x00811DDC;

            // Create each frame
            for (int i = 0; i < state.FramesCount; i++)
            {
                // Create the frame
                var frame = new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[state.LayersPerFrame]);

                if (state.Layers != null)
                {

                    // Create each layer
                    for (var layerIndex = 0; layerIndex < state.LayersPerFrame; layerIndex++)
                    {
                        var animationLayer = state.Layers[layer];
                        layer++;

                        // Create the animation part
                        Unity_ObjAnimationPart part;
                        if (eventDefinition.UShort_12 == 5 || eventDefinition.StructType == 31)
                        {
                            part = new Unity_ObjAnimationPart
                            {
                                ImageIndex = BitHelpers.ExtractBits(animationLayer.SpriteIndex, 7, 0),
                                XPosition = animationLayer.XPosition,
                                YPosition = ignoreYBit ? BitHelpers.ExtractBits(animationLayer.YPosition, 7, 0) : animationLayer.YPosition,
                                IsFlippedHorizontally = BitHelpers.ExtractBits(animationLayer.SpriteIndex, 1, 7) != 0
                            };
                        }
                        else
                        {
                            part = new Unity_ObjAnimationPart
                            {
                                ImageIndex = animationLayer.SpriteIndex,
                                XPosition = BitHelpers.ExtractBits(animationLayer.XPosition, 7, 0),
                                YPosition = ignoreYBit ? BitHelpers.ExtractBits(animationLayer.YPosition, 7, 0) : animationLayer.YPosition,
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