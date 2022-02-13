using System;
using BinarySerializer;

namespace Ray1Map
{
    public class Palm_DateTime : BinarySerializable
    {
        public uint Time { get; set; }

        public DateTime GetDateTime
        {
            get
            {
                var baseDate = new DateTime(1904, 1, 1);
                return baseDate.AddSeconds(Time);
            }
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Time = s.Serialize<uint>(Time, name: nameof(Time));
            s.Log("Time: {0}", GetDateTime);
        }
    }
}