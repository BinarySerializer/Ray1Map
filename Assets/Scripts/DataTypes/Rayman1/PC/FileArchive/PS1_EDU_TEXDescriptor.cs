using System;

namespace R1Engine
{
    public class PS1_EDU_TEXDescriptor : R1Serializable
    {
        public byte XInPage { get; set; }
        public byte YInPage { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte PageIndex { get; set; }

        // The next fields are set at runtime, so they can be ignored
        public byte BitDepth { get; set; }
        public ushort PageInfo { get; set; }
        public ushort Unk1 { get; set; }
        public ushort Unk2 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            XInPage = s.Serialize<byte>(XInPage, name: nameof(XInPage));
            YInPage = s.Serialize<byte>(YInPage, name: nameof(YInPage));
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            PageIndex = s.Serialize<byte>(PageIndex, name: nameof(PageIndex));
            BitDepth = s.Serialize<byte>(BitDepth, name: nameof(BitDepth));
            PageInfo = s.Serialize<ushort>(PageInfo, name: nameof(PageInfo));
            Unk1 = s.Serialize<ushort>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<ushort>(Unk2, name: nameof(Unk2));
        }
    }
}