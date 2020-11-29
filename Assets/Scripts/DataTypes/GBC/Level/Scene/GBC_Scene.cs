using System.Linq;

namespace R1Engine
{
    public class GBC_Scene : GBC_BaseBlock
    {
        public ushort ActorsCount { get; set; }
        public ushort ActorsOffset { get; set; }
        public byte KnotsHeight { get; set; }
        public byte KnotsWidth { get; set; }
        public ushort KnotsOffset { get; set; }
        public ushort Height { get; set; }
        public ushort Width { get; set; }
        public byte Timeout { get; set; } // For bonus levels
        public byte Index_PlayField { get; set; }
        public byte IndexMin_ActorModels { get; set; }
        public byte IndexMax_ActorModels { get; set; }

        public ARGB1555Color[] ObjPalette { get; set; }
        public ARGB1555Color[] TilePalette { get; set; }
        public ushort MainActor_0 { get; set; }
        public ushort MainActor_1 { get; set; }
        public ushort MainActor_2 { get; set; }
        public byte Index_SoundBank { get; set; }
        public byte[] UnknownData { get; set; }
        public GBC_UnkActorStruct[] UnkActorStructs { get; set; }

        // Parsed from offsets
        public GBC_Actor[] Actors { get; set; }
        public GBC_Knot[] Knots { get; set; } // Sectors

        // Parsed from offset table
        public GBC_PlayField PlayField { get; set; }

        // TODO: Parse sound bank block

        public override void SerializeBlock(SerializerObject s)
        {
            var blockOffset = s.CurrentPointer;

            // Serialize data (always little endian)
            s.DoEndian(R1Engine.Serialize.BinaryFile.Endian.Little, () => 
            {
                // Parse data
                ActorsCount = s.Serialize<ushort>(ActorsCount, name: nameof(ActorsCount));
                ActorsOffset = s.Serialize<ushort>(ActorsOffset, name: nameof(ActorsOffset));
                KnotsHeight = s.Serialize<byte>(KnotsHeight, name: nameof(KnotsHeight));
                KnotsWidth = s.Serialize<byte>(KnotsWidth, name: nameof(KnotsWidth));
                KnotsOffset = s.Serialize<ushort>(KnotsOffset, name: nameof(KnotsOffset));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));
                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Timeout = s.Serialize<byte>(Timeout, name: nameof(Timeout));
                Index_PlayField = s.Serialize<byte>(Index_PlayField, name: nameof(Index_PlayField));
                IndexMin_ActorModels = s.Serialize<byte>(IndexMin_ActorModels, name: nameof(IndexMin_ActorModels));
                IndexMax_ActorModels = s.Serialize<byte>(IndexMax_ActorModels, name: nameof(IndexMax_ActorModels));
                ObjPalette = s.SerializeObjectArray<ARGB1555Color>(ObjPalette, 8 * 4, name: nameof(ObjPalette));
                TilePalette = s.SerializeObjectArray<ARGB1555Color>(TilePalette, 8 * 4, name: nameof(TilePalette));
                MainActor_0 = s.Serialize<ushort>(MainActor_0, name: nameof(MainActor_0));
                MainActor_1 = s.Serialize<ushort>(MainActor_1, name: nameof(MainActor_1));
                MainActor_2 = s.Serialize<ushort>(MainActor_2, name: nameof(MainActor_2));
                Index_SoundBank = s.Serialize<byte>(Index_SoundBank, name: nameof(Index_SoundBank));

                // TODO: Parse data (UnkActorStructs?)
                UnknownData = s.SerializeArray<byte>(UnknownData, (blockOffset + ActorsOffset).AbsoluteOffset - s.CurrentPointer.AbsoluteOffset, name: nameof(UnknownData));

                // Parse from pointers
                Actors = s.DoAt(blockOffset + ActorsOffset, () => s.SerializeObjectArray<GBC_Actor>(Actors, ActorsCount, name: nameof(Actors)));
                Knots = s.DoAt(blockOffset + KnotsOffset, () => s.SerializeObjectArray<GBC_Knot>(Knots, KnotsHeight * KnotsWidth, name: nameof(Knots)));
                s.Goto(Knots.Last().Offset + Knots.Last().ActorsCount * 2 + 1); // Go to end of the block
            });

            // Parse data from pointers
            PlayField = s.DoAt(OffsetTable.GetPointer(Index_PlayField - 1), () => s.SerializeObject<GBC_PlayField>(PlayField, name: nameof(PlayField)));

            // Parse actor models
            foreach (var actor in Actors.Where(x => x.Index_ActorModel > 1))
                actor.ActorModel = s.DoAt(OffsetTable.GetPointer(actor.Index_ActorModel - 1), () => s.SerializeObject<GBC_ActorModel>(actor.ActorModel, name: $"{nameof(actor.ActorModel)}[{actor.Index_ActorModel}]"));
        }
    }
}