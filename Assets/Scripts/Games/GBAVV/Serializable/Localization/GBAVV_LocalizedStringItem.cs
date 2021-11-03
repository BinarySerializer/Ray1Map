using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_LocalizedStringItem : BinarySerializable
    {
        public Pointer TextPointer { get; set; }

        public string Text { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TextPointer = s.SerializePointer(TextPointer, name: nameof(TextPointer));

            Text = s.DoAt(TextPointer, () => s.SerializeString(Text, name: nameof(Text)));
        }
    }
}