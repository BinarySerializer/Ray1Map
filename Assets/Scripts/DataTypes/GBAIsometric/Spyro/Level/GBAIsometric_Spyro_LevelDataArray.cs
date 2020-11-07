namespace R1Engine
{
    public class GBAIsometric_Spyro_LevelDataArray : R1Serializable
    {
        // Set before serializing
        public bool UesPointerArray { get; set; }
        public bool Is2D { get; set; }
        public int Length { get; set; }
        public int SerializeDataForID { get; set; }

        public GBAIsometric_Spyro_LevelData[] LevelData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (UesPointerArray)
            {
                var pointers = s.SerializePointerArray(default, Length, name: $"{nameof(LevelData)}Pointers");

                if (LevelData== null)
                    LevelData= new GBAIsometric_Spyro_LevelData[Length];

                for (int levelIndex = 0; levelIndex < LevelData.Length; levelIndex++)
                {
                    LevelData[levelIndex] = s.DoAt(pointers[levelIndex], () => s.SerializeObject<GBAIsometric_Spyro_LevelData>(LevelData[levelIndex], x =>
                    {
                        x.Is2D = Is2D;
                        x.SerializeDataForID = SerializeDataForID;
                    }, name: $"{nameof(LevelData)}[{levelIndex}]"));
                }
            }
            else
            {
                if (LevelData == null)
                    LevelData = new GBAIsometric_Spyro_LevelData[Length];

                for (int levelIndex = 0; levelIndex < LevelData.Length; levelIndex++)
                {
                    LevelData[levelIndex] = s.SerializeObject<GBAIsometric_Spyro_LevelData>(LevelData[levelIndex], x =>
                    {
                        x.Is2D = Is2D;
                        x.SerializeDataForID = SerializeDataForID;
                    }, name: $"{nameof(LevelData)}[{levelIndex}]");
                }
            }
        }
    }
}