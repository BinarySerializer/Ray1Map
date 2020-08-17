namespace R1Engine
{
    public class R1_PS1Edu_GSP : R1Serializable
    {
        /// <summary>
        /// Indices for the descriptor array in PS1_EDU_TEX
        /// </summary>
        public ushort[] Indices { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Indices = s.SerializeArraySize<ushort, ushort>(Indices, name: nameof(Indices));
            Indices = s.SerializeArray<ushort>(Indices, Indices.Length, name: nameof(Indices));
        }
    }
}