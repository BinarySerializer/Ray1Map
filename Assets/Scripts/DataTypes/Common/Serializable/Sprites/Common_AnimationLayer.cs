namespace R1Engine
{
    /// <summary>
    /// Common animation layer data
    /// </summary>
    public class Common_AnimationLayer : R1Serializable
    {
        /// <summary>
        /// Indicates if the layer is flipped horizontally
        /// </summary>
        public bool IsFlippedHorizontally { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public byte XPosition { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public byte YPosition { get; set; }

        /// <summary>
        /// The image index as it appears in the image block
        /// </summary>
        public ushort ImageIndex { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            if (s.GameSettings.EngineVersion == EngineVersion.Ray2PS1)
            {
                ushort value = 0;

                value = (ushort)BitHelpers.SetBits(value, ImageIndex, 12, 0);
                // TODO: There are most likely other flags here too, such as for flipping vertically (check the cannon target indicator sprite!)
                value = (ushort)BitHelpers.SetBits(value, IsFlippedHorizontally ? 1 : 0, 4, 12);

                value = s.Serialize<ushort>(value, name: nameof(value));

                ImageIndex = (ushort)(BitHelpers.ExtractBits(value, 12, 0));
                IsFlippedHorizontally = BitHelpers.ExtractBits(value, 4, 12) != 0;

                XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
            }
            else
            {
                IsFlippedHorizontally = s.Serialize<bool>(IsFlippedHorizontally, name: nameof(IsFlippedHorizontally));
                XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
                ImageIndex = s.Serialize<byte>((byte)ImageIndex, name: nameof(ImageIndex));
            }
        }
    }
}