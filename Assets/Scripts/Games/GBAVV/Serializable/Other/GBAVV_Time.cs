using System;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Time : BinarySerializable
    {
        public int Value { get; set; }
        public TimeSpan Time
        {
            get
            {
                var v = Context.GetR1Settings().GBAVV_IsFusion ? 60 : 10;

                var seconds = Value / v;
                var centiSeconds = Value % v;
                return new TimeSpan(0, 0, 0, seconds, centiSeconds * v);
            }
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Value = s.Serialize<int>(Value, name: nameof(Value));
            s.Log("Time: {0:g}", Time);
        }
    }
}