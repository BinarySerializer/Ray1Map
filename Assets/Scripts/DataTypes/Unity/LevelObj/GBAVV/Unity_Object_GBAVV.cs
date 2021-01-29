using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAVV : Unity_Object
    {
        public Unity_Object_GBAVV(Unity_ObjectManager_GBAVV objManager, GBAVV_Map2D_Object obj, int objGroupIndex, int objIndex)
        {
            ObjManager = objManager;
            Object = obj;
            ObjGroupIndex = objGroupIndex;
            ObjIndex = objIndex;

            // Init the object
            GBAVV_ObjInit.InitObj(ObjManager.Context.Settings.EngineVersion, ObjManager.Context.Settings.GameModeSelection, this);

            // Set link group
            if (IsLinked)
                EditorLinkGroup = ObjParams?.ElementAtOrDefault(4) ?? 0;
        }

        public int ObjGroupIndex { get; }
        public int ObjIndex { get; }

        public Unity_ObjectManager_GBAVV ObjManager { get; }
        public GBAVV_Map2D_Object Object { get; set; }

        public bool IsLinked { get; set; }

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

        public override string DebugText => $"Params: {Util.ByteArrayToHexString(ObjParams)}{Environment.NewLine}" +
                                            $"Group: {ObjGroupIndex}{Environment.NewLine}" +
                                            $"Index: {ObjIndex}{Environment.NewLine}";

        public byte[] ObjParams => ObjManager.ObjParams?.ElementAtOrDefault(Object.ObjParamsIndex);

        public Unity_ObjectManager_GBAVV.AnimSet AnimSet => ObjManager.AnimSets.ElementAtOrDefault(AnimSetIndex);
        public Unity_ObjectManager_GBAVV.AnimSet.Animation Animation => AnimSet?.Animations.ElementAtOrDefault(AnimIndex);

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{(int)Object.ObjType}";
        public override string SecondaryName
        {
            get
            {
                if (ObjManager.Context.Settings.EngineVersion == EngineVersion.GBAVV_Crash1)
                    return $"{(GBAVV_Map2D_Crash1_ObjType)Object.ObjType}";

                if (ObjManager.Context.Settings.EngineVersion == EngineVersion.GBAVV_Crash2 && ObjManager.MapType == GBAVV_MapInfo.GBAVV_MapType.WorldMap)
                    return $"{(GBAVV_Crash2_WorldMap_ObjType)Object.ObjType}";

                return $"{(GBAVV_Map2D_Crash2_ObjType)Object.ObjType}";
            }
        }

        public override bool FlipHorizontally => (ObjParams?.FirstOrDefault() & 2) != 0;
        public override bool FlipVertically => (ObjParams?.FirstOrDefault() & 4) != 0;

        public override bool CanBeLinkedToGroup => true;

        public override ObjectType Type => AnimSetIndex == -1 ? ObjectType.Trigger : ObjectType.Object;

        private int _animSetIndex;
        private byte _animIndex;

        public int AnimSetIndex
        {
            get => _animSetIndex;
            set
            {
                if (AnimSetIndex == -1)
                    return;

                _animSetIndex = value;
                AnimIndex = 0;
                FreezeFrame = false;
            }
        }

        public byte AnimIndex
        {
            get => _animIndex;
            set
            {
                _animIndex = value;
                FreezeFrame = false;
            }
        }

        public bool FreezeFrame { get; set; }

        public override Unity_ObjAnimationCollisionPart[] ObjCollision => Animation?.AnimHitBox;

        public override Unity_ObjAnimation CurrentAnimation => Animation?.ObjAnimation;
        public override int AnimSpeed => FreezeFrame ? 0 : (Animation?.CrashAnim.AnimSpeed + 1) ?? 0;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Animation?.AnimFrames;

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAVV obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAVV Obj { get; }

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

        protected class GBAVV_UIState : UIState
        {
            public GBAVV_UIState(string displayName, byte animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GBAVV)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GBAVV)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = AnimSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (byte i = 0; i < (AnimSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new GBAVV_UIState($"Animation {i}", animIndex: i));

            UIStates = uiStates.ToArray();
        }
        
        #endregion
    }
}