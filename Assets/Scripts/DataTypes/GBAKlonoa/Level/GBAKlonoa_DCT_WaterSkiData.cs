using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_DCT_WaterSkiData : BinarySerializable
    {
        public Pointer Pointer_00 { get; set; }
        public Pointer Pointer_04 { get; set; }
        public Pointer CommandsPointer { get; set; }

        // Serialized from pointers
        public byte[] Data1 { get; set; }
        public byte[] Data2 { get; set; }
        public GBAKlonoa_DCT_WaterSkiCommands Commands { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Pointer_00 = s.SerializePointer(Pointer_00, name: nameof(Pointer_00));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));
            CommandsPointer = s.SerializePointer(CommandsPointer, name: nameof(CommandsPointer));
            s.SerializePadding(4, logIfNotNull: true);

            s.DoAt(Pointer_00, () => Data1 = s.SerializeArray(Data1, Pointer_04.AbsoluteOffset - Pointer_00.AbsoluteOffset, name: nameof(Data1)));
            s.DoAt(Pointer_04, () => Data2 = s.SerializeArray(Data2, CommandsPointer.AbsoluteOffset - Pointer_04.AbsoluteOffset, name: nameof(Data2)));
            s.DoAt(CommandsPointer, () => Commands = s.SerializeObject<GBAKlonoa_DCT_WaterSkiCommands>(Commands, name: nameof(Commands)));
        }
    }
}