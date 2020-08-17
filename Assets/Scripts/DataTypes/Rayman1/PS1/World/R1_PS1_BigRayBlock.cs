namespace R1Engine
{
    public class R1_PS1_BigRayBlock : R1Serializable
    {
        /// <summary>
        /// The data length, set before serializing
        /// </summary>
        public long Length { get; set; }

        public R1_EventData BigRay { get; set; }

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
            BigRay = s.SerializeObject<R1_EventData>(BigRay, name: nameof(BigRay));
            DataBlock = s.SerializeArray<byte>(DataBlock, Length - (s.CurrentPointer - p), name: nameof(DataBlock));
        }
    }
}