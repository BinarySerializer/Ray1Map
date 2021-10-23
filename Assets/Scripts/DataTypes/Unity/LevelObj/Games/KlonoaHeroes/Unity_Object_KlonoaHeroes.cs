using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa.KH;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_KlonoaHeroes : Unity_SpriteObject_3D
    {
        public Unity_Object_KlonoaHeroes(Unity_ObjectManager_KlonoaHeroes objManager, EnemyObject obj)
        {
            ObjManager = objManager;
            EnemyObject = obj;
            AnimSetIndex = ObjManager.AnimSets.FindItemIndex(x => x?.EnemyAnimIndex == ObjManager.ROM.MapObjectTypes.ElementAtOrDefault(obj.ObjType)?.AnimFileIndex);
        }

        public Unity_Object_KlonoaHeroes(Unity_ObjectManager_KlonoaHeroes objManager, TriggerObject obj)
        {
            ObjManager = objManager;
            TriggerObject = obj;
            AnimSetIndex = 0;
        }

        public Unity_ObjectManager_KlonoaHeroes ObjManager { get; }
        public EnemyObject EnemyObject { get; set; }
        public TriggerObject TriggerObject { get; set; }

        public override short XPosition
        {
            get => EnemyObject?.XPos ?? TriggerObject.XPos;
            set
            {
                if (EnemyObject != null)
                    EnemyObject.XPos = value;
                else
                    TriggerObject.XPos = value;
            }
        }

        public override short YPosition
        {
            get => EnemyObject?.YPos ?? TriggerObject.YPos;
            set
            {
                if (EnemyObject != null)
                    EnemyObject.YPos = value;
                else
                    TriggerObject.YPos = value;
            }
        }

        public override Vector3 Position
        {
            get => new Vector3(EnemyObject?.XPos ?? TriggerObject.XPos, EnemyObject?.YPos ?? TriggerObject.YPos, EnemyObject?.ZPos ?? TriggerObject.ZPos);
            set
            {
                if (EnemyObject != null)
                {
                    EnemyObject.XPos = (short)value.x;
                    EnemyObject.YPos = (short)value.y;
                    EnemyObject.ZPos = (byte)value.z;
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

        public override BinarySerializable SerializableData => (BinarySerializable)EnemyObject ?? TriggerObject;
        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => EnemyObject != null ? $"Enemy_{EnemyObject.ObjType}" : $"Trigger_{TriggerObject.ObjType}";
        public override string SecondaryName => null;

        public override Unity_ObjectType Type => EnemyObject != null ? Unity_ObjectType.Object : Unity_ObjectType.Trigger;
        public override bool IsEditor => EnemyObject == null;

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