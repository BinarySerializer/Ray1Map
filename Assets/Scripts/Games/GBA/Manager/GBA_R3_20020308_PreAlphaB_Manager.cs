using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.GBA
{
    public class GBA_R3_20020308_PreAlphaB_Manager : GBA_R3_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 11),
        };

        public override ModifiedActorState[] ModifiedActorStates => new ModifiedActorState[0];

		public override int[] AdditionalSprites4bpp => new int[0];
		public override int[] AdditionalSprites8bpp => new int[0];
	}
}