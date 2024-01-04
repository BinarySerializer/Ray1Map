using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_Milan_ActorsBlock : GBA_BaseBlock
    {
        public bool IsCaptor { get; set; } // Set before serializing

        public uint ActorsCount { get; set; }
        public GBA_Actor[] Actors { get; set; }

        // Parsed from offsets
        public GBA_ActorModel[] ActorModels { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            s.Goto(ShanghaiOffsetTable.GetPointer(0));
            ActorsCount = s.Serialize<uint>(ActorsCount, name: nameof(ActorsCount));

            if (ShanghaiOffsetTable.Length > 1) {
                s.Goto(ShanghaiOffsetTable.GetPointer(1));
                Actors = s.SerializeObjectArray<GBA_Actor>(Actors, ActorsCount, x => x.Type = IsCaptor ? GBA_Actor.ActorType.Captor : GBA_Actor.ActorType.Actor, name: nameof(Actors));
            } else {
                Actors = new GBA_Actor[0];
            }
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (IsCaptor)
                return;

            if (ActorModels == null)
                ActorModels = new GBA_ActorModel[OffsetTable.OffsetsCount];

            for (int i = 0; i < ActorModels.Length; i++)
                ActorModels[i] = s.DoAt(OffsetTable.GetPointer(i), () => s.SerializeObject<GBA_ActorModel>(ActorModels[i], name: $"{nameof(ActorModels)}[{i}]"));

            // Set the actor models
            foreach (var a in Actors)
                a.ActorModel = ActorModels[a.Index_ActorModel];
        }

        public override long GetShanghaiOffsetTableLength => BlockSize == 8 ? 1 : 2;
    }
}