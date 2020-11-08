using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_Spyro_AnimationBlock : R1Serializable
    {
        public GBAIsometric_Animation[] Animations { get; set; }
        public GBAIsometric_AnimFrame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var animLength = Animations?.Length ?? -1;

            if (animLength == -1)
            {
                var firstAnim = s.DoAt(s.CurrentPointer, () => s.SerializeObject<GBAIsometric_Animation>(default, name: $"{nameof(Animations)}[0]"));
                animLength = firstAnim.StartFrameIndex;
            }

            Animations = s.SerializeObjectArray<GBAIsometric_Animation>(Animations, animLength, name: nameof(Animations));
            Frames = s.SerializeObjectArray<GBAIsometric_AnimFrame>(Frames, Animations.Max(x => x.StartFrameIndex + x.FrameCount) - animLength, name: nameof(Frames));
        }
    }
}