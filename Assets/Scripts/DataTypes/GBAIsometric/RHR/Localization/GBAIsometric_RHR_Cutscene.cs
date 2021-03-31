using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_RHR_Cutscene : BinarySerializable
    {
        public uint DialogCount { get; set; }
        public Pointer DialogPointer { get; set; }

        public GBAIsometric_RHR_CutsceneDialog[] Dialog { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DialogCount = s.Serialize<uint>(DialogCount, name: nameof(DialogCount));
            DialogPointer = s.SerializePointer(DialogPointer, name: nameof(DialogPointer));

            Dialog = s.DoAt(DialogPointer, () => s.SerializeObjectArray<GBAIsometric_RHR_CutsceneDialog>(Dialog, DialogCount, name: nameof(Dialog)));
        }
    }
}