using System;
using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Sprite group data for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_SpriteGroup : ISerializableFile
    {
        /// <summary>
        /// Indicates if the sprite has some gradation and requires clearing
        /// </summary>
        public bool RequiresBackgroundClearing { get; set; }

        /// <summary>
        /// The length of the image data
        /// </summary>
        public uint ImageDataLength { get; set; }

        /// <summary>
        /// The image data
        /// </summary>
        public byte[] ImageData { get; set; }

        /// <summary>
        /// The checksum for <see cref="ImageData"/>
        /// </summary>
        public byte ImageDataChecksum { get; set; }

        /// <summary>
        /// The amount of image descriptors
        /// </summary>
        public ushort ImageDescriptorCount { get; set; }

        /// <summary>
        /// The image descriptors
        /// </summary>
        public PC_R1_ImageDescriptor[] ImageDescriptors { get; set; }

        /// <summary>
        /// The amount of animation descriptors
        /// </summary>
        public byte AnimationDescriptorCount { get; set; }

        /// <summary>
        /// The animation descriptors
        /// </summary>
        public PC_R1_AnimationDescriptor[] AnimationDescriptors { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            // Read header
            RequiresBackgroundClearing = stream.Read<bool>();
            ImageDataLength = stream.Read<uint>();
            ImageData = stream.ReadBytes((int)ImageDataLength);
            ImageDataChecksum = stream.Read<byte>();
            ImageDescriptorCount = stream.Read<ushort>();
            ImageDescriptors = stream.Read<PC_R1_ImageDescriptor>(ImageDescriptorCount);
            AnimationDescriptorCount = stream.Read<byte>();
            AnimationDescriptors = stream.Read<PC_R1_AnimationDescriptor>(AnimationDescriptorCount);
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