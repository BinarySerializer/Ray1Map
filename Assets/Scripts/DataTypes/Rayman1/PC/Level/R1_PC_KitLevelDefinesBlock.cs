using BinarySerializer;

namespace R1Engine
{
    public class R1_PC_KitLevelDefinesBlock : BinarySerializable
    {
        public byte LevelDefineChecksum { get; set; }
        public byte[] LevelDefine_0 { get; set; }
        public byte MusicTrack { get; set; }

        // Why are there 3 background values?
        public byte BG_0 { get; set; }
        public byte BG_1 { get; set; }
        public byte BG_2 { get; set; }

        public byte[] LevelDefine_1 { get; set; }
        
        public R1_RayEvtsFlags RayEvts { get; set; }
        public byte UnkByte { get; set; } // Padding?

        public byte BackgroundDefineNormalChecksum { get; set; }
        public R1_BackgroundLayerPosition[] BackgroundDefineNormal { get; set; }

        public byte BackgroundDefineDiffChecksum { get; set; }
        public R1_BackgroundLayerPosition[] BackgroundDefineDiff { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            var isEncryptedAndChecksum = s.GetR1Settings().EngineVersion != EngineVersion.R1_PS1_Edu;

            LevelDefineChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR((byte)(isEncryptedAndChecksum ? 0x57 : 0), () =>
                {
                    LevelDefine_0 = s.SerializeArray<byte>(LevelDefine_0, 6, name: nameof(LevelDefine_0));
                    MusicTrack = s.Serialize<byte>(MusicTrack, name: nameof(MusicTrack));
                    BG_0 = s.Serialize<byte>(BG_0, name: nameof(BG_0));
                    BG_1 = s.Serialize<byte>(BG_1, name: nameof(BG_1));
                    BG_2 = s.Serialize<byte>(BG_2, name: nameof(BG_2));
                    LevelDefine_1 = s.SerializeArray<byte>(LevelDefine_1, 3, name: nameof(LevelDefine_1));
                    RayEvts = s.Serialize<R1_RayEvtsFlags>(RayEvts, name: nameof(RayEvts));
                    UnkByte = s.Serialize<byte>(UnkByte, name: nameof(UnkByte));
                });
            }, ChecksumPlacement.Before, calculateChecksum: isEncryptedAndChecksum, name: nameof(LevelDefineChecksum));

            BackgroundDefineNormalChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR((byte)(isEncryptedAndChecksum ? 0xA5 : 0), () => BackgroundDefineNormal = s.SerializeObjectArray<R1_BackgroundLayerPosition>(BackgroundDefineNormal, 6, name: nameof(BackgroundDefineNormal)));
            }, ChecksumPlacement.Before, calculateChecksum: isEncryptedAndChecksum, name: nameof(BackgroundDefineNormalChecksum));

            BackgroundDefineDiffChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR((byte)(isEncryptedAndChecksum ? 0xA5 : 0), () => BackgroundDefineDiff = s.SerializeObjectArray<R1_BackgroundLayerPosition>(BackgroundDefineDiff, 6, name: nameof(BackgroundDefineDiff)));
            }, ChecksumPlacement.Before, calculateChecksum: isEncryptedAndChecksum, name: nameof(BackgroundDefineDiffChecksum));
        }
    }
}