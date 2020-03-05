using System;

namespace R1Engine
{
    /// <summary>
    /// DES item data for PC
    /// </summary>
    public class PC_DesItem : IBinarySerializable
    {
        // TODO: Only there for .wld
        /// <summary>
        /// Indicates if the sprite has some gradation and requires clearing
        /// </summary>
        public bool RequiresBackgroundClearing { get; set; }

        // TODO: Only there for allfix - 12 bytes
        public byte[] Unknown1 { get; set; }

        /// <summary>
        /// The length of the image data
        /// </summary>
        public uint ImageDataLength { get; set; }

        /// <summary>
        /// The image data
        /// </summary>
        public byte[] ImageData { get; set; }

        // TODO: In kit and edu this comes before the image data
        /// <summary>
        /// The checksum for <see cref="ImageData"/>
        /// </summary>
        public byte ImageDataChecksum { get; set; }

        // TODO: Only there for allfix - 4 bytes
        public uint Unknown2 { get; set; }

        /// <summary>
        /// The amount of image descriptors
        /// </summary>
        public ushort ImageDescriptorCount { get; set; }

        /// <summary>
        /// The image descriptors
        /// </summary>
        public PC_ImageDescriptor[] ImageDescriptors { get; set; }

        /// <summary>
        /// The amount of animation descriptors
        /// </summary>
        public byte AnimationDescriptorCount { get; set; }

        /// <summary>
        /// The animation descriptors
        /// </summary>
        public PC_AnimationDescriptor[] AnimationDescriptors { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            // Read header
            RequiresBackgroundClearing = deserializer.Read<bool>();
            ImageDataLength = deserializer.Read<uint>();
            ImageData = deserializer.ReadBytes((int)ImageDataLength);
            ImageDataChecksum = deserializer.Read<byte>();
            ImageDescriptorCount = deserializer.Read<ushort>();
            ImageDescriptors = deserializer.Read<PC_ImageDescriptor>(ImageDescriptorCount);
            AnimationDescriptorCount = deserializer.Read<byte>();
            AnimationDescriptors = deserializer.Read<PC_AnimationDescriptor>(AnimationDescriptorCount);
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