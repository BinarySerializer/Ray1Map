using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBA_Scene : GBA_BaseBlock
    {
        #region Level Data

        public byte PlayFieldIndex { get; set; }
        public byte Unk_01 { get; set; }
        public byte Unk_02 { get; set; }
        public byte Unk_03 { get; set; }

        public byte ActorsCountTotal { get; set; }
        public byte Always1ActorsCount { get; set; }
        public byte NormalActorsCount { get; set; }
        public byte Always2ActorsCount { get; set; }

        public byte Unk_08 { get; set; }

        public byte UnkActorStructsCount { get; set; }

        public byte Unk_0A { get; set; }
        public byte UnkSceneStructsCount { get; set; }

        public GBA_Actor[] Always1Actors { get; set; }
        public GBA_Actor[] NormalActors { get; set; }
        public GBA_Actor[] Always2Actors { get; set; }

        public IEnumerable<GBA_Actor> GetAllActors => Always1Actors.Concat(NormalActors).Concat(Always2Actors);

        public GBA_UnkActorStruct[] UnkActorStructs { get; set; }

        public GBA_UnkSceneStruct[] UnkSceneStructs { get; set; }

        public byte[] RemainingData { get; set; }

        #endregion

        #region Parsed

        public GBA_PlayField PlayField { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeBlock(SerializerObject s)
        {
            PlayFieldIndex = s.Serialize<byte>(PlayFieldIndex, name: nameof(PlayFieldIndex));
            Unk_01 = s.Serialize<byte>(Unk_01, name: nameof(Unk_01));
            Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
            Unk_03 = s.Serialize<byte>(Unk_03, name: nameof(Unk_03));

            ActorsCountTotal = s.Serialize<byte>(ActorsCountTotal, name: nameof(ActorsCountTotal));
            Always1ActorsCount = s.Serialize<byte>(Always1ActorsCount, name: nameof(Always1ActorsCount));
            NormalActorsCount = s.Serialize<byte>(NormalActorsCount, name: nameof(NormalActorsCount));
            Always2ActorsCount = s.Serialize<byte>(Always2ActorsCount, name: nameof(Always2ActorsCount));

            Unk_08 = s.Serialize<byte>(Unk_08, name: nameof(Unk_08));
            UnkActorStructsCount = s.Serialize<byte>(UnkActorStructsCount, name: nameof(UnkActorStructsCount));
            Unk_0A = s.Serialize<byte>(Unk_0A, name: nameof(Unk_0A));
            UnkSceneStructsCount = s.Serialize<byte>(UnkSceneStructsCount, name: nameof(UnkSceneStructsCount));

            if (s.GameSettings.EngineVersion >= EngineVersion.GBA_SplinterCellPandoraTomorrow) 
            {
                if (Always1Actors == null) Always1Actors = new GBA_Actor[Always1ActorsCount];
                if (NormalActors == null) NormalActors = new GBA_Actor[NormalActorsCount];
                if (Always2Actors == null) Always2Actors = new GBA_Actor[Always2ActorsCount];

                void SerializeActors(GBA_Actor[] actors, ushort? prevSize)
                {
                    for (int i = 0; i < actors.Length; i++)
                    {
                        ushort size = 12;

                        if (i > 0)
                            size = actors[i - 1].NextActorSize;
                        else if (prevSize != null)
                            size = prevSize.Value;

                        actors[i] = s.SerializeObject<GBA_Actor>(actors[i], onPreSerialize: a => a.ThisActorSize = size, name: $"{nameof(actors)}[{i}]");
                    }
                }

                SerializeActors(Always1Actors, null);
                SerializeActors(NormalActors, Always1Actors.LastOrDefault()?.NextActorSize);
                SerializeActors(Always2Actors, NormalActors.LastOrDefault()?.NextActorSize ?? Always1Actors.LastOrDefault()?.NextActorSize);

                // TODO: Parse remaining data
                RemainingData = s.SerializeArray<byte>(RemainingData, BlockSize - (s.CurrentPointer - Offset), name: nameof(RemainingData));
            }
            else 
            {
                Always1Actors = s.SerializeObjectArray<GBA_Actor>(Always1Actors, Always1ActorsCount, name: nameof(Always1Actors));
                NormalActors = s.SerializeObjectArray<GBA_Actor>(NormalActors, NormalActorsCount, name: nameof(NormalActors));
                Always2Actors = s.SerializeObjectArray<GBA_Actor>(Always2Actors, Always2ActorsCount, name: nameof(Always2Actors));

                UnkActorStructs = s.SerializeObjectArray<GBA_UnkActorStruct>(UnkActorStructs, UnkActorStructsCount, name: nameof(UnkActorStructs));

                if (s.GameSettings.EngineVersion != EngineVersion.GBA_PrinceOfPersia)
                    UnkSceneStructs = s.SerializeObjectArray<GBA_UnkSceneStruct>(UnkSceneStructs, UnkSceneStructsCount, name: nameof(UnkSceneStructs));
                else
                    // TODO: Parse remaining data
                    RemainingData = s.SerializeArray<byte>(RemainingData, BlockSize - (s.CurrentPointer - Offset), name: nameof(RemainingData));
            }
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            // Parse level block data
            if (OffsetTable.Offsets[PlayFieldIndex] != -1)
                PlayField = s.DoAt(OffsetTable.GetPointer(PlayFieldIndex), () => s.SerializeObject<GBA_PlayField>(PlayField, name: nameof(PlayField)));

            // Parse actor data
            var actors = GetAllActors.ToArray();
            for (var i = 0; i < actors.Length; i++)
            {
                if (actors[i].GraphicsDataIndex < OffsetTable.OffsetsCount && (s.GameSettings.EngineVersion < EngineVersion.GBA_SplinterCellPandoraTomorrow || actors[i].GraphicsDataIndex != 0))
                    actors[i].GraphicData = s.DoAt(OffsetTable.GetPointer(actors[i].GraphicsDataIndex),
                        () => s.SerializeObject<GBA_ActorGraphicData>(actors[i].GraphicData,
                            name: $"{nameof(GBA_Actor.GraphicData)}[{i}]"));
            }
        }

        #endregion
    }
}