namespace R1Engine
{
    public class GBC_Sector : R1Serializable
    {
        public byte ActorsCount { get; set; }
        public ushort[] Actors { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ActorsCount = s.Serialize<byte>(ActorsCount, name: nameof(ActorsCount));
            Actors = s.SerializeArray<ushort>(Actors, ActorsCount, name: nameof(Actors));
        }
    }
}