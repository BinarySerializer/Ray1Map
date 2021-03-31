using System;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class Unity_Object_GBAVV : Unity_Object_BaseGBAVV
    {
        public Unity_Object_GBAVV(Unity_ObjectManager_GBAVV objManager, GBAVV_Map2D_Object obj, int objGroupIndex, int objIndex) : base(objManager)
        {
            Object = obj;
            ObjGroupIndex = objGroupIndex;
            ObjIndex = objIndex;
            _prevTimeTrialMode = Settings.GBAVV_Crash_TimeTrialMode;

            // Init the object
            InitObj();

            // Set link group
            if (IsLinked_4)
                EditorLinkGroup = ObjParams?.ElementAtOrDefault(4) ?? 0;
            if (IsLinked_6)
                EditorLinkGroup = ObjParams?.ElementAtOrDefault(6) + 0xFF ?? 0;
        }

        public int ObjGroupIndex { get; }
        public int ObjIndex { get; }

        public void InitObj()
        {
            var objType = Object.ObjType;

            if (Settings.GBAVV_Crash_TimeTrialMode && (ObjParams?.ElementAtOrDefault(0) & 0x20) != 0)
                objType = ObjParams?.ElementAtOrDefault(4) ?? Object.ObjType;

            GBAVV_ObjInit.InitObj(ObjManager.Context.GetR1Settings(), this, objType);
        }

        public GBAVV_Map2D_Object Object { get; set; }

        public bool IsLinked_4 { get; set; }
        public bool IsLinked_6 { get; set; }

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

        public override string DebugText => $"Params: {ObjParams.ToHexString()}{Environment.NewLine}" +
                                            $"Group: {ObjGroupIndex}{Environment.NewLine}" +
                                            $"Index: {ObjIndex}{Environment.NewLine}";

        public override int? GetLayer(int index)
        {
            if (ObjManager.Context.GetR1Settings().EngineVersion < EngineVersion.GBAVV_CrashFusion)
                return null;

            if (Object.ObjType <= 10)
                return 0;

            return -index;
        }

        public byte[] ObjParams => ObjManager.ObjParams?.ElementAtOrDefault(Object.ObjParamsIndex);

        public override GBAVV_Script DialogScript => ScriptHasDialog ? ObjManager.DialogScripts?.TryGetItem(ObjParams?.ElementAtOrDefault(8) ?? -1) : null;
        public override bool ScriptHasDialog => Script?.DisplayName == "genericNPC";

        public override BinarySerializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => ScriptName != null ? $"{ScriptName.Replace("Script", "")}" : $"Type_{(int)Object.ObjType}";
        public override string SecondaryName
        {
            get
            {
                if (ObjManager.Context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1)
                    return $"{(GBAVV_Map2D_Crash1_ObjType)Object.ObjType}";

                if (ObjManager.Context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash2 && ObjManager.Crash2_IsWorldMap)
                    return $"{(GBAVV_Crash2_WorldMapObjType)Object.ObjType}";

                if (ObjManager.Context.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash2)
                    return $"{(GBAVV_Map2D_Crash2_ObjType)Object.ObjType}";

                return null;
            }
        }

        public override bool FlipHorizontally => GetFlipFlagsIndex() != -1 && BitHelpers.ExtractBits(ObjParams?.FirstOrDefault() ?? 0, 1, GetFlipFlagsIndex()) == 1;
        public override bool FlipVertically => GetFlipFlagsIndex() != -1 && BitHelpers.ExtractBits(ObjParams?.FirstOrDefault() ?? 0, 1, GetFlipFlagsIndex() + 1) == 1;
        public int GetFlipFlagsIndex()
        {
            switch (ObjManager.Context.GetR1Settings().EngineVersion)
            {
                case EngineVersion.GBAVV_Crash1:
                case EngineVersion.GBAVV_Crash2:
                    return 1;

                case EngineVersion.GBAVV_CrashFusion:
                case EngineVersion.GBAVV_SpyroFusion:
                case EngineVersion.GBAVV_ThatsSoRaven:
                case EngineVersion.GBAVV_SharkTale:
                case EngineVersion.GBAVV_Shrek2BegForMercy:
                case EngineVersion.GBAVV_BatmanBegins:
                case EngineVersion.GBAVV_Madagascar:
                case EngineVersion.GBAVV_MadagascarOperationPenguin:
                case EngineVersion.GBAVV_UltimateSpiderMan:
                case EngineVersion.GBAVV_SpiderMan3:
                    return 0;

                default:
                    return -1;
            }
        }

        public override bool CanBeLinkedToGroup => true;

        public bool _prevTimeTrialMode;
        public override void OnUpdate()
        {
            if (_prevTimeTrialMode == Settings.GBAVV_Crash_TimeTrialMode)
                return;

            _prevTimeTrialMode = Settings.GBAVV_Crash_TimeTrialMode;
            InitObj();
        }

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAVV obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAVV Obj { get; }

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