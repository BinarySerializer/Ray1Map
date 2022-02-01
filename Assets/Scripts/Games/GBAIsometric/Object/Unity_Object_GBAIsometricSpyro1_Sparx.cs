using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public class Unity_Object_GBAIsometricSpyro1_Sparx : Unity_SpriteObject_3D
    {
        public Unity_Object_GBAIsometricSpyro1_Sparx(int objType, Unity_ObjectManager_GBAIsometricSpyro1_Sparx objManager)
        {
            ObjType = objType;
            ObjManager = objManager;
            AnimSetIndex = objType;
            AnimIndex = ObjManager.ObjectTypes[ObjType].AnimIndex;
        }

        public int ObjType { get; set; }
        public Unity_ObjectManager_GBAIsometricSpyro1_Sparx ObjManager { get; }

        public override short XPosition { get; set; }
        public override short YPosition { get; set; }

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

        public int AnimIndex { get; set; }

        public Unity_ObjectManager_GBAIsometricSpyro1_Sparx.AnimSet AnimSet => ObjManager.AnimSets?.ElementAtOrDefault(AnimSetIndex);
        public Unity_ObjectManager_GBAIsometricSpyro1_Sparx.AnimSet.Animation Anim => AnimSet?.Animations?.ElementAtOrDefault(AnimIndex);

        public override BinarySerializable SerializableData => ObjManager.ObjectTypes[ObjType];

        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{ObjType}";
        public override string SecondaryName => null;

        public override Unity_ObjAnimation CurrentAnimation => Anim?.ObjAnimation;
        public override int AnimSpeed => 4;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Anim?.Frames;

        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAIsometricSpyro1_Sparx obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAIsometricSpyro1_Sparx Obj { get; }

            public override ushort Type
            {
                get => (ushort)Obj.ObjType;
                set => Obj.ObjType = value;
            }

            public override int DES
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }
        }

        #region UI States
        protected int UIStates_AnimSetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimSetIndex == UIStates_AnimSetIndex;

        protected class GBAIsometricSpyro_UIState : UIState
        {
            public GBAIsometricSpyro_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GBAIsometricSpyro1_Sparx)obj).AnimIndex = AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GBAIsometricSpyro1_Sparx)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = AnimSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (int i = 0; i < (AnimSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new GBAIsometricSpyro_UIState($"{i}", i));

            UIStates = uiStates.ToArray();
        }
        #endregion
    }
}