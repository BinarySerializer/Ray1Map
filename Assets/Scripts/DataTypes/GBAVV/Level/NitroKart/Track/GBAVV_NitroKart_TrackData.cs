using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_NitroKart_TrackData : BinarySerializable
    {
        public Pointer TrackWaypointsPointer_Normal { get; set; }
        public Pointer TrackWaypointsPointer_TimeTrial { get; set; }
        public Pointer TrackWaypointsPointer_BossRace { get; set; }
        public int TrackWaypointsCount_Normal { get; set; }
        public int TrackWaypointsCount_TimeTrial { get; set; }
        public int TrackWaypointsCount_BossRace { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_TrackWaypoint[] TrackWaypoints_Normal { get; set; }
        public GBAVV_NitroKart_TrackWaypoint[] TrackWaypoints_TimeTrial { get; set; }
        public GBAVV_NitroKart_TrackWaypoint[] TrackWaypoints_BossRace { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion != EngineVersion.GBAVV_CrashNitroKart_NGage)
            {
                TrackWaypointsPointer_Normal = s.SerializePointer(TrackWaypointsPointer_Normal, name: nameof(TrackWaypointsPointer_Normal));
                TrackWaypointsPointer_TimeTrial = s.SerializePointer(TrackWaypointsPointer_TimeTrial, name: nameof(TrackWaypointsPointer_TimeTrial));
                TrackWaypointsPointer_BossRace = s.SerializePointer(TrackWaypointsPointer_BossRace, name: nameof(TrackWaypointsPointer_BossRace));

                TrackWaypointsCount_Normal = s.Serialize<int>(TrackWaypointsCount_Normal, name: nameof(TrackWaypointsCount_Normal));
                TrackWaypointsCount_TimeTrial = s.Serialize<int>(TrackWaypointsCount_TimeTrial, name: nameof(TrackWaypointsCount_TimeTrial));
                TrackWaypointsCount_BossRace = s.Serialize<int>(TrackWaypointsCount_BossRace, name: nameof(TrackWaypointsCount_BossRace));
            }
            else
            {
                TrackWaypointsPointer_Normal = s.SerializePointer(TrackWaypointsPointer_Normal, name: nameof(TrackWaypointsPointer_Normal));
                TrackWaypointsCount_Normal = s.Serialize<int>(TrackWaypointsCount_Normal, name: nameof(TrackWaypointsCount_Normal));

                TrackWaypointsPointer_TimeTrial = s.SerializePointer(TrackWaypointsPointer_TimeTrial, name: nameof(TrackWaypointsPointer_TimeTrial));
                TrackWaypointsCount_TimeTrial = s.Serialize<int>(TrackWaypointsCount_TimeTrial, name: nameof(TrackWaypointsCount_TimeTrial));

                TrackWaypointsPointer_BossRace = s.SerializePointer(TrackWaypointsPointer_BossRace, name: nameof(TrackWaypointsPointer_BossRace));
                TrackWaypointsCount_BossRace = s.Serialize<int>(TrackWaypointsCount_BossRace, name: nameof(TrackWaypointsCount_BossRace));
            }

            TrackWaypoints_Normal = s.DoAt(TrackWaypointsPointer_Normal, () => s.SerializeObjectArray<GBAVV_NitroKart_TrackWaypoint>(TrackWaypoints_Normal, TrackWaypointsCount_Normal, name: nameof(TrackWaypoints_Normal)));
            TrackWaypoints_TimeTrial = s.DoAt(TrackWaypointsPointer_TimeTrial, () => s.SerializeObjectArray<GBAVV_NitroKart_TrackWaypoint>(TrackWaypoints_TimeTrial, TrackWaypointsCount_TimeTrial, name: nameof(TrackWaypoints_TimeTrial)));
            TrackWaypoints_BossRace = s.DoAt(TrackWaypointsPointer_BossRace, () => s.SerializeObjectArray<GBAVV_NitroKart_TrackWaypoint>(TrackWaypoints_BossRace, TrackWaypointsCount_BossRace, name: nameof(TrackWaypoints_BossRace)));
        }
    }
}