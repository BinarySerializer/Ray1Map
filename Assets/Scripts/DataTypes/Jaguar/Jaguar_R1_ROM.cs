using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// ROM data for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_ROM : R1Serializable
    {
        /// <summary>
        /// The map data for the current level
        /// </summary>
        public PS1_R1_MapBlock MapData { get; set; }

        /// <summary>
        /// The current sprite palette
        /// </summary>
        public RGB556Color[] SpritePalette { get; set; }

        /// <summary>
        /// The current tileset data
        /// </summary>
        public RGB556Color[] TileData { get; set; }

        public byte[] ImageBuffer { get; set; }

        public Jaguar_R1_DESData[] DESData { get; set; }
        public Pointer[] WorldSpritesLoadCommandPointers { get; set; }
        public Pointer[][] MapDataLoadCommandPointers { get; set; }

        // Parsed
        public Jaguar_R1_LevelLoadCommandCollection FixSpritesLoadCommands { get; set; }
        public Jaguar_R1_LevelLoadCommandCollection[] WorldSpritesLoadCommands { get; set; }
        public Jaguar_R1_LevelLoadCommandCollection[][] MapDataLoadCommands { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            var pointerTable = PointerTables.GetJaguarPointerTable(s.GameSettings.GameModeSelection, this.Offset.file);
            s.DoAt(pointerTable[Jaguar_R1_Pointer.DES], () => {
                // Copied to 0x001f9000 in memory. All pointers to 0x001Fxxxx likely point to an entry in this table
                // A lot of those pointers can be found in the blocks that load level data.
                DESData = s.SerializeObjectArray<Jaguar_R1_DESData>(DESData, 0x1C4, name: nameof(DESData));
            });
            s.DoAt(pointerTable[Jaguar_R1_Pointer.FixSprites], () => {
                FixSpritesLoadCommands = s.SerializeObject(FixSpritesLoadCommands, name: nameof(FixSpritesLoadCommands));
            });
            s.DoAt(pointerTable[Jaguar_R1_Pointer.WorldSprites], () => {
                if (WorldSpritesLoadCommandPointers == null) WorldSpritesLoadCommandPointers = new Pointer[6];
                if (WorldSpritesLoadCommands == null) WorldSpritesLoadCommands = new Jaguar_R1_LevelLoadCommandCollection[6];
                for (int i = 0; i < 6; i++) {
                    Pointer FunctionPointer = s.SerializePointer(null, name: nameof(FunctionPointer));
                    s.DoAt(FunctionPointer, () => {
                        // Parse assembly
                        ushort Instruction = s.Serialize<ushort>(0x41f9, name: nameof(Instruction)); // Load effective address
                        WorldSpritesLoadCommandPointers[i] = s.SerializePointer(WorldSpritesLoadCommandPointers[i], name: $"{nameof(WorldSpritesLoadCommandPointers)}[{i}]");
                        s.DoAt(WorldSpritesLoadCommandPointers[i], () => {
                            WorldSpritesLoadCommands[i] = s.SerializeObject(WorldSpritesLoadCommands[i], name: $"{nameof(WorldSpritesLoadCommands)}[{i}]");
                        });
                    });
                }
            });
            s.DoAt(pointerTable[Jaguar_R1_Pointer.MapData], () => {
                Dictionary<World, int> levels = new Jaguar_R1_Manager().GetNumLevels;
                if (MapDataLoadCommandPointers == null) MapDataLoadCommandPointers = new Pointer[6][];
                if (MapDataLoadCommands == null) MapDataLoadCommands = new Jaguar_R1_LevelLoadCommandCollection[6][];
                for (int i = 0; i < 6; i++) {
                    int numLevels = levels[(World)i];
                    if (MapDataLoadCommandPointers[i] == null) MapDataLoadCommandPointers[i] = new Pointer[numLevels];
                    if (MapDataLoadCommands[i] == null) MapDataLoadCommands[i] = new Jaguar_R1_LevelLoadCommandCollection[numLevels];
                    Pointer FunctionListPointer = s.SerializePointer(null, name: nameof(FunctionListPointer));
                    s.DoAt(FunctionListPointer, () => {
                        for (int j = 0; j < numLevels; j++) {
                            Pointer FunctionPointer = s.SerializePointer(null, name: nameof(FunctionPointer));
                            s.DoAt(FunctionPointer, () => {
                                // Parse assembly
                                ushort Instruction = s.Serialize<ushort>(0x41f9, name: nameof(Instruction)); // Load effective address
                                while (Instruction == 0x23fc) { // Move
                                    s.Serialize<uint>(0, name: "MoveArg0"); // arguments differ for each level
                                    s.Serialize<uint>(0, name: "MoveArg1");
                                    Instruction = s.Serialize<ushort>(0x41f9, name: nameof(Instruction)); // Load effective address
                                }
                                MapDataLoadCommandPointers[i][j] = s.SerializePointer(MapDataLoadCommandPointers[i][j], name: $"{nameof(MapDataLoadCommandPointers)}[{i}][{j}]");
                                s.DoAt(MapDataLoadCommandPointers[i][j], () => {
                                    MapDataLoadCommands[i][j] = s.SerializeObject(MapDataLoadCommands[i][j], name: $"{nameof(MapDataLoadCommands)}[{i}][{j}]");
                                });
                            });
                        }
                    });
                }
            });
            // TODO: there are a few more special load commands in a "7th" world. Probably for the menu, breakout etc
            // Some functions in that list don't have load a LevelLoadCommand block, so they can't be parsed normally.


            // TODO: Don't hard-code this!
            var mapPointer = this.Offset + 3415677;
            var palPointer = new Pointer(0x009B8C6A, this.Offset.file);
            var tilesPointer = this.Offset + 3222288;
            var bufferPointer = this.Offset + 486418;

            // Serialize map data
            s.DoAt(mapPointer, () => s.DoEncoded(new RNCEncoder(), () => MapData = s.SerializeObject<PS1_R1_MapBlock>(MapData, name: nameof(MapData))));
            
            // Serialize sprite palette
            s.DoAt(palPointer, () => SpritePalette = s.SerializeObjectArray<RGB556Color>(SpritePalette, 256, name: nameof(SpritePalette)));

            // Serialize tile data
            s.DoAt(tilesPointer, () => s.DoEncoded(new RNCEncoder(), () => TileData = s.SerializeObjectArray<RGB556Color>(TileData, s.CurrentLength / 2, name: nameof(TileData))));

            s.DoAt(bufferPointer, () => s.DoEncoded(new RNCEncoder(), () => ImageBuffer = s.SerializeArray<byte>(ImageBuffer, s.CurrentLength, name: nameof(ImageBuffer))));

            /*
            var output = new byte[(ImageBuffer.Length / 2) * 3];

            for (int i = 0; i < ImageBuffer.Length; i += 2)
            {
                var v = SpritePalette[ImageBuffer[i]];

                // Write RGB values
                output[(i / 2) * 3 + 0] = v.Red;
                output[(i / 2) * 3 + 1] = v.Green;
                output[(i / 2) * 3 + 2] = v.Blue;
            }

            Util.ByteArrayToFile(@"C:\Users\RayCarrot\Downloads\test.dat", output);*/
        }
    }
}