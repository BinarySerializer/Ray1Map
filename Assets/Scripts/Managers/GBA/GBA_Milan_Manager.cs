using R1Engine.Serialize;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBA_Milan_Manager : GBA_BatmanVengeance_Manager
    {
        public override Unity_ObjectManager GetObjectManager(Context context, GBA_Scene scene, GBA_Data data) => new Unity_ObjectManager_GBA(context, LoadActorModels(context, data.Milan_SceneList.Scene.ActorsBlock.Actors, data));

        public override IEnumerable<Unity_Object> GetObjects(Context context, GBA_Scene scene, Unity_ObjectManager objManager, GBA_Data data) => data.Milan_SceneList.Scene.ActorsBlock.Actors.Concat(data.Milan_SceneList.Scene.CaptorsBlock.Actors).Select(x => new Unity_Object_GBA(x, (Unity_ObjectManager_GBA)objManager));

        public override Unity_Sector[] GetSectors(GBA_Scene scene, GBA_Data data) => null;

        protected override BaseColor[] GetSpritePalette(GBA_BatmanVengeance_Puppet puppet, GBA_Data data) => null;

        public virtual long Milan_LocTableLength => 0;
        public virtual long Milan_LocTableLangCount => 5;
        public virtual string[] Milan_LocTableLanguages => new string[]
        {
            "English",
            "French",
            "German",
            "Spanish",
            "Italian",
        };

        public override Dictionary<string, string[]> LoadLocalization(Context context)
        {
            var locTable = FileFactory.Read<GBA_ROM>(GetROMFilePath(context), context).Milan_Localization;

            Dictionary<string, string[]> loc = null;

            if (locTable != null)
            {
                loc = new Dictionary<string, string[]>();

                var lang = Milan_LocTableLanguages;

                for (int i = 0; i < lang.Length; i++)
                    loc.Add(lang[i], locTable.Strings.Skip(i).Where((x, stringIndex) => stringIndex % lang.Length == 0).ToArray());
            }

            return loc;
        }
    }
}