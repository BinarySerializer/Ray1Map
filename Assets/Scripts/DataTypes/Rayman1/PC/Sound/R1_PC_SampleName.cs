namespace R1Engine
{
    public class R1_PC_SampleName : R1Serializable
    {
        /// <summary>
        /// The name of the sample
        /// </summary>
        public string SampleName { get; set; }

        /// <summary>
        /// The time before the sample can be repeated, in seconds
        /// </summary>
        public ushort RepeatTime { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            SampleName = s.SerializeString(SampleName, 9, name: nameof(SampleName));
            RepeatTime = s.Serialize<ushort>(RepeatTime, name: nameof(RepeatTime));
        }
    }
}