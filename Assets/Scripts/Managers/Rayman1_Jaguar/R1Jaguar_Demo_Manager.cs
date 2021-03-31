
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Game manager for the Jaguar demo
    /// </summary>
    public class R1Jaguar_Demo_Manager : R1Jaguar_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            // Hard-code this since most levels don't have maps we can load
            new GameInfo_World(1, Enumerable.Range(1, 14).ToArray()),
            new GameInfo_World(2, Enumerable.Range(1, 5).ToArray()),
        });

        /// <summary>
        /// Gets the file path to the ROM file
        /// </summary>
        public override string GetROMFilePath => $"ROM.rom";

        /// <summary>
        /// Gets the base address for the ROM file
        /// </summary>
        protected override uint GetROMBaseAddress => 0x00802000;

        /// <summary>
        /// Gets the available levels ordered based on the global level array
        /// </summary>
        public override KeyValuePair<R1_World, int>[] GetNumLevels => new KeyValuePair<R1_World, int>[]
        {
            new KeyValuePair<R1_World, int>(R1_World.Jungle, 14),
            new KeyValuePair<R1_World, int>(R1_World.Mountain, 4),
            new KeyValuePair<R1_World, int>(R1_World.Cave, 2),
            new KeyValuePair<R1_World, int>(R1_World.Music, 5),
            new KeyValuePair<R1_World, int>(R1_World.Image, 2),
            new KeyValuePair<R1_World, int>(R1_World.Cake, 2)
        };

        public override int[] ExtraMapCommands => new int[] {
            0, 1, 2
        };

        public override uint EventCount => 0xB5;

        /// <summary>
        /// Gets the vignette addresses and widths
        /// </summary>
        public override KeyValuePair<uint, int>[] GetVignette => new KeyValuePair<uint, int>[]
        {
            // World map
            new KeyValuePair<uint, int>(0x875BC0, 320), 
            
            // Breakout
            new KeyValuePair<uint, int>(0x8855F8, 320), 

            // Jungle
            new KeyValuePair<uint, int>(0x8B083E, 192), 
            new KeyValuePair<uint, int>(0x8D735E, 144), 
            new KeyValuePair<uint, int>(0x8D8F8A, 48), 

            // Music
            new KeyValuePair<uint, int>(0x8F64E6, 384), 
        };
        protected override Dictionary<SpecialEventType, Pointer> GetSpecialEventPointers(Context context) {
            // Read the rom
            var rom = FileFactory.Read<R1Jaguar_ROM>(GetROMFilePath, context);
            Pointer baseOff = rom.EventDefinitions[0].Offset;
            return new Dictionary<SpecialEventType, Pointer>() {
                [SpecialEventType.RayPos] = baseOff + 0x1BF8,
                [SpecialEventType.Gendoor] = null, // Doesn't exist
                [SpecialEventType.Piranha] = baseOff + 0xDE8,
                [SpecialEventType.Piranha2] = baseOff + 0xE10,
                [SpecialEventType.ScrollFast] = baseOff + 0xFA0,
                [SpecialEventType.ScrollSlow] = baseOff + 0xFC8,
                [SpecialEventType.RayOnBzzit] = null, // baseOff + 0x1C20
                [SpecialEventType.BzzitDemo] = rom.Offset + 0x1183C8,

                [SpecialEventType.RaymanVisual] = baseOff,
                [SpecialEventType.GendoorVisual] = baseOff + 0x5A0,
                [SpecialEventType.PiranhaVisual] = baseOff + 0xE38,
                [SpecialEventType.ScrollVisual] = baseOff + 0x1040,
                [SpecialEventType.RayOnBzzitVisual] = null,
                [SpecialEventType.BzzitDemoVisual] = rom.Offset + 0x11DCC8,
                //[SpecialEventType.BetillaDemoVisual] = rom.Offset + 0x6BA20,
            };
        }

        public override uint[] AdditionalEventDefinitionPointers => new uint[] {
            0x0086da20,
            0x0091fcc8,

            0x0086940E,
            0x0091FD24,
            0x0091FD4C,
            0x0091FE4A,
            0x0091FE72,

            0x00872194,
            0x00872202,
            0x008722DE,
            0x00872426,
            0x0087244e,
            0x00872476,
            0x0087249e,
            0x008724c6,
            0x008724ee,
            0x00872516,

            0x008695DA,
            0x00869602,
            0x0086962A,
            0x00869652,
            0x0086967A,
            0x008696A2,
            0x008696CA,
            0x008696F2,
            0x0086971A,

            0x00873B52,
            0x00873B7A,
            0x00873CF2,
            0x00873D1A,
            0x00873e1e,
            0x00873e46,
            0x00873e6e,
            0x00873e96,
            0x00873ebe,
            0x00873ee6,
            0x00873f0e,
            0x00873f8e,
            0x00873fb6,
            0x00874134,
            0x0087415c,
            0x008741a8,

            0x0086DA20,

            0x00866598,
            0x0086869E,
            0x00868C22,

            0x0084E772,
            0x00850CEE,
            0x00851332,

            0x00857D96,

            0x0085A2D0,
            0x00860E2E,
            0x00860ED6,

            0x0085F88C,
            0x0086080C,

            0x008726FE,

            0x00862C00,
            0x00863182,
            
            0x0086DAE6,

            0x0086F23A,
        };
        #endregion
    }
}