using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class R1_GBA_ETA : BinarySerializable
    {
        public byte[] Lengths { get; set; }
        public R1_EventState[][] ETA { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (ETA == null)
                ETA = new R1_EventState[Lengths.Length][];

            for (int i = 0; i < ETA.Length; i++)
            {
                s.DoAt(s.SerializePointer(ETA[i]?.FirstOrDefault()?.Offset, name: $"EtatPointers[{i}]"), () =>
                {
                    ETA[i] = s.SerializeObjectArray<R1_EventState>(ETA[i], Lengths[i], name: $"{nameof(ETA)}[{i}]");
                });
            }
        }
    }
}