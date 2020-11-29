namespace R1Engine
{
    public class GBC_ActorModel : GBC_BaseBlock
    {
        public byte ActorModelByte00 { get; set; } // Important: Also at offset 0 in memory Actor struct
        public byte Anim0 { get; set; }
        public byte PuppetChannelCount { get; set; }
        public sbyte RenderBoxY { get; set; }
        public sbyte RenderBoxX { get; set; }
        public byte RenderBoxHeight { get; set; }
        public byte RenderBoxWidth { get; set; }
        public byte[] Data { get; set; }

        public GBC_ActionTable ActionTable { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            ActorModelByte00 = s.Serialize<byte>(ActorModelByte00, name: nameof(ActorModelByte00));
            Anim0 = s.Serialize<byte>(Anim0, name: nameof(Anim0));
            PuppetChannelCount = s.Serialize<byte>(PuppetChannelCount, name: nameof(PuppetChannelCount));
            RenderBoxY = s.Serialize<sbyte>(RenderBoxY, name: nameof(RenderBoxY));
            RenderBoxX = s.Serialize<sbyte>(RenderBoxX, name: nameof(RenderBoxX));
            RenderBoxHeight = s.Serialize<byte>(RenderBoxHeight, name: nameof(RenderBoxHeight));
            RenderBoxWidth = s.Serialize<byte>(RenderBoxWidth, name: nameof(RenderBoxWidth));
            Data = s.SerializeArray<byte>(Data, 4, name: nameof(Data));
            ActionTable = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_ActionTable>(ActionTable, name: $"{nameof(ActionTable)}"));
        }
    }
}