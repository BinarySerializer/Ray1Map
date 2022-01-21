using System;
using System.Linq;
using BinarySerializer;

using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public class Unity_ObjectManager_GBAIsometricSpyro1 : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBAIsometricSpyro1(Context context, SpriteSet[] spriteSets) : base(context)
        {
            SpriteSets = spriteSets;
        }
        
        public SpriteSet[] SpriteSets { get; }

        public override string[] LegacyDESNames => SpriteSets.
            Select((x, i) => $"{i} - 0x{x.SpriteSetObj.Offset.StringAbsoluteOffset}").
            ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class SpriteSet
        {
            public SpriteSet(GBAIsometric_Ice_SpriteSet spriteSetObj, Func<Sprite[]> animFramesFunc)
            {
                SpriteSetObj = spriteSetObj;
                AnimFramesFunc = animFramesFunc;
                Animations = new Animation(
                        animFrameFunc: () => Frames, 
                        positions: spriteSetObj.Sprites.
                            Select(x => new Vector2Int(x.XPos, x.YPos)).
                            ToArray()).Yield().Concat(spriteSetObj.Sprites.
                        Select((x, i) => new Animation(
                            animFrameFunc: () => Frames[i].YieldToArray(), 
                            positions: new Vector2Int(spriteSetObj.Sprites[i].XPos, spriteSetObj.Sprites[i].YPos).YieldToArray()))).ToArray();
            }

            public GBAIsometric_Ice_SpriteSet SpriteSetObj { get; }
            public Func<Sprite[]> AnimFramesFunc { get; }
            public Animation[] Animations { get; } // 0 = all, others are one frame each

            public Sprite[] _frames;
            public Sprite[] Frames => _frames ??= AnimFramesFunc();

            public class Animation
            {
                public Animation(Func<Sprite[]> animFrameFunc, Vector2Int[] positions)
                {
                    AnimFrameFunc = animFrameFunc;
                    Positions = positions;
                }

                private Sprite[] Frames;
                private Unity_ObjAnimation Anim;
                protected Func<Sprite[]> AnimFrameFunc { get; }
                private Vector2Int[] Positions { get; }

                public Sprite[] AnimFrames => Frames ??= AnimFrameFunc();

                public Unity_ObjAnimation ObjAnimation => Anim ??= new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, AnimFrames.Length).Select(x => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                    {
                        new Unity_ObjAnimationPart()
                        {
                            ImageIndex = x,
                            XPosition = Positions[x].x,
                            YPosition = Positions[x].y
                        }
                    })).ToArray()
                };
            }
        }
    }
}