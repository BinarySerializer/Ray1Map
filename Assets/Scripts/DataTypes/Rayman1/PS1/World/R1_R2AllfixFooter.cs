namespace R1Engine
{
    /// <summary>
    /// Allfix footer data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class R1_R2AllfixFooter : R1Serializable
    {
        public Pointer UnkPointer1 { get; set; }
        public Pointer UnkPointer2 { get; set; }

        /// <summary>
        /// The pointer to Rayman's animation group
        /// </summary>
        public Pointer RaymanAnimGroupPointer { get; set; }

        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public uint Unk3 { get; set; }

        public Pointer UnkPointer3 { get; set; }

        public byte[] Unk4 { get; set; }


        /// <summary>
        /// Rayman's animation group
        /// </summary>
        public R1_R2EventAnimGroup RaymanAnimGroup { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize pointers
            UnkPointer1 = s.SerializePointer(UnkPointer1, name: nameof(UnkPointer1));
            UnkPointer2 = s.SerializePointer(UnkPointer2, name: nameof(UnkPointer2));
            RaymanAnimGroupPointer = s.SerializePointer(RaymanAnimGroupPointer, name: nameof(RaymanAnimGroupPointer));

            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<uint>(Unk2, name: nameof(Unk2));
            Unk3 = s.Serialize<uint>(Unk3, name: nameof(Unk3));

            UnkPointer3 = s.SerializePointer(UnkPointer3, name: nameof(UnkPointer3));

            Unk4 = s.SerializeArray<byte>(Unk4, 66, name: nameof(Unk4));

            // Serialize Rayman's animation group
            s.DoAt(RaymanAnimGroupPointer, () => RaymanAnimGroup = s.SerializeObject<R1_R2EventAnimGroup>(RaymanAnimGroup, name: nameof(RaymanAnimGroup)));
        }
    }
}