using System;
using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.GBA
{
    public class GBA_R3_20020513_E3GameCube_Manager : GBA_R3_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 16)
        };

        public override int DLCLevelCount => 0;
        public override int[] MenuLevels => new int[] { 19 };
        public override bool HasR3SinglePakLevel => false;
        public override ModifiedActorState[] ModifiedActorStates => new ModifiedActorState[0];

        public override int[] AdditionalSprites4bpp => new int[] { 17, 18, 20, 21 };
		public override int[] AdditionalSprites8bpp => new int[0];
	}
}