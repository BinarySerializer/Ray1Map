using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_IDXEntry : BinarySerializable
    {
        public uint DestinationPointer { get; set; } // The game copies the load commands pointer to this location
        public Pointer LoadCommandsPointer { get; set; }

        public PS1Klonoa_IDXLoadCommand[] LoadCommands { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DestinationPointer = s.Serialize<uint>(DestinationPointer, name: nameof(DestinationPointer));
            s.Log($"{nameof(DestinationPointer)}: 0x{DestinationPointer:X8}");
            LoadCommandsPointer = s.SerializePointer(LoadCommandsPointer, name: nameof(LoadCommandsPointer));

            s.DoAt(LoadCommandsPointer, () => LoadCommands = s.SerializeObjectArrayUntil(LoadCommands, x => x.Type == 0, name: nameof(LoadCommands)));
        }
    }
}