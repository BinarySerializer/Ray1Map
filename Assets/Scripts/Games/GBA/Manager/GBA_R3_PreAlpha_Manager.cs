using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using UnityEngine;

namespace Ray1Map.GBA
{
    public class GBA_R3_PreAlpha_Manager : GBA_R3_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 7), // World 1
        };

        public override ModifiedActorState[] ModifiedActorStates => new ModifiedActorState[0];

		public override int[] AdditionalSprites4bpp => new int[0];
		public override int[] AdditionalSprites8bpp => new int[0];
	}
}