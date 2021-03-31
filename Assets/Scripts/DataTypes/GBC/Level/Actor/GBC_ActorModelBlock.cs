using BinarySerializer;

namespace R1Engine
{
    public class GBC_ActorModelBlock : GBC_BaseBlock
    {
        public GBC_ActorModel ActorModel { get; set; }
        public GBC_ActionTableBlock ActionTable { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            ActorModel = s.SerializeObject<GBC_ActorModel>(ActorModel, name: nameof(ActorModel));
            ActionTable = s.DoAt(DependencyTable.GetPointer(0), () => s.SerializeObject<GBC_ActionTableBlock>(ActionTable, name: $"{nameof(ActionTable)}"));
        }
    }
}