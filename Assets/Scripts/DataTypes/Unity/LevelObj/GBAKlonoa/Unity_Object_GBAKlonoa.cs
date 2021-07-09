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

            var settings = objManager.Context.GetR1Settings();
            var engineVersion = settings.EngineVersion;

            // Hack for final boss
            if (engineVersion == EngineVersion.KlonoaGBA_EOD && objManager.Context.GetR1Settings().World == 5 && objManager.Context.GetR1Settings().Level == 8 && obj.ObjType == 23)
            {
                oamCollection.OAMs[0].TileIndex = 492;

                if (obj.Index == 28)
                    AnimIndex = 1;
                else if (obj.Index == 29)
                    AnimIndex = 2;
            }

            if (oamCollection != null)
                AnimSetIndex = objManager.AnimSets.FindItemIndex(x => x.DCT_GraphisIndex == -1 && x.OAMCollections.Any(o => o[0].TileIndex == oamCollection.OAMs[0].TileIndex));
            else if (engineVersion == EngineVersion.KlonoaGBA_DCT)
                AnimSetIndex = objManager.AnimSets.FindItemIndex(x => x.DCT_GraphisIndex == Object.DCT_GraphicsIndex);

            if (AnimSetIndex == -1)
            {
                AnimSetIndex = 0;
                Debug.LogWarning($"No matching animation set found for object {obj.Index} of type {obj.ObjType} and OAM index {obj.OAMIndex}");
            }

            if (engineVersion == EngineVersion.KlonoaGBA_EOD)
            {
                // Anim 0 is blank by default for these world map objects
                if (Object.ObjType == 82)
                    AnimIndex = 1;

                // Waterfall fix
                if (Object.ObjType == 58)
                    AnimIndex = 1;

                // Boss 2 fix
                if (objManager.Context.GetR1Settings().World == 2 && objManager.Context.GetR1Settings().Level == 8)
                {
                    if (obj.Index == 25 || obj.Index == 26)
                        AnimIndex = 1;
                }

                // Final boss fix
                if (objManager.Context.GetR1Settings().World == 5 && objManager.Context.GetR1Settings().Level == 8 && obj.ObjType == 23)
                {
                    if (obj.Index == 28)
                        AnimIndex = 1;
                    else if (obj.Index == 29)
                        AnimIndex = 2;
                }

                Rotation = Object.ObjType == 54 || Object.ObjType == 117 ? 90 * (Object.Param_2 - 1) : (float?)null;
            }
            else if (engineVersion == EngineVersion.KlonoaGBA_DCT)
            {
                // Rotate tornados
                if (Object.ObjType == 78)
                {
                    if (Object.Param_1 == 1)
                        Rotation = -45;
                    else if (Object.Param_1 == 2)
                        Rotation = 45;
                }

                // Hack for world map objects
                if (settings.Level == 0 && Object.Index >= GBAKlonoa_DCT_Manager.FixCount)
                {
                    var rom = ObjManager.Context.GetMainFileObject<GBAKlonoa_DCT_ROM>(GBAKlonoa_BaseManager.GetROMFilePath);
                    var graphics = rom.WorldMapObjectGraphics[settings.World - 1];

                    for (int i = 0; i < graphics.Length; i++)
                    {
                        var g = graphics[i];

                        if ((g.VRAMPointer - 0x06010000) / 0x20 == OAMCollection.OAMs[0].TileIndex)
                        {
                            AnimIndex = (byte)i;
                            break;
                        }
                    }
                }
            }
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

        public override float? Rotation { get; }
        public override Vector2 Pivot
        {
            get
            {
                var animLayer = Animation?.ObjAnimation?.Frames?.ElementAtOrDefault(AnimationFrame)?.SpriteLayers.FirstOrDefault();
                var frame = Animation?.AnimFrames?.ElementAtOrDefault(AnimationFrame);

                if (animLayer == null || frame == null)
                    return base.Pivot;

                return new Vector2(animLayer.XPosition + frame.rect.width / 2, -(animLayer.YPosition + frame.rect.height / 2));
            }
        }

        public override string DebugText => $"Index: {Object.Index}{Environment.NewLine}" +
                                            (OAMCollection != null ? String.Join(Environment.NewLine, OAMCollection.OAMs.Select((x, i) =>
                                                $"Pal_{i}: {x.PaletteIndex}{Environment.NewLine}" +
                                                $"Tile_{i}: {x.TileIndex}{Environment.NewLine}" +
                                                $"Shape_{i}: {x.Shape}{Environment.NewLine}")) : String.Empty) +
                                            $"Param1: {Object.Param_1}";

        public Unity_ObjectManager_GBAKlonoa.AnimSet AnimSet => ObjManager.AnimSets.ElementAtOrDefault(AnimSetIndex);
        public Unity_ObjectManager_GBAKlonoa.AnimSet.Animation Animation => AnimSet?.Animations.ElementAtOrDefault(AnimIndex);

        public override BinarySerializable SerializableData => Serializable;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{Object.ObjType}";
        public override string SecondaryName => null;

        public override bool FlipHorizontally => (Object.Param_2 & 1) == 1 || Serializable is GBAKlonoa_LevelStartInfo start && start.IsFlipped;

        public override int? GetLayer(int index) => Object.Index == 0 ? 0 : -index; // Force Klonoa to be in front

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
        public override int AnimSpeed => (Animation?.AnimSpeeds?.ElementAtOrDefault(AnimationFrame) + 1) ?? 6;
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
                uiStates.Add(new GBAKlonoa_UIState($"Animation {i}{(AnimSet.Animations[i].LinkedAnimIndex != null ? $" -> {AnimSet.Animations[i].LinkedAnimIndex}" : String.Empty)}", animIndex: i));

            UIStates = uiStates.ToArray();
        }
        
        #endregion
    }
}