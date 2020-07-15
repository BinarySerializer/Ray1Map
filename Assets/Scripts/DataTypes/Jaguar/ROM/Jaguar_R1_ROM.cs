using System;
using System.Collections.Generic;
using R1Engine.Serialize;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// ROM data for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_ROM : R1Serializable
    {
        #region Prototype Reference Data

        public Jaguar_ReferenceEntry[] References { get; set; }
        public uint UnkReferenceValue { get; set; }

        #endregion

        #region Global Data

        public Pointer[] WorldLoadCommandPointers { get; set; }
        public Pointer[][] LevelLoadCommandPointers { get; set; }

        #endregion

        #region Global Data (Parsed)

        public Jaguar_R1_LevelLoadCommandCollection AllfixLoadCommands { get; set; }
        public Jaguar_R1_LevelLoadCommandCollection[] WorldLoadCommands { get; set; }
        public Jaguar_R1_LevelLoadCommandCollection[][] LevelLoadCommands { get; set; }

        #endregion

        #region Map Data

        /// <summary>
        /// The map data for the current level
        /// </summary>
        public MapData MapData { get; set; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        public Jaguar_R1_EventBlock EventData { get; set; }

        /// <summary>
        /// The current sprite palette
        /// </summary>
        public RGB556Color[] SpritePalette { get; set; }

        /// <summary>
        /// The current tileset data
        /// </summary>
        public RGB556Color[] TileData { get; set; }

        public Jaguar_R1_EventDefinition[] EventDefinitions { get; set; }
        public Jaguar_R1_EventDefinition[] AdditionalEventDefinitions { get; set; }

        /// <summary>
        /// The image buffers for the current level, with the key being the memory pointer pointer
        /// </summary>
        public Dictionary<uint, byte[]> ImageBuffers { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a data block pointer for the Jaguar prototype
        /// </summary>
        /// <param name="reference">The reference type</param>
        /// <returns>The pointer</returns>
        public Pointer GetProtoDataPointer(Jaguar_R1Proto_References reference)
        {
            var s = reference.ToString();
            return References.First(x => x.String.Replace(".", "__") == s).DataPointer;
        }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get info
            var pointerTable = s.GameSettings.EngineVersion != EngineVersion.RayJaguarProto ? PointerTables.GetJaguarPointerTable(s.GameSettings.EngineVersion, this.Offset.file) : null;
            var manager = (Jaguar_R1_Manager)s.GameSettings.GetGameManager;
            var levels = manager.GetNumLevels;

            // Serialize the references for the prototype
            if (s.GameSettings.EngineVersion == EngineVersion.RayJaguarProto)
            {
                s.DoAt(new Pointer(0x8BB6A8, Offset.file), () =>
                {
                    References = s.SerializeObjectArray<Jaguar_ReferenceEntry>(References, 1676, onPreSerialize: (x => x.StringBase = new Pointer(0x8C0538, Offset.file)), name: nameof(References));

                    // Unknown initial 4 bytes, part of the string table
                    UnkReferenceValue = s.Serialize<uint>(UnkReferenceValue, name: nameof(UnkReferenceValue));
                });
            }

            // Serialize event definition data
            if (s.GameSettings.EngineVersion == EngineVersion.RayJaguar)
            {
                if (!s.Context.FileExists("RAM_EventDefinitions"))
                {
                    // Copied to 0x001f9000 in memory. All pointers to 0x001Fxxxx likely point to an entry in this table
                    s.DoAt(pointerTable[Jaguar_R1_Pointer.EventDefinitions], () =>
                    {
                        byte[] EventDefsDataBytes = s.SerializeArray<byte>(null, manager.EventCount * 0x28,
                            name: nameof(EventDefsDataBytes));
                        var file = new MemoryMappedByteArrayFile("RAM_EventDefinitions", EventDefsDataBytes, s.Context,
                            0x001f9000)
                        {
                            Endianness = BinaryFile.Endian.Big
                        };
                        s.Context.AddFile(file);
                        s.DoAt(file.StartPointer,
                            () => EventDefinitions = s.SerializeObjectArray<Jaguar_R1_EventDefinition>(EventDefinitions,
                                manager.EventCount, name: nameof(EventDefinitions)));
                    });
                }
            }
            else
            {
                // TODO: Remove this - it's only here to avoid a crash temporarily
                if (s.GameSettings.EngineVersion != EngineVersion.RayJaguarProto)
                {
                    var offset = s.GameSettings.EngineVersion == EngineVersion.RayJaguarProto ? GetProtoDataPointer(Jaguar_R1Proto_References.MS_rayman) : pointerTable[Jaguar_R1_Pointer.EventDefinitions];

                    // Pointers all point to the ROM, not RAM
                    s.DoAt(offset, () => EventDefinitions = s.SerializeObjectArray<Jaguar_R1_EventDefinition>(EventDefinitions,
                        manager.EventCount, name: nameof(EventDefinitions)));
                }
            }

            if (AdditionalEventDefinitions == null) 
            {
                if (s.GameSettings.EngineVersion != EngineVersion.RayJaguarProto)
                {
                    AdditionalEventDefinitions = manager.AdditionalEventDefinitionPointers.Select(p =>
                    {
                        return s.DoAt(new Pointer(p, pointerTable[Jaguar_R1_Pointer.EventDefinitions].file), () => s.SerializeObject<Jaguar_R1_EventDefinition>(default, name: nameof(AdditionalEventDefinitions)));
                    }).ToArray();
                }
                else
                {
                    // TODO: This doesn't seem fully correct
                    //AdditionalEventDefinitions = References.Where(x => x.String.StartsWith("obj_")).Select(x => s.DoAt(x.DataPointer, () => s.SerializeObject<Jaguar_R1_EventDefinition>(default, name: nameof(AdditionalEventDefinitions)))).ToArray();
                }
            }

            if (s.GameSettings.EngineVersion != EngineVersion.RayJaguarProto)
            {
                // Serialize allfix sprite data
                s.DoAt(pointerTable[Jaguar_R1_Pointer.FixSprites], () => AllfixLoadCommands = s.SerializeObject<Jaguar_R1_LevelLoadCommandCollection>(AllfixLoadCommands, name: nameof(AllfixLoadCommands)));

                // Serialize world sprite data
                s.DoAt(pointerTable[Jaguar_R1_Pointer.WorldSprites], () =>
                {
                    if (WorldLoadCommandPointers == null)
                        WorldLoadCommandPointers = new Pointer[6];

                    if (WorldLoadCommands == null)
                        WorldLoadCommands = new Jaguar_R1_LevelLoadCommandCollection[6];

                    for (int i = 0; i < 6; i++)
                    {
                        Pointer FunctionPointer = s.SerializePointer(null, name: nameof(FunctionPointer));
                        s.DoAt(FunctionPointer, () => {
                            // Parse assembly
                            ushort instruction = s.Serialize<ushort>(0x41f9, name: nameof(instruction)); // Load effective address
                            WorldLoadCommandPointers[i] = s.SerializePointer(WorldLoadCommandPointers[i], name: $"{nameof(WorldLoadCommandPointers)}[{i}]");
                            s.DoAt(WorldLoadCommandPointers[i], () => {
                                WorldLoadCommands[i] = s.SerializeObject<Jaguar_R1_LevelLoadCommandCollection>(WorldLoadCommands[i], name: $"{nameof(WorldLoadCommands)}[{i}]");
                            });
                        });
                    }
                });

                s.DoAt(pointerTable[Jaguar_R1_Pointer.MapData], () =>
                {
                    if (LevelLoadCommandPointers == null)
                        LevelLoadCommandPointers = new Pointer[7][];

                    if (LevelLoadCommands == null)
                        LevelLoadCommands = new Jaguar_R1_LevelLoadCommandCollection[7][];

                    // Serialize map data for each world (6 + extra)
                    for (int i = 0; i < 7; i++)
                    {
                        // Get the number of levels in the world
                        int numLevels = i < 6 ? levels[i].Value : manager.ExtraMapCommands.Length;

                        if (LevelLoadCommandPointers[i] == null)
                            LevelLoadCommandPointers[i] = new Pointer[numLevels];

                        if (LevelLoadCommands[i] == null)
                            LevelLoadCommands[i] = new Jaguar_R1_LevelLoadCommandCollection[numLevels];

                        // Get the levels to serialize for the extra map commands
                        int[] extraMapCommands = i == 6 ? manager.ExtraMapCommands : null;

                        // Serialize the pointer to the load function pointer list
                        Pointer FunctionListPointer = s.SerializePointer(null, name: nameof(FunctionListPointer));

                        // Parse the load functions
                        s.DoAt(FunctionListPointer, () =>
                        {
                            // Serialize every level load function
                            for (int j = 0; j < numLevels; j++)
                            {
                                // If the special world, go to the valid map
                                if (i == 6)
                                    s.Goto(FunctionListPointer + 4 * extraMapCommands[j]);

                                // Serialize the pointer to the function
                                Pointer FunctionPointer = s.SerializePointer(null, name: nameof(FunctionPointer));

                                s.DoAt(FunctionPointer, () => {
                                    // Parse assembly
                                    ushort instruction = s.Serialize<ushort>(0x41f9, name: nameof(instruction)); // Load effective address
                                    while (instruction != 0x41f9)
                                    {
                                        switch (instruction)
                                        {
                                            case 0x23fc: // Move (q)
                                                s.Serialize<uint>(0, name: "MoveArg0"); // arguments differ for each level
                                                s.Serialize<uint>(0, name: "MoveArg1");
                                                break;
                                            case 0x33fc: // Move (w)
                                                s.Serialize<ushort>(0, name: "MoveArg0"); // arguments differ for each level
                                                s.Serialize<ushort>(0, name: "MoveArg1");
                                                break;
                                            case 0x0839: // btst (b)
                                                s.SerializeArray<byte>(null, 6, name: "BtstArgs");
                                                break;
                                            case 0x5279: // addq (w)
                                                s.Serialize<uint>(0, name: "AddqArg0");
                                                break;
                                        }
                                        instruction = s.Serialize<ushort>(0x41f9, name: nameof(instruction)); // Load effective address
                                    }

                                    // Serialize the load command pointer
                                    LevelLoadCommandPointers[i][j] = s.SerializePointer(LevelLoadCommandPointers[i][j], allowInvalid: true, name: $"{nameof(LevelLoadCommandPointers)}[{i}][{j}]");

                                    // Serialize the load commands
                                    s.DoAt(LevelLoadCommandPointers[i][j], () => {
                                        LevelLoadCommands[i][j] = s.SerializeObject<Jaguar_R1_LevelLoadCommandCollection>(LevelLoadCommands[i][j], name: $"{nameof(LevelLoadCommands)}[{i}][{j}]");
                                    });
                                });
                            }
                        });
                    }
                });

                // Get the current map load commands
                var wldCommands = WorldLoadCommands[levels.FindItemIndex(x => x.Key == s.GameSettings.World)];
                var mapCommands = LevelLoadCommands[levels.FindItemIndex(x => x.Key == s.GameSettings.World)][s.GameSettings.Level - 1].Commands;

                // Get pointers
                var mapPointer = mapCommands.FindItem(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.LevelMap).LevelMapBlockPointer;
                var eventPointer = mapCommands.FindItem(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.LevelMap).LevelEventBlockPointer;
                var palPointer = mapCommands.FindItem(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Palette || x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.PaletteDemo).PalettePointer;

                Pointer tilesPointer;

                if (s.GameSettings.EngineVersion == EngineVersion.RayJaguar)
                    tilesPointer = mapCommands.LastOrDefault(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Graphics && x.ImageBufferMemoryPointer == 0x001B3B68)?.ImageBufferPointer ?? WorldLoadCommands[levels.FindItemIndex(x => x.Key == s.GameSettings.World)].Commands.First(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Graphics && x.ImageBufferMemoryPointer == 0x001B3B68).ImageBufferPointer;
                else
                    tilesPointer = WorldLoadCommands[levels.FindItemIndex(x => x.Key == s.GameSettings.World)].Commands.Last(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Graphics && x.ImageBufferMemoryPointer == 0x001BD800).ImageBufferPointer;

                // Serialize map and event data
                s.DoAt(mapPointer, () => s.DoEncoded(new RNCEncoder(), () => MapData = s.SerializeObject<MapData>(MapData, name: nameof(MapData))));

                if (s.GameSettings.EngineVersion == EngineVersion.RayJaguar)
                    s.DoAt(eventPointer, () => s.DoEncoded(new RNCEncoder(), () => EventData = s.SerializeObject<Jaguar_R1_EventBlock>(EventData, name: nameof(EventData))));
                else if (s.GameSettings.EngineVersion == EngineVersion.RayJaguarDemo)
                    s.DoAt(eventPointer, () => EventData = s.SerializeObject<Jaguar_R1_EventBlock>(EventData, name: nameof(EventData)));

                // Serialize sprite palette
                s.DoAt<RGB556Color[]>(palPointer, () => SpritePalette = s.SerializeObjectArray<RGB556Color>(SpritePalette, 256, name: nameof(SpritePalette)));

                // Serialize tile data
                s.DoAt(tilesPointer, () => s.DoEncoded(new RNCEncoder(), () => TileData = s.SerializeObjectArray<RGB556Color>(TileData, s.CurrentLength / 2, name: nameof(TileData))));

                // Serialize image buffers
                if (ImageBuffers == null)
                {
                    ImageBuffers = new Dictionary<uint, byte[]>();

                    var index = 0;
                    foreach (var cmd in AllfixLoadCommands.Commands.Concat(wldCommands.Commands).Concat(mapCommands).Where(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Sprites))
                    {
                        // Later commands overwrite previous ones
                        /*if (ImageBuffers.ContainsKey(cmd.ImageBufferMemoryPointerPointer))
                            continue;
                        */

                        // Temp fix for certain demo buffers
                        try
                        {
                            s.DoAt(cmd.ImageBufferPointer, () => s.DoEncoded(new RNCEncoder(), () =>
                            {
                                ImageBuffers[cmd.ImageBufferMemoryPointerPointer] = s.SerializeArray<byte>(default, s.CurrentLength, $"ImageBuffer[{index}]");
                            }));
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Failed to serialize image buffer at {cmd.ImageBufferMemoryPointerPointer} with error {ex.Message}");
                            ImageBuffers[cmd.ImageBufferMemoryPointerPointer] = new byte[0];
                        }
                        index++;
                    }
                }
            }
            else
            {
                // TODO: Parse load commands at ml_jun

                // NOTE: We can also get these pointers from the load commands

                // Palette
                s.DoAt(GetProtoDataPointer(Jaguar_R1Proto_References.coltable), () => SpritePalette = s.SerializeObjectArray<RGB556Color>(SpritePalette, 256, name: nameof(SpritePalette)));
                
                // Map
                s.DoAt(GetProtoDataPointer(Jaguar_R1Proto_References.jun_map), () => MapData = s.SerializeObject<MapData>(MapData, name: nameof(MapData)));
                
                // Events
                s.DoAt(GetProtoDataPointer(Jaguar_R1Proto_References.test_mapevent), () => EventData = s.SerializeObject<Jaguar_R1_EventBlock>(EventData, name: nameof(EventData)));
                
                // Tilemap
                s.DoAt(GetProtoDataPointer(Jaguar_R1Proto_References.jun_block), () => TileData = s.SerializeObjectArray<RGB556Color>(TileData, 440 * (16 * 16), name: nameof(TileData)));
            }
        }

        #endregion
    }
}