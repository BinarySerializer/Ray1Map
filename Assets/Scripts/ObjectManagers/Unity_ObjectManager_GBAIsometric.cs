using System;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBAIsometric : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBAIsometric(Context context, GBAIsometric_ObjectType[] types, AnimSet[] animSets, int waypointsStartIndex = 0) : base(context)
        {
            Types = types;
            AnimSets = animSets;
            WaypointsStartIndex = waypointsStartIndex;
        }
        
        public GBAIsometric_ObjectType[] Types { get; }
        public AnimSet[] AnimSets { get; }
        public int WaypointsStartIndex { get; }

        public override string[] LegacyDESNames => AnimSets.Select(x => x.Name).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class AnimSet
        {
            public AnimSet(Pointer pointer, Animation[] animations, string name)
            {
                Pointer = pointer;
                Animations = animations;
                Name = name;
            }

            public Pointer Pointer { get; }
            public Animation[] Animations { get; }
            public string Name { get; }

            public class Animation
            {
                public Animation(Func<Sprite[]> animFrameFunc, byte animSpeed, int xPos, int yPos)
                {
                    AnimFrameFunc = animFrameFunc;
                    AnimSpeed = animSpeed;
                    XPosition = xPos;
                    YPosition = yPos;
                }

                private Sprite[] Frames;
                private Unity_ObjAnimation Anim;
                protected Func<Sprite[]> AnimFrameFunc { get; }
                public int XPosition { get; }
                public int YPosition { get; }

                public Sprite[] AnimFrames => Frames ?? (Frames = AnimFrameFunc());

                public Unity_ObjAnimation ObjAnimation => Anim ?? (Anim = new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, AnimFrames.Length).Select(x => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                    {
                        new Unity_ObjAnimationPart()
                        {
                            ImageIndex = x,
                            XPosition = XPosition,
                            YPosition = YPosition
                        }
                    })).ToArray()
                });
                public byte AnimSpeed { get; }
            }
        }
    }
}