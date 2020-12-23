using System.Linq;

namespace R1Engine
{
    public class GBA_Shanghai_Scene : GBA_BaseBlock
    {
        public ushort GameObjectsCount { get; set; }
        public byte KnotsWidth { get; set; }
        public byte KnotsHeight { get; set; }

        public ushort MainActor_0 { get; set; }
        public ushort MainActor_1 { get; set; }
        public ushort MainActor_2 { get; set; }

        public byte[] Bytes_0A { get; set; }

        public ushort Index_Actors { get; set; }
        public ushort Index_ObjPal { get; set; }
        public ushort Index_Captors { get; set; }
        public ushort Index_Knots { get; set; }
        public ushort Index_PlayFieldFG { get; set; }
        public ushort Index_TilePal { get; set; }

        public ushort CrouchingTiger_Ushort { get; set; }
        public ushort Index_PlayFieldBG { get; set; }

        // Parsed from offsets

        public GBA_Shanghai_ActorsBlock Actors { get; set; }
        public GBA_SpritePalette ObjPal { get; set; }
        public GBA_Shanghai_CaptorsBlock Captors { get; set; }
        public GBA_Shanghai_KnotsBlock Knots { get; set; }
        public GBA_PlayField PlayFieldFG { get; set; }
        public GBA_PlayField PlayFieldBG { get; set; }
        public GBA_Palette TilePal { get; set; }

        public GBA_PlayField CombinedPlayField => new GBA_PlayField()
        {
            Layers = new GBA_PlayField[]
            {
                PlayFieldBG,
                PlayFieldFG,
            }.Where(x => x != null).SelectMany(x => x.Layers).ToArray(),
        };

        public override void SerializeBlock(SerializerObject s)
        {
            GameObjectsCount = s.Serialize<ushort>(GameObjectsCount, name: nameof(GameObjectsCount));

            KnotsWidth = s.Serialize<byte>(KnotsWidth, name: nameof(KnotsWidth));
            KnotsHeight = s.Serialize<byte>(KnotsHeight, name: nameof(KnotsHeight));

            MainActor_0 = s.Serialize<ushort>(MainActor_0, name: nameof(MainActor_0));
            MainActor_1 = s.Serialize<ushort>(MainActor_1, name: nameof(MainActor_1));
            MainActor_2 = s.Serialize<ushort>(MainActor_2, name: nameof(MainActor_2));

            Bytes_0A = s.SerializeArray<byte>(Bytes_0A, 6, name: nameof(Bytes_0A));

            Index_Actors = s.Serialize<ushort>(Index_Actors, name: nameof(Index_Actors));
            Index_ObjPal = s.Serialize<ushort>(Index_ObjPal, name: nameof(Index_ObjPal));
            Index_Captors = s.Serialize<ushort>(Index_Captors, name: nameof(Index_Captors));
            Index_Knots = s.Serialize<ushort>(Index_Knots, name: nameof(Index_Knots));
            Index_PlayFieldFG = s.Serialize<ushort>(Index_PlayFieldFG, name: nameof(Index_PlayFieldFG));
            Index_TilePal = s.Serialize<ushort>(Index_TilePal, name: nameof(Index_TilePal));

            if (s.GameSettings.EngineVersion >= EngineVersion.GBA_CrouchingTiger)
            {
                CrouchingTiger_Ushort = s.Serialize<ushort>(CrouchingTiger_Ushort, name: nameof(CrouchingTiger_Ushort));
                Index_PlayFieldBG = s.Serialize<ushort>(Index_PlayFieldBG, name: nameof(Index_PlayFieldBG));
            }
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Actors = s.DoAt(OffsetTable.GetPointer(Index_Actors), () => s.SerializeObject<GBA_Shanghai_ActorsBlock>(Actors, name: nameof(Actors)));
            ObjPal = s.DoAt(OffsetTable.GetPointer(Index_ObjPal), () => s.SerializeObject<GBA_SpritePalette>(ObjPal, name: nameof(ObjPal)));
            Captors = s.DoAt(OffsetTable.GetPointer(Index_Captors), () => s.SerializeObject<GBA_Shanghai_CaptorsBlock>(Captors, name: nameof(Captors)));
            Knots = s.DoAt(OffsetTable.GetPointer(Index_Knots), () => s.SerializeObject<GBA_Shanghai_KnotsBlock>(Knots, x => x.Length = KnotsWidth * KnotsHeight, name: nameof(Knots)));
            PlayFieldFG = s.DoAt(OffsetTable.GetPointer(Index_PlayFieldFG), () => s.SerializeObject<GBA_PlayField>(PlayFieldFG, name: nameof(PlayFieldFG)));
            PlayFieldBG = s.DoAt(OffsetTable.GetPointer(Index_PlayFieldBG), () => s.SerializeObject<GBA_PlayField>(PlayFieldBG, name: nameof(PlayFieldBG)));
            TilePal = s.DoAt(OffsetTable.GetPointer(Index_TilePal), () => s.SerializeObject<GBA_Palette>(TilePal, name: nameof(TilePal)));
        }
    }
}