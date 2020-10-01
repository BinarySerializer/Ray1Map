namespace R1Engine
{
    public class GBA_BoxTriggerActorDataBlock : GBA_BaseBlock
    {
        public GBA_BoxTriggerActorData[] Data { get; set; }
        public int Length { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            Data = s.SerializeObjectArray<GBA_BoxTriggerActorData>(Data, Length, name: nameof(Data));
        }
    }
}