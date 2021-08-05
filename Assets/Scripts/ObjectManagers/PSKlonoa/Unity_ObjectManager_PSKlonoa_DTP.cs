using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_PSKlonoa_DTP : Unity_ObjectManager
    {
        public Unity_ObjectManager_PSKlonoa_DTP(Context context, SpriteSet[] spriteSets) : base(context)
        {
            SpriteSets = spriteSets;
        }

        public SpriteSet[] SpriteSets { get; }

        public class SpriteSet
        {
            public SpriteSet(Sprite[] sprites, int index)
            {
                Sprites = sprites;
                Index = index;
                Animations = Sprites.Select((x, i) => new Unity_ObjAnimation
                {
                    Frames = new Unity_ObjAnimationFrame[]
                    {
                        new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                        {
                            new Unity_ObjAnimationPart()
                            {
                                ImageIndex = i,
                                YPosition = (int)(-x.rect.height),
                                XPosition = (int)(-x.rect.width / 2),
                            }
                        })
                    }
                }).ToArray();
            }

            public Sprite[] Sprites { get; }
            public Unity_ObjAnimation[] Animations { get; }
            public int Index { get; }
            public string DisplayName => $"{Index}";
        }
    }
}