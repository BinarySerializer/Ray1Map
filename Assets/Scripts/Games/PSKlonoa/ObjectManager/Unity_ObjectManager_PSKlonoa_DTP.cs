using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.PSKlonoa
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
            public SpriteSet(Sprite[] sprites, SpritesType type, int index)
            {
                Sprites = sprites;
                Type = type;
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
            public SpritesType Type { get; }
            public Unity_ObjAnimation[] Animations { get; }
            public int Index { get; }
            public string DisplayName => $"{Index}";
        }

        public enum SpritesType
        {
            CommonSprites,
            Cutscene,
            Player,
            Boss,
        }
    }
}