using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public class Unity_ObjectManager_PSKlonoa_DTP : Unity_ObjectManager
    {
        public Unity_ObjectManager_PSKlonoa_DTP(Context context, SpriteSet[] spriteSets, CutsceneScript[] cutsceneScripts) : base(context)
        {
            SpriteSets = spriteSets;
            CutsceneScripts = cutsceneScripts;
        }

        public SpriteSet[] SpriteSets { get; }
        public CutsceneScript[] CutsceneScripts { get; }

        public override string[] LegacyDESNames => SpriteSets.Select(x => x.DisplayName).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class SpriteSet
        {
            public SpriteSet(IEnumerable<Func<(Sprite Sprite, Vector2Int Pos)>> sprites, SpritesType type, int index = 0)
            {
                Animations = sprites.Select(x => new Animation(() =>
                {
                    var sprite = x();

                    return (
                        Frames: sprite.Sprite.YieldToArray(), 
                        Positions: sprite.Pos.YieldToArray());
                })).ToArray();
                Type = type;
                Index = index;
            }
            public SpriteSet(Animation[] animations, SpritesType type, int index = 0)
            {
                Animations = animations;
                Type = type;
                Index = index;
            }

            public Animation[] Animations { get; }
            public SpritesType Type { get; }
            public int Index { get; }
            public string DisplayName => HasMultipleSets ? $"{Type} {Index}" : $"{Type}";
            public bool HasAnimations => Type == SpritesType.Cutscene;
            public bool HasMultipleSets => Type == SpritesType.Common || Type == SpritesType.Additional;

            public class Animation
            {
                public Animation(Func<(Sprite[] Frames, Vector2Int[] Positions)> animFrameFunc, int[] animSpeeds = null)
                {
                    AnimFrameFunc = animFrameFunc;
                    AnimSpeeds = animSpeeds;
                }

                private (Sprite[] Frames, Vector2Int[] Positions)? Frames;
                private Unity_ObjAnimation Anim;
                protected Func<(Sprite[] frames, Vector2Int[] positions)> AnimFrameFunc { get; }
                public int[] AnimSpeeds { get; }

                public (Sprite[] Frames, Vector2Int[] Positions) AnimFrames => Frames ??= AnimFrameFunc();

                public Unity_ObjAnimation ObjAnimation => AnimFrames.Frames == null ? null : (Anim ??= new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, AnimFrames.Frames.Length).Select(x => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                    {
                        new Unity_ObjAnimationPart()
                        {
                            ImageIndex = x,
                            XPosition = AnimFrames.Positions[x].x,
                            YPosition = AnimFrames.Positions[x].y
                        }
                    })).ToArray(),
                    AnimSpeeds = AnimSpeeds
                });
            }
        }

        public class CutsceneScript
        {
            public CutsceneScript(string displayName, int cutsceneIndex, bool isNormalCutscene, string formattedScript)
            {
                DisplayName = displayName;
                CutsceneIndex = cutsceneIndex;
                IsNormalCutscene = isNormalCutscene;
                FormattedScript = formattedScript;
            }

            public string DisplayName { get; }
            public int CutsceneIndex { get; }
            public bool IsNormalCutscene { get; } // False = skip cutscene
            public string FormattedScript { get; }
        }

        public enum SpritesType
        {
            Common,
            Cutscene,
            Player,
            Additional,
        }
    }
}