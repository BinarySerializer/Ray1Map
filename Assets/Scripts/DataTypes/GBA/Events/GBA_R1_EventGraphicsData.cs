namespace R1Engine
{
    /// <summary>
    /// Event graphic data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_EventGraphicsData : R1Serializable
    {
        // sub_50BA4((_BYTE*)eventPointer, 0, 116u);

        public Pointer ImageBufferPointer { get; set; }

        // Gets set to offset 30 in memory, which gets overwritten by the position
        public uint ImageBufferSize { get; set; }

        public Pointer ImageDescriptorsPointer { get; set; }

        // *(_WORD*)(eventOffsetInMemory + 50) = sub_4EC90(*(_DWORD*)(structOffset + 12), 0xCu);
        public uint ImageDescriptorLength { get; set; }

        public Pointer ETAPointer { get; set; }

        // Usually between 1-3
        public uint Unk { get; set; }

        public Pointer AnimDescriptorsPointer { get; set; }

        public uint AnimDescriptorCount { get; set; }

        // Some event values gets reset here:
        /*
         
            *(_BYTE*)(eventOffsetInMemory + 88) = 0;
            *(_WORD*)(eventOffsetInMemory + 72) = 0;
            *(_WORD*)(eventOffsetInMemory + 56) = -1;
            *(_WORD*)(eventOffsetInMemory + 66) = (unsigned int)&dword_2710;
             
             */

        #region Parsed from Pointers

        public Common_ImageDescriptor[] ImageDescriptors { get; set; }

        public PS1_R1_AnimationDescriptor[] AnimDescriptors { get; set; }

        public byte[] ImageBuffer { get; set; }

        #endregion

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize data

            ImageBufferPointer = s.SerializePointer(ImageBufferPointer, name: nameof(ImageBufferPointer));
            ImageBufferSize = s.Serialize<uint>(ImageBufferSize, name: nameof(ImageBufferSize));
            ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
            ImageDescriptorLength = s.Serialize<uint>(ImageDescriptorLength, name: nameof(ImageDescriptorLength));
            ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));
            Unk = s.Serialize<uint>(Unk, name: nameof(Unk));
            AnimDescriptorsPointer = s.SerializePointer(AnimDescriptorsPointer, name: nameof(AnimDescriptorsPointer));
            AnimDescriptorCount = s.Serialize<uint>(AnimDescriptorCount, name: nameof(AnimDescriptorCount));

            // Uncomment this when running ExportUnusedSpritesAsync to avoid parsing invalid data
            /*
            if (ImageBufferSize > 30000)
                return;
            if (ImageDescriptorLength > 200*12)
                return;
            if (AnimDescriptorCount > 200)
                return;
            if (ImageBufferPointer == null)
                return;
            if (ImageDescriptorsPointer == null)
                return;
            if (AnimDescriptorsPointer == null)
                return;*/

            // Serialize data from pointers

            s.DoAt(AnimDescriptorsPointer, () => AnimDescriptors = s.SerializeObjectArray<PS1_R1_AnimationDescriptor>(AnimDescriptors, AnimDescriptorCount, name: nameof(AnimDescriptors)));

            s.DoAt(ImageDescriptorsPointer, () => ImageDescriptors = s.SerializeObjectArray<Common_ImageDescriptor>(ImageDescriptors, ImageDescriptorLength / 12, name: nameof(ImageDescriptors)));

            s.DoAt(ImageBufferPointer, () => ImageBuffer = s.SerializeArray<byte>(ImageBuffer, ImageBufferSize, name: nameof(ImageBuffer)));
        }
    }
}