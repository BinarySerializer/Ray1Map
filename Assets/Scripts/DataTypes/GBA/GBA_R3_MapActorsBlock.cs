namespace R1Engine
{
    public class GBA_R3_MapActorsBlock : GBA_R3_BaseBlock
    {
        public uint Unk_00 { get; set; }

        public byte ObjectsCount { get; set; }
        public byte Unk_01 { get; set; }
        public byte Unk_02 { get; set; }
        public byte Unk_03 { get; set; }

        // Might not be the exact same struct, but seems to match
        public GBA_R3_Actor StartPosActor { get; set; }

        public GBA_R3_Actor[] Actors { get; set; }

        // There are more bytes after this. Count seems to match ObjectsCount*4, but the data doesn't.

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize block header
            base.SerializeImpl(s);

            Unk_00 = s.Serialize<uint>(Unk_00, name: nameof(Unk_00));
            
            ObjectsCount = s.Serialize<byte>(ObjectsCount, name: nameof(ObjectsCount));
            Unk_01 = s.Serialize<byte>(Unk_01, name: nameof(Unk_01));
            Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
            Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

            StartPosActor = s.SerializeObject<GBA_R3_Actor>(StartPosActor, name: nameof(StartPosActor));

            Actors = s.SerializeObjectArray<GBA_R3_Actor>(Actors, ObjectsCount, name: nameof(Actors));
        }
    }
}