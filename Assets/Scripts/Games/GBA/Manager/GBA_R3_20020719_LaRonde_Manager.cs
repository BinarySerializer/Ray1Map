using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.GBA
{
    public class GBA_R3_20020719_LaRonde_Manager : GBA_R3_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
		{
			Enumerable.Range(0, 8), // World 1
            Enumerable.Range(8, 9), // World 2
            Enumerable.Range(17, 13), // World 3
            Enumerable.Range(30, 10), // World 4
            Enumerable.Range(40, 8), // Bonus
            Enumerable.Range(48, 6), // Ly
            //Enumerable.Range(54, 5), // World
            //Enumerable.Range(59, 6), // Multiplayer
        };

        public override ModifiedActorState[] ModifiedActorStates => new ModifiedActorState[0];

		public override int[] AdditionalSprites4bpp => new int[0];
		public override int[] AdditionalSprites8bpp => new int[0];
	}
}