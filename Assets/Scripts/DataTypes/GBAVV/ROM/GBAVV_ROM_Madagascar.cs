namespace R1Engine
{
    public class GBAVV_ROM_Madagascar : GBAVV_BaseROM
    {
        public GBAVV_Madagascar_Manager.LevInfo CurrentLevInfo { get; set; } // Set before serializing

        // Helpers
        public GBAVV_Map CurrentMap => Volumes[CurrentLevInfo.Volume].LevelInfos[CurrentLevInfo.Level].MapInfos[CurrentLevInfo.Map].Map;

        // Common
        public GBAVV_Madagascar_Volume[] Volumes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.GBAVV_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Serialize level infos
            s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.LevelInfo), () =>
            {
                if (Volumes == null)
                    Volumes = new GBAVV_Madagascar_Volume[2];

                for (int i = 0; i < Volumes.Length; i++)
                    Volumes[i] = s.SerializeObject<GBAVV_Madagascar_Volume>(Volumes[i], x => x.CurrentLevInfo = i == CurrentLevInfo.Volume ? CurrentLevInfo : null, name: $"{nameof(Volumes)}[{i}]");
            });

            // Serialize graphics
            SerializeGraphics(s);

            // Serialize scripts
            SerializeScripts(s);
        }
    }
}