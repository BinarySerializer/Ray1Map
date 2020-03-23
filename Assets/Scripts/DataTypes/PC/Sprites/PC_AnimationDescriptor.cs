using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for PC
    /// </summary>
    public class PC_AnimationDescriptor : R1Serializable
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
        public override void SerializeImpl(SerializerObject s) {
            LayersPerFrame = s.Serialize<byte>(LayersPerFrame, name: "LayersPerFrame");
            Unknown1 = s.Serialize<byte>(Unknown1, name: "Unknown1");
            FrameCount = s.Serialize<byte>(FrameCount, name: "FrameCount");
            Unknown2 = s.Serialize<byte>(Unknown2, name: "Unknown2");
            Unknown3 = s.Serialize<uint>(Unknown3, name: "Unknown3");
            FrameTableOffset = s.Serialize<ushort>(FrameTableOffset, name: "FrameTableOffset");
            
            if (FrameTableOffset != 4 * (LayersPerFrame * FrameCount + 1))
                Debug.LogWarning("Frame table offset is wrong");
            
            Layers = s.SerializeObjectArray<PC_AnimationLayer>(Layers, LayersPerFrame * FrameCount, name: "Layers");
            Frames = s.SerializeObjectArray<PC_AnimationFrame>(Frames, FrameCount + 1, name: "Frames");
        }
    }
}