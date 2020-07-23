namespace R1Engine
{
    /// <summary>
    /// Level save data for Rayman 1 on PC
    /// </summary>
    public class Rayman1PCSaveDataLevel : R1Serializable
    {
        /// <summary>
        /// Indicates if the level has been unlocked on the world map
        /// </summary>
        public bool IsUnlocked { get; set; }

        public byte Unk_01 { get; set; }

        /// <summary>
        /// The amount of cages in the level (0-6)
        /// </summary>
        public byte Cages { get; set; }

        public byte Unk_05 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            byte value = 0;

            value = (byte)BitHelpers.SetBits(value, IsUnlocked ? 1 : 0, 1, 0);
            value = (byte)BitHelpers.SetBits(value, Unk_01, 1, 1);
            value = (byte)BitHelpers.SetBits(value, Cages, 3, 2);
            value = (byte)BitHelpers.SetBits(value, Unk_05, 3, 5);

            value = s.Serialize<byte>(value, name: nameof(value));

            IsUnlocked = BitHelpers.ExtractBits(value, 1, 0) == 1;
            Unk_01 = (byte)BitHelpers.ExtractBits(value, 1, 1);
            Cages = (byte)BitHelpers.ExtractBits(value, 3, 2);
            Unk_05 = (byte)BitHelpers.ExtractBits(value, 3, 5);
        }
    }
}