using BinarySerializer;

namespace Ray1Map.Jade
{
    public class TEXPRO_Photo : BinarySerializable {
        public uint FileSize { get; set; } // Set in onPreSerialize

        public uint DataSize { get; set; }
        public byte Photo { get; set; } = 0xFF;

        public override void SerializeImpl(SerializerObject s) {
            if (FileSize > 0) {
                LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
                if (!Loader.IsBinaryData) DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
                Photo = s.Serialize<byte>(Photo, name: nameof(Photo));
                if (!Loader.IsBinaryData && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_BGE_Anniversary_20230403)) {
					s.SerializePadding(3, logIfNotNull: true);
				}
            }
        }
	}
}