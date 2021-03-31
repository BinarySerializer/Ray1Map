using BinarySerializer;

namespace R1Engine
{
    public class GBA_Shanghai_ActorsBlock : GBA_BaseBlock
    {
        public byte ActorModelsCount { get; set; }
        public ushort ActorsCount { get; set; }

        public GBC_GameObject[] Actors { get; set; }

        // Parsed from offsets
        public GBA_Shanghai_ActorModel[] ActorModels { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            ActorModelsCount = s.Serialize<byte>(ActorModelsCount, name: nameof(ActorModelsCount));
            ActorsCount = s.Serialize<ushort>(ActorsCount, name: nameof(ActorsCount));
            Actors = s.SerializeObjectArray<GBC_GameObject>(Actors, ActorsCount, name: nameof(Actors));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (ActorModels == null)
                ActorModels = new GBA_Shanghai_ActorModel[ActorModelsCount];

            for (int i = 0; i < ActorModels.Length; i++)
                ActorModels[i] = s.DoAt(OffsetTable.GetPointer(i), () => s.SerializeObject<GBA_Shanghai_ActorModel>(ActorModels[i], name: $"{nameof(ActorModels)}[{i}]"));

            // Set the actor models
            foreach (var a in Actors)
                a.GBA_ActorModel = ActorModels[a.Index_ActorModel];
        }
    }
}