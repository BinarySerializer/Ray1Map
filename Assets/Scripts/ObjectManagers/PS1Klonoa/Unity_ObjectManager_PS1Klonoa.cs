using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_PS1Klonoa : Unity_ObjectManager
    {
        public Unity_ObjectManager_PS1Klonoa(Context context, FrameSet[] frameSets) : base(context)
        {
            FrameSets = frameSets;
        }

        public FrameSet[] FrameSets { get; }

        public class FrameSet
        {
            public FrameSet(Sprite[] frames, int index)
            {
                Frames = frames;
                Index = index;
                Animations = Frames.Select((x, i) => new Unity_ObjAnimation
                {
                    Frames = new Unity_ObjAnimationFrame[]
                    {
                        new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                        {
                            new Unity_ObjAnimationPart()
                            {
                                ImageIndex = i,
                                YPosition = (int)(-x.rect.height)
                            }
                        })
                    }
                }).ToArray();
            }

            public Sprite[] Frames { get; }
            public Unity_ObjAnimation[] Animations { get; }
            public int Index { get; }
            public string DisplayName => $"{Index}";
        }
    }
}