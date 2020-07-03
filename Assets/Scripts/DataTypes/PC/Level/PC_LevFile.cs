using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Level data for PC
    /// </summary>
    public class PC_LevFile : PC_BaseFile
    {
        #region Public Properties

        /// <summary>
        /// The pointer to the event block. The game uses this to skip the texture block if the game should use the rough textures.
        /// </summary>
        public Pointer EventBlockPointer { get; set; }

        /// <summary>
        /// The pointer to the texture block. The game uses this to skip the rough the textures if the game should use the normal textures.
        /// </summary>
        public Pointer TextureBlockPointer { get; set; }

        public PC_KitLevelDefinesBlock KitLevelDefines { get; set; }

        /// <summary>
        /// The map data
        /// </summary>
        public PC_MapBlock MapData { get; set; }

        /// <summary>
        /// The index of the background image
        /// </summary>
        public byte BackgroundIndex { get; set; }

        /// <summary>
        /// The index of the parallax background image
        /// </summary>
        public byte ParallaxBackgroundIndex { get; set; }

        /// <summary>
        /// The DES for the background sprites when parallax scrolling is enabled
        /// </summary>
        public int BackgroundSpritesDES { get; set; }

        /// <summary>
        /// The rough tile texture data
        /// </summary>
        public PC_RoughTileTextureBlock RoughTileTextureData { get; set; }

        // Leftover data for the rough textures in versions which don't use it. In RayKit there is still a lot of data there, but it doesn't appear to match with how it's being parsed.
        public byte[] LeftoverRoughTextureBlock { get; set; }

        /// <summary>
        /// The tile texture data
        /// </summary>
        public PC_TileTextureBlock TileTextureData { get; set; }

        /// <summary>
        /// The event data
        /// </summary>
        public PC_EventBlock EventData { get; set; }

        public PC_ProfileDefine ProfileDefine { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // Serialize the header
            base.SerializeImpl(s);

            // Serialize the pointers
            bool allowInvalid = s.GameSettings.EngineVersion == EngineVersion.RayPocketPC || s.GameSettings.GameModeSelection == GameModeSelection.RaymanClassicMobile;
            EventBlockPointer = s.SerializePointer(EventBlockPointer, allowInvalid: allowInvalid, name: nameof(EventBlockPointer));
            TextureBlockPointer = s.SerializePointer(TextureBlockPointer, allowInvalid: allowInvalid, name: nameof(TextureBlockPointer));

            // Serialize the level defines
            if (s.GameSettings.EngineVersion == EngineVersion.RayKitPC || s.GameSettings.EngineVersion == EngineVersion.RayEduPC)
                KitLevelDefines = s.SerializeObject<PC_KitLevelDefinesBlock>(KitLevelDefines, name: nameof(KitLevelDefines));

            // Serialize the map data
            MapData = s.SerializeObject<PC_MapBlock>(MapData, name: nameof(MapData));

            // Serialize the background data
            if (s.GameSettings.EngineVersion == EngineVersion.RayPC || s.GameSettings.EngineVersion == EngineVersion.RayPocketPC)
            {
                // Serialize the background data
                BackgroundIndex = s.Serialize<byte>(BackgroundIndex, name: nameof(BackgroundIndex));
                ParallaxBackgroundIndex = s.Serialize<byte>(ParallaxBackgroundIndex, name: nameof(ParallaxBackgroundIndex));
                BackgroundSpritesDES = s.Serialize<int>(BackgroundSpritesDES, name: nameof(BackgroundSpritesDES));
            }

            // Serialize the rough tile textures
            if (s.GameSettings.EngineVersion == EngineVersion.RayPC)
                RoughTileTextureData = s.SerializeObject<PC_RoughTileTextureBlock>(RoughTileTextureData, name: nameof(RoughTileTextureData));
            else
                LeftoverRoughTextureBlock = s.SerializeArray<byte>(LeftoverRoughTextureBlock, TextureBlockPointer.FileOffset - s.CurrentPointer.FileOffset, name: nameof(LeftoverRoughTextureBlock));

            // At this point the stream position should match the texture block offset
            if (s.CurrentPointer != TextureBlockPointer)
                Debug.LogError("Texture block offset is incorrect");

            // Serialize the tile textures
            TileTextureData = s.SerializeObject<PC_TileTextureBlock>(TileTextureData, name: nameof(TileTextureData));

            // At this point the stream position should match the event block offset (ignore the Pocket PC version here since it uses leftover pointers from PC version)
            if (s.GameSettings.EngineVersion != EngineVersion.RayPocketPC && s.CurrentPointer != EventBlockPointer)
                Debug.LogError("Event block offset is incorrect");

            // Serialize the event data
            EventData = s.SerializeObject<PC_EventBlock>(EventData, name: nameof(EventData));

            // Serialize the profile define data (only on By his Fans and 60 Levels)
            if (s.GameSettings.GameModeSelection == GameModeSelection.RaymanByHisFansPC || s.GameSettings.GameModeSelection == GameModeSelection.Rayman60LevelsPC)
                ProfileDefine = s.SerializeObject<PC_ProfileDefine>(ProfileDefine, name: nameof(ProfileDefine));
        }

        #endregion
    }
}