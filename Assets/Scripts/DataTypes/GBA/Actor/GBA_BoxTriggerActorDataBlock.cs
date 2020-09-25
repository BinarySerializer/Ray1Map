namespace R1Engine
{
    public class GBA_BoxTriggerActorDataBlock : GBA_BaseBlock
    {
        public GBA_BoxTriggerActorData[] Data { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            if (s.GameSettings.EngineVersion >= EngineVersion.GBA_PrinceOfPersia) {
                Data = s.SerializeObjectArray<GBA_BoxTriggerActorData>(Data, BlockSize / 8, name: nameof(Data));
            } else {
                Data = s.SerializeObjectArray<GBA_BoxTriggerActorData>(Data, BlockSize / 6, name: nameof(Data));
            }
        }
    }
}