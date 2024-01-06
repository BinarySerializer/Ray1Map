using System;
using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.GBA
{
    public class GBA_R3_20020719_LaRonde_Manager : GBA_R3_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
		{
			Enumerable.Range(0, 9), // World 1
            Enumerable.Range(9, 11), // World 2
            Enumerable.Range(20, 12), // World 3
            Enumerable.Range(32, 11), // World 4
            Enumerable.Range(43, 11), // Bonus
            //Enumerable.Range(48, 6), // Ly
            //Enumerable.Range(54, 5), // World
            //Enumerable.Range(59, 6), // Multiplayer
        };

        public override int DLCLevelCount => 0;
        public override int[] MenuLevels => new int[] { 63 };
        public override bool HasR3SinglePakLevel => false;
        //public override ModifiedActorState[] ModifiedActorStates => new ModifiedActorState[0];

        public override int[] AdditionalSprites4bpp => new int[] { 59, 60, 61, 62, 64, 65 };
		public override int[] AdditionalSprites8bpp => new int[0];
	}
}