using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAVVNitroKart : Unity_Object
    {
        public Unity_Object_GBAVVNitroKart(Unity_ObjectManager_GBAVV objManager, GBAVV_NitroKart_Object obj)
        {
            ObjManager = objManager;
            Object = obj;

            // Init the object
            InitObj();
        }

        public void InitObj()
        {
            GBAVV_ObjInit.InitObj(ObjManager.Context.Settings, this, Object.ObjType);
        }

        public Unity_ObjectManager_GBAVV ObjManager { get; }
        public GBAVV_NitroKart_Object Object { get; set; }

        public override short XPosition
        {
            get => (short)Object.XPos;
            set => Object.XPos = value;
        }

        public override short YPosition
        {
            get => (short)Object.YPos;
            set => Object.YPos = value;
        }

        public override string DebugText => null;

        public Unity_ObjectManager_GBAVV.AnimSet AnimSet => ObjManager.AnimSets.ElementAtOrDefault(AnimSetIndices.Item1)?.ElementAtOrDefault(AnimSetIndices.Item2);
        public Unity_ObjectManager_GBAVV.AnimSet.Animation Animation
        {
            get
            {
                var anim = AnimSet?.Animations.ElementAtOrDefault(AnimIndex);

                if (anim?.AnimFrames.Length == 0)
                    return null;
                
                return anim;
            }
        }

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{Object.ObjType}";
        public override string SecondaryName => $"{(GBAVV_NitroKart_ObjType)Object.ObjType}";

        public override ObjectType Type => AnimSetIndex == -1 ? (IsWaypoint ? ObjectType.Waypoint : ObjectType.Trigger) : ObjectType.Object;

        public bool IsWaypoint { get; set; }

        public void SetAnimation(int graphics, int animSet, byte anim)
        {
            AnimSetIndices = (graphics, animSet);
            AnimIndex = anim;
        }

        private int _animSetIndex;

        public int AnimSetIndex
        {
            get => _animSetIndex;
            set
            {
                if (AnimSetIndex == -1)
                    return;

                _animSetIndex = value;
                AnimIndex = 0;
            }
        }

        public (int, int) AnimSetIndices
        {
            get => AnimSetIndex == -1 ? (-1, -1) : ObjManager.AnimSetsLookupTable.ElementAtOrDefault(AnimSetIndex);
            set => AnimSetIndex = ObjManager.AnimSets.ElementAtOrDefault(value.Item1)?.ElementAtOrDefault(value.Item2)?.Index ?? -1;
        }

        public byte AnimIndex { get; set; }

        public override Unity_ObjAnimationCollisionPart[] ObjCollision => Animation?.AnimHitBox;

        public override Unity_ObjAnimation CurrentAnimation => Animation?.ObjAnimation;
        public override int AnimSpeed => Animation?.CrashAnim.AnimSpeed + 1 ?? 0;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Animation?.AnimFrames;

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAVVNitroKart obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAVVNitroKart Obj { get; }

            public ushort Type
            {
                get => (ushort)Obj.Object.ObjType;
                set => Obj.Object.ObjType = (short)value;
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

        protected class GBAVVNitroKart_UIState : UIState
        {
            public GBAVVNitroKart_UIState(string displayName, byte animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GBAVVNitroKart)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GBAVVNitroKart)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = AnimSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (byte i = 0; i < (AnimSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new GBAVVNitroKart_UIState($"Animation {i}", animIndex: i));

            UIStates = uiStates.ToArray();
        }
        
        #endregion
    }
}