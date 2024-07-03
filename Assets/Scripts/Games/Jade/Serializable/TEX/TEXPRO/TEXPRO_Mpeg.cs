using BinarySerializer;

namespace Ray1Map.Jade
{
    public class TEXPRO_Mpeg : BinarySerializable {
        public uint FileSize { get; set; } // Set in onPreSerialize

        public uint SerializedFileSize { get; set; }
        public Jade_Reference<TEXPRO_Mpeg_Content> Content { get; set; }
        public uint PSX2_IPUKey { get; set; }
        public uint MaxBufSize { get; set; }
        public uint ImagesCount { get; set; }
        public uint MpegSize { get; set; }
        public uint SizeY { get; set; }
        public uint SizeX { get; set; }
        public uint BINK_Key { get; set; }
        public uint BinkSizeX { get; set; }
        public uint BinkSizeY { get; set; }
        public uint BinkBpp { get; set; }
        public uint XMV_Key { get; set; }

        public uint ContentFileSize { get; set; }

        public Jade_Key[] BINK_Keys { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            if (FileSize > 0) {
                SerializedFileSize = s.Serialize<uint>(SerializedFileSize, name: nameof(SerializedFileSize));
                if (FileSize != SerializedFileSize) return;
                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_BGE_Anniversary)) {
                    BINK_Keys = s.SerializeObjectArray<Jade_Key>(BINK_Keys, 3, name: nameof(BINK_Keys));
                } else {
                    Content = s.SerializeObject<Jade_Reference<TEXPRO_Mpeg_Content>>(Content, name: nameof(Content));
                    if (SerializedFileSize >= 0x20) {
                        PSX2_IPUKey = s.Serialize<uint>(PSX2_IPUKey, name: nameof(PSX2_IPUKey));
                        MaxBufSize = s.Serialize<uint>(MaxBufSize, name: nameof(MaxBufSize));
                        ImagesCount = s.Serialize<uint>(ImagesCount, name: nameof(ImagesCount));
                        MpegSize = s.Serialize<uint>(MpegSize, name: nameof(MpegSize));
                        SizeY = s.Serialize<uint>(SizeY, name: nameof(SizeY));
                        SizeX = s.Serialize<uint>(SizeX, name: nameof(SizeX));
                    }
                    if (SerializedFileSize >= 0x30) {
                        BINK_Key = s.Serialize<uint>(BINK_Key, name: nameof(BINK_Key));
                        BinkSizeX = s.Serialize<uint>(BinkSizeX, name: nameof(BinkSizeX));
                        BinkSizeY = s.Serialize<uint>(BinkSizeY, name: nameof(BinkSizeY));
                        BinkBpp = s.Serialize<uint>(BinkBpp, name: nameof(BinkBpp));
                    }
                    if (SerializedFileSize > 0x30) {
                        XMV_Key = s.Serialize<uint>(XMV_Key, name: nameof(XMV_Key));
                    }

                    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
                    Loader.RequestFileSize(Content.Key, ContentFileSize, (size) => ContentFileSize = size, name: nameof(ContentFileSize));
                }
            }
        }
	}
}