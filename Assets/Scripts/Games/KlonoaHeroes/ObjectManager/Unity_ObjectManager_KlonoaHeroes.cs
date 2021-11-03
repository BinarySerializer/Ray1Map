using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.KH;
using UnityEngine;

namespace Ray1Map.KlonoaHeroes
{
    public class Unity_ObjectManager_KlonoaHeroes : Unity_ObjectManager
    {
        public Unity_ObjectManager_KlonoaHeroes(Context context, IEnumerable<AnimSet> animSets, KlonoaHeroesROM rom, KlonoaHeroes_Manager.LevelEntry levelEntry) : base(context)
        {
            // Set the first set to null for objects without graphics
            AnimSets = new AnimSet[]
            {
                null
            }.Concat(animSets).ToArray();
            ROM = rom;
            LevelEntry = levelEntry;
        }

        public AnimSet[] AnimSets { get; }
        public KlonoaHeroesROM ROM { get; }
        public KlonoaHeroes_Manager.LevelEntry LevelEntry { get; }

        public override string[] LegacyDESNames => AnimSets.Select(animSet => animSet?.GetDisplayName() ?? "NULL").ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class AnimSet
        {
            public AnimSet(IList<Animation> animations, int fileIndex, FilePackType packType, ArchiveFile pack)
            {
                Animations = animations;
                FileIndex = fileIndex;
                PackType = packType;
                Pack = pack;
            }

            public IList<Animation> Animations { get; }
            public int FileIndex { get; }
            public FilePackType PackType { get; }
            public ArchiveFile Pack { get; }
            public string GetDisplayName() => $"0x{Pack.Offset.StringAbsoluteOffset}_{FileIndex}";

            public enum FilePackType
            {
                Enemy,
                Gameplay,
            }

            public class Animation
            {
                public Animation(Func<Sprite[]> animFrameFunc, BinarySerializer.Klonoa.KH.Animation klonoaAnim, int xPos, int yPos, int animGroupIndex, int animIndex)
                {
                    AnimFrameFunc = animFrameFunc;
                    KlonoaAnim = klonoaAnim;
                    XPos = xPos;
                    YPos = yPos;
                    AnimGroupIndex = animGroupIndex;
                    AnimIndex = animIndex;
                }

                private Sprite[] Frames;
                private Unity_ObjAnimation Anim;
                protected Func<Sprite[]> AnimFrameFunc { get; }
                public BinarySerializer.Klonoa.KH.Animation KlonoaAnim { get; }
                public int XPos { get; }
                public int YPos { get; }
                public int AnimGroupIndex { get; }
                public int AnimIndex { get; }

                public Sprite[] AnimFrames => Frames ?? (Frames = AnimFrameFunc());

                public Unity_ObjAnimation ObjAnimation => Anim ?? (Anim = new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, AnimFrames.Length).Select(x => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                    {
                        new Unity_ObjAnimationPart()
                        {
                            ImageIndex = x,
                            XPosition = XPos,
                            YPosition = YPos
                        }
                    })).ToArray(),
                    AnimSpeeds = KlonoaAnim.Frames.Select(x => (int)x.Speed).ToArray()
                });
            }
        }
    }
}