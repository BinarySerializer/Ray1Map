using System;
using System.IO;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Animation descriptor data for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_AnimationDescriptor : ISerializableFile
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
        public PC_R1_AnimationLayer[] Layers { get; set; }

        /// <summary>
        /// The animation frames
        /// </summary>
        public PC_R1_AnimationFrame[] Frames { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            LayersPerFrame = stream.Read<byte>();
            Unknown1 = stream.Read<byte>();
            FrameCount = stream.Read<byte>();
            Unknown2 = stream.Read<byte>();
            Unknown3 = stream.Read<uint>();
            FrameTableOffset = stream.Read<ushort>();
            
            if (FrameTableOffset != 4 * (LayersPerFrame * FrameCount + 1))
                Debug.LogWarning("Frame table offset is wrong");
            
            Layers = stream.Read<PC_R1_AnimationLayer>((ulong)(LayersPerFrame * FrameCount));
            Frames = stream.Read<PC_R1_AnimationFrame>(((ulong)FrameCount + 1));
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}