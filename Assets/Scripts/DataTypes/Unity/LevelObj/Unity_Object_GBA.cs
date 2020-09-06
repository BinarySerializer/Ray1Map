using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBA : Unity_Object
    {
        public Unity_Object_GBA(GBA_Actor actor, Unity_ObjectManager_GBA objManager)
        {
            // Set properties
            Actor = actor;
            ObjManager = objManager;
        }

        public GBA_Actor Actor { get; }

        public Unity_ObjectManager_GBA ObjManager { get; }

        public GBA_ActorState State => GraphicsData?.States.ElementAtOrDefault(Actor.StateIndex);
        public Unity_ObjectManager_GBA.GraphicsData GraphicsData => ObjManager.GraphicsDatas.ElementAtOrDefault(GraphicsDataIndex);
        
        public int GraphicsDataIndex
        {
            get => ObjManager.GraphicsDatas.FindItemIndex(x => x.Index == Actor.GraphicsDataIndex);
            set => Actor.GraphicsDataIndex = (byte)ObjManager.GraphicsDatas[value].Index;
        }


        public override short XPosition
        {
            get => (short)Actor.XPos;
            set => Actor.XPos = (ushort)value;
        }
        public override short YPosition
        {
            get => (short)Actor.YPos;
            set => Actor.YPos = (ushort)value;
        }

        public override string DebugText => $"{nameof(Actor.Link_0)}: {Actor.Link_0}{Environment.NewLine}" +
                                            $"{nameof(Actor.Link_1)}: {Actor.Link_1}{Environment.NewLine}" +
                                            $"{nameof(Actor.Link_2)}: {Actor.Link_2}{Environment.NewLine}" +
                                            $"{nameof(Actor.Link_3)}: {Actor.Link_3}{Environment.NewLine}" +
                                            $"{nameof(Actor.Byte_04)}: {Actor.Byte_04}{Environment.NewLine}" +
                                            $"{nameof(Actor.ActorID)}: {Actor.ActorID}{Environment.NewLine}" +
                                            $"{nameof(Actor.GraphicsDataIndex)}: {Actor.GraphicsDataIndex}{Environment.NewLine}" +
                                            $"{nameof(Actor.StateIndex)}: {Actor.StateIndex}";

        [Obsolete]
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public bool IsAlwaysEvent { get; set; }
        public override bool IsAlways => IsAlwaysEvent;
        public override string PrimaryName => $"ID_{Actor.ActorID}";
        public override string SecondaryName => ObjManager.Context.Settings.Game == Game.GBA_Rayman3 ? $"{(GBA_R3_ActorID)Actor.ActorID}" : null;

        public override bool FlipHorizontally => State?.Flags.HasFlag(GBA_ActorState.ActorStateFlags.HorizontalFlip) ?? false;
        public override bool FlipVertically => State?.Flags.HasFlag(GBA_ActorState.ActorStateFlags.VerticalFlip) ?? false;

        public override Unity_ObjAnimation CurrentAnimation => GraphicsData?.Graphics.Animations.ElementAtOrDefault(AnimationIndex);
        public override byte AnimSpeed => CurrentAnimation.AnimSpeed.Value;

        public override byte GetAnimIndex => State?.AnimationIndex ?? Actor.StateIndex;
        public override IList<Sprite> Sprites => GraphicsData?.Graphics.Sprites;

        [Obsolete]
        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBA obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBA Obj { get; }

            public ushort Type
            {
                get => Obj.Actor.ActorID;
                set => Obj.Actor.ActorID = (byte)value;
            }

            public int DES
            {
                get => Obj.GraphicsDataIndex;
                set => Obj.GraphicsDataIndex = value;
            }

            public int ETA
            {
                get => Obj.GraphicsDataIndex;
                set => Obj.GraphicsDataIndex = value;
            }

            public byte Etat { get; set; }

            public byte SubEtat
            {
                get => Obj.Actor.StateIndex;
                set => Obj.Actor.StateIndex = value;
            }

            public int EtatLength => 0;
            public int SubEtatLength => Obj.GraphicsData?.States?.Length > 0 ? Obj.GraphicsData?.States?.Length ?? 0 : Obj.GraphicsData?.Graphics.Animations.Count ?? 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }
    }
}