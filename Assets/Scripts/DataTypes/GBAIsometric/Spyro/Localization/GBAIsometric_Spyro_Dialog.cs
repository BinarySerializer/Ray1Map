namespace R1Engine
{
    public class GBAIsometric_Spyro_Dialog : R1Serializable
    {
        public uint ID { get; set; }
        public Pointer DialogDataPointer { get; set; }

        public GBAIsometric_Spyro_DialogData DialogData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<uint>(ID, name: nameof(ID));
            DialogDataPointer = s.SerializePointer(DialogDataPointer, name: nameof(DialogDataPointer));

            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
                DialogData = s.DoAt(DialogDataPointer, () => s.SerializeObject<GBAIsometric_Spyro_DialogData>(DialogData, name: nameof(DialogData)));
        }
    }
}