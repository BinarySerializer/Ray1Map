namespace R1Engine
{
    public class GBAVV_NitroKart_TileAnimations : R1Serializable
    {
        public Pointer[] AnimationPointers { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_TileAnimation[] Animations { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimationPointers = s.SerializePointerArrayUntil(AnimationPointers, x => x == null, includeLastObj: false, name: nameof(AnimationPointers));

            if (Animations == null)
                Animations = new GBAVV_NitroKart_TileAnimation[AnimationPointers.Length];

            for (int i = 0; i < Animations.Length; i++)
                Animations[i] = s.DoAt(AnimationPointers[i], () => s.SerializeObject<GBAVV_NitroKart_TileAnimation>(Animations[i], name: $"{nameof(Animations)}[{i}]"));
        }
    }
}