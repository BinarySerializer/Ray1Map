namespace R1Engine
{
    public class R1_PS1_AllfixBlock : R1Serializable
    {
        /// <summary>
        /// The data length, set before serializing
        /// </summary>
        public long Length { get; set; }

        public R1_PS1_FontData[] FontData { get; set; }

        // Things like Rayman, the fist, game over clock etc.
        public R1_EventData[] MenuEvents { get; set; }

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
            MenuEvents = s.SerializeObjectArray<R1_EventData>(MenuEvents, 28, name: nameof(MenuEvents));
            DataBlock = s.SerializeArray<byte>(DataBlock, Length - (s.CurrentPointer - p), name: nameof(DataBlock));
        }
    }
}