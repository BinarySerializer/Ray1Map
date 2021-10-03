using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAIsometricRHR : Unity_SpriteObject_3D
    {
        public Unity_Object_GBAIsometricRHR(GBAIsometric_Object obj, Unity_ObjectManager_GBAIsometricRHR objManager, bool isChildObj = false, byte? animIndex = null)
        {
            Object = obj;
            ObjManager = objManager;
            IsChildObj = isChildObj;

            var type = ObjManager.Types?.ElementAtOrDefault(Object.ObjectType);
            AnimSetIndex = type == null ? -1 : ObjManager.AnimSets.FindItemIndex(x => x.Pointer == type.Data?.AnimSetPointer?.PointerValue);
            AnimIndex = animIndex ?? type?.Data?.AnimationIndex ?? 0;
        }

        public GBAIsometric_Object Object { get; }
        public Unity_ObjectManager_GBAIsometricRHR ObjManager { get; }

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
        public override Vector3 Position {
            get => new Vector3(Object.XPosition, Object.YPosition, Object.Height);
            set {
                Object.XPosition = (short)Mathf.RoundToInt(value.x);
                Object.YPosition = (short)Mathf.RoundToInt(value.y);
                Object.Height = (short)Mathf.RoundToInt(value.z);

            }
        }

        public override string DebugText => $"AnimSet: {AnimGroupName}{Environment.NewLine}";

        public string AnimGroupName => ObjType?.Data?.AnimSetPointer?.Value?.Name;
        public GBAIsometric_ObjectType ObjType => ObjManager.Types?.ElementAtOrDefault(Object.ObjectType);

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

        public Unity_ObjectManager_GBAIsometricRHR.AnimSet AnimSet => ObjManager.AnimSets?.ElementAtOrDefault(AnimSetIndex);

        public bool IsWaypoint => ObjManager.Context.GetR1Settings().EngineVersion != EngineVersion.GBAIsometric_RHR && Object.ObjectType == 0;
        public bool IsChildObj { get; }

        public override BinarySerializable SerializableData => Object;
        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => IsWaypoint ? "Waypoint" : $"{AnimGroupName?.Replace("AnimSet", String.Empty) ?? $"Type_{Object.ObjectType}"}";
        public override string SecondaryName => null;

        public override bool IsEditor => IsWaypoint;
        public override bool IsAlways => IsChildObj;
        public override Unity_ObjectType Type => IsWaypoint ? Unity_ObjectType.Waypoint : Unity_ObjectType.Object;

        public override bool CanBeLinked => true;
        public override IEnumerable<int> Links
        {
            get
            {
                // Normal link
                if (Object.LinkIndex != 0xFF)
                    yield return Object.LinkIndex;

                // Waypoint links
                for (int i = 0; i < Object.WaypointCount; i++)
                    yield return ObjManager.WaypointsStartIndex + Object.WaypointIndex + i;
            }
        }

        public Unity_ObjectManager_GBAIsometricRHR.AnimSet.Animation Anim => AnimSet?.Animations?.ElementAtOrDefault(AnimIndex);
        public override Unity_ObjAnimation CurrentAnimation => Anim?.ObjAnimation;
        public override int AnimSpeed => Anim?.AnimSpeed ?? 0;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Anim?.AnimFrames;
        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAIsometricRHR obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAIsometricRHR Obj { get; }

            public override ushort Type
            {
                get => Obj.Object.ObjectType;
                set => Obj.Object.ObjectType = value;
            }

            public override int DES
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }

            public override int ETA
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }

            public override byte SubEtat
            {
                get => Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public override int SubEtatLength => Obj.AnimSet?.Animations?.Length ?? 0;
        }


        #region UI States
        protected int UIStates_AnimSetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimSetIndex == UIStates_AnimSetIndex;

        protected class GBAIsometricRHR_UIState : UIState
        {
            public GBAIsometricRHR_UIState(string displayName, byte animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GBAIsometricRHR)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GBAIsometricRHR)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = AnimSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (byte i = 0; i < (AnimSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new GBAIsometricRHR_UIState($"Animation {i}", animIndex: i));

            UIStates = uiStates.ToArray();
        }
        #endregion
    }
}