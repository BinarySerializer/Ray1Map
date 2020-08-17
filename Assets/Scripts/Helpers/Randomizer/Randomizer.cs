using System;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The Rayman 1 event randomizer
    /// </summary>
    public static class Randomizer
    {
        /// <summary>
        /// Randomizes the events in a level based on the flags
        /// </summary>
        /// <param name="editorManager">The level editor manager</param>
        /// <param name="flags">The flags</param>
        /// <param name="seed">An optional seed to use</param>
        /// <param name="map">The map index</param>
        public static void Randomize(BaseEditorManager editorManager, RandomizerFlags flags, int? seed, int map)
        {
            var random = seed != null ? new Random(seed.Value) : new Random();
            var maxX = editorManager.Level.Maps[map].Width * Settings.CellSize;
            var maxY = editorManager.Level.Maps[map].Height * Settings.CellSize;

            // Enumerate every event
            foreach (Unity_Obj eventData in editorManager.Level.EventData
                .Select(eventData => new
                {
                    eventData, 
                    isAlways = eventData.GetIsAlways(),
                    isEditor = eventData.GetIsEditor()
                })
                .Where(x => !x.isAlways && !x.isEditor)
                .Where(x => (R1_EventType)x.eventData.Type != R1_EventType.TYPE_RAY_POS &&
                            (R1_EventType)x.eventData.Type != R1_EventType.TYPE_PANCARTE &&
                            (R1_EventType)x.eventData.Type != R1_EventType.TYPE_SIGNPOST)
                .Select(x => x.eventData))
            {
                if (flags.HasFlag(RandomizerFlags.Pos))
                {
                    eventData.Data.XPosition = (short)random.Next(0, maxX);
                    eventData.Data.YPosition = (short)random.Next(0, maxY);
                }

                if (flags.HasFlag(RandomizerFlags.Des))
                    eventData.DESKey = editorManager.DES.ElementAt(random.Next(0, editorManager.DES.Count - 1)).Key;

                if (flags.HasFlag(RandomizerFlags.Eta))
                    eventData.ETAKey = editorManager.ETA.ElementAt(random.Next(0, editorManager.ETA.Count - 1)).Key;

                if (flags.HasFlag(RandomizerFlags.CommandOrder))
                {
                    int n = eventData.CommandCollection.Commands.Length - 1;

                    while (n > 1)
                    {
                        n--;
                        int k = random.Next(n + 1);
                        var value = eventData.CommandCollection.Commands[k];
                        eventData.CommandCollection.Commands[k] = eventData.CommandCollection.Commands[n];
                        eventData.CommandCollection.Commands[n] = value;
                    }
                }

                if (flags.HasFlag(RandomizerFlags.Follow))
                {
                    eventData.Data.SetFollowEnabled(editorManager.Settings, random.Next(0, 1) == 1);
                    eventData.Data.OffsetHY = (byte)random.Next(0, 10);
                }

                if (flags.HasFlag(RandomizerFlags.States))
                {
                    eventData.Data.Etat = (byte)random.Next(0, editorManager.ETA[eventData.ETAKey].Length - 1);
                    eventData.Data.SubEtat = (byte)random.Next(0, editorManager.ETA[eventData.ETAKey][eventData.Data.Etat].Length - 1);
                }

                if (flags.HasFlag(RandomizerFlags.Type))
                    eventData.Type = (R1_EventType)random.Next(0, 255);
            }
        }
    }
}