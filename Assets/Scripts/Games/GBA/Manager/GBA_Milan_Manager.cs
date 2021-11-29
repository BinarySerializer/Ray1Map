using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ray1Map.GBA
{
    public class GBA_Milan_Manager : GBA_BatmanVengeance_Manager
    {
        public override Unity_ObjectManager GetObjectManager(Context context, GBA_Scene scene, GBA_Data data) => new Unity_ObjectManager_GBA(context, LoadActorModels(context, data.Milan_SceneList.Scene.ActorsBlock.Actors, data));

        public override IEnumerable<Unity_SpriteObject> GetObjects(Context context, GBA_Scene scene, Unity_ObjectManager objManager, GBA_Data data) => data.Milan_SceneList.Scene.ActorsBlock.Actors.Concat(data.Milan_SceneList.Scene.CaptorsBlock.Actors).Select(x => new Unity_Object_GBA(x, (Unity_ObjectManager_GBA)objManager, false));

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

        public override KeyValuePair<string, string[]>[] LoadLocalization(Context context)
        {
            var locTable = FileFactory.Read<GBA_ROM>(GetROMFilePath(context), context).Data.Milan_Localization;

            KeyValuePair<string, string[]>[] loc = null;

            if (locTable != null)
            {
                var lang = Milan_LocTableLanguages;

                loc = lang.Select((t, i) => new KeyValuePair<string, string[]>(t, locTable.Strings.Skip(i).Where((x, stringIndex) => stringIndex % lang.Length == 0).ToArray())).ToArray();
            }

            return loc;
        }

        public override async UniTask ExportSpritesAsync(GameSettings settings, string outputDir, bool exportAnimFrames)
        {
            try
            {
                var exported = new HashSet<Pointer>();

                // Enumerate every level
                for (int lev = 0; lev < LevelCount; lev++)
                {
                    Debug.Log($"Exporting level {lev + 1}/{LevelCount}");

                    settings.Level = lev;

                    using (var context = new Ray1MapContext(settings))
                    {
                        // Load the ROM
                        await LoadFilesAsync(context);

                        // Read the level
                        var gbaData = LoadDataBlock(context);
                        var scene = gbaData.Milan_SceneList.Scene;

                        // Enumerate every graphic group
                        await Controller.WaitFrame();

                        foreach (var model in scene.ActorsBlock.ActorModels)
                        {
                            foreach (var puppet in model.GetPuppets.Cast<GBA_Puppet>())
                            {
                                if (exported.Contains(puppet.Offset))
                                    continue;

                                exported.Add(puppet.Offset);

                                if (puppet.Milan_TileKit.TileSet4bppSize == 0 && puppet.Milan_TileKit.TileSet8bppSize == 0)
                                    continue;

                                if (exportAnimFrames)
                                    await ExportAnimations(puppet, Path.Combine(outputDir, $"{model.Milan_ActorID}_0x{puppet.Offset.StringAbsoluteOffset}"), puppet.Milan_TileKit.Is8bpp, gbaData);
                                else
                                    ExportSpriteTileSet(puppet, outputDir, puppet.Milan_TileKit.Is8bpp, -1);
                            }
                        }
                    }
                }

                Debug.Log("Finished export");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error: {ex}");
            }
        }
    }
}