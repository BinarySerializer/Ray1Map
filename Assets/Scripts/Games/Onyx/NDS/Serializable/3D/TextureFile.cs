namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class TextureFile : OnyxFile
    {
        public uint ImgDataLength { get; set; }
        public byte[] ImgData { get; set; }
        public uint Unknown { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public override void SerializeFile(SerializerObject s)
        {
            ImgDataLength = s.Serialize<uint>(ImgDataLength, name: nameof(ImgDataLength));
            ImgData = s.SerializeArray<byte>(ImgData, ImgDataLength, name: nameof(ImgData));
            Unknown = s.Serialize<uint>(Unknown, name: nameof(Unknown));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
        }
    }
}