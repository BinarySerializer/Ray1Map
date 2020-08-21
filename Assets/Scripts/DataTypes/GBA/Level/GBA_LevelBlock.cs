using UnityEngine;

namespace R1Engine
{
    public class GBA_LevelBlock : GBA_BaseBlock
    {
        #region Level Data

        public ushort PlayFieldIndex { get; set; }
        public byte Unk_02 { get; set; }
        public byte Unk_03 { get; set; }

        public byte ObjectsCountTotal { get; set; }
        public byte AlwaysActorsCount { get; set; }
        public byte NormalActorsCount { get; set; }
        public byte UnkCount { get; set; }

        // Not included in ObjectsCountTotal
        public byte Unk_08 { get; set; }

        public byte UnkStructsCount { get; set; }

        public byte Unk_0A { get; set; }
        public byte Unk_0B { get; set; }

        public GBA_Actor[] Actors { get; set; }
        public GBA_UnkLevelBlockStruct[] UnkStructs { get; set; }

        public byte[] UnkData { get; set; }

        #endregion

        #region Parsed

        public GBA_PlayField PlayField { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeBlock(SerializerObject s)
        {
            PlayFieldIndex = s.Serialize<ushort>(PlayFieldIndex, name: nameof(PlayFieldIndex));
            Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
            Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

            ObjectsCountTotal = s.Serialize<byte>(ObjectsCountTotal, name: nameof(ObjectsCountTotal));
            AlwaysActorsCount = s.Serialize<byte>(AlwaysActorsCount, name: nameof(AlwaysActorsCount));
            NormalActorsCount = s.Serialize<byte>(NormalActorsCount, name: nameof(NormalActorsCount));
            UnkCount = s.Serialize<byte>(UnkCount, name: nameof(UnkCount));

            Unk_08 = s.Serialize<byte>(Unk_08, name: nameof(Unk_08));
            UnkStructsCount = s.Serialize<byte>(UnkStructsCount, name: nameof(UnkStructsCount));
            Unk_0A = s.Serialize<byte>(Unk_0A, name: nameof(Unk_0A));
            Unk_0B = s.Serialize<byte>(Unk_0B, name: nameof(Unk_0B));

            if (s.GameSettings.EngineVersion >= EngineVersion.GBA_StarWarsTrilogy) {
                if (Actors == null) Actors = new GBA_Actor[AlwaysActorsCount + NormalActorsCount];
                for (int i = 0; i < Actors.Length; i++) {
                    ushort size = 12;
                    if (i > 0) size = Actors[i - 1].NextActorSize;
                    Actors[i] = s.SerializeObject<GBA_Actor>(Actors[i], onPreSerialize: a => a.ThisActorSize = size, name: $"{nameof(Actors)}[{i}]");
                }
            } else {
                Actors = s.SerializeObjectArray<GBA_Actor>(Actors, AlwaysActorsCount + NormalActorsCount, name: nameof(Actors));
            }

            if (UnkCount != 0 || Unk_08 != 0)
                Debug.LogWarning($"Potentially missed data");

            UnkStructs = s.SerializeObjectArray<GBA_UnkLevelBlockStruct>(UnkStructs, UnkStructsCount, name: nameof(UnkStructs));

            // TODO: What is this data?
            //Controller.print(Actors.Sum(a => a.Unk_0B));
            Controller.print("Length of unknown data: " + (BlockSize - (s.CurrentPointer - Offset)));
            UnkData = s.SerializeArray<byte>(UnkData, BlockSize - (s.CurrentPointer - Offset), name: nameof(UnkData));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            // Parse level block data
            PlayField = s.DoAt(OffsetTable.GetPointer(PlayFieldIndex), () => s.SerializeObject<GBA_PlayField>(PlayField, name: nameof(PlayField)));

            // Parse actor data
            for (var i = 0; i < Actors.Length; i++)
            {
                if (Actors[i].GraphicsDataIndex < OffsetTable.OffsetsCount && (s.GameSettings.EngineVersion < EngineVersion.GBA_StarWarsTrilogy || Actors[i].GraphicsDataIndex != 0))
                    Actors[i].GraphicData = s.DoAt(OffsetTable.GetPointer(Actors[i].GraphicsDataIndex),
                        () => s.SerializeObject<GBA_ActorGraphicData>(Actors[i].GraphicData,
                            name: $"{nameof(GBA_Actor.GraphicData)}[{i}]"));
            }
        }

        #endregion
    }
}