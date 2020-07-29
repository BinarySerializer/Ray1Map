namespace R1Engine
{
    /// <summary>
    /// Background block data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_BackgroundBlock : R1Serializable
    {
        public BackgroundLayerPosition[] BackgroundDefineNormal { get; set; }
        public BackgroundLayerPosition[] BackgroundDefineDiff { get; set; }

        // LevelDefine_0?
        public byte[] Unknown3 { get; set; }

        /// <summary>
        /// The background layer info items
        /// </summary>
        public Common_ImageDescriptor[] BackgroundLayerInfos { get; set; }

        public byte[] Unknown4 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the background layer information
            BackgroundDefineNormal = s.SerializeObjectArray<BackgroundLayerPosition>(BackgroundDefineNormal, 6, name: nameof(BackgroundDefineNormal));
            BackgroundDefineDiff = s.SerializeObjectArray<BackgroundLayerPosition>(BackgroundDefineDiff, 6, name: nameof(BackgroundDefineDiff));

            Unknown3 = s.SerializeArray<byte>(Unknown3, 16, name: nameof(Unknown3));

            BackgroundLayerInfos = s.SerializeObjectArray<Common_ImageDescriptor>(BackgroundLayerInfos, 12, name: nameof(BackgroundLayerInfos));

            Unknown4 = s.SerializeArray<byte>(Unknown4, s.GameSettings.EngineVersion == EngineVersion.RayPS1JP ? 208 : 80, name: nameof(Unknown4));
        }
    }
}