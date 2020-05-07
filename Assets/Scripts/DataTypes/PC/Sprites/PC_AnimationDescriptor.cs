using UnityEngine;

namespace R1Engine
{
    // TODO: Merge with PS1_R1_AnimationDescriptor
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
        public Common_AnimationLayer[] Layers { get; set; }

        /// <summary>
        /// The default animation frame (seems to always match the first frame)
        /// </summary>
        public Common_AnimationFrame DefaultFrame { get; set; }

        /// <summary>
        /// The animation frames
        /// </summary>
        public Common_AnimationFrame[] Frames { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            LayersPerFrame = s.Serialize<byte>(LayersPerFrame, name: nameof(LayersPerFrame));
            Unknown1 = s.Serialize<byte>(Unknown1, name: nameof(Unknown1));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            Unknown2 = s.Serialize<byte>(Unknown2, name: nameof(Unknown2));
            Unknown3 = s.Serialize<uint>(Unknown3, name: nameof(Unknown3));
            FrameTableOffset = s.Serialize<ushort>(FrameTableOffset, name: nameof(FrameTableOffset));
            
            if (FrameTableOffset != 4 * (LayersPerFrame * FrameCount + 1))
                Debug.LogWarning("Frame table offset is wrong");
            
            Layers = s.SerializeObjectArray<Common_AnimationLayer>(Layers, LayersPerFrame * FrameCount, name: nameof(Layers));
            DefaultFrame = s.SerializeObject<Common_AnimationFrame>(DefaultFrame, name: nameof(DefaultFrame));
            Frames = s.SerializeObjectArray<Common_AnimationFrame>(Frames, FrameCount, name: nameof(Frames));
        }
    }
}