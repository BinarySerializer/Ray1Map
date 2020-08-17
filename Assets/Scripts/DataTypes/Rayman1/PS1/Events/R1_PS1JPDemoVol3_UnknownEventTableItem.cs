namespace R1Engine
{
    public class R1_PS1JPDemoVol3_UnknownEventTableItem : R1Serializable
    {
        public byte LinkIndex { get; set; }
        public byte Value { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            LinkIndex = s.Serialize(LinkIndex, name: nameof(LinkIndex));
            Value = s.Serialize(Value, name: nameof(Value));
        }
    }
}