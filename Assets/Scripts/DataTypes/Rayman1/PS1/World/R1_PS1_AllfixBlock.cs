using BinarySerializer;

namespace R1Engine
{
    public class R1_PS1_AllfixBlock : BinarySerializable
    {
        /// <summary>
        /// The data length, set before serializing
        /// </summary>
        public long Length { get; set; }

        // Alpha, Alpha2
        public R1_PS1_FontData[] FontData { get; set; }

        // Ray, RayLittle, ClockObj, DivObj, MapObj, Unknown
        public R1_EventData[] WldObj { get; set; }

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
            FontData = s.SerializeObjectArray<R1_PS1_FontData>(FontData, 2, name: nameof(FontData));
            WldObj = s.SerializeObjectArray<R1_EventData>(WldObj, 29, name: nameof(WldObj));
            DataBlock = s.SerializeArray<byte>(DataBlock, Length - (s.CurrentPointer - p), name: nameof(DataBlock));
        }
    }
}