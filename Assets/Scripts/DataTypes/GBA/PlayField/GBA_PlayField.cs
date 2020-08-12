namespace R1Engine
{
    // 0x03000e20 has pointer to this struct for the current level during runtime

    /// <summary>
    /// Base GBA PlayField
    /// </summary>
    public class GBA_PlayField : GBA_BaseBlock
    {
        /// <summary>
        /// Indicates if the PlayField is of type <see cref="GBA_PlayFieldMode7"/>
        /// </summary>
        public bool IsMode7 { get; set; }

        public GBA_PlayField2D PlayField2D { get; set; }
        public GBA_PlayFieldMode7 PlayFieldMode7 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.PrinceOfPersiaGBA || s.GameSettings.EngineVersion == EngineVersion.StarWarsGBA || s.GameSettings.EngineVersion == EngineVersion.BatmanVengeanceGBA)
                IsMode7 = false;
            else
                IsMode7 = s.Serialize<bool>(IsMode7, name: nameof(IsMode7));

            if (IsMode7)
                PlayFieldMode7 = s.SerializeObject<GBA_PlayFieldMode7>(PlayFieldMode7, name: nameof(PlayFieldMode7));
            else
                PlayField2D = s.SerializeObject<GBA_PlayField2D>(PlayField2D, name: nameof(PlayField2D));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (IsMode7)
                PlayFieldMode7.SerializeOffsetData(s, OffsetTable);
            else
                PlayField2D.SerializeOffsetData(s, OffsetTable);
        }
    }
}