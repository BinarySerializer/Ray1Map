using System;
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

            var def = ObjManager.ROM.EnemyObjectDefinitions.ElementAtOrDefault(obj.ObjType);
            SetAnimation(def?.AnimFileIndex ?? -1, Unity_ObjectManager_KlonoaHeroes.AnimSet.FilePackType.Enemy, def?.AnimGroupIndex ?? 0, def?.AnimIndex ?? 0);
        }

        public Unity_Object_KlonoaHeroes(Unity_ObjectManager_KlonoaHeroes objManager, GenericObject obj)
        {
            ObjManager = objManager;
            GenericObject = obj;
            Init(obj);
        }

        public Unity_ObjectManager_KlonoaHeroes ObjManager { get; }
        public EnemyObject EnemyObject { get; set; }
        public GenericObject GenericObject { get; set; }

        public override short XPosition
        {
            get => EnemyObject?.XPos ?? GenericObject.XPos;
            set
            {
                if (EnemyObject != null)
                    EnemyObject.XPos = value;
                else
                    GenericObject.XPos = value;
            }
        }

        public override short YPosition
        {
            get => EnemyObject?.YPos ?? GenericObject.YPos;
            set
            {
                if (EnemyObject != null)
                    EnemyObject.YPos = value;
                else
                    GenericObject.YPos = value;
            }
        }

        public override Vector3 Position
        {
            get => new Vector3(EnemyObject?.XPos ?? GenericObject.XPos, EnemyObject?.YPos ?? GenericObject.YPos, EnemyObject?.ZPos ?? GenericObject.ZPos);
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
                    GenericObject.XPos = (short)value.x;
                    GenericObject.YPos = (short)value.y;
                    GenericObject.ZPos = (short)value.z;
                }
            }
        }

        public Unity_ObjectManager_KlonoaHeroes.AnimSet AnimSet => ObjManager.AnimSets.ElementAtOrDefault(AnimSetIndex);
        public Unity_ObjectManager_KlonoaHeroes.AnimSet.Animation Animation => AnimSet?.Animations.ElementAtOrDefault(AnimIndex);

        public override BinarySerializable SerializableData => (BinarySerializable)EnemyObject ?? GenericObject;
        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => EnemyObject != null ? $"Enemy_{EnemyObject.ObjType}" : $"Object_{GenericObject.ObjType}";
        public override string SecondaryName => null;

        public override Unity_ObjectType Type => EnemyObject != null ? Unity_ObjectType.Object : Unity_ObjectType.Trigger;

        public override string DebugText => $"Palette Indices {String.Join(", ", Animation?.KlonoaAnim.Frames.SelectMany(x => x.Sprites).Select(x => x.PaletteIndex).Distinct() ?? new byte[0])}{Environment.NewLine}" +
                                            $"Palette Modes {String.Join(", ", Animation?.KlonoaAnim.Frames.SelectMany(x => x.Sprites).Select(x => x.PaletteMode).Distinct() ?? new byte[0])}{Environment.NewLine}";

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

        public int AnimIndex { get; set; }

        public override Unity_ObjAnimation CurrentAnimation => Animation?.ObjAnimation;
        public override int AnimSpeed => CurrentAnimation?.AnimSpeeds?.ElementAtOrDefault(AnimationFrame) + 1 ?? 1;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Animation?.AnimFrames;

        protected void SetAnimation(int fileIndex, Unity_ObjectManager_KlonoaHeroes.AnimSet.FilePackType pack, int animGroup = 0, int animIndex = 0)
        {
            AnimSetIndex = fileIndex == -1 ? 0 : ObjManager.AnimSets.FindItemIndex(x => x?.PackType == pack && x.FileIndex == fileIndex);
            AnimIndex = AnimSet?.Animations.FindItemIndex(x => x.AnimGroupIndex == animGroup && x.AnimIndex == animIndex) ?? 0;

            if (AnimIndex == -1)
                AnimIndex = 0;
        }

        protected void Init(GenericObject obj)
        {
            int fileIndex = -1;
            int animGroup = 0;
            int animIndex = 0;

            int id1 = ObjManager.LevelEntry.ID1;
            int id2 = ObjManager.LevelEntry.ID2;
            int id3 = ObjManager.LevelEntry.ID3;

            // FUN_0803bfd0
            switch (obj.ObjType)
            {
                // Map entrance/exit
                case 1:
                    if (obj.Bytes_08[15 - 8] == 1)
                        return;

                    fileIndex = 0x28;
                    animGroup = 0x15;

                    bool f = obj.Bytes_08[13 - 8] == 2;

                    switch (obj.Bytes_08[11 - 8])
                    {
                        default:
                            animIndex = f ? 0 : 4;
                            YPosition -= 16;
                            break;

                        case 2:
                            animIndex = f ? 4 : 0;
                            YPosition += 16;
                            break;

                        case 5:
                        case 7:
                        case 8:
                            animIndex = f ? 0 : 4;
                            YPosition -= 32;
                            break;

                        case 6:
                            animIndex = f ? 4 : 0;
                            YPosition += 32;
                            break;
                    }

                    if (obj.Bytes_08[15 - 8] == 2)
                        XPosition -= 16;
                    else if (obj.Bytes_08[15 - 8] == 3)
                        XPosition += 16;
                    break;

                // Door
                case 2:
                    fileIndex = 0x26;
                    animIndex = obj.Bytes_08[9 - 8];

                    int param;

                    if (9 < id1)
                    {
                        if (id1 < 0xb)
                            param = id2;
                        else
                            param = id1 - 10;
                    }
                    else
                    {
                        param = id1;
                    }

                    switch (param)
                    {
                        default:
                            animGroup = 0;
                            break;

                        case 2:
                            if (id1 == 2 && (byte)(id2 - 3) < 2 || id1 == 0x20a || id1 == 0xc)
                            {
                                animIndex += 2;
                                XPosition += 16;
                                YPosition -= 16;
                            }
                            animGroup = 1;
                            break;

                        case 3:
                            animGroup = 2;
                            break;

                        case 4:
                            XPosition += 16;
                            YPosition -= 16;
                            animGroup = 3;
                            break;

                        case 5:
                            if (id1 == 0xf)
                            {
                                if (2 < id3)
                                    animIndex += 2;
                            }
                            else
                            {
                                if (id1 == 10 || (id1 & 0x100) == 0)
                                    animIndex += 2;
                            }
                            animGroup = 4;
                            break;

                        case 6:
                            XPosition += 16;
                            YPosition -= 16;
                            animGroup = 5;
                            break;

                        case 7:
                            animGroup = 6;
                            break;
                    }
                    break;

                // Digit
                case 8:
                    fileIndex = 0x2C;
                    animGroup = 1;
                    animIndex = 2;
                    break;

                // Effect
                case 9:
                    fileIndex = 0x2E;

                    var b1 = obj.Bytes_08[8 - 8];
                    var b2 = BitHelpers.ExtractBits(obj.Bytes_08[9 - 8], 3, 0);

                    animGroup = b1 switch
                    {
                        1 when b2 == 0 => 3,
                        1 when b2 == 1 => 4,
                        1 when b2 == 2 => 7,
                        2 when b2 == 0 => 2,
                        3 when b2 == 0 => 8,
                        5 when b2 == 0 => 9,
                        5 when b2 == 1 => 0,
                        8 when b2 == 0 => 10,
                        _ => animGroup
                    };

                    break;

                // Crumbling object
                case 12:
                    fileIndex = 0x2A;

                    switch ((id1 - 3) * 0x1000000 >> 0x18)
                    {
                        case 1:
                        case 0xb:
                            if (id2 == 2)
                                animIndex = 1;

                            animGroup = 1;
                            break;

                        case 2:
                            if ((id2 & 1) == 0)
                                animIndex = 1;

                            if (3 < id3)
                                animIndex = 1;

                            animGroup = 2;
                            break;

                        case 0xc:
                            if (3 < id3)
                                animIndex = 1;

                            animGroup = 2;
                            break;

                        case 3:
                        case 0xd:
                            if (2 < id2)
                                animIndex = 1;

                            animGroup = 3;
                            break;

                        case 4:
                        case 0xe:
                            animGroup = 4;
                            break;
                    }
                    break;

                // Chest
                case 15:
                    fileIndex = 0x27;
                    break;
            }

            SetAnimation(fileIndex, Unity_ObjectManager_KlonoaHeroes.AnimSet.FilePackType.Gameplay, animGroup, animIndex);
        }

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
        }

        #region UI States

        protected int UIStates_AnimSetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimSetIndex == UIStates_AnimSetIndex;

        protected class KlonoaHeroes_UIState : UIState
        {
            public KlonoaHeroes_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_KlonoaHeroes)obj).AnimIndex = AnimIndex;
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
                for (int i = 0; i < AnimSet.Animations.Count; i++)
                    uiStates.Add(new KlonoaHeroes_UIState($"Animation {AnimSet.Animations[i].AnimGroupIndex}-{AnimSet.Animations[i].AnimIndex}", animIndex: i));
            }

            UIStates = uiStates.ToArray();
        }

        #endregion
    }
}