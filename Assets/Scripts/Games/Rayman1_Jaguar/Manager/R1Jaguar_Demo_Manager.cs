using BinarySerializer;
using BinarySerializer.Ray1;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Ray1.Jaguar;

namespace Ray1Map.Rayman1_Jaguar
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

        protected override Dictionary<SpecialEventType, Pointer> GetSpecialEventPointers(Context context) {
            // Read the rom
            var rom = FileFactory.Read<JAG_ROM>(context, GetROMFilePath);
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

        public override void AddContextPointers(Context context)
        {
            context.AddPreDefinedPointers(JAG_DefinedPointers.JAG_Demo);
        }

        #endregion
    }
}