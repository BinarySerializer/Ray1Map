using System;
using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.GBA
{
    public class GBA_R3_20020301_PreAlpha_Manager : GBA_R3_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 10),
        };

        public override int DLCLevelCount => 0;
        public override int[] MenuLevels => Array.Empty<int>();
        public override bool HasR3SinglePakLevel => false;
        public override ModifiedActorState[] ModifiedActorStates => new ModifiedActorState[0];

		public override int[] AdditionalSprites4bpp => new int[] { 11 };
		public override int[] AdditionalSprites8bpp => new int[0];
	}
}