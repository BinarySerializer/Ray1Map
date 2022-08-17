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
                new GameAction("Calculate Tings", false, false, (input, output) => CalculateTingsCountAsync(settings)),
            };
        }

        public async UniTask CalculateTingsCountAsync(GameSettings settings)
        {
            await LevelEditorData.InitAsync(settings);

            int count = 0;

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

                        count += level.EventData.OfType<Unity_Object_R1>().Count(x => x.EventData.Type == ObjType.TYPE_WIZ &&
                            x.EventData.XPosition != -32000 &&
                            x.EventData.YPosition != -32000);

                        Debug.Log($"{newSettings.R1_World} {map}");
                    }
                }
            }

            Debug.Log($"Tings: {count}");
        }
    }
}