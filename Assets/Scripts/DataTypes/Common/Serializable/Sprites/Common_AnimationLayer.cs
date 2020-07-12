using System;

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
        public bool IsFlippedHorizontally
        {
            get => Flags.HasFlag(Common_AnimationLayerFlags.FlippedHorizontally);
            set
            {
                if (value)
                    Flags |= Common_AnimationLayerFlags.FlippedHorizontally;
                else
                    Flags &= ~Common_AnimationLayerFlags.FlippedHorizontally;
            }
        }

        /// <summary>
        /// Indicates if the layer is flipped vertically
        /// </summary>
        public bool IsFlippedVertically
        {
            get => Flags.HasFlag(Common_AnimationLayerFlags.FlippedVertically);
            set
            {
                if (value)
                    Flags |= Common_AnimationLayerFlags.FlippedVertically;
                else
                    Flags &= ~Common_AnimationLayerFlags.FlippedVertically;
            }
        }

        /*private byte JaguarXByte {
            get {
                byte val = 0;
                val = (byte)BitHelpers.SetBits(val, XPosition, 7, 0);
                val = (byte)BitHelpers.SetBits(val, IsFlippedHorizontally ? 1 : 0, 1, 7);
                return val;
            }
            set {
                XPosition = (byte)BitHelpers.ExtractBits(value, 7, 0);
                IsFlippedHorizontally = BitHelpers.ExtractBits(value, 1, 7) != 0;
            }
        }

        private byte JaguarImageIndexByte {
            get {
                byte val = 0;
                val = (byte)BitHelpers.SetBits(val, ImageIndex, 7, 0);
                val = (byte)BitHelpers.SetBits(val, IsFlippedHorizontally ? 1 : 0, 1, 7);
                return val;
            }
            set {
                ImageIndex = (byte)BitHelpers.ExtractBits(value, 7, 0);
                IsFlippedHorizontally = BitHelpers.ExtractBits(value, 1, 7) != 0;
            }
        }
        public bool FlipFlagInX = false;*/
        public byte JaguarXByte { get; set; }
        public byte JaguarImageIndexByte { get; set; }

        /// <summary>
        /// The animation layer flags
        /// </summary>
        public Common_AnimationLayerFlags Flags { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public byte XPosition { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public byte YPosition { get; set; }

        /// <summary>
        /// The image index from the available sprites
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
                value = (ushort)BitHelpers.SetBits(value, (int)Flags, 4, 12);

                value = s.Serialize<ushort>(value, name: nameof(value));

                ImageIndex = (ushort)(BitHelpers.ExtractBits(value, 12, 0));
                Flags = (Common_AnimationLayerFlags)BitHelpers.ExtractBits(value, 4, 12);

                XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
            }
            else if (s.GameSettings.MajorEngineVersion == MajorEngineVersion.Jaguar)
            {
                /*if (FlipFlagInX) {
                    JaguarXByte = s.Serialize<byte>(JaguarXByte, name: nameof(JaguarXByte));
                    YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
                    ImageIndex = s.Serialize<byte>((byte)ImageIndex, name: nameof(ImageIndex));
                } else {
                    XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
                    YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
                    JaguarImageIndexByte = s.Serialize<byte>(JaguarImageIndexByte, name: nameof(JaguarImageIndexByte));
                }*/
                XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
                ImageIndex = s.Serialize<byte>((byte)ImageIndex, name: nameof(ImageIndex));
            }
            else
            {
                IsFlippedHorizontally = s.Serialize<bool>(IsFlippedHorizontally, name: nameof(IsFlippedHorizontally));
                XPosition = s.Serialize<byte>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<byte>(YPosition, name: nameof(YPosition));
                ImageIndex = s.Serialize<byte>((byte)ImageIndex, name: nameof(ImageIndex));
            }
        }

        /// <summary>
        /// Flags for <see cref="Common_AnimationLayer"/>
        /// </summary>
        [Flags]
        public enum Common_AnimationLayerFlags
        {
            None = 0,
            UnkFlag_0 = 1 << 0,
            UnkFlag_1 = 1 << 1,
            FlippedHorizontally = 1 << 2,
            FlippedVertically = 1 << 3,
        }
    }
}