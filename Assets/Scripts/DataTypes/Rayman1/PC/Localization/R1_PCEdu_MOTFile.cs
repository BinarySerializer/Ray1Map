using BinarySerializer;

namespace R1Engine
{
    public class R1_PCEdu_MOTFile : BinarySerializable
    {
        public ushort TextDefineCount { get; set; }
        public R1_PC_LocFileString[] TextDefine { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            TextDefineCount = s.Serialize<ushort>(TextDefineCount, name: nameof(TextDefineCount));
            TextDefine = s.SerializeObjectArray<R1_PC_LocFileString>(TextDefine, TextDefineCount, name: nameof(TextDefine));
        }
    }
}