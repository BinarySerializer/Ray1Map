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
            else if (s.GameSettings.EngineVersion == EngineVersion.RayJaguar)
            {
                // TODO: Where is flip flag?

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