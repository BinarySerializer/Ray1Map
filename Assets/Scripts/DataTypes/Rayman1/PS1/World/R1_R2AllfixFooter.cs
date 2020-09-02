namespace R1Engine
{
    /// <summary>
    /// Allfix footer data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class R1_R2AllfixFooter : R1Serializable
    {
        public Pointer RaymanCollisionDataPointer { get; set; }
        public Pointer RaymanBehaviorPointer { get; set; }
        public Pointer RaymanAnimGroupPointer { get; set; }

        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public uint Unk3 { get; set; }

        public Pointer UnkPointer3 { get; set; }

        public byte[] Unk4 { get; set; }


        public R1_R2EventCollision CollisionData { get; set; }
        public R1_R2EventAnimGroup RaymanAnimGroup { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize pointers
            RaymanCollisionDataPointer = s.SerializePointer(RaymanCollisionDataPointer, name: nameof(RaymanCollisionDataPointer));
            RaymanBehaviorPointer = s.SerializePointer(RaymanBehaviorPointer, name: nameof(RaymanBehaviorPointer));
            RaymanAnimGroupPointer = s.SerializePointer(RaymanAnimGroupPointer, name: nameof(RaymanAnimGroupPointer));

            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<uint>(Unk2, name: nameof(Unk2));
            Unk3 = s.Serialize<uint>(Unk3, name: nameof(Unk3));

            UnkPointer3 = s.SerializePointer(UnkPointer3, name: nameof(UnkPointer3));

            Unk4 = s.SerializeArray<byte>(Unk4, 66, name: nameof(Unk4));

            // Serialize Rayman's animation group
            s.DoAt(RaymanAnimGroupPointer, () => RaymanAnimGroup = s.SerializeObject<R1_R2EventAnimGroup>(RaymanAnimGroup, name: nameof(RaymanAnimGroup)));

            // Serialize collision data
            s.DoAt(RaymanCollisionDataPointer, () => CollisionData = s.SerializeObject<R1_R2EventCollision>(CollisionData, name: nameof(CollisionData)));
        }
    }
}