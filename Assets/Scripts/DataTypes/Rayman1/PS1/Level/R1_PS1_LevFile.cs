namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 1 (PS1)
    /// </summary>
    public class R1_PS1_LevFile : R1_PS1BaseFile
    {
        /// <summary>
        /// The pointer to the background block
        /// </summary>
        public Pointer BackgroundBlockPointer => BlockPointers[0];

        /// <summary>
        /// The pointer to the event block
        /// </summary>
        public Pointer EventBlockPointer => BlockPointers[1];

        /// <summary>
        /// The pointer to the map block
        /// </summary>
        public Pointer MapBlockPointer => BlockPointers[2];

        /// <summary>
        /// The pointer to the texture block
        /// </summary>
        public Pointer TextureBlockPointer => BlockPointers[3];

        /// <summary>
        /// The background block data
        /// </summary>
        public R1_PS1_BackgroundBlock BackgroundData { get; set; }

        /// <summary>
        /// The event block data
        /// </summary>
        public R1_PS1_EventBlock EventData { get; set; }

        /// <summary>
        /// The map block data
        /// </summary>
        public MapData MapData { get; set; }

        /// <summary>
        /// The texture block
        /// </summary>
        public byte[] TextureBlock { get; set; }

        // NOTE: This block is not part of the original game and gets added by the Ray1Map editor!
        public R1_PS1_EditedLevelBlock EditedBlock { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // HEADER
            base.SerializeImpl(s);

            // BACKGROUND BLOCK
            s.DoAt(BackgroundBlockPointer, () => {
                BackgroundData = s.SerializeObject<R1_PS1_BackgroundBlock>(BackgroundData, name: nameof(BackgroundData));
            });

            // EVENT BLOCK
            s.DoAt(EventBlockPointer, () => {
                EventData = s.SerializeObject<R1_PS1_EventBlock>(EventData, name: nameof(EventData));
            });

            // MAP BLOCK
            s.DoAt(MapBlockPointer, () => {
                MapData = s.SerializeObject<MapData>(MapData, name: nameof(MapData));
            });

            // TEXTURE BLOCK
            s.DoAt(TextureBlockPointer, () => {
                TextureBlock = s.SerializeArray<byte>(TextureBlock, FileSize - TextureBlockPointer.FileOffset, name: nameof(TextureBlock));
            });

            // Only serialize if there is data at the end of the file or if the block is not null (then we're assumed to be writing)
            if (s.CurrentLength > FileSize || EditedBlock != null)
                s.DoAt(Offset + FileSize, () => EditedBlock = s.SerializeObject<R1_PS1_EditedLevelBlock>(EditedBlock, name: nameof(EditedBlock)));
        }
    }
}