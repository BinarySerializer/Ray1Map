using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBACrash : Unity_Object
    {
        public Unity_Object_GBACrash(Unity_ObjectManager_GBACrash objManager, GBACrash_Object obj)
        {
            ObjManager = objManager;
            Object = obj;

            // Init the object
            GBACrash_ObjInit.InitObj(ObjManager.Context.Settings.EngineVersion, this);
        }

        public Unity_ObjectManager_GBACrash ObjManager { get; }
        public GBACrash_Object Object { get; set; }

        public override short XPosition
        {
            get => Object.XPos;
            set => Object.XPos = value;
        }

        public override short YPosition
        {
            get => Object.YPos;
            set => Object.YPos = value;
        }

        public override string DebugText => $"Params: {Util.ByteArrayToHexString(ObjManager.ObjParams[Object.ObjParamsIndex])}{Environment.NewLine}";

        public Unity_ObjectManager_GBACrash.AnimSet AnimSet => ObjManager.AnimSets.ElementAtOrDefault(AnimSetIndex);
        public Unity_ObjectManager_GBACrash.AnimSet.Animation Animation => AnimSet?.Animations.ElementAtOrDefault(AnimIndex);

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{(int)Object.ObjType}";
        public override string SecondaryName => $"{Object.ObjType}";

        private int _animSetIndex;

        public int AnimSetIndex
        {
            get => _animSetIndex;
            set
            {
                _animSetIndex = value;
                AnimIndex = 0;
            }
        }

        public byte AnimIndex { get; set; }

        public override Unity_ObjAnimationCollisionPart[] ObjCollision => Animation?.AnimHitBox;

        public override Unity_ObjAnimation CurrentAnimation => Animation?.ObjAnimation;
        public override int AnimSpeed => Animation?.CrashAnim.AnimSpeed ?? 0;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Animation?.AnimFrames;

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBACrash obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBACrash Obj { get; }

            public ushort Type
            {
                get => (ushort)Obj.Object.ObjType;
                set => Obj.Object.ObjType = (GBACrash_ObjType)value;
            }

            public int DES
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }

            public int ETA
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }

            public byte Etat { get; set; }

            public byte SubEtat
            {
                get => Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public int EtatLength => 0;
            public int SubEtatLength => Obj.AnimSet?.Animations?.Length ?? 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }

        #region UI States

        protected int UIStates_AnimSetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimSetIndex == UIStates_AnimSetIndex;

        protected class GBACrash_UIState : UIState
        {
            public GBACrash_UIState(string displayName, byte animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GBACrash)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GBACrash)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = AnimSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (byte i = 0; i < (AnimSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new GBACrash_UIState($"Animation {i}", animIndex: i));

            UIStates = uiStates.ToArray();
        }
        
        #endregion
    }
}