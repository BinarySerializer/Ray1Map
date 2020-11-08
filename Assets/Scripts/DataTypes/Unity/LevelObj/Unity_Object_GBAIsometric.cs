using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAIsometric : Unity_Object_3D
    {
        public Unity_Object_GBAIsometric(GBAIsometric_Object obj, Unity_ObjectManager_GBAIsometric objManager)
        {
            Object = obj;
            ObjManager = objManager;

            var type = ObjManager.Types?.ElementAtOrDefault(Object.ObjectType);
            AnimSetIndex = type == null ? -1 : ObjManager.AnimSets.FindItemIndex(x => x.Pointer == type.DataPointer?.Value?.AnimSetPointer?.pointer);
            AnimationIndex = 0; // TODO: Set to correct value
        }

        public GBAIsometric_Object Object { get; }
        public Unity_ObjectManager_GBAIsometric ObjManager { get; }

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

        public string AnimGroupName => ObjType?.DataPointer?.Value?.AnimSetPointer?.Value?.Name;
        public GBAIsometric_ObjectType ObjType => ObjManager.Types?.ElementAtOrDefault(Object.ObjectType);

        private int _animSetIndex;
        public int AnimSetIndex
        {
            get => _animSetIndex;
            set
            {
                _animSetIndex = value;
                AnimationIndex = 0;
            }
        }

        public byte AnimIndex { get; set; }

        public Unity_ObjectManager_GBAIsometric.AnimSet AnimSet => ObjManager.AnimSets?.ElementAtOrDefault(AnimSetIndex);

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"{AnimGroupName?.Replace("AnimSet", String.Empty) ?? $"Type_{Object.ObjectType}"}";
        public override string SecondaryName => null;

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

        public Unity_ObjectManager_GBAIsometric.AnimSet.Animation Anim => AnimSet?.Animations?.ElementAtOrDefault(AnimIndex);
        public override Unity_ObjAnimation CurrentAnimation => Anim?.ObjAnimation;
        public override int AnimSpeed => Anim?.AnimSpeed ?? 0;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Anim?.AnimFrames;
        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAIsometric obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAIsometric Obj { get; }

            public ushort Type
            {
                get => Obj.Object.ObjectType;
                set => Obj.Object.ObjectType = value;
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

        protected class GBAIsometric_UIState : UIState
        {
            public GBAIsometric_UIState(string displayName, byte animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GBAIsometric)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GBAIsometric)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = AnimSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (byte i = 0; i < (AnimSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new GBAIsometric_UIState($"Animation {i}", animIndex: i));

            UIStates = uiStates.ToArray();
        }
        #endregion
    }
}