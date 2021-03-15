using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBARRRMode7Unused : Unity_Object_3D
    {
        public Unity_Object_GBARRRMode7Unused(GBARRR_Object obj, Unity_ObjectManager_GBARRRMode7Unused objManager)
        {
            Object = obj;
            ObjManager = objManager;

            AnimationGroupIndex = 1; // Default to the pencil
        }

        public GBARRR_Object Object { get; }
        public Unity_ObjectManager_GBARRRMode7Unused ObjManager { get; }

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

        public float Height { get; set; }
        public override float Scale => IsRayman ? 0.25f : 0.5f;
        public bool IsRayman => AnimationGroupIndex == 0;

        public override Vector3 Position
        {
            get => new Vector3(Object.XPosition, Object.YPosition, Height);
            set
            {
                Object.XPosition = (short)value.x;
                Object.YPosition = (short)value.y;
                Height = value.z;
            }
        }

        private int _AnimationGroupIndex { get; set; }
        public int AnimationGroupIndex {
            get => _AnimationGroupIndex;
            set {
                if (value != _AnimationGroupIndex) {
                    _AnimationGroupIndex = value;
                    AnimIndex = 0;
                }
            }
        }
        public int AnimIndex { get; set; }

        public override string DebugText => String.Empty;

        public override R1Serializable SerializableData => Object;

        public override string PrimaryName => $"Type_{(byte)Object.ObjectType}";
        public override string SecondaryName => null;

        public Unity_ObjectManager_GBARRRMode7Unused.GraphicsData GraphicsData => ObjManager.GraphicsDatas.ElementAtOrDefault(AnimationGroupIndex)?.Sprites.ElementAtOrDefault(AnimIndex);

        public override Unity_ObjAnimation CurrentAnimation => GraphicsData?.Animation;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimationGroupIndex;
        public override IList<Sprite> Sprites => GraphicsData?.AnimFrames;

        #region UI States
        protected int UIStates_AnimGroupIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimationGroupIndex == UIStates_AnimGroupIndex;

		protected override void RecalculateUIStates()
        {
            UIStates_AnimGroupIndex = AnimationGroupIndex;

            UIStates = ObjManager?.GraphicsDatas.ElementAtOrDefault(AnimationGroupIndex)?.Sprites.Select((x, i) => (UIState)new RRR_UIState($"Sprite {i}", i)).ToArray() ?? new UIState[0];
        }

        protected class RRR_UIState : UIState
        {
            public RRR_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                var rrrObj = (Unity_Object_GBARRRMode7Unused)obj;
                rrrObj.AnimIndex = AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj) => AnimIndex == ((Unity_Object_GBARRRMode7Unused)obj).AnimIndex;
        }
		#endregion

		#region LegacyEditorWrapper
		public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);
        private class LegacyEditorWrapper : ILegacyEditorWrapper {
            public LegacyEditorWrapper(Unity_Object_GBARRRMode7Unused obj) {
                Obj = obj;
            }

            private Unity_Object_GBARRRMode7Unused Obj { get; }

            public ushort Type {
                get => (ushort)Obj.Object.ObjectType;
                set => Obj.Object.ObjectType = (GBARRR_ObjectType)(byte)value;
            }

            public int DES {
                get => Obj.AnimationGroupIndex;
                set => Obj.AnimationGroupIndex = value;
            }

            public int ETA {
                get => Obj.AnimationGroupIndex;
                set => Obj.AnimationGroupIndex = value;
            }

            public byte Etat { get; set; }

            public byte SubEtat {
                get => (byte)Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public int EtatLength => 0;
            public int SubEtatLength => Obj.ObjManager?.GraphicsDatas?.ElementAtOrDefault(Obj.AnimationGroupIndex)?.Sprites.Length ?? 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }
		#endregion
	}
}