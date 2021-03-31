using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAVVNitroKart : Unity_Object_BaseGBAVV
    {
        public Unity_Object_GBAVVNitroKart(Unity_ObjectManager_GBAVV objManager, GBAVV_NitroKart_Object obj, int? objectGroupIndex) : base(objManager)
        {
            Object = obj;
            ObjectGroupIndex = objectGroupIndex;

            // Init the object
            InitObj();
        }

        public void InitObj()
        {
            var typeData = ObjManager.NitroKart_ObjTypeData?.ElementAtOrDefault(Object.ObjType);

            if (typeData == null)
            {
                SetAnimation(-1, -1, 0);
            }
            else
            {
                if (ObjManager.Context.GetR1Settings().EngineVersion != EngineVersion.GBAVV_CrashNitroKart_NGage)
                {
                    var graphicsIndex = ObjManager.Graphics.FindItemIndex(x => x.Offset == typeData.GraphicsDataPointer);
                    SetAnimation(graphicsIndex, typeData.AnimSetIndex, (byte)(typeData.AnimationIndices?.FirstOrDefault() ?? 0));

                }
                else
                {
                    var animSetIndex = ObjManager.AnimSets.FirstOrDefault()?.FindItemIndex(x => x.NGage_FilePath == typeData.NGage_GFXFilePath.FilePath) ?? -1;
                    SetAnimation(0, animSetIndex, (byte)(typeData.AnimationIndices?.FirstOrDefault() ?? 0));
                }
            }
        }

        public GBAVV_NitroKart_Object Object { get; set; }

        public override short XPosition
        {
            get => (short)Object.XPos;
            set => Object.XPos = value;
        }

        public override short YPosition
        {
            get => (short)Object.YPos;
            set => Object.YPos = value;
        }

        public override Vector3 Position
        {
            get {
                if (ObjManager.LevelWidthNitroKartNGage.HasValue) {
                    return new Vector3(Object.XPos, ObjManager.LevelWidthNitroKartNGage.Value-Object.YPos, Object.Height);
                } else {
                    return new Vector3(Object.XPos, Object.YPos, Object.Height);
                }
            }
            set
            {
                Object.XPos = (int)value.x;
                if (ObjManager.LevelWidthNitroKartNGage.HasValue) {
                    Object.YPos = (int)(ObjManager.LevelWidthNitroKartNGage.Value - value.y);
                } else {
                    Object.YPos = (int)value.y;
                }
                Object.Height = (int)value.z;
            }
        }

		public override float Scale => 0.5f;

        public override BinarySerializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{Object.ObjType}";
        public override string SecondaryName => null;

        public override int? ObjectGroupIndex { get; }

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAVVNitroKart obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAVVNitroKart Obj { get; }

            public ushort Type
            {
                get => (ushort)Obj.Object.ObjType;
                set => Obj.Object.ObjType = (short)value;
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
    }
}