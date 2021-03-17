namespace R1Engine
{
    public class GBAVV_Crash2_CutsceneEntry : R1Serializable
    {
        public int FLCIndex { get; set; }
        public int LocIndex { get; set; }
        public int MusicIndex { get; set; }
        public int Int_0C { get; set; }
        public int Int_10 { get; set; }
        public int Int_14 { get; set; }
        public int Int_18 { get; set; }
        public int Int_1C { get; set; }
        public int Int_20 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FLCIndex = s.Serialize<int>(FLCIndex, name: nameof(FLCIndex));
            LocIndex = s.Serialize<int>(LocIndex, name: nameof(LocIndex));
            s.Log($"LevelName: {s.Context.GetStoredObject<GBAVV_Crash_LocTable>(GBAVV_Crash_BaseManager.LocTableID)?.Strings[LocIndex]}");
            MusicIndex = s.Serialize<int>(MusicIndex, name: nameof(MusicIndex));
            Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));
            Int_18 = s.Serialize<int>(Int_18, name: nameof(Int_18));
            Int_1C = s.Serialize<int>(Int_1C, name: nameof(Int_1C));
            Int_20 = s.Serialize<int>(Int_20, name: nameof(Int_20));
        }
    }
}