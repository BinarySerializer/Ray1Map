using System;

namespace R1Engine
{
    public class Palm_DateTime : R1Serializable
    {
        public uint Time { get; set; }

        public DateTime GetDateTime
        {
            get
            {
                var baseDate = new DateTime(1904, 1, 1);
                return baseDate.AddSeconds(Time); // TODO: This doesn't match the time it should be - why?
            }
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Time = s.Serialize<uint>(Time, name: nameof(Time));
            s.Log($"Time: {GetDateTime}");
        }
    }
}