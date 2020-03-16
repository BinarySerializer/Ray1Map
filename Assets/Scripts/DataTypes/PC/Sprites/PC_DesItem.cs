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
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            if (serializer.FileName.Contains(".wld"))
                serializer.Serialize(nameof(RequiresBackgroundClearing));
            else
                RequiresBackgroundClearing = true;

            if (serializer.FileName.Contains("allfix.dat"))
                serializer.SerializeArray<byte>(nameof(Unknown1), 12);

            serializer.Serialize(nameof(ImageDataLength));

            if (serializer.FileName.Contains(".wld") && (serializer.GameSettings.GameMode == GameMode.RayKit || serializer.GameSettings.GameMode == GameMode.RayEduPC))
            {
                serializer.Serialize(nameof(ImageDataChecksum));
                serializer.SerializeArray<byte>(nameof(ImageData), ImageDataLength);
            }
            else
            {
                serializer.SerializeArray<byte>(nameof(ImageData), ImageDataLength);

                if (!serializer.FileName.Contains("bray.dat") && !serializer.FileName.Contains("bigray.dat"))
                    serializer.Serialize(nameof(ImageDataChecksum));
            }

            if (serializer.FileName.Contains("allfix.dat"))
                serializer.Serialize(nameof(Unknown2));

            serializer.Serialize(nameof(ImageDescriptorCount));
            serializer.SerializeArray<PC_ImageDescriptor>(nameof(ImageDescriptors), ImageDescriptorCount);
            serializer.Serialize(nameof(AnimationDescriptorCount));
            serializer.SerializeArray<PC_AnimationDescriptor>(nameof(AnimationDescriptors), AnimationDescriptorCount);
        }
    }
}