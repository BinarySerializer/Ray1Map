namespace BinarySerializer.PSP 
{
    public class GE_Command_TextureMode : GE_CommandData 
    {
        public bool SwizzleEnable { get; set; }
		public byte Unknown { get; set; }
		public byte MaxMipmapLevel { get; set; }

        public override void SerializeImpl(BitSerializerObject b) {
			SwizzleEnable = b.SerializeBits<bool>(SwizzleEnable, 1, name: nameof(SwizzleEnable));
			b.SerializePadding(7, logIfNotNull: true);
			Unknown = b.SerializeBits<byte>(Unknown, 8, name: nameof(Unknown));
			MaxMipmapLevel = b.SerializeBits<byte>(MaxMipmapLevel, 5, name: nameof(MaxMipmapLevel));
			b.SerializePadding(3, logIfNotNull: true);
		}
    }
}