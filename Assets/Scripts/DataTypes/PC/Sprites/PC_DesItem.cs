using System;

namespace R1Engine
{
    /// <summary>
    /// DES item data for PC
    /// </summary>
    public class PC_DesItem : IBinarySerializable
    {
        /// <summary>
        /// Indicates if the sprite has some gradation and requires clearing
        /// </summary>
        public bool RequiresBackgroundClearing { get; set; }

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
            if (deserializer.FileName.Contains(".wld"))
                RequiresBackgroundClearing = deserializer.Read<bool>();
            else
                RequiresBackgroundClearing = true;

            if (deserializer.FileName.Contains("allfix.dat"))
                Unknown1 = deserializer.ReadArray<byte>(12);
            
            ImageDataLength = deserializer.Read<uint>();
            ImageData = deserializer.ReadArray<byte>(ImageDataLength);
            
            if (!deserializer.FileName.Contains("bray.dat") && !deserializer.FileName.Contains("bigray.dat"))
                ImageDataChecksum = deserializer.Read<byte>();

            if (deserializer.FileName.Contains("allfix.dat"))
                Unknown2 = deserializer.Read<uint>();

            ImageDescriptorCount = deserializer.Read<ushort>();
            ImageDescriptors = deserializer.ReadArray<PC_ImageDescriptor>(ImageDescriptorCount);
            AnimationDescriptorCount = deserializer.Read<byte>();
            AnimationDescriptors = deserializer.ReadArray<PC_AnimationDescriptor>(AnimationDescriptorCount);
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