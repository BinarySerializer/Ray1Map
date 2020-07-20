namespace R1Engine
{
    public class PC_SampleName : R1Serializable
    {
        public string SampleName { get; set; }

        public ushort Value { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            SampleName = s.SerializeString(SampleName, 9, name: nameof(SampleName));
            Value = s.Serialize<ushort>(Value, name: nameof(Value));
        }
    }
}