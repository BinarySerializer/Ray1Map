using System.Collections.Generic;
using System.Linq;
using BinarySerializer.KlonoaDTP;
using UnityEngine;

namespace R1Engine
{
    public abstract class Unity_Object_BasePS1Klonoa : Unity_Object_3D
    {
        protected Unity_Object_BasePS1Klonoa(Unity_ObjectManager_PS1Klonoa objManager)
        {
            ObjManager = objManager;
        }

        public Unity_ObjectManager_PS1Klonoa ObjManager { get; }

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
        
        public Unity_ObjectManager_PS1Klonoa.SpriteSet SpriteSet => ObjManager.SpriteSets.ElementAtOrDefault(SpriteSetIndex);

        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{(int)PrimaryType}_{SecondaryType}";

        public override ObjectType Type => ObjectType.Object;

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

        public override Unity_ObjAnimation CurrentAnimation => SpriteSet?.Animations.ElementAtOrDefault(AnimIndex);
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => SpriteSetIndex;
        public override IList<Sprite> Sprites => SpriteSet?.Sprites;

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_BasePS1Klonoa obj)
            {
                Obj = obj;
            }

            private Unity_Object_BasePS1Klonoa Obj { get; }

            public ushort Type { get; set; }

            public int DES
            {
                get => Obj.SpriteSetIndex;
                set => Obj.SpriteSetIndex = value;
            }

            public int ETA
            {
                get => Obj.SpriteSetIndex;
                set => Obj.SpriteSetIndex = value;
            }

            public byte Etat { get; set; }

            public byte SubEtat
            {
                get => (byte)Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public int EtatLength => 0;
            public int SubEtatLength => Obj.SpriteSet?.Animations?.Length ?? 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }

        #region UI States

        private int UIStates_AnimSetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => SpriteSetIndex == UIStates_AnimSetIndex;

        private class PS1Klonoa_UIState : UIState
        {
            public PS1Klonoa_UIState(string displayName, byte animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_BasePS1Klonoa)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_BasePS1Klonoa)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = SpriteSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (byte i = 0; i < (SpriteSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new PS1Klonoa_UIState($"Sprite {i}", animIndex: i));

            UIStates = uiStates.ToArray();
        }

        #endregion
    }
}