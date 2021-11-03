using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBC
{
    /// <summary>
    /// Contains all the data necessary to play one animation
    /// Better name: Animation
    /// </summary>
    public class GBC_RomChannel : GBC_BaseBlock {

        public ushort Count { get; set; }
        public GBC_Keyframe[] Keyframes { get; set; }

        public List<int> Temp_LayerSpriteCountState { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            s.DoEndian(Endian.Little, () => {
                Count = s.Serialize<ushort>(Count, name: nameof(Count));
            });
            if (Keyframes == null) {
                // To serialize the keyframes, we need to keep track of the layer sprite count changes between them
                Temp_LayerSpriteCountState = new List<int>();
                Keyframes = s.SerializeObjectArray<GBC_Keyframe>(Keyframes, Count - 1, onPreSerialize: x => x.ChannelData = this, name: nameof(Keyframes));
                Temp_LayerSpriteCountState = null;// We serialized this now, so we can remove this list
            } else {
                Keyframes = s.SerializeObjectArray<GBC_Keyframe>(Keyframes, Count - 1, name: nameof(Keyframes));
            }
        }
    }
}