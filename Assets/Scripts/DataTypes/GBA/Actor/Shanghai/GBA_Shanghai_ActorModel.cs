using BinarySerializer;

namespace R1Engine
{
    public class GBA_Shanghai_ActorModel : GBA_BaseBlock
    {
        public GBC_ActorModel Shanghai_ActorModel { get; set; }
        public GBA_Shanghai_ActionTableBlock Shanghai_ActionTable { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Shanghai_ActorModel = s.SerializeObject<GBC_ActorModel>(Shanghai_ActorModel, name: nameof(Shanghai_ActorModel));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Shanghai_ActionTable = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_Shanghai_ActionTableBlock>(Shanghai_ActionTable, name: nameof(Shanghai_ActionTable)));
        }
    }
}