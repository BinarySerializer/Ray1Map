using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Klonoa.DTP;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public abstract class Unity_Object_BasePSKlonoa_DTP : Unity_SpriteObject_3D
    {
        protected Unity_Object_BasePSKlonoa_DTP(Unity_ObjectManager_PSKlonoa_DTP objManager)
        {
            ObjManager = objManager;
        }

        public Unity_ObjectManager_PSKlonoa_DTP ObjManager { get; }

        public override short XPosition
        {
            get => (short)Position.x;
            set => Position = new Vector3(value, Position.y, Position.z);
        }

        public override short YPosition
        {
            get => (short)Position.y;
            set => Position = new Vector3(Position.x, value, Position.z);
        }

        public override Vector3 Position { get; set; }
        
        public Unity_ObjectManager_PSKlonoa_DTP.SpriteSet SpriteSet => ObjManager.SpriteSets.ElementAtOrDefault(SpriteSetIndex);

        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{(int)PrimaryType}_{SecondaryType}";

        public override Unity_ObjectType Type => Unity_ObjectType.Object;

        public abstract PrimaryObjectType PrimaryType { get; }
        public abstract int SecondaryType { get; }

        private int spriteSetIndex;

        public int SpriteSetIndex
        {
            get => spriteSetIndex;
            set
            {
                spriteSetIndex = value;
                AnimIndex = 0;
            }
        }

        public int AnimIndex { get; set; }

        public override Unity_ObjAnimation CurrentAnimation => SpriteSet?.Animations.ElementAtOrDefault(AnimIndex)?.ObjAnimation;
        public override int AnimSpeed => SpriteSet?.Animations.ElementAtOrDefault(AnimIndex)?.AnimSpeeds?.ElementAtOrDefault(AnimationFrame) ?? 0;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => SpriteSetIndex;
        public override IList<Sprite> Sprites => SpriteSet?.Animations.ElementAtOrDefault(AnimIndex)?.AnimFrames.Frames;

        protected int GetSpriteSetIndex(Unity_ObjectManager_PSKlonoa_DTP.SpritesType type, int index = 0)
        {
            return ObjManager.SpriteSets.FindItemIndex(x => x.Type == type && x.Index == index);
        }

        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_BasePSKlonoa_DTP obj)
            {
                Obj = obj;
            }

            private Unity_Object_BasePSKlonoa_DTP Obj { get; }

            public override int DES
            {
                get => Obj.SpriteSetIndex;
                set => Obj.SpriteSetIndex = value;
            }

            public override int ETA
            {
                get => Obj.SpriteSetIndex;
                set => Obj.SpriteSetIndex = value;
            }

            public override byte SubEtat
            {
                get => (byte)Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public override int SubEtatLength => Obj.SpriteSet?.Animations?.Length ?? 0;
        }

        #region UI States

        private int UIStates_AnimSetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => SpriteSetIndex == UIStates_AnimSetIndex;

        private class PS1Klonoa_UIState : UIState
        {
            public PS1Klonoa_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_BasePSKlonoa_DTP)obj).AnimIndex = AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_BasePSKlonoa_DTP)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = SpriteSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (int i = 0; i < (SpriteSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new PS1Klonoa_UIState(SpriteSet!.HasAnimations ? $"Animation {i}" : $"Sprite {i}", animIndex: i));

            UIStates = uiStates.ToArray();
        }

        #endregion
    }
}