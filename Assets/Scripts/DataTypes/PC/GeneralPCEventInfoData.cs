using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// General event information for PC
    /// </summary>
    public class GeneralEventInfoData
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mapperId"></param>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <param name="etat"></param>
        /// <param name="subEtat"></param>
        /// <param name="flag"></param>
        /// <param name="desR1"></param>
        /// <param name="etaR1"></param>
        /// <param name="desKit"></param>
        /// <param name="etaKit"></param>
        /// <param name="offsetBx"></param>
        /// <param name="offsetBy"></param>
        /// <param name="offsetHy"></param>
        /// <param name="followSprite"></param>
        /// <param name="hitPoints"></param>
        /// <param name="hitSprite"></param>
        /// <param name="followEnabled"></param>
        /// <param name="layer"></param>
        /// <param name="connectedEvents"></param>
        /// <param name="labelOffsets"></param>
        /// <param name="commands"></param>
        /// <param name="localCommands"></param>
        public GeneralEventInfoData(string name, string mapperId, int type, string typeName, int etat, int subEtat, EventFlag? flag, IDictionary<World, int?> desR1, IDictionary<World, int?> etaR1, IDictionary<World, string> desKit, IDictionary<World, string> etaKit, int offsetBx, int offsetBy, int offsetHy, int followSprite, int hitPoints, int hitSprite, bool followEnabled, int layer, string[] connectedEvents, ushort[] labelOffsets, byte[] commands, byte[] localCommands)
        {
            Name = name;
            MapperID = mapperId;
            Type = type;
            TypeName = typeName;
            Etat = etat;
            SubEtat = subEtat;
            Flag = flag;
            DesR1 = new ReadOnlyDictionary<World, int?>(desR1);
            EtaR1 = new ReadOnlyDictionary<World, int?>(etaR1);
            DesKit = new ReadOnlyDictionary<World, string>(desKit);
            EtaKit = new ReadOnlyDictionary<World, string>(etaKit);
            OffsetBX = offsetBx;
            OffsetBY = offsetBy;
            OffsetHY = offsetHy;
            FollowSprite = followSprite;
            HitPoints = hitPoints;
            HitSprite = hitSprite;
            FollowEnabled = followEnabled;
            Layer = layer;
            ConnectedEvents = connectedEvents;
            LabelOffsets = labelOffsets;
            Commands = commands;
            LocalCommands = localCommands;
        }

        #endregion

        #region Public Properties

        public string Name { get; }

        public string MapperID { get; }
        
        public int Type { get; }

        public string TypeName { get; }

        public int Etat { get; }

        public int SubEtat { get; }

        public EventFlag? Flag { get; }

        public IReadOnlyDictionary<World, int?> DesR1 { get; }

        public IReadOnlyDictionary<World, int?> EtaR1 { get; }

        public IReadOnlyDictionary<World, string> DesKit { get; }

        public IReadOnlyDictionary<World, string> EtaKit { get; }

        public int OffsetBX { get; }

        public int OffsetBY { get; }

        public int OffsetHY { get; }

        public int FollowSprite { get; }

        public int HitPoints { get; }

        public int HitSprite { get; }

        public bool FollowEnabled { get; }

        public int Layer { get; }

        public string[] ConnectedEvents { get; }

        public ushort[] LabelOffsets { get; }

        public byte[] Commands { get; }

        public byte[] LocalCommands { get; }

        #endregion

        #region Static Methods

        /// <summary>
        /// Reads the event info data from a .csv file
        /// </summary>
        /// <param name="fileStream">The file stream to read from</param>
        /// <returns>The read data</returns>
        public static IList<GeneralEventInfoData> ReadCSV(Stream fileStream)
        {
            // Use a reader
            using (var reader = new StreamReader(fileStream))
            {
                // Create the output
                var output = new List<GeneralEventInfoData>();

                // Skip header
                reader.ReadLine();

                // Read every line
                while (!reader.EndOfStream)
                {
                    // Read the line
                    var line = reader.ReadLine()?.Split(',');

                    // Make sure we read something
                    if (line == null)
                        break;

                    // Keep track of the value index
                    var index = 0;

                    try
                    {
                        // Helper methods for parsing values
                        string nextValue() => line[index++];
                        bool nextBoolValue() => Boolean.Parse(line[index++]);
                        int nextIntValue() => Int32.Parse(nextValue());
                        T? nextEnumValue<T>() where T : struct => Enum.TryParse(nextValue(), out T parsedEnum) ? (T?)parsedEnum : null;
                        ushort[] next16ArrayValue() => nextValue().Split('-').Where(x => !String.IsNullOrWhiteSpace(x)).Select(UInt16.Parse).ToArray();
                        int?[] next32NullableArrayValue() => nextValue().Split('-').Select(x => String.IsNullOrWhiteSpace(x) ? null : (int?)Int32.Parse(x)).ToArray();
                        byte[] next8ArrayValue() => nextValue().Split('-').Where(x => !String.IsNullOrWhiteSpace(x)).Select(Byte.Parse).ToArray();
                        string[] nextStringArrayValue() => nextValue().Split('-').ToArray();

                        IDictionary<World, T> toDictionary<T>(IList<T> values)
                        {
                            var dict = EnumHelpers.GetValues<World>().ToDictionary(x => x, x => default(T));

                            for (int i = 0; i < values.Count; i++)
                                dict[(World)i] = values[i];

                            return dict;
                        }

                        // Add the item to the output
                        output.Add(new GeneralEventInfoData(nextValue(), nextValue(), nextIntValue(), nextValue(), nextIntValue(), nextIntValue(), nextEnumValue<EventFlag>(), toDictionary(next32NullableArrayValue()), toDictionary(next32NullableArrayValue()), toDictionary(nextStringArrayValue()), toDictionary(nextStringArrayValue()), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextIntValue(), nextBoolValue(), nextIntValue(), nextStringArrayValue(), next16ArrayValue(), next8ArrayValue(), next8ArrayValue()));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to parse event info. Index: {index}, items: {String.Join(" - ", line)} , exception: {ex.Message}");
                        throw;
                    }
                }

                // Return the output
                return output.OrderBy(x => x.Name).ThenBy(x => x.Type).ToArray();
            }
        }

        /// <summary>
        /// Writes the event info data to a .csv file
        /// </summary>
        /// <param name="fileStream">The file stream to write to</param>
        /// <param name="eventInfoDatas">The data to write</param>
        public static void WriteCSV(Stream fileStream, IEnumerable<GeneralEventInfoData> eventInfoDatas)
        {
            using (var writer = new StreamWriter(fileStream))
            {
                // Helper method for writing a new line
                void WriteLine(params object[] values)
                {
                    foreach (var value in values)
                    {
                        var toWrite = value?.ToString();

                        if (value is IDictionary dict)
                        {
                            toWrite = dict.Values.Cast<object>().Aggregate(String.Empty, (current, o) =>
                            {
                                const string separator = "-";

                                return current + $"{separator}{o}";
                            });

                            if (toWrite.Length > 1)
                                toWrite = toWrite.Remove(0, 1);
                        }
                        else if (value is IEnumerable enu && !(enu is string))
                        {
                            toWrite = enu.Cast<object>().Aggregate(String.Empty, (current, o) =>
                            {
                                const string separator = "-";

                                return current + $"{separator}{o}";
                            });

                            if (toWrite.Length > 1)
                                toWrite = toWrite.Remove(0, 1);
                        }

                        toWrite = toWrite?.Replace(",", " -");

                        writer.Write($"{toWrite},");
                    }

                    writer.Flush();

                    fileStream.Position--;

                    writer.Write(Environment.NewLine);
                }

                // Write header
                WriteLine("Name", "MapperID", "Type", "TypeName", "Etat", "SubEtat", "Flag", "DesR1", "EtaR1", "DesKit", "EtaKit", "OffsetBX", "OffsetBY", "OffsetHY", "FollowSprite", "HitPoints", "HitSprite", "FollowEnabled", "Layer", "ConnectedEvents", "LabelOffsets", "Commands", "LocalCommands");

                // Write every item on a new line
                foreach (var e in eventInfoDatas.OrderBy(x => x.Type).ThenBy(x => x.Etat).ThenBy(x => x.SubEtat))
                {
                    WriteLine(e.Name, e.MapperID, e.Type, e.TypeName, e.Etat, e.SubEtat, e.Flag, e.DesR1, e.EtaR1, e.DesKit, e.EtaKit, e.OffsetBX, e.OffsetBY, e.OffsetHY, e.FollowSprite, e.HitPoints, e.HitSprite, e.FollowEnabled, e.Layer, e.ConnectedEvents, e.LabelOffsets, e.Commands, e.LocalCommands);
                }
            }
        }

        #endregion
    }
}