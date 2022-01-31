using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class Sparx_AnimSet : BinarySerializable
    {
        public int AnimationsCount { get; set; }
        public Pointer AnimationsPointer { get; set; }

        public Sparx_Animation[] Animations { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimationsCount = s.Serialize<int>(AnimationsCount, name: nameof(AnimationsCount));
            AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));

            s.DoAt(AnimationsPointer, () => Animations = s.SerializeObjectArray<Sparx_Animation>(Animations, AnimationsCount, name: nameof(Animations)));
        }
    }
}