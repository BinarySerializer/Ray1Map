namespace BinarySerializer.PSP 
{
    public class GE_Command_TextureFunction : GE_CommandData 
    {
        public GE_TextureEffect TextureEffect { get; set; }
        public bool TextureColorComponent { get; set; } // Read texture alpha, or ignore it?
        public bool FragmentDoubleEnable { get; set; } // Double fragment color, or leave it untouched?

        public override void SerializeImpl(BitSerializerObject b) {
            TextureEffect = b.SerializeBits<GE_TextureEffect>(TextureEffect, 3, name: nameof(TextureEffect));
			b.SerializePadding(5, logIfNotNull: true);
			TextureColorComponent = b.SerializeBits<bool>(TextureColorComponent, 1, name: nameof(TextureColorComponent));
			b.SerializePadding(7, logIfNotNull: true);
			FragmentDoubleEnable = b.SerializeBits<bool>(FragmentDoubleEnable, 1, name: nameof(FragmentDoubleEnable));
			b.SerializePadding(7, logIfNotNull: true);
		}
    }
}