namespace BinarySerializer.PSP 
{
    public class GE_Command_TexturePixelStorageMode : GE_CommandData 
    {
        public GE_PixelStorageMode PixelStorageMode { get; set; }

        public override void SerializeImpl(BitSerializerObject b) {
            PixelStorageMode = b.SerializeBits<GE_PixelStorageMode>(PixelStorageMode, 24, name: nameof(PixelStorageMode));
		}
    }
}