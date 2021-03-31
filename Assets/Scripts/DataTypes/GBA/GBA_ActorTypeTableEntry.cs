using BinarySerializer;

namespace R1Engine
{
    public class GBA_ActorTypeTableEntry : BinarySerializable
    {
        public string ActorID { get; set; }
        public uint Uint_04 { get; set; }
        public Pointer ActorFunctionPointer { get; set; }
        public uint Uint_0C { get; set; }
        public uint Uint_20 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ActorID = s.SerializeString(ActorID, length: 4, name: nameof(ActorID));
            Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));
            ActorFunctionPointer = s.SerializePointer(ActorFunctionPointer, name: nameof(ActorFunctionPointer));
            Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));
            Uint_20 = s.Serialize<uint>(Uint_20, name: nameof(Uint_20));
        }
    }
}