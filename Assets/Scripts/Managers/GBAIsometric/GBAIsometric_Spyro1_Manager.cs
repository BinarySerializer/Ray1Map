using System.Collections.Generic;

namespace R1Engine
{
    public class GBAIsometric_Spyro1_Manager : GBAIsometric_Spyro_Manager
    {
        public override int DataTableCount => 83;

        public override IEnumerable<string> GetLanguages(GameModeSelection gameModeSelection)
        {
            yield return "English";

            //if (gameModeSelection == GameModeSelection.)
            //{
            //    // TODO: Other languages
            //}
        }

        public override LevelInfo[] LevelInfos => new LevelInfo[0];
    }
}