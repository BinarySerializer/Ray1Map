using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa.KH;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_KlonoaHeroes : Unity_SpriteObject_3D
    {
        public Unity_Object_KlonoaHeroes(Unity_ObjectManager_KlonoaHeroes objManager, MapObject obj)
        {
            ObjManager = objManager;
            MapObject = obj;
            AnimSetIndex = ObjManager.AnimSets.FindItemIndex(x => x?.Index == ObjManager.ROM.MapObjectTypes.ElementAtOrDefault(obj.ObjType)?.AnimFileIndex);
        }

        public Unity_Object_KlonoaHeroes(Unity_ObjectManager_KlonoaHeroes objManager, MapTriggerObject obj)
        {
            ObjManager = objManager;
            TriggerObject = obj;
            AnimSetIndex = 0;
        }

        public Unity_ObjectManager_KlonoaHeroes ObjManager { get; }
        public MapObject MapObject { get; set; }
        public MapTriggerObject TriggerObject { get; set; }

        public override short XPosition
        {
            get => MapObject?.XPos ?? TriggerObject.XPos;
            set
            {
                if (MapObject != null)
                    MapObject.XPos = value;
                else
                    TriggerObject.XPos = value;
            }
        }

        public override short YPosition
        {
            get => MapObject?.YPos ?? TriggerObject.YPos;
            set
            {
                if (MapObject != null)
                    MapObject.YPos = value;
                else
                    TriggerObject.YPos = value;
            }
        }

        public override Vector3 Position
        {
            get => new Vector3(MapObject?.XPos ?? TriggerObject.XPos, MapObject?.YPos ?? TriggerObject.YPos, MapObject?.ZPos ?? TriggerObject.ZPos);
            set
            {
                if (MapObject != null)
                {
                    MapObject.XPos = (short)value.x;
                    MapObject.YPos = (short)value.y;
                    MapObject.ZPos = (byte)value.z;
                }
                else
                {
                    TriggerObject.XPos = (short)value.x;
                    TriggerObject.YPos = (short)value.y;
                    TriggerObject.ZPos = (short)value.z;
                }
            }
        }

        public Unity_ObjectManager_KlonoaHeroes.AnimSet AnimSet => ObjManager.AnimSets.ElementAtOrDefault(AnimSetIndex);
        public Unity_ObjectManager_KlonoaHeroes.AnimSet.Animation Animation => AnimSet?.Animations.ElementAtOrDefault(AnimIndex);

        public override BinarySerializable SerializableData => (BinarySerializable)MapObject ?? TriggerObject;
        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => MapObject != null ? $"Object_{MapObject.ObjType}" : $"Trigger_{TriggerObject.ObjType}";
        public override string SecondaryName => null;

        public override Unity_ObjectType Type => MapObject != null ? Unity_ObjectType.Object : Unity_ObjectType.Trigger;
        public override bool IsEditor => MapObject == null;

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
        public override int AnimSpeed => CurrentAnimation?.AnimSpeeds?.ElementAtOrDefault(AnimationFrame) + 1 ?? 1;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Animation?.AnimFrames;

        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_KlonoaHeroes obj)
            {
                Obj = obj;
            }

            private Unity_Object_KlonoaHeroes Obj { get; }

            public override int DES
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }

            public override int ETA
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }

            public override byte SubEtat
            {
                get => Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public override int SubEtatLength => Obj.AnimSet?.Animations?.Count ?? 0;
        }

        #region UI States

        protected int UIStates_AnimSetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimSetIndex == UIStates_AnimSetIndex;

        protected class KlonoaHeroes_UIState : UIState
        {
            public KlonoaHeroes_UIState(string displayName, byte animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_KlonoaHeroes)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_KlonoaHeroes)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = AnimSetIndex;

            List<UIState> uiStates = new List<UIState>();

            if (AnimSet?.Animations != null)
            {
                for (byte i = 0; i < AnimSet.Animations.Count; i++)
                    uiStates.Add(new KlonoaHeroes_UIState($"Animation {AnimSet.Animations[i].AnimGroupIndex}-{AnimSet.Animations[i].AnimIndex}", animIndex: i));
            }

            UIStates = uiStates.ToArray();
        }

        #endregion
    }
}