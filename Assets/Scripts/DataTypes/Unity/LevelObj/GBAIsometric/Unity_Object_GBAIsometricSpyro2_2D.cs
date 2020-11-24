using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAIsometricSpyro2_2D : Unity_Object
    {
        public Unity_Object_GBAIsometricSpyro2_2D(GBAIsometric_Spyro2_Object2D obj, Unity_ObjectManager_GBAIsometricSpyro objManager)
        {
            Object = obj;
            ObjManager = objManager;
        }

        public GBAIsometric_Spyro2_Object2D Object { get; }
        public Unity_ObjectManager_GBAIsometricSpyro ObjManager { get; }

        public override short XPosition { get; set; }
        public override short YPosition { get; set; }

        public override string DebugText => $"Category: {Object.Category}{Environment.NewLine}";

        private Unity_ObjAnimationCollisionPart[] objCollision;
        private short objCollision_x;
        private short objCollision_y;
        public override Unity_ObjAnimationCollisionPart[] ObjCollision {
            get {
                if (objCollision == null || XPosition != objCollision_x || YPosition != objCollision_y) {
                    objCollision = new Unity_ObjAnimationCollisionPart[] {
                        new Unity_ObjAnimationCollisionPart()
                        {
                            XPosition = Object.MinX - XPosition,
                            YPosition = Object.MinY - YPosition,
                            Width = Object.MaxX - Object.MinX,
                            Height = Object.MaxY - Object.MinY,
                            Type = Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox
                        }
                    };
                    objCollision_x = XPosition;
                    objCollision_y = YPosition;
                }
                return objCollision;
            }
        }

        public override int? GetLayer(int index) => -index;

        private int _animSetIndex;
        private byte _animationGroupIndex;

        public int AnimSetIndex
        {
            get => _animSetIndex;
            set
            {
                _animSetIndex = value;
                AnimationGroupIndex = 0;
            }
        }

        public byte AnimationGroupIndex
        {
            get => _animationGroupIndex;
            set
            {
                _animationGroupIndex = value;
                AnimIndex = (byte)((AnimGroup?.AnimCount ?? 1) - 1); // Default to the last animation in group to get the one where the character faces the camera
            }
        }

        public byte AnimIndex { get; set; } // Relative to the group
        public byte? ForceFrame { get; set; }

        public Unity_ObjectManager_GBAIsometricSpyro.AnimSet AnimSet => ForceNoGraphics ? null : ObjManager.AnimSets?.ElementAtOrDefault(AnimSetIndex);
        public GBAIsometric_Spyro_AnimGroup AnimGroup => AnimSet?.AnimSetObj?.AnimGroups?.ElementAtOrDefault(AnimationGroupIndex);
        public Unity_ObjectManager_GBAIsometricSpyro.AnimSet.Animation Anim => AnimSet?.Animations?.ElementAtOrDefault(AnimGroup?.AnimIndex + AnimIndex ?? -1);

        public bool IsEditorObj { get; set; } // True for collision objects, trigger objects etc.
        public bool ForceNoGraphics => AnimSetIndex == -1 || IsEditorObj;

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{Object.ObjType}";
        public override string SecondaryName => null;

        public override bool IsEditor => IsEditorObj;
        public override ObjectType Type => IsEditorObj ? ObjectType.Trigger : ObjectType.Object;

        public override bool CanBeLinked => Object.Category == GBAIsometric_Spyro2_Object2D.ObjCategory.Character;
        public int LinkIndex { get; set; } = -1;
        public override IEnumerable<int> Links
        {
            get
            {
                if (LinkIndex != -1)
                    yield return LinkIndex;
            }
        }

        public override Unity_ObjAnimation CurrentAnimation => Anim?.ObjAnimation;
        public override int AnimSpeed => Anim?.AnimSpeed ?? 0;
        public override int? GetAnimIndex => AnimGroup?.AnimIndex + AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Anim?.AnimFrames;

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

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAIsometricSpyro2_2D obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAIsometricSpyro2_2D Obj { get; }

            public ushort Type
            {
                get => Obj.Object.ObjType;
                set => Obj.Object.ObjType = value;
            }

            public int DES
            {
                get => Obj.ForceNoGraphics ? -1 : Obj.AnimSetIndex;
                set
                {
                    if (!Obj.ForceNoGraphics)
                        Obj.AnimSetIndex = value;
                }
            }

            public int ETA { get; set; }

            public byte Etat
            {
                get => Obj.AnimationGroupIndex;
                set => Obj.AnimationGroupIndex = value;
            }

            public byte SubEtat
            {
                get => Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public int EtatLength => Obj.AnimSet?.AnimSetObj?.AnimGroups?.Length ?? 0;
            public int SubEtatLength => Obj.AnimGroup?.AnimCount ?? 0;

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

        protected class GBAIsometricSpyro_UIState : UIState
        {
            public GBAIsometricSpyro_UIState(string displayName, byte animGroupIndex, byte animIndex) : base(displayName, animIndex) => AnimGroupIndex = animGroupIndex;

            public byte AnimGroupIndex { get; }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GBAIsometricSpyro2_2D)obj).AnimationGroupIndex = AnimGroupIndex;
                ((Unity_Object_GBAIsometricSpyro2_2D)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GBAIsometricSpyro2_2D)obj).AnimIndex && AnimGroupIndex == ((Unity_Object_GBAIsometricSpyro2_2D)obj).AnimationGroupIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = AnimSetIndex;

            List<UIState> uiStates = new List<UIState>();

            byte groupIndex = 0;

            foreach (var group in AnimSet?.AnimSetObj?.AnimGroups ?? new GBAIsometric_Spyro_AnimGroup[0])
            {
                for (byte i = 0; i < group.AnimCount; i++)
                    uiStates.Add(new GBAIsometricSpyro_UIState($"Animation {groupIndex}-{i}", animIndex: i, animGroupIndex: groupIndex));

                groupIndex++;
            }

            UIStates = uiStates.ToArray();
        }
        #endregion
    }
}