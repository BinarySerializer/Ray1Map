using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GameloftRRR : Unity_Object
    {
        public Unity_Object_GameloftRRR(Unity_ObjectManager_GameloftRRR objManager, Gameloft_RRR_Objects.Object obj)
        {
            ObjManager = objManager;
            Object = obj;
        }

        public Unity_ObjectManager_GameloftRRR ObjManager { get; }
        public Gameloft_RRR_Objects.Object Object { get; set; }

        public override short XPosition
        {
            get => Object.XPosition;
            set => Object.XPosition = value;
        }

        public override short YPosition
        {
            get => Object.YPosition;
            set => Object.YPosition = value;
        }

        public override string DebugText =>
            $"AnimIndex: {Object.AnimationIndex}{Environment.NewLine}" +
            $"ObjectID: {Object.ObjectID}{Environment.NewLine}" +
            $"Flags: {Object.Flags}{Environment.NewLine}" +
            $"Params: {string.Join(", ",Object.Shorts.Select(s => $"{s}"))}";


        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{Object.Type}";
        public override string SecondaryName => PuppetData?.Name;
        public Unity_ObjectManager_GameloftRRR.PuppetData PuppetData => ObjManager.Puppets.ElementAtOrDefault(Object.Type);

        public override bool FlipHorizontally => (Object.Flags & 1) == 1;
        public override bool FlipVertically => false;

        public override bool CanBeLinkedToGroup => true;

        public override Unity_ObjAnimation CurrentAnimation => PuppetData?.Puppet?.Animations?.ElementAtOrDefault(AnimationIndex ?? -1);
        public override int AnimSpeed => CurrentAnimation?.AnimSpeed ?? 0;
        public override int? GetAnimIndex => OverrideAnimIndex ?? Object.AnimationIndex;
        protected override int GetSpriteID => Object.Type;
        public override IList<Sprite> Sprites => PuppetData?.Puppet?.Sprites;


        private class LegacyEditorWrapper : ILegacyEditorWrapper {
            public LegacyEditorWrapper(Unity_Object_GameloftRRR obj) {
                Obj = obj;
            }

            private Unity_Object_GameloftRRR Obj { get; }

            public ushort Type {
                get => (ushort)Obj.Object.Type;
                set => Obj.Object.Type = (short)value;
            }

            public int DES {
                get => Obj.Object.Type;
                set => Obj.Object.Type = (short)value;
            }

            public int ETA {
                get => Obj.Object.Type;
                set => Obj.Object.Type = (short)value;
            }

            public byte Etat { get; set; }

            public byte SubEtat {
                get => (byte)Obj.Object.AnimationIndex;
                set => Obj.Object.AnimationIndex = value;
            }

            public int EtatLength => 0;
            public int SubEtatLength => Obj.PuppetData?.Puppet?.Animations?.Count ?? 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }
        #region UI States

        protected int UIStates_PuppetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => Object.Type == UIStates_PuppetIndex;

        protected class GameloftRRR_UIState : UIState
        {
            public GameloftRRR_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GameloftRRR)obj).Object.AnimationIndex = (short)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GameloftRRR)obj).Object.AnimationIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_PuppetIndex = Object.Type;

            List<UIState> uiStates = new List<UIState>();
            int count = (PuppetData?.Puppet?.Animations?.Count ?? 0);

            for (int i = 0; i < count; i++)
                uiStates.Add(new GameloftRRR_UIState($"Animation {i}", animIndex: i));

            UIStates = uiStates.ToArray();
        }
        
        #endregion
    }
}