﻿using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class Unity_Object_GBARRR : Unity_SpriteObject
    {
        public Unity_Object_GBARRR(GBARRR_Object obj, Unity_ObjectManager_GBARRR objManager)
        {
            Object = obj;
            ObjManager = objManager;

            // P_FunctionPointer is filled in later, can't do this check here
            /*if (actor.ObjectType == GBARRR_ActorType.Special && Actor.P_GraphicsOffset == 0 && !Enum.IsDefined(typeof(SpecialType_Function), (SpecialType_Function)actor.P_FunctionPointer))
                Debug.LogWarning($"Special type with function pointer 0x{actor.P_FunctionPointer:X8} is not defined at ({Actor.XPosition}, {Actor.YPosition})");*/
        }

        public GBARRR_Object Object { get; }
        public Unity_ObjectManager_GBARRR ObjManager { get; }

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

        // We group animations so we can switch between them easier
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

        public override string DebugText =>
              $"UShort_0C: {Object.Ushort_0C}{Environment.NewLine}" +
              $"P_GraphicsIndex: {Object.P_GraphicsIndex}{Environment.NewLine}" +
              $"P_GraphicsOffset: 0x{Object.P_GraphicsOffset:X8}{Environment.NewLine}" +
              $"P_FunctionPointer: 0x{Object.P_FunctionPointer:X8}{Environment.NewLine}" +
              $"P_PaletteIndex: {Object.P_PaletteIndex}{Environment.NewLine}" +
              $"P_SpriteSize: {Object.P_SpriteSize}{Environment.NewLine}" +
              $"P_FrameCount: {Object.P_FrameCount}{Environment.NewLine}" +
              $"Rotation: {Object.Ushort_0E}{Environment.NewLine}";

        public override BinarySerializable SerializableData => Object;

        public override bool IsEditor => CurrentAnimation == null;

        public override bool FlipHorizontally => BitHelpers.ExtractBits(Object.Data1[3], 1, 4) == 1;

        public override Vector2 Pivot
        {
            get
            {
                var sprite = Sprites?.ElementAtOrDefault(CurrentAnimation?.Frames.ElementAtOrDefault(AnimationFrame)?.SpriteLayers.FirstOrDefault()?.ImageIndex ?? 0);

                // Set the pivot to the center of the sprite
                if (sprite != null) {
                    return new Vector2(sprite.rect.width / 2 - sprite.pivot.x, sprite.rect.height / 2 - sprite.pivot.y);
                } else {
                    return Vector2.zero;
                }
            }
        }

        public override bool CanBeLinkedToGroup => true;
        public override bool CanBeLinked => LinkIndex != null;
        public int? LinkIndex { get; set; }
        public override IEnumerable<int> Links
        {
            get
            {
                if (LinkIndex != null && LinkIndex != -1)
                    yield return LinkIndex.Value;
            }
        }

        public override string PrimaryName => $"Type_{(byte)Object.ObjectType}";
        public override string SecondaryName {
            get {
                if (Object.ObjectType == GBARRR_ObjectType.Special) {
                    if (Enum.IsDefined(typeof(SpecialType_Function), (SpecialType_Function)Object.P_FunctionPointer)) {
                        return ((SpecialType_Function)Object.P_FunctionPointer).ToString();
                    } else if (Enum.IsDefined(typeof(SpecialType_GraphicsIndex), (SpecialType_GraphicsIndex)Object.P_GraphicsIndex)) {
                        return ((SpecialType_GraphicsIndex)Object.P_GraphicsIndex).ToString();
                    } else {
                        return Object.ObjectType.ToString();
                    }
                } else {
                    return Object.ObjectType.ToString();
                }
            }
        }

        public bool IsRayman => Object.ObjectType == GBARRR_ObjectType.Special
            && !Enum.IsDefined(typeof(SpecialType_Function), (SpecialType_Function)Object.P_FunctionPointer)
            && Object.P_GraphicsIndex == 2;

        public Unity_ObjectManager_GBARRR.GraphicsData GraphicsData => IsTriggerType ? null : ObjManager.GraphicsDatas.ElementAtOrDefault(AnimationGroupIndex)?.ElementAtOrDefault(AnimIndex);

        public bool IsTriggerType => Object.ObjectType == GBARRR_ObjectType.DoorTrigger ||
                                     Object.ObjectType == GBARRR_ObjectType.MurfyTrigger ||
                                     Object.ObjectType == GBARRR_ObjectType.SizeTrigger ||
                                     (Object.ObjectType == GBARRR_ObjectType.Special && Object.P_GraphicsOffset == 0);
        public Unity_ObjAnimationCollisionPart.CollisionType GetCollisionType
        {
            get
            {
                switch (Object.ObjectType)
                {
                    case GBARRR_ObjectType.MurfyTrigger:
                        return Unity_ObjAnimationCollisionPart.CollisionType.Gendoor;

                    case GBARRR_ObjectType.SizeTrigger:
                        return Unity_ObjAnimationCollisionPart.CollisionType.SizeChange;

                    case GBARRR_ObjectType.Special:
                        switch ((SpecialType_Function)Object.P_FunctionPointer)
                        {
                            case SpecialType_Function.LevelEndTrigger:
                            case SpecialType_Function.LevelEntranceTrigger:
                            case SpecialType_Function.MinigameTrigger:
                                return Unity_ObjAnimationCollisionPart.CollisionType.ExitLevel;
                        }
                        break;
                }
                return Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox;
            }
        }
        public enum SpecialType_Function
        {
            LevelEndTrigger = 0x08037B9D,
            LevelEntranceTrigger = 0x08037CA1,
            MinigameTrigger = 0x0804DC65
        }
        public enum SpecialType_GraphicsIndex {
            Rayman = 2,
            LevelExit = 3,
            LevelEntrance = 4
        }


        private Unity_ObjAnimationCollisionPart[] objCollision;
        public override Unity_ObjAnimationCollisionPart[] ObjCollision {
            get {
                if (objCollision == null) {
                    objCollision = IsTriggerType ? new Unity_ObjAnimationCollisionPart[] {
                        new Unity_ObjAnimationCollisionPart()
                        {
                            XPosition = 0,
                            YPosition = 0,
                            Width = (int)Object.P_SpriteSize,
                            Height = (int)Object.P_SpriteSize,
                            Type = GetCollisionType
                        }
                    } : new Unity_ObjAnimationCollisionPart[0];
                }
                return objCollision;
            }
        }

        public int? ForceFrame { get; set; }
        protected override bool ShouldUpdateFrame()
        {
            if (ForceFrame != null)
            {
                AnimationFrame = ForceFrame.Value;
                AnimationFrameFloat = ForceFrame.Value;
                return false;
            }

            return true;
        }

        public override Unity_ObjAnimation CurrentAnimation => GraphicsData?.Animation;
        public override int AnimSpeed => GraphicsData?.AnimSpeed ?? 0;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimationGroupIndex;
        public override IList<Sprite> Sprites => GraphicsData?.AnimFrames;
        public override int? GetLayer(int index) => -index;
        public override Unity_ObjectType Type {

            get {
                switch (Object.ObjectType) {
                    case GBARRR_ObjectType.MurfyTrigger:
                    case GBARRR_ObjectType.SizeTrigger:
                        return Unity_ObjectType.Trigger;

                    case GBARRR_ObjectType.Special:
                        switch ((SpecialType_Function)Object.P_FunctionPointer) {
                            case SpecialType_Function.LevelEndTrigger:
                            case SpecialType_Function.LevelEntranceTrigger:
                            case SpecialType_Function.MinigameTrigger:
                                return Unity_ObjectType.Trigger;
                        }
                        break;
                }
                return Unity_ObjectType.Object;
            }
        }

        #region UI States
        protected int UIStates_AnimGroupIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimationGroupIndex == UIStates_AnimGroupIndex;

		protected override void RecalculateUIStates()
        {
            UIStates_AnimGroupIndex = AnimationGroupIndex;

            UIStates = ObjManager?.GraphicsDatas.ElementAtOrDefault(AnimationGroupIndex)?.Select((x, i) => (UIState)new RRR_UIState($"Animation {x.BlockIndex}", i)).ToArray() ?? new UIState[0];
        }

        protected class RRR_UIState : UIState
        {
            public RRR_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                var rrrObj = (Unity_Object_GBARRR)obj;
                rrrObj.AnimIndex = AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj) => AnimIndex == ((Unity_Object_GBARRR)obj).AnimIndex;
        }
		#endregion

		#region LegacyEditorWrapper
		public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);
        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBARRR obj) {
                Obj = obj;
            }

            private Unity_Object_GBARRR Obj { get; }

            public override ushort Type {
                get => (ushort)Obj.Object.ObjectType;
                set => Obj.Object.ObjectType = (GBARRR_ObjectType)(byte)value;
            }

            public override int DES {
                get => Obj.AnimationGroupIndex;
                set => Obj.AnimationGroupIndex = value;
            }

            public override int ETA {
                get => Obj.AnimationGroupIndex;
                set => Obj.AnimationGroupIndex = value;
            }

            public override byte SubEtat {
                get => (byte)Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public override int SubEtatLength => Obj.IsTriggerType ? 0 : Obj.ObjManager?.GraphicsDatas?.ElementAtOrDefault(Obj.AnimationGroupIndex)?.Length ?? 0;
        }
		#endregion
	}
}