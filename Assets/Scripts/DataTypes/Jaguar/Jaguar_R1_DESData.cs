using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// DES data for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_DESData : R1Serializable
    {
        #region DES Data

        public short Short_00 { get; set; }
        public Pointer Pointer_02 { get; set; } // Points to a struct of size 0x26. just some shorts, no pointers
        public ushort StructType { get; set; }

        // Points to a struct with 4 pointers:
        // 1. A list of structs of size 0x6. Animation descriptors?
        // 2. A code pointer
        // 3. A pointer to itself
        // 4. Another code pointer
        // With the two code pointers, maybe it's related to event behavior
        public Pointer Pointer_08 { get; set; } 
        public Pointer Pointer_0C { get; set; }
        public ushort UShort_10 { get; set; }
        public ushort UShort_12 { get; set; }
        public Pointer ImageDescriptorsPointer { get; set; }

        public uint ImageBufferMemoryPointerPointer { get; set; }
        public uint UInt_1C { get; set; }
        public byte Byte_20 { get; set; }
        public byte Byte_21 { get; set; }
        public byte Byte_22 { get; set; }
        public byte Byte_23 { get; set; }
        public ushort UShort_24 { get; set; }
        public ushort UShort_26 { get; set; }

        #endregion

        #region Parsed from Pointers

        public Common_ImageDescriptor[] ImageDescriptors { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Short_00 = s.Serialize<short>(Short_00, name: nameof(Short_00));
            Pointer_02 = s.SerializePointer(Pointer_02, name: nameof(Pointer_02));
            StructType = s.Serialize<ushort>(StructType, name: nameof(StructType));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
            UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
            UShort_12 = s.Serialize<ushort>(UShort_12, name: nameof(UShort_12));
            ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
            ImageBufferMemoryPointerPointer = s.Serialize<uint>(ImageBufferMemoryPointerPointer, name: nameof(ImageBufferMemoryPointerPointer));
            UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
            Byte_20 = s.Serialize<byte>(Byte_20, name: nameof(Byte_20));
            Byte_21 = s.Serialize<byte>(Byte_21, name: nameof(Byte_21));
            Byte_22 = s.Serialize<byte>(Byte_22, name: nameof(Byte_22));
            Byte_23 = s.Serialize<byte>(Byte_23, name: nameof(Byte_23));
            UShort_24 = s.Serialize<ushort>(UShort_24, name: nameof(UShort_24));
            UShort_26 = s.Serialize<ushort>(UShort_26, name: nameof(UShort_26));

            s.DoAt(ImageDescriptorsPointer, () =>
            {
                // TODO: Find way to get the length
                var temp = new List<Common_ImageDescriptor>();

                var index = 0;
                while (true)
                {
                    var i = s.SerializeObject<Common_ImageDescriptor>(default, name: $"{nameof(ImageDescriptors)}[{index}]");

                    if (temp.Any() && i.Index != 0xFF && i.ImageBufferOffset < temp.Last().ImageBufferOffset)
                        break;

                    temp.Add(i);

                    index++;
                }

                ImageDescriptors = temp.ToArray();
            });
        }

        #endregion
    }
}