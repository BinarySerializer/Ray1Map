namespace R1Engine
{
    public class PS1_R1_BigRayBlock : R1Serializable
    {
        /// <summary>
        /// The data length, set before serializing
        /// </summary>
        public long Length { get; set; }

        public EventData BigRay { get; set; }

        /// <summary>
        /// The data block
        /// </summary>
        public byte[] DataBlock { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            var p = s.CurrentPointer;
            BigRay = s.SerializeObject<EventData>(BigRay, name: nameof(BigRay));
            DataBlock = s.SerializeArray<byte>(DataBlock, Length - (s.CurrentPointer - p), name: nameof(DataBlock));
        }
    }
}