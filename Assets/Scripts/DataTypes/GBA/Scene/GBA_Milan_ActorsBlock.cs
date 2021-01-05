namespace R1Engine
{
    public class GBA_Milan_ActorsBlock : GBA_BaseBlock
    {
        public uint ActorsCount { get; set; }
        public GBA_Milan_Actor[] Actors { get; set; }

        // Parsed from offsets
        public GBA_Milan_ActorModel[] ActorModels { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            s.Goto(ShanghaiOffsetTable.GetPointer(0));
            ActorsCount = s.Serialize<uint>(ActorsCount, name: nameof(ActorsCount));

            s.Goto(ShanghaiOffsetTable.GetPointer(1));
            Actors = s.SerializeObjectArray<GBA_Milan_Actor>(Actors, ActorsCount, name: nameof(Actors));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (ActorModels == null)
                ActorModels = new GBA_Milan_ActorModel[OffsetTable.OffsetsCount];

            for (int i = 0; i < ActorModels.Length; i++)
                ActorModels[i] = s.DoAt(OffsetTable.GetPointer(i), () => s.SerializeObject<GBA_Milan_ActorModel>(ActorModels[i], name: $"{nameof(ActorModels)}[{i}]"));
        }

        public override long GetShanghaiOffsetTableLength => 2;
    }
}