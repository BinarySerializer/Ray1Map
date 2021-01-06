using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBA_Scene : GBA_BaseBlock
    {
        #region Level Data

        public byte Index_PlayField { get; set; }
        public byte Unk_01 { get; set; }
        public byte Unk_02 { get; set; }
        public byte IndexMin_ActorModels { get; set; }

        public byte ActorsCountTotal { get; set; }

        public byte ActorsCount1 { get; set; }
        public byte ActorsCount2 { get; set; }
        public byte ActorsCount3 { get; set; }
        public byte ActorsCount4 { get; set; }
        public byte ActorsCount5 { get; set; }

        public byte IndexMax_ActorModels { get; set; }
        public byte SectorsCount { get; set; }

        public GBA_Actor[] MainActors { get; set; }
        public GBA_Actor[] AlwaysActors { get; set; }
        public GBA_Actor[] NormalActors { get; set; }

        public GBA_Actor[] Captors { get; set; }
        public GBA_Actor[] Waypoints { get; set; }
        public GBA_Actor[] UnkActors { get; set; }

        public IEnumerable<GBA_Actor> GetAllActors(GameSettings settings)
        {
            if (settings.EngineVersion >= EngineVersion.GBA_SplinterCellPandoraTomorrow)
                return AlwaysActors.Concat(NormalActors).Concat(Captors).Concat(Waypoints).Concat(UnkActors);
            else if (settings.EngineVersion < EngineVersion.GBA_R3_Proto)
                return MainActors.Concat(NormalActors).Concat(AlwaysActors).Concat(Captors);
            else
                return AlwaysActors.Concat(NormalActors).Concat(Captors);
        }

        public GBA_Knot[] Knots { get; set; }

        public byte[] RemainingData { get; set; }

        #endregion

        #region Parsed

        public GBA_PlayField PlayField { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeBlock(SerializerObject s)
        {
            Index_PlayField = s.Serialize<byte>(Index_PlayField, name: nameof(Index_PlayField));
            Unk_01 = s.Serialize<byte>(Unk_01, name: nameof(Unk_01));
            Unk_02 = s.Serialize<byte>(Unk_02, name: nameof(Unk_02));
            IndexMin_ActorModels = s.Serialize<byte>(IndexMin_ActorModels, name: nameof(IndexMin_ActorModels));

            ActorsCountTotal = s.Serialize<byte>(ActorsCountTotal, name: nameof(ActorsCountTotal));

            ActorsCount1 = s.Serialize<byte>(ActorsCount1, name: nameof(ActorsCount1));
            ActorsCount2 = s.Serialize<byte>(ActorsCount2, name: nameof(ActorsCount2));
            ActorsCount3 = s.Serialize<byte>(ActorsCount3, name: nameof(ActorsCount3));
            ActorsCount4 = s.Serialize<byte>(ActorsCount4, name: nameof(ActorsCount4));
            ActorsCount5 = s.Serialize<byte>(ActorsCount5, name: nameof(ActorsCount5));

            IndexMax_ActorModels = s.Serialize<byte>(IndexMax_ActorModels, name: nameof(IndexMax_ActorModels));

            SectorsCount = s.Serialize<byte>(SectorsCount, name: nameof(SectorsCount));

            if (s.GameSettings.EngineVersion >= EngineVersion.GBA_SplinterCellPandoraTomorrow) {

                AlwaysActors = s.SerializeObjectArray<GBA_Actor>(AlwaysActors, ActorsCount1, onPreSerialize: a => a.Type = GBA_Actor.ActorType.AlwaysActor, name: nameof(AlwaysActors));
                NormalActors = s.SerializeObjectArray<GBA_Actor>(NormalActors, ActorsCount2, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Actor, name: nameof(NormalActors));
                Captors = s.SerializeObjectArray<GBA_Actor>(Captors, ActorsCount5, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Captor, name: nameof(Captors));
                Waypoints = s.SerializeObjectArray<GBA_Actor>(Waypoints, ActorsCount3, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Waypoint, name: nameof(Waypoints));
                UnkActors = s.SerializeObjectArray<GBA_Actor>(UnkActors, ActorsCount4, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Unk, name: nameof(UnkActors));
            }
            else if (s.GameSettings.EngineVersion < EngineVersion.GBA_R3_Proto)
            {
                MainActors = s.SerializeObjectArray<GBA_Actor>(MainActors, ActorsCount1, onPreSerialize: a => a.Type = GBA_Actor.ActorType.MainActor, name: nameof(MainActors));
                NormalActors = s.SerializeObjectArray<GBA_Actor>(NormalActors, ActorsCount2, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Actor, name: nameof(NormalActors));
                AlwaysActors = s.SerializeObjectArray<GBA_Actor>(AlwaysActors, ActorsCount3, onPreSerialize: a => a.Type = GBA_Actor.ActorType.AlwaysActor, name: nameof(AlwaysActors));
                Captors = s.SerializeObjectArray<GBA_Actor>(Captors, ActorsCount5, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Captor, name: nameof(Captors));
            }
            else 
            {
                AlwaysActors = s.SerializeObjectArray<GBA_Actor>(AlwaysActors, ActorsCount1, onPreSerialize: a => a.Type = GBA_Actor.ActorType.AlwaysActor, name: nameof(AlwaysActors));
                NormalActors = s.SerializeObjectArray<GBA_Actor>(NormalActors, ActorsCount2, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Actor, name: nameof(NormalActors));

                if (ActorsCount3 > 0)
                    Debug.LogWarning($"Unparsed actors for count 3 in level {s.GameSettings.Level}");

                Captors = s.SerializeObjectArray<GBA_Actor>(Captors, ActorsCount5, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Captor, name: nameof(Captors));
            }

            Knots = s.SerializeObjectArray<GBA_Knot>(Knots, SectorsCount, name: nameof(Knots));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            // Parse level block data
            if (OffsetTable.Offsets[Index_PlayField] != -1)
                PlayField = s.DoAt(OffsetTable.GetPointer(Index_PlayField), () => s.SerializeObject<GBA_PlayField>(PlayField, name: nameof(PlayField)));

            // Parse actor data
            var actors = GetAllActors(s.GameSettings).ToArray();

            for (var i = 0; i < actors.Length; i++)
            {
                if (actors[i].Type == GBA_Actor.ActorType.Captor)
                {
                    actors[i].CaptorData = s.DoAt(OffsetTable.GetPointer(actors[i].Index_CaptorData, isRelativeOffset: IsGCNBlock),
                        () => s.SerializeObject<GBA_CaptorData>(actors[i].CaptorData,
                        onPreSerialize: bab => {
                            bab.IsGCNBlock = IsGCNBlock;
                            bab.Length = actors[i].LinkedActorsCount;
                            }, name: $"{nameof(GBA_Actor.CaptorData)}[{i}]"));
                }
                else if (actors[i].Type != GBA_Actor.ActorType.Waypoint && actors[i].Type != GBA_Actor.ActorType.Unk && actors[i].Index_ActorModel < OffsetTable.OffsetsCount)
                {
                    actors[i].ActorModel = s.DoAt(OffsetTable.GetPointer(actors[i].Index_ActorModel),
                        () => s.SerializeObject<GBA_ActorModel>(actors[i].ActorModel,
                            name: $"{nameof(GBA_Actor.ActorModel)}[{i}]"));
                }
            }
        }

		public override int GetOffsetTableLengthGCN(SerializerObject s) {
            int max = Index_PlayField + 1;
            // Parse actor data
            var actors = GetAllActors(s.GameSettings).ToArray();

            for (var i = 0; i < actors.Length; i++) {
                if (actors[i].Type == GBA_Actor.ActorType.Captor) {
                    var ind = actors[i].Index_CaptorData;
                    if (ind != 0xFF && (ind + 1) >= max) max = ind + 1;
                } else if (actors[i].Type != GBA_Actor.ActorType.Waypoint && actors[i].Type != GBA_Actor.ActorType.Unk) {
                    var ind = actors[i].Index_ActorModel;
                    if (ind != 0xFF && (ind + 1) >= max) max = ind + 1;
                }
            }
            return max;
		}

		#endregion
	}
}