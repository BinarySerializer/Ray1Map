using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_Dialog : BinarySerializable
    {
        public uint ID { get; set; }
        public Pointer DialogDataPointer { get; set; }

        public GBAIsometric_Spyro_DialogData DialogData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<uint>(ID, name: nameof(ID));
            DialogDataPointer = s.SerializePointer(DialogDataPointer, name: nameof(DialogDataPointer));

            DialogData = s.DoAt(DialogDataPointer, () => s.SerializeObject<GBAIsometric_Spyro_DialogData>(DialogData, name: nameof(DialogData)));
        }
    }
}