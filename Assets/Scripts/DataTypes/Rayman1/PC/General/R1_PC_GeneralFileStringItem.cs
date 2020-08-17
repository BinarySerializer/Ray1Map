namespace R1Engine
{
    public class R1_PC_GeneralFileStringItem : R1Serializable
    {
        public byte[] Unk1 { get; set; }

        public R1_PC_LocFileString String { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.SerializeArray<byte>(Unk1, 16, name: nameof(Unk1));
            String = s.SerializeObject<R1_PC_LocFileString>(String, name: nameof(String));
        }
    }
}