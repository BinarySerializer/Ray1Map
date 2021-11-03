﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;

using UnityEngine;

namespace Ray1Map.GBAVV
{
    public class Unity_ObjectManager_GBAVV : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBAVV(Context context, AnimSet[][] animSets, GBAVV_Map2D_ObjData objData = null, GBAVV_Generic_MapInfo.GBAVV_Crash_MapType? mapType = null, GBAVV_Script[] scripts = null, GBAVV_Graphics[] graphics = null, GBAVV_DialogScript[] dialogScripts = null, GBAVV_NitroKart_ObjTypeData[] nitroKart_ObjTypeData = null, Dictionary<Pointer, int> locPointerTable = null, bool addDummyAnimSet = false) : base(context)
        {
            if (addDummyAnimSet && animSets?.Length > 0)
            {
                animSets[0] = new AnimSet[]
                {
                    new AnimSet(new AnimSet.Animation[0])
                }.Concat(animSets[0]).ToArray();
            }

            AnimSets = animSets;
            ObjData = objData;
            MapType = mapType;
            Scripts = scripts;
            Graphics = graphics;
            DialogScripts = dialogScripts?.ToDictionary(x => x.ID, x => x.Script);
            NitroKart_ObjTypeData = nitroKart_ObjTypeData;
            LocPointerTable = locPointerTable;
            HasDummyAnimSet = addDummyAnimSet;

            // Set indices if there are multiple array
            var index = 0;
            var lookup = new List<(int, int)>();

            for (var graphicsIndex = 0; graphicsIndex < AnimSets.Length; graphicsIndex++)
            {
                for (int animSetIndex = 0; animSetIndex < AnimSets[graphicsIndex].Length; animSetIndex++)
                {
                    AnimSets[graphicsIndex][animSetIndex].Index = index++;
                    lookup.Add((graphicsIndex, animSetIndex));
                }
            }

            AnimSetsLookupTable = lookup.ToArray();

            if (Context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash2)
                Crash2_IsWorldMap = Context.GetR1Settings().GetGameManagerOfType<GBAVV_Crash2_Manager>().LevInfos[Context.GetR1Settings().Level].IsWorldMap;
        }

        public AnimSet[][] AnimSets { get; }
        public (int, int)[] AnimSetsLookupTable { get; }
        public bool MultipleAnimSetArrays => AnimSets.Length > 1;
        public GBAVV_Map2D_ObjData ObjData { get; }
        public GBAVV_Generic_MapInfo.GBAVV_Crash_MapType? MapType { get; }
        public byte[][] ObjParams => ObjData?.ObjParams;
        public GBAVV_Script[] Scripts { get; }
        public GBAVV_Graphics[] Graphics { get; }
        public GBAVV_NitroKart_ObjTypeData[] NitroKart_ObjTypeData { get; }
        public Dictionary<int, GBAVV_Script> DialogScripts { get; }
        public Dictionary<Pointer, int> LocPointerTable { get; }
        public float? LevelWidthNitroKartNGage { get; set; }
        public bool Crash2_IsWorldMap { get; }
        public bool HasDummyAnimSet { get; }

        public override string[] LegacyDESNames => AnimSets.SelectMany((graphics, graphicsIndex) => graphics.Select((animSet, animSetIndex) =>
        {
            if (HasDummyAnimSet)
                animSetIndex--;

            return MultipleAnimSetArrays
                    ? $"{graphicsIndex}-{animSet.GetDisplayName(animSetIndex)}"
                    : $"{animSet.GetDisplayName(animSetIndex)}";
        })).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class AnimSet
        {
            public AnimSet(Animation[] animations, string nGageFilePath = null)
            {
                Animations = animations;
                NGage_FilePath = nGageFilePath;
            }

            public Animation[] Animations { get; }
            public int Index { get; set; }
            public string NGage_FilePath { get; }
            public string GetDisplayName(int index) => NGage_FilePath != null ? Path.GetFileNameWithoutExtension(NGage_FilePath) : $"{index}";

            public class Animation
            {
                public Animation(Func<Sprite[]> animFrameFunc, GBAVV_Animation crashAnim, int xPos, int yPos)
                {
                    AnimFrameFunc = animFrameFunc;
                    CrashAnim = crashAnim;
                    XPos = xPos;
                    YPos = yPos;

                    AnimHitBox = CrashAnim.HitBox == null ? new Unity_ObjAnimationCollisionPart[0] : new Unity_ObjAnimationCollisionPart[]
                    {
                        new Unity_ObjAnimationCollisionPart()
                        {
                            Type = Unity_ObjAnimationCollisionPart.CollisionType.HitTriggerBox,
                            XPosition = CrashAnim.HitBox.X,
                            YPosition = CrashAnim.HitBox.Y,
                            Width = CrashAnim.HitBox.Width + 1,
                            Height = CrashAnim.HitBox.Height + 1,
                        },
                    };
                }

                private Sprite[] Frames;
                private Unity_ObjAnimation Anim;
                protected Func<Sprite[]> AnimFrameFunc { get; }
                public GBAVV_Animation CrashAnim { get; }
                public int XPos { get; }
                public int YPos { get; }

                public Sprite[] AnimFrames => Frames ?? (Frames = AnimFrameFunc());

                public Unity_ObjAnimation ObjAnimation => Anim ?? (Anim = new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, AnimFrames.Length).Select(x =>
                    {
                        Unity_ObjAnimationCollisionPart[] c = null;

                        if (CrashAnim.AnimSet != null)
                        {
                            var cc = new List<Unity_ObjAnimationCollisionPart>();

                            void addHitBox(GBAVV_AnimationRect box, Unity_ObjAnimationCollisionPart.CollisionType type)
                            {
                                if (box == null)
                                    return;

                                cc.Add(new Unity_ObjAnimationCollisionPart()
                                {
                                    Type = type,
                                    XPosition = box.X,
                                    YPosition = box.Y,
                                    Width = box.Width + 1, // TODO: Should the +1 be here? It appears to be needed for the render frame to match.
                                    Height = box.Height + 1,
                                });
                            }

                            var frame = CrashAnim.AnimSet.AnimationFrames[CrashAnim.FrameIndexTable[x]];

                            // TODO: Fix collision types
                            addHitBox(frame.Fusion_HitBox1, Unity_ObjAnimationCollisionPart.CollisionType.HitTriggerBox);
                            addHitBox(frame.Fusion_HitBox2, Unity_ObjAnimationCollisionPart.CollisionType.VulnerabilityBox);
                            addHitBox(frame.Fusion_HitBox3, Unity_ObjAnimationCollisionPart.CollisionType.AttackBox);

                            c = cc.ToArray();
                        }

                        return new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                        {
                            new Unity_ObjAnimationPart()
                            {
                                ImageIndex = x,
                                XPosition = XPos,
                                YPosition = YPos
                            }
                        }, c);
                    }).ToArray()
                });

                public Unity_ObjAnimationCollisionPart[] AnimHitBox { get; } 
            }
        }
    }
}