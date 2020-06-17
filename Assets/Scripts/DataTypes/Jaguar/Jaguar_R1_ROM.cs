using System.Collections.Generic;
using R1Engine.Serialize;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// ROM data for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_ROM : R1Serializable
    {
        #region Global Data

        public Pointer[] WorldSpritesLoadCommandPointers { get; set; }
        public Pointer[][] MapDataLoadCommandPointers { get; set; }

        #endregion

        #region Global Data (Parsed)

        public Jaguar_R1_LevelLoadCommandCollection FixSpritesLoadCommands { get; set; }
        public Jaguar_R1_LevelLoadCommandCollection[] WorldSpritesLoadCommands { get; set; }
        public Jaguar_R1_LevelLoadCommandCollection[][] MapDataLoadCommands { get; set; }

        #endregion

        #region Map Data

        /// <summary>
        /// The map data for the current level
        /// </summary>
        public PS1_R1_MapBlock MapData { get; set; }

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

        /// <summary>
        /// The image buffers for the current level, with the key being the memory pointer pointer
        /// </summary>
        public Dictionary<uint, byte[]> ImageBuffers { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get info
            var pointerTable = PointerTables.GetJaguarPointerTable(s.GameSettings.GameModeSelection, this.Offset.file);
            var levels = new Jaguar_R1_Manager().GetNumLevels;

            // Serialize event definition data
            /*MemoryMappedByteArrayFile ram = s.Context.GetFile("RAM") as MemoryMappedByteArrayFile;
            if (s is BinaryDeserializer) {
                s.DoAt(pointerTable[Jaguar_R1_Pointer.DES], () => {
                    byte[] DESDataBytes = s.SerializeArray<byte>(null, 0x1C4 * 0x28, name: nameof(DESDataBytes));
                    ram.WriteBytes(0x001f9000, DESDataBytes);
                });
            }*/
            if (!s.Context.FileExists("RAM_EventDefinitions")) {
                // Copied to 0x001f9000 in memory. All pointers to 0x001Fxxxx likely point to an entry in this table
                s.DoAt(pointerTable[Jaguar_R1_Pointer.EventDefinitions], () => {
                    byte[] EventDefsDataBytes = s.SerializeArray<byte>(null, 0x1C4 * 0x28, name: nameof(EventDefsDataBytes));
                    var file = new MemoryMappedByteArrayFile("RAM_EventDefinitions", EventDefsDataBytes, s.Context, 0x001f9000) {
                        Endianness = BinaryFile.Endian.Big
                    };
                    s.Context.AddFile(file);
                    s.DoAt(file.StartPointer, () => {
                        EventDefinitions = s.SerializeObjectArray<Jaguar_R1_EventDefinition>(EventDefinitions, 0x1C4, name: nameof(EventDefinitions));
                    });
                });
            }

            // Serialize allfix sprite data
            s.DoAt(pointerTable[Jaguar_R1_Pointer.FixSprites], () => {
                FixSpritesLoadCommands = s.SerializeObject<Jaguar_R1_LevelLoadCommandCollection>(FixSpritesLoadCommands, name: nameof(FixSpritesLoadCommands));
            });

            // Serialize world sprite data
            s.DoAt(pointerTable[Jaguar_R1_Pointer.WorldSprites], () => 
            {
                if (WorldSpritesLoadCommandPointers == null) 
                    WorldSpritesLoadCommandPointers = new Pointer[6];

                if (WorldSpritesLoadCommands == null) 
                    WorldSpritesLoadCommands = new Jaguar_R1_LevelLoadCommandCollection[6];

                for (int i = 0; i < 6; i++)
                {
                    Pointer FunctionPointer = s.SerializePointer(null, name: nameof(FunctionPointer));
                    s.DoAt(FunctionPointer, () => {
                        // Parse assembly
                        ushort instruction = s.Serialize<ushort>(0x41f9, name: nameof(instruction)); // Load effective address
                        WorldSpritesLoadCommandPointers[i] = s.SerializePointer(WorldSpritesLoadCommandPointers[i], name: $"{nameof(WorldSpritesLoadCommandPointers)}[{i}]");
                        s.DoAt(WorldSpritesLoadCommandPointers[i], () => {
                            WorldSpritesLoadCommands[i] = s.SerializeObject<Jaguar_R1_LevelLoadCommandCollection>(WorldSpritesLoadCommands[i], name: $"{nameof(WorldSpritesLoadCommands)}[{i}]");
                        });
                    });
                }
            });

            s.DoAt(pointerTable[Jaguar_R1_Pointer.MapData], () =>
            {
                if (MapDataLoadCommandPointers == null)
                    MapDataLoadCommandPointers = new Pointer[6][];

                if (MapDataLoadCommands == null)
                    MapDataLoadCommands = new Jaguar_R1_LevelLoadCommandCollection[6][];

                for (int i = 0; i < 6; i++)
                {
                    int numLevels = levels[i].Value;

                    if (MapDataLoadCommandPointers[i] == null)
                        MapDataLoadCommandPointers[i] = new Pointer[numLevels];

                    if (MapDataLoadCommands[i] == null)
                        MapDataLoadCommands[i] = new Jaguar_R1_LevelLoadCommandCollection[numLevels];

                    Pointer FunctionListPointer = s.SerializePointer(null, name: nameof(FunctionListPointer));
                    s.DoAt(FunctionListPointer, () => {
                        for (int j = 0; j < numLevels; j++)
                        {
                            Pointer FunctionPointer = s.SerializePointer(null, name: nameof(FunctionPointer));
                            s.DoAt(FunctionPointer, () => {
                                // Parse assembly
                                ushort instruction = s.Serialize<ushort>(0x41f9, name: nameof(instruction)); // Load effective address
                                while (instruction != 0x41f9) {
                                    switch (instruction) {
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
                                MapDataLoadCommandPointers[i][j] = s.SerializePointer(MapDataLoadCommandPointers[i][j], name: $"{nameof(MapDataLoadCommandPointers)}[{i}][{j}]");
                                s.DoAt(MapDataLoadCommandPointers[i][j], () => {
                                    MapDataLoadCommands[i][j] = s.SerializeObject<Jaguar_R1_LevelLoadCommandCollection>(MapDataLoadCommands[i][j], name: $"{nameof(MapDataLoadCommands)}[{i}][{j}]");
                                });
                            });
                        }
                    });
                }
            });
            // TODO: there are a few more special load commands in a "7th" world. Probably for the menu, breakout etc
            // Some functions in that list don't have load a LevelLoadCommand block, so they can't be parsed normally.



            // Get the current map load commands
            var wldCommands = WorldSpritesLoadCommands[levels.FindItemIndex(x => x.Key == s.GameSettings.World)];
            var mapCommands = MapDataLoadCommands[levels.FindItemIndex(x => x.Key == s.GameSettings.World)][s.GameSettings.Level - 1].Commands;

            // Get pointers
            var mapPointer = mapCommands.FindItem(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.LevelMap).LevelMapBlockPointer;
            var eventPointer = mapCommands.FindItem(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.LevelMap).LevelEventBlockPointer;
            var palPointer = mapCommands.FindItem(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Palette).PalettePointer;

            var tilesPointer = mapCommands.LastOrDefault(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Graphics && x.ImageBufferMemoryPointer == 0x001B3B68)?.ImageBufferPointer ?? WorldSpritesLoadCommands[levels.FindItemIndex(x => x.Key == s.GameSettings.World)].Commands.First(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Graphics && x.ImageBufferMemoryPointer == 0x001B3B68).ImageBufferPointer;

            // Serialize map and event data
            s.DoAt(mapPointer, () => s.DoEncoded(new RNCEncoder(), () => MapData = s.SerializeObject<PS1_R1_MapBlock>(MapData, name: nameof(MapData))));
            s.DoAt(eventPointer, () => s.DoEncoded(new RNCEncoder(), () => EventData = s.SerializeObject<Jaguar_R1_EventBlock>(EventData, name: nameof(EventData))));

            // Serialize sprite palette
            s.DoAt<RGB556Color[]>(palPointer, () => SpritePalette = s.SerializeObjectArray<RGB556Color>(SpritePalette, 256, name: nameof(SpritePalette)));

            // Serialize tile data
            s.DoAt(tilesPointer, () => s.DoEncoded(new RNCEncoder(), () => TileData = s.SerializeObjectArray<RGB556Color>(TileData, s.CurrentLength / 2, name: nameof(TileData))));

            // Serialize image buffers
            if (ImageBuffers == null)
            {
                ImageBuffers = new Dictionary<uint, byte[]>();

                var index = 0;
                foreach (var cmd in FixSpritesLoadCommands.Commands.Concat(wldCommands.Commands).Concat(mapCommands).Where(x => x.Type == Jaguar_R1_LevelLoadCommand.LevelLoadCommandType.Sprites))
                {
                    // Later commands overwrite previous ones
                    /*if (ImageBuffers.ContainsKey(cmd.ImageBufferMemoryPointerPointer))
                        continue;
                    */
                    s.DoAt(cmd.ImageBufferPointer, () => s.DoEncoded(new RNCEncoder(), () =>
                    {
                        ImageBuffers[cmd.ImageBufferMemoryPointerPointer] = s.SerializeArray<byte>(default, s.CurrentLength, $"ImageBuffer[{index}]");
                    }));
                    index++;
                }
            }
        }

        #endregion
    }
}