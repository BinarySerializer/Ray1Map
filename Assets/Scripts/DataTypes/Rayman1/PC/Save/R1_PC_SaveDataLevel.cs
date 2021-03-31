using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Level save data for Rayman 1 on PC
    /// </summary>
    public class R1_PC_SaveDataLevel : BinarySerializable
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
            s.SerializeBitValues<byte>(bitFunc =>
            {
                IsUnlocked = bitFunc(IsUnlocked ? 1 : 0, 1, name: nameof(IsUnlocked)) == 1;
                Unk_01 = (byte)bitFunc(Unk_01, 1, name: nameof(Unk_01));
                Cages = (byte)bitFunc(Cages, 3, name: nameof(Cages));
                Unk_05 = (byte)bitFunc(Unk_05, 3, name: nameof(Unk_05));
            });
        }
    }
}