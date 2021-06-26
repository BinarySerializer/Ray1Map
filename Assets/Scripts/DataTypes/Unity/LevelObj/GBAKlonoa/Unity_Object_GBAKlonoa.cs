using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAKlonoa : Unity_Object
    {
        public Unity_Object_GBAKlonoa(Unity_ObjectManager_GBAKlonoa objManager, GBAKlonoa_Object obj, int sectorIndex)
        {
            ObjManager = objManager;
            Object = obj;
            SectorIndex = sectorIndex;
        }

        public Unity_ObjectManager_GBAKlonoa ObjManager { get; }
        public GBAKlonoa_Object Object { get; set; }
        public GBAKlonoa_Object.SectorState SectorState => Object.SectorStates[SectorIndex];
        public int SectorIndex { get; }


        public override short XPosition
        {
            get => SectorState.XPos;
            set => SectorState.XPos = value;
        }

        public override short YPosition
        {
            get => SectorState.YPos;
            set => SectorState.YPos = value;
        }

        public override string DebugText => String.Empty;

        public Unity_ObjectManager_GBAKlonoa.AnimSet AnimSet => ObjManager.AnimSets.ElementAtOrDefault(AnimSetIndex);
        public Unity_ObjectManager_GBAKlonoa.AnimSet.Animation Animation => AnimSet?.Animations.ElementAtOrDefault(AnimIndex);

        public override BinarySerializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{Object.ObjType}";
        public override string SecondaryName => null;

        public override ObjectType Type => ObjectType.Object;

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

        public override Unity_ObjAnimation CurrentAnimation => Animation?.ObjAnimation;
        public override int AnimSpeed => 2;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Animation?.AnimFrames;

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAKlonoa obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAKlonoa Obj { get; }

            public ushort Type
            {
                get => Obj.Object.ObjType;
                set => Obj.Object.ObjType = (byte)value;
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

        protected class GBAKlonoa_UIState : UIState
        {
            public GBAKlonoa_UIState(string displayName, byte animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GBAKlonoa)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GBAKlonoa)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = AnimSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (byte i = 0; i < (AnimSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new GBAKlonoa_UIState($"Animation {i}", animIndex: i));

            UIStates = uiStates.ToArray();
        }
        
        #endregion
    }
}