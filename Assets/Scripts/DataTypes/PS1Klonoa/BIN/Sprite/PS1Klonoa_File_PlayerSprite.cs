using BinarySerializer.PS1;

namespace BinarySerializer.KlonoaDTP
{
    public class PS1Klonoa_File_PlayerSprite : PS1Klonoa_BaseFile
    {
        public PS1_TIM TIM { get; set; }

        public uint Raw_Size { get; set; }
        public ushort Raw_Width { get; set; }
        public ushort Raw_Height { get; set; }
        public byte[] Raw_ImgData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // If ULZ compressed then it's a TIM file (the x and y coordinates are irrelevant though)
            if (Pre_IsCompressed)
            {
                TIM = s.SerializeObject<PS1_TIM>(TIM, name: nameof(TIM));
            }
            // If not ULZ compressed it's raw data compressed using an unknown compression type
            else
            {
                Raw_Size = s.Serialize<uint>(Raw_Size, name: nameof(Raw_Size));
                Raw_Width = s.Serialize<ushort>(Raw_Width, name: nameof(Raw_Width));
                Raw_Height = s.Serialize<ushort>(Raw_Height, name: nameof(Raw_Height));
                s.DoEncoded(new PS1Klonoa_TextureBlockEncoder(Raw_Size), () =>
                {
                    Raw_ImgData = s.SerializeArray<byte>(Raw_ImgData, Raw_Size, name: nameof(Raw_ImgData));
                });
            }
        }
    }
}