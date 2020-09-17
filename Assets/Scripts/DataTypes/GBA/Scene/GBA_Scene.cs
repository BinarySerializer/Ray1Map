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

        public byte UnkActorsCount { get; set; }

        public byte BoxActorsCount { get; set; }

        public byte Unk_0A { get; set; }
        public byte UnkSceneStructsCount { get; set; }

        public GBA_Actor[] Always1Actors { get; set; }
        public GBA_Actor[] NormalActors { get; set; }
        public GBA_Actor[] Always2Actors { get; set; }

        public GBA_Actor[] BoxActors { get; set; }
        public GBA_Actor[] UnkActors { get; set; }

        public IEnumerable<GBA_Actor> GetAllActors => Always1Actors.Concat(NormalActors).Concat(Always2Actors);

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

            UnkActorsCount = s.Serialize<byte>(UnkActorsCount, name: nameof(UnkActorsCount));
            BoxActorsCount = s.Serialize<byte>(BoxActorsCount, name: nameof(BoxActorsCount));
            Unk_0A = s.Serialize<byte>(Unk_0A, name: nameof(Unk_0A));
            UnkSceneStructsCount = s.Serialize<byte>(UnkSceneStructsCount, name: nameof(UnkSceneStructsCount));

            if (s.GameSettings.EngineVersion >= EngineVersion.GBA_SplinterCellPandoraTomorrow) {

                Always1Actors = s.SerializeObjectArray<GBA_Actor>(Always1Actors, Always1ActorsCount, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Always1, name: nameof(Always1Actors));
                NormalActors = s.SerializeObjectArray<GBA_Actor>(NormalActors, NormalActorsCount, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Normal, name: nameof(NormalActors));
                BoxActors = s.SerializeObjectArray<GBA_Actor>(BoxActors, BoxActorsCount, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Box, name: nameof(BoxActors));
                Always2Actors = s.SerializeObjectArray<GBA_Actor>(Always2Actors, Always2ActorsCount, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Always2, name: nameof(Always2Actors));
                UnkActors = s.SerializeObjectArray<GBA_Actor>(UnkActors, UnkActorsCount, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Unk, name: nameof(UnkActors));

                UnkSceneStructs = s.SerializeObjectArray<GBA_UnkSceneStruct>(UnkSceneStructs, UnkSceneStructsCount, name: nameof(UnkSceneStructs));
            } else {
                Always1Actors = s.SerializeObjectArray<GBA_Actor>(Always1Actors, Always1ActorsCount, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Always1, name: nameof(Always1Actors));
                NormalActors = s.SerializeObjectArray<GBA_Actor>(NormalActors, NormalActorsCount, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Normal, name: nameof(NormalActors));
                Always2Actors = s.SerializeObjectArray<GBA_Actor>(Always2Actors, Always2ActorsCount, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Always2, name: nameof(Always2Actors));
                BoxActors = s.SerializeObjectArray<GBA_Actor>(BoxActors, BoxActorsCount, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Box, name: nameof(BoxActors));

                UnkSceneStructs = s.SerializeObjectArray<GBA_UnkSceneStruct>(UnkSceneStructs, UnkSceneStructsCount, name: nameof(UnkSceneStructs));
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