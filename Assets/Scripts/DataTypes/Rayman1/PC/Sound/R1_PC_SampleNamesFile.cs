using BinarySerializer;

namespace R1Engine
{
    public class R1_PC_SampleNamesFile : BinarySerializable
    {
        public ushort SamplesCount { get; set; }

        public R1_PC_SampleName[] SampleNames { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            SamplesCount = s.Serialize<ushort>(SamplesCount, name: nameof(SamplesCount));
            SampleNames = s.SerializeObjectArray<R1_PC_SampleName>(SampleNames, SamplesCount, name: nameof(SampleNames));
        }
    }
}