using BinarySerializer;

namespace Ray1Map.Jade
{
    public class EVE_Event_MagicKey : BinarySerializable
    {
        public uint Type { get; set; }
        public float Focale { get; set; }
        public float LightNear { get; set; }
        public float LightFar { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Type = s.Serialize<uint>(Type, name: nameof(Type));
            Focale = s.Serialize<float>(Focale, name: nameof(Focale));
            LightNear = s.Serialize<float>(LightNear, name: nameof(LightNear));
            LightFar = s.Serialize<float>(LightFar, name: nameof(LightFar));
        }
    }
}