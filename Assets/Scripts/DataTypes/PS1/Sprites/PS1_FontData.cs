namespace R1Engine
{
    public class PS1_FontData : R1Serializable
    {
        public Pointer ImageDescriptorsPointer { get; set; }
        public Pointer ImageBufferPointer { get; set; }
        public byte ImageDescriptorsCount { get; set; }

        public Common_ImageDescriptor[] ImageDescriptors { get; set; }

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

            s.DoAt(ImageDescriptorsPointer, () => ImageDescriptors = s.SerializeObjectArray<Common_ImageDescriptor>(ImageDescriptors, ImageDescriptorsCount, name: nameof(ImageDescriptors)));
        }
    }
}