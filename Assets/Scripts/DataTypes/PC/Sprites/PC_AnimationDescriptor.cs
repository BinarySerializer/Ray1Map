using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for PC
    /// </summary>
    public class PC_AnimationDescriptor : IBinarySerializable
    {
        /// <summary>
        /// The number of layers to use per frame
        /// </summary>
        public byte LayersPerFrame { get; set; }

        public byte Unknown1 { get; set; }

        /// <summary>
        /// The number of frames in the animation
        /// </summary>
        public byte FrameCount { get; set; }

        public byte Unknown2 { get; set; }

        // ID?
        public uint Unknown3 { get; set; }

        public ushort FrameTableOffset { get; set; }

        /// <summary>
        /// The animation layers
        /// </summary>
        public PC_AnimationLayer[] Layers { get; set; }

        /// <summary>
        /// The animation frames
        /// </summary>
        public PC_AnimationFrame[] Frames { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.Serialize(nameof(LayersPerFrame));
            serializer.Serialize(nameof(Unknown1));
            serializer.Serialize(nameof(FrameCount));
            serializer.Serialize(nameof(Unknown2));
            serializer.Serialize(nameof(Unknown3));
            serializer.Serialize(nameof(FrameTableOffset));
            
            if (FrameTableOffset != 4 * (LayersPerFrame * FrameCount + 1))
                Debug.LogWarning("Frame table offset is wrong");
            
            serializer.SerializeArray<PC_AnimationLayer>(nameof(Layers), LayersPerFrame * FrameCount);
            serializer.SerializeArray<PC_AnimationFrame>(nameof(Frames), FrameCount + 1);
        }
    }
}