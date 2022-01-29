using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public class Unity_Object_GBAIsometricSpyro1_Level3D : Unity_SpriteObject_3D
    {
        public Unity_Object_GBAIsometricSpyro1_Level3D(GBAIsometric_Ice_Level3D_Object obj, Unity_ObjectManager_GBAIsometricSpyro1 objManager)
        {
            Object = obj;
            ObjManager = objManager;

            InitObjType();
        }

        public GBAIsometric_Ice_Level3D_Object Object { get; }
        public Unity_ObjectManager_GBAIsometricSpyro1 ObjManager { get; }

        public override short XPosition
        {
            get => (short)Object.Positions.X.AsFloat;
            set => Object.Positions.X = value;
        }
        public override short YPosition
        {
            get => (short)Object.Positions.Y.AsFloat;
            set => Object.Positions.Y = value;
        }
        public override Vector3 Position {
            get => new Vector3(Object.Positions.X, Object.Positions.Y, Object.Positions.Height);
            set {
                Object.Positions.X = value.x;
                Object.Positions.Y = value.y;
                Object.Positions.Height = value.z;
            }
        }

        private int spriteSetIndex;

        public int SpriteSetIndex
        {
            get => spriteSetIndex;
            set
            {
                spriteSetIndex = value;
                SpriteIndex = 0;
            }
        }

        public int SpriteIndex { get; set; }

        public Unity_ObjectManager_GBAIsometricSpyro1.SpriteSet SpriteSet => ForceNoGraphics 
            ? null 
            : ObjManager.SpriteSets?.ElementAtOrDefault(SpriteSetIndex);
        public Unity_ObjectManager_GBAIsometricSpyro1.SpriteSet.Animation Anim => SpriteSet?.Animations?.ElementAtOrDefault(SpriteIndex);
        public bool ForceNoGraphics => SpriteSetIndex == -1;

        public override BinarySerializable SerializableData => Object;

        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{Object.ObjectType}";
        public override string SecondaryName => null;

        public override bool FlipHorizontally => FlipX;
        public bool FlipX { get; set; }

        public override Unity_ObjAnimation CurrentAnimation => Anim?.ObjAnimation;
        public override int AnimSpeed => 4;
        public override int? GetAnimIndex => SpriteIndex;
        protected override int GetSpriteID => SpriteSetIndex;
        public override IList<Sprite> Sprites => Anim?.AnimFrames;

        public void InitObjType()
        {
            var settings = ObjManager.Context.GetR1Settings();
            var s = GBAIsometric_Ice_Level3D_ObjInit.GetSprite(settings.Level, Object.ObjectType);

            SpriteSetIndex = s.SpriteSet;
            SpriteIndex = s.Sprite + 1; // 0 is animation for all sprites, so start at 1
            FlipX = s.FlipX;
        }

        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAIsometricSpyro1_Level3D obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAIsometricSpyro1_Level3D Obj { get; }

            public override ushort Type
            {
                get => (ushort)Obj.Object.ObjectType;
                set => Obj.Object.ObjectType = (short)value;
            }

            public override int DES
            {
                get => Obj.ForceNoGraphics ? -1 : Obj.SpriteSetIndex;
                set
                {
                    if (!Obj.ForceNoGraphics)
                        Obj.SpriteSetIndex = value;
                }
            }
        }

        #region UI States
        protected int UIStates_AnimSetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => SpriteSetIndex == UIStates_AnimSetIndex;

        protected class GBAIsometricSpyro_UIState : UIState
        {
            public GBAIsometricSpyro_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GBAIsometricSpyro1_Level3D)obj).SpriteIndex = AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GBAIsometricSpyro1_Level3D)obj).SpriteIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = SpriteSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (int i = 0; i < (SpriteSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new GBAIsometricSpyro_UIState(i == 0 ? "All sprites" : $"Sprite {i - 1}", i));

            UIStates = uiStates.ToArray();
        }
        #endregion
    }
}