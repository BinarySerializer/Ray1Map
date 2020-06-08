namespace R1Engine
{
    public class PC_KitLevelDefinesBlock : R1Serializable
    {
        public byte LevelDefineChecksum { get; set; }
        public byte[] LevelDefine_0 { get; set; }
        public byte[] LevelDefine_1 { get; set; }

        public byte BackGroundDefineNormalChecksum { get; set; }
        public byte[] BackGroundDefineNormal { get; set; }

        public byte BackGroundDefineDiffChecksum { get; set; }
        public byte[] BackGroundDefineDiff { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            LevelDefineChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR(0x57, () =>
                {
                    LevelDefine_0 = s.SerializeArray<byte>(LevelDefine_0, 13, name: nameof(LevelDefine_0));
                    LevelDefine_1 = s.SerializeArray<byte>(LevelDefine_1, 3, name: nameof(LevelDefine_1));
                });
            }, ChecksumPlacement.Before, name: nameof(LevelDefineChecksum));

            BackGroundDefineNormalChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR(0xA5, () => BackGroundDefineNormal = s.SerializeArray<byte>(LevelDefine_0, 24, name: nameof(LevelDefine_0)));
            }, ChecksumPlacement.Before, name: nameof(BackGroundDefineNormalChecksum));

            BackGroundDefineDiffChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR(0xA5, () => BackGroundDefineDiff = s.SerializeArray<byte>(LevelDefine_0, 24, name: nameof(LevelDefine_0)));
            }, ChecksumPlacement.Before, name: nameof(BackGroundDefineDiffChecksum));
        }
    }
}