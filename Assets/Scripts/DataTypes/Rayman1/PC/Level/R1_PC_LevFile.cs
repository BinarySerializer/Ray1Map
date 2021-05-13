using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Level data for PC
    /// </summary>
    public class R1_PC_LevFile : R1_PCBaseFile
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

        public R1_PC_KitLevelDefinesBlock KitLevelDefines { get; set; }

        /// <summary>
        /// The map data
        /// </summary>
        public R1_PC_MapBlock MapData { get; set; }

        /// <summary>
        /// The index of the background image
        /// </summary>
        public byte FNDIndex { get; set; }

        /// <summary>
        /// The index of the parallax background image
        /// </summary>
        public byte ScrollDiffFNDIndex { get; set; }

        /// <summary>
        /// The DES for the background sprites when parallax scrolling is enabled
        /// </summary>
        public int ScrollDiffSprites { get; set; }

        /// <summary>
        /// The rough tile texture data
        /// </summary>
        public R1_PC_RoughTileTextureBlock RoughTileTextureData { get; set; }

        // Leftover data for the rough textures in versions which don't use it. In RayKit there is still a lot of data there, but it doesn't appear to match with how it's being parsed.
        public byte[] LeftoverRoughTextureBlock { get; set; }

        /// <summary>
        /// The tile texture data
        /// </summary>
        public R1_PC_TileTextureBlock TileTextureData { get; set; }

        /// <summary>
        /// The event data
        /// </summary>
        public R1_PC_EventBlock EventData { get; set; }

        public R1_PC_ProfileDefine ProfileDefine { get; set; }

        public byte EDU_AlphaChecksum { get; set; }
        public byte[][] EDU_Alpha { get; set; }

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
            bool allowInvalid = s.GetR1Settings().EngineVersion == EngineVersion.R1_PocketPC || s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanClassicMobile;
            EventBlockPointer = s.SerializePointer(EventBlockPointer, allowInvalid: allowInvalid, name: nameof(EventBlockPointer));
            TextureBlockPointer = s.SerializePointer(TextureBlockPointer, allowInvalid: allowInvalid, name: nameof(TextureBlockPointer));

            // Serialize the level defines
            if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Kit || s.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Edu)
                KitLevelDefines = s.SerializeObject<R1_PC_KitLevelDefinesBlock>(KitLevelDefines, name: nameof(KitLevelDefines));

            // Serialize the map data
            MapData = s.SerializeObject<R1_PC_MapBlock>(MapData, name: nameof(MapData));

            // Serialize the background data
            if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PC || s.GetR1Settings().EngineVersion == EngineVersion.R1_PocketPC)
            {
                // Serialize the background data
                FNDIndex = s.Serialize<byte>(FNDIndex, name: nameof(FNDIndex));
                ScrollDiffFNDIndex = s.Serialize<byte>(ScrollDiffFNDIndex, name: nameof(ScrollDiffFNDIndex));
                ScrollDiffSprites = s.Serialize<int>(ScrollDiffSprites, name: nameof(ScrollDiffSprites));
            }

            // Serialize the rough tile textures
            if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PC)
                RoughTileTextureData = s.SerializeObject<R1_PC_RoughTileTextureBlock>(RoughTileTextureData, name: nameof(RoughTileTextureData));
            else
                LeftoverRoughTextureBlock = s.SerializeArray<byte>(LeftoverRoughTextureBlock, TextureBlockPointer.FileOffset - s.CurrentPointer.FileOffset, name: nameof(LeftoverRoughTextureBlock));

            // At this point the stream position should match the texture block offset
            if (s.CurrentPointer != TextureBlockPointer)
                Debug.LogError("Texture block offset is incorrect");

            // Serialize the tile textures
            TileTextureData = s.SerializeObject<R1_PC_TileTextureBlock>(TileTextureData, name: nameof(TileTextureData));

            // At this point the stream position should match the event block offset (ignore the Pocket PC version here since it uses leftover pointers from PC version)
            if (s.GetR1Settings().EngineVersion != EngineVersion.R1_PocketPC && s.CurrentPointer != EventBlockPointer)
                Debug.LogError("Event block offset is incorrect");

            // Serialize the event data
            EventData = s.SerializeObject<R1_PC_EventBlock>(EventData, name: nameof(EventData));

            // Serialize the profile define data (only on By his Fans and 60 Levels)
            if (s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanByHisFansPC || s.GetR1Settings().GameModeSelection == GameModeSelection.Rayman60LevelsPC)
                ProfileDefine = s.SerializeObject<R1_PC_ProfileDefine>(ProfileDefine, name: nameof(ProfileDefine));

            // Serialize alpha data (only on EDU)
            if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Edu)
            {
                EDU_AlphaChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
                {
                    if (EDU_Alpha == null)
                        EDU_Alpha = new byte[480][];

                    for (int i = 0; i < EDU_Alpha.Length; i++)
                        EDU_Alpha[i] = s.SerializeArray<byte>(EDU_Alpha[i], 256, name: $"{nameof(EDU_Alpha)}[{i}]");
                }, ChecksumPlacement.Before, name: nameof(EDU_AlphaChecksum));
            }
        }

        #endregion
    }
}