using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.KlonoaDTP;
using UnityEngine;

namespace R1Engine
{
    public sealed class Unity_Object_PS1Klonoa : Unity_Object_3D
    {
        public Unity_Object_PS1Klonoa(Unity_ObjectManager_PS1Klonoa objManager, EnemyObject obj, float scale)
        {
            ObjManager = objManager;
            Object = obj;
            FrameSetIndex = objManager.FrameSets.FindItemIndex(x => x.Index == obj.GraphicsIndex - 1);

            if (FrameSetIndex == -1)
                Debug.LogWarning($"Enemy graphics was not set");

            Position = PS1Klonoa_Manager.GetPosition(obj.XPos.Value, obj.YPos.Value, obj.ZPos.Value, scale);
        }

        public Unity_ObjectManager_PS1Klonoa ObjManager { get; }
        public EnemyObject Object { get; set; }

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
        
        public Unity_ObjectManager_PS1Klonoa.FrameSet FrameSet => ObjManager.FrameSets.ElementAtOrDefault(FrameSetIndex);

        public override BinarySerializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_1_{Object.SecondaryType}";

        public override ObjectType Type => ObjectType.Object;

        private int _frameSetIndex;

        public int FrameSetIndex
        {
            get => _frameSetIndex;
            set
            {
                _frameSetIndex = value;
                AnimIndex = 0;
            }
        }

        public byte AnimIndex { get; set; }

        public override Unity_ObjAnimation CurrentAnimation => FrameSet?.Animations.ElementAtOrDefault(AnimIndex);
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => FrameSetIndex;
        public override IList<Sprite> Sprites => FrameSet?.Frames;

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_PS1Klonoa obj)
            {
                Obj = obj;
            }

            private Unity_Object_PS1Klonoa Obj { get; }

            public ushort Type { get; set; }

            public int DES
            {
                get => Obj.FrameSetIndex;
                set => Obj.FrameSetIndex = value;
            }

            public int ETA
            {
                get => Obj.FrameSetIndex;
                set => Obj.FrameSetIndex = value;
            }

            public byte Etat { get; set; }

            public byte SubEtat
            {
                get => Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public int EtatLength => 0;
            public int SubEtatLength => Obj.FrameSet?.Animations?.Length ?? 0;

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
        protected override bool IsUIStateArrayUpToDate => FrameSetIndex == UIStates_AnimSetIndex;

        private class PS1Klonoa_UIState : UIState
        {
            public PS1Klonoa_UIState(string displayName, byte animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_PS1Klonoa)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_PS1Klonoa)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = FrameSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (byte i = 0; i < (FrameSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new PS1Klonoa_UIState($"Sprite {i}", animIndex: i));

            UIStates = uiStates.ToArray();
        }

        #endregion
    }
}