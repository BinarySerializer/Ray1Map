namespace R1Engine
{
    /// <summary>
    /// Manifest data for GameCube DLC maps
    /// </summary>
    public class GBA_GameCubeMapManifest : R1Serializable
    {
        public uint MapCount { get; set; }
        public GBA_GameCubeMapInfo[] MapInfos { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            MapCount = s.Serialize<uint>(MapCount, name: nameof(MapCount));
            MapInfos = s.SerializeObjectArray<GBA_GameCubeMapInfo>(MapInfos, MapCount, name: nameof(MapInfos));
        }
    }
}