using System.Linq;

namespace R1Engine
{
    public class GBC_ChannelData : GBC_BaseBlock
    {
        public GBC_ChannelEventInstruction.LayerInfo[][] AnimLayerInfos { get; set; }

        public GBC_BaseChannelEvent CountEvent { get; set; }
        public GBC_ChannelEvent HeaderEvent { get; set; }
        public GBC_ChannelEvent[] Events { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            CountEvent = s.SerializeObject<GBC_BaseChannelEvent>(CountEvent, name: nameof(CountEvent));
            HeaderEvent = s.SerializeObject<GBC_ChannelEvent>(HeaderEvent, name: nameof(HeaderEvent));

            // Get the layer infos
            AnimLayerInfos = HeaderEvent.Instructions.Where(x => x.Command == GBC_ChannelEventInstruction.InstructionCommand.SetLayerInfos).Select(x => x.LayerInfos).ToArray();

            Events = s.SerializeObjectArray<GBC_ChannelEvent>(Events, CountEvent.Byte_00 - 2, onPreSerialize: x => x.AnimLayerInfos = AnimLayerInfos, name: nameof(Events));
        }
    }
}