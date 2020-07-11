using System.Diagnostics;

namespace R1Engine
{
    /// <summary>
    /// DES item data for PC
    /// </summary>
    public class PC_DES : R1Serializable {
        public Type FileType { get; set; }

        // TODO: Is that what this property is for? It seems it's true for all DES except the parallax ones.
        /// <summary>
        /// Indicates if the sprite has some gradation and requires clearing
        /// </summary>
        public bool RequiresBackgroundClearing { get; set; }

        public uint Allfix_Unk1 { get; set; }
        public uint RaymanExeSize { get; set; }
        public uint RaymanExeCheckSum1 { get; set; }

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

        public uint RaymanExeCheckSum2 { get; set; }

        /// <summary>
        /// The amount of image descriptors
        /// </summary>
        public ushort ImageDescriptorCount { get; set; }

        /// <summary>
        /// The image descriptors
        /// </summary>
        public Common_ImageDescriptor[] ImageDescriptors { get; set; }

        /// <summary>
        /// The amount of animation descriptors
        /// </summary>
        public byte AnimationDescriptorCount { get; set; }

        /// <summary>
        /// The animation descriptors
        /// </summary>
        public PC_AnimationDescriptor[] AnimationDescriptors { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            if (FileType == Type.World)
                RequiresBackgroundClearing = s.Serialize<bool>(RequiresBackgroundClearing, name: nameof(RequiresBackgroundClearing));
            else
                RequiresBackgroundClearing = true;

            if (FileType == Type.AllFix)
            {
                Allfix_Unk1 = s.Serialize<uint>(Allfix_Unk1, name: nameof(Allfix_Unk1));
                RaymanExeSize = s.Serialize<uint>(RaymanExeSize, name: nameof(RaymanExeSize));
                RaymanExeCheckSum1 = s.Serialize<uint>(RaymanExeCheckSum1, name: nameof(RaymanExeCheckSum1));
            }

            ImageDataLength = s.Serialize<uint>(ImageDataLength, name: nameof(ImageDataLength));

            var isChecksumBefore = FileType == Type.World && (s.GameSettings.EngineVersion == EngineVersion.RayKitPC || s.GameSettings.EngineVersion == EngineVersion.RayEduPC);
            var hasChecksum = !(!isChecksumBefore && FileType == Type.BigRay);

            ImageDataChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR(0x8F, () =>
                {
                    ImageData = s.SerializeArray<byte>(ImageData, ImageDataLength, name: nameof(ImageData));
                });
            }, isChecksumBefore ? ChecksumPlacement.Before : ChecksumPlacement.After, calculateChecksum: hasChecksum, name: nameof(ImageDataChecksum));

            if (FileType == Type.AllFix)
                RaymanExeCheckSum2 = s.Serialize<uint>(RaymanExeCheckSum2, name: nameof(RaymanExeCheckSum2));

            ImageDescriptorCount = s.Serialize<ushort>(ImageDescriptorCount, name: nameof(ImageDescriptorCount));
            ImageDescriptors = s.SerializeObjectArray<Common_ImageDescriptor>(ImageDescriptors, ImageDescriptorCount, name: nameof(ImageDescriptors));
            AnimationDescriptorCount = s.Serialize<byte>(AnimationDescriptorCount, name: nameof(AnimationDescriptorCount));
            AnimationDescriptors = s.SerializeObjectArray<PC_AnimationDescriptor>(AnimationDescriptors, AnimationDescriptorCount, name: nameof(AnimationDescriptors));
        }

        public enum Type
        {
            World,
            AllFix,
            BigRay
        }
    }
}