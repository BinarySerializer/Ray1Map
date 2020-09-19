namespace R1Engine
{
    public class R1_PS1_FontData : R1Serializable
    {
        public Pointer ImageDescriptorsPointer { get; set; }
        public Pointer ImageBufferPointer { get; set; }
        public byte ImageDescriptorsCount { get; set; }

        public R1_ImageDescriptor[] ImageDescriptors { get; set; }
        public byte[] ImageBuffer { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
            ImageBufferPointer = s.SerializePointer(ImageBufferPointer, allowInvalid: true, name: nameof(ImageBufferPointer));
            ImageDescriptorsCount = s.Serialize<byte>(ImageDescriptorsCount, name: nameof(ImageDescriptorsCount));
            s.SerializeArray<byte>(new byte[3], 3, name: "Padding");

            s.DoAt(ImageDescriptorsPointer, () => ImageDescriptors = s.SerializeObjectArray<R1_ImageDescriptor>(ImageDescriptors, ImageDescriptorsCount, name: nameof(ImageDescriptors)));

            if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3)
            {
                if (ImageBuffer == null && ImageBufferPointer != null && ImageDescriptors != null)
                {
                    // Determine length of image buffer
                    uint length = 0;
                    foreach (R1_ImageDescriptor img in ImageDescriptors)
                    {
                        if (img.ImageType != 2 && img.ImageType != 3)
                            continue;

                        uint curLength = img.ImageBufferOffset;

                        if (img.ImageType == 2)
                            curLength += (uint)(img.Width / 2) * img.Height;
                        else if (img.ImageType == 3)
                            curLength += (uint)img.Width * img.Height;

                        if (curLength > length)
                            length = curLength;
                    }
                    ImageBuffer = new byte[length];
                }
                s.DoAt(ImageBufferPointer, () => ImageBuffer = s.SerializeArray<byte>(ImageBuffer, ImageBuffer.Length, name: nameof(ImageBuffer)));
            }
        }
    }
}