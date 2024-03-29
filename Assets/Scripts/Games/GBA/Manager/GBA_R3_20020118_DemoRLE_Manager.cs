﻿using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_R3_20020118_DemoRLE_Manager : GBA_R3_Manager
    {
        public override IEnumerable<int>[] WorldLevels => Array.Empty<IEnumerable<int>>();

        public override int DLCLevelCount => 0;
        public override int[] MenuLevels => new int[] { 0 };
        public override bool HasR3SinglePakLevel => false;
        public override ModifiedActorState[] ModifiedActorStates => new ModifiedActorState[0];

        public override int[] AdditionalActorModels => new[] { 1, 2, 3 };
        public override int[] AdditionalSprites4bpp => new int[0];
		public override int[] AdditionalSprites8bpp => new int[0];

        protected override GBA_Scene CreateSceneForMenuLevel(Context context, GBA_Data dataBlock)
        {
            var s = context.Deserializer;
            // Create a scene to load Rayman
            return new GBA_Scene
            {
                ActorsCountTotal = 1,
                MainActors = new GBA_Actor[]
                {
                    new GBA_Actor
                    {
                        XPos = 80,
                        YPos = 130,
                        ActionIndex = 0,
                        Link_0 = 0xFF,
                        Link_1 = 0xFF,
                        Link_2 = 0xFF,
                        Link_3 = 0xFF,
                        Type = GBA_Actor.ActorType.MainActor,
                        ActorModel = s.DoAt(dataBlock.UiOffsetTable.GetPointer(1), () => s.SerializeObject<GBA_ActorModel>(default)),
                        CaptorData = null
                    },
                },
                NormalActors = Array.Empty<GBA_Actor>(),
                AlwaysActors = Array.Empty<GBA_Actor>(),
                Captors = Array.Empty<GBA_Actor>(),
                Knots = Array.Empty<GBA_Knot>(),
                PlayField = dataBlock.MenuLevelPlayfield
            };

        }
    }
}