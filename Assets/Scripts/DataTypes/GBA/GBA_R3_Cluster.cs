namespace R1Engine
{
    public class GBA_R3_Cluster : GBA_R3_BaseBlock
    {
        public byte[] Data { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize block header
            base.SerializeImpl(s);

            Data = s.SerializeArray<byte>(Data, BlockSize, name: nameof(Data));
        }
    }
}