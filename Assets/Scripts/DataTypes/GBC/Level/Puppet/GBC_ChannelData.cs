using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Contains all the data necessary to play one animation
    /// Better name: Animation
    /// </summary>
    public class GBC_ChannelData : GBC_BaseBlock {

        public ushort Count { get; set; }
        public GBC_RomChannel[] RomChannels { get; set; }

        public List<int> Temp_LayerSpriteCountState { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            s.DoEndian(R1Engine.Serialize.BinaryFile.Endian.Little, () => {
                Count = s.Serialize<ushort>(Count, name: nameof(Count));
            });
            if (RomChannels == null) {
                // To serialize the RomChannels, we need to keep track of the layer sprite count changes between them
                Temp_LayerSpriteCountState = new List<int>();
                RomChannels = s.SerializeObjectArray<GBC_RomChannel>(RomChannels, Count - 1, onPreSerialize: x => x.ChannelData = this, name: nameof(RomChannels));
                Temp_LayerSpriteCountState = null;// We serialized this now, so we can remove this list
            } else {
                RomChannels = s.SerializeObjectArray<GBC_RomChannel>(RomChannels, Count - 1, name: nameof(RomChannels));
            }
        }
    }
}