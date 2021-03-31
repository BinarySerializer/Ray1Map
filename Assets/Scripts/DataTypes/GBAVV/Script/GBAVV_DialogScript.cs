using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_DialogScript : BinarySerializable
    {
        public int ID { get; set; }
        public Pointer ScriptPointer { get; set; }

        public GBAVV_Script Script { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<int>(ID, name: nameof(ID));
            ScriptPointer = s.SerializePointer(ScriptPointer, name: nameof(ScriptPointer));

            Script = s.DoAt(ScriptPointer, () => s.SerializeObject<GBAVV_Script>(Script, name: nameof(Script)));
        }
    }
}