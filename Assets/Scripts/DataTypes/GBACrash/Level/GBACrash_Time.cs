using System;

namespace R1Engine
{
    public class GBACrash_Time : R1Serializable
    {
        public int CentiSeconds { get; set; }
        public TimeSpan Time
        {
            get
            {
                var seconds = CentiSeconds / 10;
                var centiSeconds = CentiSeconds % 10;
                return new TimeSpan(0, 0, 0, seconds, centiSeconds * 10);
            }
        }

        public override void SerializeImpl(SerializerObject s)
        {
            CentiSeconds = s.Serialize<int>(CentiSeconds, name: nameof(CentiSeconds));
            s.Log($"Time: {Time:g}");
        }
    }
}