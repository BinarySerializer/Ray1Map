using System;
using System.IO;
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
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            LayersPerFrame = deserializer.Read<byte>();
            Unknown1 = deserializer.Read<byte>();
            FrameCount = deserializer.Read<byte>();
            Unknown2 = deserializer.Read<byte>();
            Unknown3 = deserializer.Read<uint>();
            FrameTableOffset = deserializer.Read<ushort>();
            
            if (FrameTableOffset != 4 * (LayersPerFrame * FrameCount + 1))
                Debug.LogWarning("Frame table offset is wrong");
            
            Layers = deserializer.ReadArray<PC_AnimationLayer>((ulong)(LayersPerFrame * FrameCount));
            Frames = deserializer.ReadArray<PC_AnimationFrame>(((ulong)FrameCount + 1));
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}