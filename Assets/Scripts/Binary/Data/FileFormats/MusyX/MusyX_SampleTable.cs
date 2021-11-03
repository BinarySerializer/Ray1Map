using System.Text;
using BinarySerializer;

namespace Ray1Map {
    public class MusyX_SampleTable : BinarySerializable {
        // Set in OnPreSerialize
        public Pointer BaseOffset { get; set; }

        public Pointer<MusyX_Sample>[] Samples { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Hack to get length
            Pointer smpOff1 = null;
            s.DoAt(Offset, () => {
                smpOff1 = s.SerializePointer(smpOff1, anchor: BaseOffset, name: nameof(smpOff1));
            });
            Samples = s.SerializePointerArray<MusyX_Sample>(Samples, (smpOff1 - Offset) / 4, anchor: BaseOffset, resolve: true, name: nameof(Samples));
        }
    }
}