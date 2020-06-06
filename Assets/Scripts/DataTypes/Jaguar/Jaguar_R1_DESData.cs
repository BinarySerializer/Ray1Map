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

        public ushort UShort_00 { get; set; }
        public Pointer Pointer_02 { get; set; }
        public ushort UShort_06 { get; set; }

        // TODO: These are not always pointers - why?

        // Animation descriptors? This is usually valid when ImageDescriptorsPointer is valid
        public Pointer Pointer_08 { get; set; }
        public Pointer Pointer_0C { get; set; }
        public Pointer Pointer_10 { get; set; }
        public Pointer ImageDescriptorsPointer { get; set; }

        public uint ImageBufferMemoryPointerPointer { get; set; }
        public uint UInt_1C { get; set; }
        public ushort UShort_20 { get; set; }
        public uint UInt_22 { get; set; }
        public Pointer Pointer_22 { get; set; }
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
            UShort_00 = s.Serialize<ushort>(UShort_00, name: nameof(UShort_00));
            Pointer_02 = s.SerializePointer(Pointer_02, name: nameof(Pointer_02));
            UShort_06 = s.Serialize<ushort>(UShort_06, name: nameof(UShort_06));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
            Pointer_10 = s.SerializePointer(Pointer_10, name: nameof(Pointer_10));
            ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
            ImageBufferMemoryPointerPointer = s.Serialize<uint>(ImageBufferMemoryPointerPointer, name: nameof(ImageBufferMemoryPointerPointer));
            UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
            UShort_20 = s.Serialize<ushort>(UShort_20, name: nameof(UShort_20));
            UInt_22 = s.Serialize<uint>(UInt_22, name: nameof(UInt_22));
            UShort_26 = s.Serialize<ushort>(UShort_26, name: nameof(UShort_26));

            if (ImageDescriptorsPointer != null)
            {
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
        }

        #endregion
    }
}