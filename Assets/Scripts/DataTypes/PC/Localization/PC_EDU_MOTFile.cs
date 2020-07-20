namespace R1Engine
{
    public class PC_EDU_MOTFile : R1Serializable
    {
        public ushort TextDefineCount { get; set; }
        public PC_LocFileString[] TextDefine { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            TextDefineCount = s.Serialize<ushort>(TextDefineCount, name: nameof(TextDefineCount));
            TextDefine = s.SerializeObjectArray<PC_LocFileString>(TextDefine, TextDefineCount, name: nameof(TextDefine));
        }
    }
}