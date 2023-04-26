using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ray1Map.Rayman1
{
    public abstract class R1_BaseMultiplatformManager : R1_BaseManager
    {
        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new[]
            {
                new GameAction("Calculate Collectibles", false, false, (input, output) => CalculateCollectiblesCountAsync(settings)),
            };
        }

        public async UniTask CalculateCollectiblesCountAsync(GameSettings settings)
        {
            await LevelEditorData.InitAsync(settings);

            Dictionary<ObjType, int> counts = new()
            {
                [ObjType.TYPE_CAGE] = 0, // Cage
                [ObjType.TYPE_WIZARD1] = 0, // Magician
                [ObjType.TYPE_WIZ] = 0, // Ting
                [ObjType.TYPE_ONEUP] = 0, // Life
                [ObjType.TYPE_JAUGEUP] = 0, // Big power
                [ObjType.TYPE_POWERUP] = 0, // Small power
                [ObjType.TYPE_POING_POWERUP] = 0, // Fist power
                [ObjType.TYPE_SUPERHELICO] = 0, // Helicopter potion
            };

            foreach (GameInfo_Volume vol in GetLevels(settings))
            {
                foreach (GameInfo_World world in vol.Worlds)
                {
                    foreach (int map in world.Maps)
                    {
                        GameSettings newSettings = new(settings.GameModeSelection, settings.GameDirectory, world.Index, map)
                        {
                            EduVolume = vol.Name
                        };

                        using Context context = new Ray1MapContext(newSettings);

                        await LoadFilesAsync(context);

                        Unity_Level level = await LoadAsync(context);

                        foreach (ObjType type in counts.Keys.ToList())
                        {
                            counts[type] += level.EventData.OfType<Unity_Object_R1>().Count(x => x.EventData.Type == type &&
                                x.EventData.XPosition != -32000 &&
                                x.EventData.YPosition != -32000);
                        }

                        Debug.Log($"{newSettings.R1_World} {map}");
                    }
                }
            }

            foreach (ObjType type in counts.Keys)
                Debug.Log($"{type}: {counts[type]}");
        }
    }
}