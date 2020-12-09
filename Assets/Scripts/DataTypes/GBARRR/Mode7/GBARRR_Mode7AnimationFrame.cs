using System.Collections.Generic;

namespace R1Engine
{
    public class GBARRR_Mode7AnimationFrame : R1Serializable
    {
        public GBARRR_Mode7AnimationChannel[] Channels { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Channels == null) {
                var channelList = new List<GBARRR_Mode7AnimationChannel>();
                GBARRR_Mode7AnimationChannel lastChannel = null;
                while (lastChannel == null || !lastChannel.IsEndAttribute) {
                    lastChannel = s.SerializeObject<GBARRR_Mode7AnimationChannel>(default, name: $"{nameof(Channels)}[{channelList.Count}]");
                    channelList.Add(lastChannel);
                }
                Channels = channelList.ToArray();
            } else {
                Channels = s.SerializeObjectArray<GBARRR_Mode7AnimationChannel>(Channels, Channels.Length, name: nameof(Channels));
            }
        }
    }
}