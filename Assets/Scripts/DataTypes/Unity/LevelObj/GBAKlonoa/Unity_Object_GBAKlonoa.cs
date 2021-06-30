using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAKlonoa : Unity_Object_3D
    {
        public Unity_Object_GBAKlonoa(Unity_ObjectManager_GBAKlonoa objManager, GBAKlonoa_LoadedObject obj, BinarySerializable serializable, GBAKlonoa_ObjectOAMCollection oamCollection)
        {
            ObjManager = objManager;
            Object = obj;
            Serializable = serializable;
            OAMCollection = oamCollection;

            AnimSetIndex = objManager.AnimSets.FindItemIndex(x => x.OAMCollections.Any(o => o.OAMs[0].TileIndex == oamCollection.OAMs[0].TileIndex));

            if (AnimSetIndex == -1)
            {
                AnimSetIndex = 0;
                Debug.LogWarning($"No matching animation set found for object {obj.Index} of type {obj.ObjType} and OAM index {obj.OAMIndex}");
            }

            // Anim 0 is blank by default for these world map objects
            if (Object.ObjType == 82)
                AnimIndex = 1;
        }

        public Unity_ObjectManager_GBAKlonoa ObjManager { get; }
        public GBAKlonoa_LoadedObject Object { get; set; }
        public BinarySerializable Serializable { get; }
        public GBAKlonoa_ObjectOAMCollection OAMCollection { get; }

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

        public override Vector3 Position
        {
            get => new Vector3(Object.XPos, Object.YPos, Object.ZPos);
            set
            {
                Object.XPos = (short)value.x;
                Object.YPos = (short)value.y;
                Object.ZPos = (byte)value.z;
            }
        }

        public override string DebugText => $"Index: {Object.Index}{Environment.NewLine}" +
                                            String.Join(Environment.NewLine, OAMCollection.OAMs.Select((x, i) =>
                                                $"Pal_{i}: {x.PaletteIndex}{Environment.NewLine}" +
                                                $"Tile_{i}: {x.TileIndex}{Environment.NewLine}" +
                                                $"Shape_{i}: {x.Shape}{Environment.NewLine}"));

        public Unity_ObjectManager_GBAKlonoa.AnimSet AnimSet => ObjManager.AnimSets.ElementAtOrDefault(AnimSetIndex);
        public Unity_ObjectManager_GBAKlonoa.AnimSet.Animation Animation => AnimSet?.Animations.ElementAtOrDefault(AnimIndex);

        public override BinarySerializable SerializableData => Serializable;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{Object.ObjType}";
        public override string SecondaryName => null;

        public override bool FlipHorizontally => (Object.Value_7 & 1) == 1;

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
        public override int AnimSpeed => 6; // TODO: Correct this
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

            public ushort Type { get; set; }

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