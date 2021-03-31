using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for Rayman 1 (PS1)
    /// </summary>
    public class R1_PS1_AnimationDescriptor : BinarySerializable, IR1_AnimationDescriptor
    {
        /// <summary>
        /// Pointer to the animation layers
        /// </summary>
        public Pointer AnimLayersPointer { get; set; }
        
        /// <summary>
        /// Pointer to the animation frames
        /// </summary>
        public Pointer AnimFramesPointer { get; set; }

        /// <summary>
        /// The number of layers to use per frame
        /// </summary>
        public ushort LayersPerFrameSerialized { get; set; }
        public byte LayersPerFrame => (byte)(LayersPerFrameSerialized & 0xFF);

        /// <summary>
        /// The number of frames in the animation
        /// </summary>
        public ushort FrameCountSerialized { get; set; }
        public byte FrameCount => (byte)(FrameCountSerialized & 0xFF);

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
            // Serialize pointers
            AnimLayersPointer = s.SerializePointer(AnimLayersPointer, name: nameof(AnimLayersPointer));
            AnimFramesPointer = s.SerializePointer(AnimFramesPointer, name: nameof(AnimFramesPointer));
            
            // Serialize data
            LayersPerFrameSerialized = s.Serialize<ushort>(LayersPerFrameSerialized, name: nameof(LayersPerFrameSerialized));
            FrameCountSerialized = s.Serialize<ushort>(FrameCountSerialized, name: nameof(FrameCountSerialized));

            // Serialize data from pointers
            s.DoAt(AnimLayersPointer, () => Layers = s.SerializeObjectArray(Layers, LayersPerFrame * FrameCount, name: nameof(Layers)));
            s.DoAt(AnimFramesPointer, () => Frames = s.SerializeObjectArray(Frames, FrameCount, name: nameof(Frames)));
        }
    }
}