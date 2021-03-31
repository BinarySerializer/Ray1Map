using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_NitroKart_Objects : BinarySerializable
    {
        public Pointer ObjectsPointer_Normal { get; set; }
        public Pointer ObjectsPointer_TimeTrial { get; set; }
        public Pointer ObjectsPointer_BossRace { get; set; }

        public int ObjectsCount_Normal { get; set; }
        public int ObjectsCount_TimeTrial { get; set; }
        public int ObjectsCount_BossRace { get; set; }

        public GBAVV_NitroKart_Object[] Objects_Normal { get; set; }
        public GBAVV_NitroKart_Object[] Objects_TimeTrial { get; set; }
        public GBAVV_NitroKart_Object[] Objects_BossRace { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectsPointer_Normal = s.SerializePointer(ObjectsPointer_Normal, name: nameof(ObjectsPointer_Normal));

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_CrashNitroKart_NGage)
                ObjectsCount_Normal = s.Serialize<int>(ObjectsCount_Normal, name: nameof(ObjectsCount_Normal));

            ObjectsPointer_TimeTrial = s.SerializePointer(ObjectsPointer_TimeTrial, name: nameof(ObjectsPointer_TimeTrial));

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_CrashNitroKart_NGage)
                ObjectsCount_TimeTrial = s.Serialize<int>(ObjectsCount_TimeTrial, name: nameof(ObjectsCount_TimeTrial));

            ObjectsPointer_BossRace = s.SerializePointer(ObjectsPointer_BossRace, name: nameof(ObjectsPointer_BossRace));

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_CrashNitroKart_NGage)
                ObjectsCount_BossRace = s.Serialize<int>(ObjectsCount_BossRace, name: nameof(ObjectsCount_BossRace));

            Objects_Normal = s.DoAt(ObjectsPointer_Normal, () => s.SerializeObjectArrayUntil(Objects_Normal, x => x.ObjType == 0, includeLastObj: false, name: nameof(Objects_Normal)));
            Objects_TimeTrial = s.DoAt(ObjectsPointer_TimeTrial, () => s.SerializeObjectArrayUntil(Objects_TimeTrial, x => x.ObjType == 0, includeLastObj: false, name: nameof(Objects_TimeTrial)));
            Objects_BossRace = s.DoAt(ObjectsPointer_BossRace, () => s.SerializeObjectArrayUntil(Objects_BossRace, x => x.ObjType == 0, includeLastObj: false, name: nameof(Objects_BossRace)));
        }
    }
}