using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_GameCubeMapInfo : BinarySerializable
    {
        public ushort Unk1 { get; set; }
        public ushort MusicIndex { get; set; }
        public ushort Unk2 { get; set; }
        public ushort Unk3 { get; set; }
        public string LevelName { get; set; }
        public uint FileSize { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<ushort>(Unk1, name: nameof(Unk1));
            MusicIndex = s.Serialize<ushort>(MusicIndex, name: nameof(MusicIndex));
            Unk2 = s.Serialize<ushort>(Unk2, name: nameof(Unk2));
            Unk3 = s.Serialize<ushort>(Unk3, name: nameof(Unk3));
            LevelName = s.SerializeString(LevelName, 32, name: nameof(LevelName));
            FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
        }
    }
}