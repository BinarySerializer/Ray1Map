namespace R1Engine
{
    public class PS1_R1_AllfixBlock : R1Serializable
    {
        /// <summary>
        /// The data length, set before serializing
        /// </summary>
        public long Length { get; set; }

        public PS1_FontData[] FontData { get; set; }

        // Things like Rayman, the fist, game over clock etc.
        public EventData[] MenuEvents { get; set; }

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
            FontData = s.SerializeObjectArray<PS1_FontData>(FontData, 2, name: nameof(FontData));
            MenuEvents = s.SerializeObjectArray<EventData>(MenuEvents, 28, name: nameof(MenuEvents));
            DataBlock = s.SerializeArray<byte>(DataBlock, Length - (s.CurrentPointer - p), name: nameof(DataBlock));
        }
    }
}