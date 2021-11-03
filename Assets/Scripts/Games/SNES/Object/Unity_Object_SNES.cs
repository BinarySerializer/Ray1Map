using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Ray1;
using UnityEngine;
using Sprite = UnityEngine.Sprite;

namespace Ray1Map.SNES
{
    public class Unity_Object_SNES : Unity_SpriteObject
    {
        public Unity_Object_SNES(SNES_ObjData obj, Unity_ObjectManager_SNES objManager)
        {
            Object = obj;
            ObjManager = objManager;
        }

        public SNES_ObjData Object { get; }
        public Unity_ObjectManager_SNES ObjManager { get; }

        public override short XPosition
        {
            get => Object.XPosition;
            set => Object.XPosition = value;
        }

        public override short YPosition
        {
            get => Object.YPosition;
            set => Object.YPosition = value;
        }

        public override Vector2 Pivot => new Vector2(40,0); // Hardcoded

        private int _graphicsGroupIndex;
        public int GraphicsGroupIndex
        {
            get => _graphicsGroupIndex;
            set
            {
                _graphicsGroupIndex = value;
                StateIndex = 0;
            }
        }

        public int StateIndex { get; set; }

        public override string DebugText => String.Empty;

        public override BinarySerializable SerializableData => Object;
        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

		public override bool FlipHorizontally => !State.SNES_State.Flags.HasFlag(SNES_State.StateFlags.UseCurrentFlip)
            && !State.SNES_State.Flags.HasFlag(SNES_State.StateFlags.HorizontalFlip);

		public override string PrimaryName => $"Rayman";
        public override string SecondaryName => null;

        public Unity_ObjectManager_SNES.GraphicsGroup GraphicsGroup => ObjManager.GraphicsGroups.ElementAtOrDefault(GraphicsGroupIndex);
        public Unity_ObjectManager_SNES.GraphicsGroup.State State => GraphicsGroup?.States.ElementAtOrDefault(StateIndex);

        public override Unity_ObjAnimation CurrentAnimation => State?.Animation;
        public override int AnimSpeed => State?.SNES_State.AnimSpeed ?? 0;
        public override int? GetAnimIndex => StateIndex;
        protected override int GetSpriteID => GraphicsGroupIndex;
        public override IList<Sprite> Sprites => GraphicsGroup?.Sprites;


        #region UI States
        protected int UIStates_GraphicsGroupIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => GraphicsGroupIndex == UIStates_GraphicsGroupIndex;

        protected override void RecalculateUIStates() {
            UIStates_GraphicsGroupIndex = GraphicsGroupIndex;
            UIStates = GraphicsGroup?.States?.Select((x, i) =>
            {
                var animIndex = GraphicsGroup.States.Select(s => s.SNES_State.Animation).Distinct().FindItemIndex(s => s == x.SNES_State.Animation);

                return (UIState)new SNES_UIState(GraphicsGroup.IsRecreated ? $"Recreated animation {animIndex}" : $"{i} (Animation {animIndex})", i);
            }).ToArray() ?? new UIState[0];
        }

        protected class SNES_UIState : UIState {
            public SNES_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj) {
                var snesObj = (Unity_Object_SNES)obj;
                snesObj.StateIndex = (short)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj) => AnimIndex == ((Unity_Object_SNES)obj).StateIndex;
        }
        #endregion

        #region LegacyEditorWrapper
        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_SNES obj)
            {
                Obj = obj;
            }

            private Unity_Object_SNES Obj { get; }

            public override int DES
            {
                get => Obj.GraphicsGroupIndex;
                set => Obj.GraphicsGroupIndex = value;
            }

            public override int ETA
            {
                get => Obj.GraphicsGroupIndex;
                set => Obj.GraphicsGroupIndex = value;
            }

            public override byte Etat
            {
                get => (byte)Obj.StateIndex;
                set => Obj.StateIndex = value;
            }

            public override int EtatLength => Obj.GraphicsGroup?.States?.Length ?? 0;
        }
        #endregion
    }
}