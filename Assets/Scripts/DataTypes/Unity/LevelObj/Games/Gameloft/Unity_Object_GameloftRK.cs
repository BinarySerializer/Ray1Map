using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine {
    public class Unity_Object_GameloftRK : Unity_SpriteObject_3D {
        public Unity_Object_GameloftRK(Unity_ObjectManager_GameloftRK objManager, Gameloft_RK_Level.TrackObject obj, Gameloft_RK_Level.ObjectType type) {
            ObjManager = objManager;
            Object = obj;
            ObjType = type;
            PuppetIndex = type.PuppetIndex;
            AnimIndex = type.AnimationIndex;
            PaletteIndex = type.PaletteIndex;
        }

        // For custom objects
        public Unity_Object_GameloftRK(Unity_ObjectManager_GameloftRK objManager, int puppetIndex, int animIndex, int paletteIndex, string objectName, int? objectGroupIndex = null) {
            ObjManager = objManager;
            PuppetIndex = puppetIndex;
            AnimIndex = animIndex;
            PaletteIndex = paletteIndex;
            ObjectGroupIndex = objectGroupIndex;
            ObjectName = objectName;
        }

        public Unity_Object_GameloftRK(Unity_ObjectManager_GameloftRK objManager, Gameloft_RK_Level.TrackObject obj, Gameloft_RK_Level.TriggerObject triggerObject) {
            ObjManager = objManager;
            Object = obj;
            Trigger = triggerObject;
        }

        public Unity_ObjectManager_GameloftRK ObjManager { get; }
        public Gameloft_RK_Level.TrackObject Object { get; set; }
        public Gameloft_RK_Level.TrackObjectInstance Instance { get; set; }
        public Gameloft_RK_Level.ObjectType ObjType { get; set; }
        public Gameloft_RK_Level.TriggerObject Trigger { get; set; }
        public bool ForceNoGraphics => Trigger != null;

        public int PuppetIndex { get; set; }
        public int AnimIndex { get; set; }

        public override bool IsAlways => ObjectName == "Lum"; // Lums are marked as always objects
        public override bool IsEditor => Trigger != null || ObjectName == "Player";
        public override Unity_ObjectType Type => Trigger != null ? Unity_ObjectType.Trigger : Unity_ObjectType.Object;

        public override int? ObjectGroupIndex { get; }


		public override Vector3 Position { get; set; }
        public override short XPosition { get; set; }
        public override short YPosition { get; set; }

        public override string DebugText => $"ObjectType: {Object?.ObjectType}{Environment.NewLine}" +
            $"ObjType: {Instance?.ObjType}{Environment.NewLine}" +
            $"TrackObjIndex: {Instance?.TrackObjectIndex}{Environment.NewLine}" + 
            $"TriggerFlags: {TriggerFlags}";


        public override BinarySerializable SerializableData => Object;
        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"{Type}_{Object?.ObjectType.ToString() ?? ObjectName}";
        public override string SecondaryName => PuppetData?.Name;
        public Unity_ObjectManager_GameloftRK.PuppetData PuppetData => ForceNoGraphics ? null : ObjManager.Puppets.ElementAtOrDefault(PuppetIndex);

        public override bool FlipHorizontally => Instance?.FlipX ?? false;
        public override bool FlipVertically => false;

        public override bool CanBeLinkedToGroup => false;
		public override bool CanBeLinked => false;
        public string ObjectName { get; set; }
        public int? TriggerFlags => Trigger?.Flags;

		public override Unity_ObjAnimation CurrentAnimation => PuppetData?.Puppet?.Animations?.ElementAtOrDefault(AnimationIndex ?? -1);
        public override int AnimSpeed => CurrentAnimation?.AnimSpeed ?? 0;
        public override int? GetAnimIndex => OverrideAnimIndex ?? AnimIndex;
        public int PaletteIndex { get; set; } = 0;
        protected override int GetSpriteID => PuppetIndex;
        public override IList<Sprite> Sprites => PuppetData?.Puppet?.Sprites[PaletteIndex];

        private Unity_ObjAnimationCollisionPart[] objCollision;
        public override Unity_ObjAnimationCollisionPart[] ObjCollision {
            get {
                if (objCollision == null && Trigger != null) {
                    objCollision = new Unity_ObjAnimationCollisionPart[] {
                        new Unity_ObjAnimationCollisionPart
                        {
                            XPosition = -Trigger.Width / 8,
                            YPosition = -Trigger.Height / 8,
                            Width = Trigger.Width * 2 / 8,
                            Height = Trigger.Height / 8,
                            Type = Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox
                        }
                    };
                }
                return objCollision;
            }
        }


        private class LegacyEditorWrapper : BaseLegacyEditorWrapper {
            public LegacyEditorWrapper(Unity_Object_GameloftRK obj) {
                Obj = obj;
            }

            private Unity_Object_GameloftRK Obj { get; }

            public override ushort Type {
                get => (ushort)Obj.PuppetIndex;
                set => Obj.PuppetIndex = (short)value;
            }

            public override int DES {
                get => Obj.ForceNoGraphics ? -1 : Obj.PuppetIndex;
                set {
                    if(!Obj.ForceNoGraphics)
                        Obj.PuppetIndex = (short)value;
                }
            }

            public override int ETA {
                get => Obj.ForceNoGraphics ? -1 : Obj.PuppetIndex;
                set {
                    if (!Obj.ForceNoGraphics)
                        Obj.PuppetIndex = (short)value;
                }
            }

            public override byte Etat {
                get => (byte)Obj.PaletteIndex;
                set => Obj.PaletteIndex = value;
            }

            public override byte SubEtat {
                get => (byte)Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public override int EtatLength => Obj.PuppetData?.Puppet?.Sprites?.Length ?? 0;
            public override int SubEtatLength => Obj.PuppetData?.Puppet?.Animations?.Length ?? 0;
        }
        #region UI States

        protected int UIStates_PuppetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => PuppetIndex == UIStates_PuppetIndex;

		protected class GameloftRK_UIState : UIState
        {
            public int PaletteIndex { get; set; }

            public GameloftRK_UIState(string displayName, int animIndex, int paletteIndex) : base(displayName, animIndex) {
                PaletteIndex = paletteIndex;   
            }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GameloftRK)obj).AnimIndex = (short)AnimIndex;
                ((Unity_Object_GameloftRK)obj).PaletteIndex = PaletteIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GameloftRK)obj).AnimIndex && PaletteIndex == ((Unity_Object_GameloftRK)obj).PaletteIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_PuppetIndex = PuppetIndex;

            List<UIState> uiStates = new List<UIState>();
            int count = (PuppetData?.Puppet?.Animations?.Length ?? 0);
            int paletteCount = (PuppetData?.Puppet?.Sprites?.Length ?? 1);

            for (int i = 0; i < count; i++) {
                for (int p = 0; p < paletteCount; p++) {
                    uiStates.Add(new GameloftRK_UIState($"Animation {i}-{p}", animIndex: i, p));
                }
            }

            UIStates = uiStates.ToArray();
        }
        
        #endregion
    }
}