namespace R1Engine
{
    public class GBC_ActorModel : GBC_BaseBlock
    {
        public byte ActorID { get; set; }
        public byte Anim0 { get; set; } // Some animation related thing. Flags?
        public byte PuppetLayersCount { get; set; }
        public sbyte RenderBoxY { get; set; }
        public sbyte RenderBoxX { get; set; }
        public byte RenderBoxHeight { get; set; }
        public byte RenderBoxWidth { get; set; }
        public byte[] Data { get; set; }

        public GBC_ActionTable ActionTable { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            ActorID = s.Serialize<byte>(ActorID, name: nameof(ActorID));
            Anim0 = s.Serialize<byte>(Anim0, name: nameof(Anim0));
            PuppetLayersCount = s.Serialize<byte>(PuppetLayersCount, name: nameof(PuppetLayersCount));
            RenderBoxY = s.Serialize<sbyte>(RenderBoxY, name: nameof(RenderBoxY));
            RenderBoxX = s.Serialize<sbyte>(RenderBoxX, name: nameof(RenderBoxX));
            RenderBoxHeight = s.Serialize<byte>(RenderBoxHeight, name: nameof(RenderBoxHeight));
            RenderBoxWidth = s.Serialize<byte>(RenderBoxWidth, name: nameof(RenderBoxWidth));
            Data = s.SerializeArray<byte>(Data, 4, name: nameof(Data));
            ActionTable = s.DoAt(DependencyTable.GetPointer(0), () => s.SerializeObject<GBC_ActionTable>(ActionTable, name: $"{nameof(ActionTable)}"));
        }
    }
}