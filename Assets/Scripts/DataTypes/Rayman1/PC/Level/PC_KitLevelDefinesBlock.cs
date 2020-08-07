namespace R1Engine
{
    public class PC_KitLevelDefinesBlock : R1Serializable
    {
        public byte LevelDefineChecksum { get; set; }
        public byte[] LevelDefine_0 { get; set; }
        public byte[] LevelDefine_1 { get; set; }

        public byte BackgroundDefineNormalChecksum { get; set; }
        public BackgroundLayerPosition[] BackgroundDefineNormal { get; set; }

        public byte BackgroundDefineDiffChecksum { get; set; }
        public BackgroundLayerPosition[] BackgroundDefineDiff { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            var isEncryptedAndChecksum = s.GameSettings.EngineVersion != EngineVersion.RayEduPS1;

            LevelDefineChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR((byte)(isEncryptedAndChecksum ? 0x57 : 0), () =>
                {
                    LevelDefine_0 = s.SerializeArray<byte>(LevelDefine_0, 13, name: nameof(LevelDefine_0));
                    LevelDefine_1 = s.SerializeArray<byte>(LevelDefine_1, 3, name: nameof(LevelDefine_1));
                });
            }, ChecksumPlacement.Before, calculateChecksum: isEncryptedAndChecksum, name: nameof(LevelDefineChecksum));

            BackgroundDefineNormalChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR((byte)(isEncryptedAndChecksum ? 0xA5 : 0), () => BackgroundDefineNormal = s.SerializeObjectArray<BackgroundLayerPosition>(BackgroundDefineNormal, 6, name: nameof(BackgroundDefineNormal)));
            }, ChecksumPlacement.Before, calculateChecksum: isEncryptedAndChecksum, name: nameof(BackgroundDefineNormalChecksum));

            BackgroundDefineDiffChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR((byte)(isEncryptedAndChecksum ? 0xA5 : 0), () => BackgroundDefineDiff = s.SerializeObjectArray<BackgroundLayerPosition>(BackgroundDefineDiff, 6, name: nameof(BackgroundDefineDiff)));
            }, ChecksumPlacement.Before, calculateChecksum: isEncryptedAndChecksum, name: nameof(BackgroundDefineDiffChecksum));
        }
    }
}