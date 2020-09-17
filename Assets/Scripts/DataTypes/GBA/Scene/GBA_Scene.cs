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

        public byte ActorsCount1 { get; set; }
        public byte ActorsCount2 { get; set; }
        public byte ActorsCount3 { get; set; }
        public byte ActorsCount4 { get; set; }
        public byte ActorsCount5 { get; set; }

        public byte Unk_0A { get; set; }
        public byte UnkSceneStructsCount { get; set; }

        public GBA_Actor[] MainActors { get; set; }
        public GBA_Actor[] AlwaysActors { get; set; }
        public GBA_Actor[] NormalActors { get; set; }

        public GBA_Actor[] BoxTriggerActors { get; set; }
        public GBA_Actor[] TriggerActors { get; set; }
        public GBA_Actor[] UnkActors { get; set; }

        public IEnumerable<GBA_Actor> GetAllActors(GameSettings settings)
        {
            if (settings.EngineVersion >= EngineVersion.GBA_SplinterCellPandoraTomorrow)
                return AlwaysActors.Concat(NormalActors).Concat(BoxTriggerActors).Concat(TriggerActors).Concat(UnkActors);
            else if (settings.EngineVersion < EngineVersion.GBA_R3_Proto)
                return MainActors.Concat(NormalActors).Concat(AlwaysActors).Concat(BoxTriggerActors);
            else
                return AlwaysActors.Concat(NormalActors).Concat(BoxTriggerActors);
        }

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

            ActorsCount1 = s.Serialize<byte>(ActorsCount1, name: nameof(ActorsCount1));
            ActorsCount2 = s.Serialize<byte>(ActorsCount2, name: nameof(ActorsCount2));
            ActorsCount3 = s.Serialize<byte>(ActorsCount3, name: nameof(ActorsCount3));
            ActorsCount4 = s.Serialize<byte>(ActorsCount4, name: nameof(ActorsCount4));
            ActorsCount5 = s.Serialize<byte>(ActorsCount5, name: nameof(ActorsCount5));

            Unk_0A = s.Serialize<byte>(Unk_0A, name: nameof(Unk_0A));

            UnkSceneStructsCount = s.Serialize<byte>(UnkSceneStructsCount, name: nameof(UnkSceneStructsCount));

            if (s.GameSettings.EngineVersion >= EngineVersion.GBA_SplinterCellPandoraTomorrow) {

                AlwaysActors = s.SerializeObjectArray<GBA_Actor>(AlwaysActors, ActorsCount1, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Always, name: nameof(AlwaysActors));
                NormalActors = s.SerializeObjectArray<GBA_Actor>(NormalActors, ActorsCount2, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Normal, name: nameof(NormalActors));
                BoxTriggerActors = s.SerializeObjectArray<GBA_Actor>(BoxTriggerActors, ActorsCount5, onPreSerialize: a => a.Type = GBA_Actor.ActorType.BoxTrigger, name: nameof(BoxTriggerActors));
                TriggerActors = s.SerializeObjectArray<GBA_Actor>(TriggerActors, ActorsCount3, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Trigger, name: nameof(TriggerActors));
                UnkActors = s.SerializeObjectArray<GBA_Actor>(UnkActors, ActorsCount4, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Unk, name: nameof(UnkActors));
            }
            else if (s.GameSettings.EngineVersion < EngineVersion.GBA_R3_Proto)
            {
                MainActors = s.SerializeObjectArray<GBA_Actor>(MainActors, ActorsCount1, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Main, name: nameof(MainActors));
                NormalActors = s.SerializeObjectArray<GBA_Actor>(NormalActors, ActorsCount2, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Normal, name: nameof(NormalActors));
                AlwaysActors = s.SerializeObjectArray<GBA_Actor>(AlwaysActors, ActorsCount3, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Always, name: nameof(AlwaysActors));
                BoxTriggerActors = s.SerializeObjectArray<GBA_Actor>(BoxTriggerActors, ActorsCount5, onPreSerialize: a => a.Type = GBA_Actor.ActorType.BoxTrigger, name: nameof(BoxTriggerActors));
            }
            else 
            {
                AlwaysActors = s.SerializeObjectArray<GBA_Actor>(AlwaysActors, ActorsCount1, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Always, name: nameof(AlwaysActors));
                NormalActors = s.SerializeObjectArray<GBA_Actor>(NormalActors, ActorsCount2, onPreSerialize: a => a.Type = GBA_Actor.ActorType.Normal, name: nameof(NormalActors));

                if (ActorsCount3 > 0)
                    Debug.LogWarning($"Unparsed actors for count 3 in level {s.GameSettings.Level}");

                BoxTriggerActors = s.SerializeObjectArray<GBA_Actor>(BoxTriggerActors, ActorsCount5, onPreSerialize: a => a.Type = GBA_Actor.ActorType.BoxTrigger, name: nameof(BoxTriggerActors));
            }

            UnkSceneStructs = s.SerializeObjectArray<GBA_UnkSceneStruct>(UnkSceneStructs, UnkSceneStructsCount, name: nameof(UnkSceneStructs));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            // Parse level block data
            if (OffsetTable.Offsets[PlayFieldIndex] != -1)
                PlayField = s.DoAt(OffsetTable.GetPointer(PlayFieldIndex), () => s.SerializeObject<GBA_PlayField>(PlayField, name: nameof(PlayField)));

            // Parse actor data
            var actors = GetAllActors(s.GameSettings).ToArray();

            for (var i = 0; i < actors.Length; i++)
            {
                if (actors[i].Type == GBA_Actor.ActorType.BoxTrigger || actors[i].Type == GBA_Actor.ActorType.Trigger || actors[i].Type == GBA_Actor.ActorType.Unk)
                    continue;

                if (actors[i].GraphicsDataIndex < OffsetTable.OffsetsCount)
                    actors[i].GraphicData = s.DoAt(OffsetTable.GetPointer(actors[i].GraphicsDataIndex),
                        () => s.SerializeObject<GBA_ActorGraphicData>(actors[i].GraphicData,
                            name: $"{nameof(GBA_Actor.GraphicData)}[{i}]"));
            }
        }

        #endregion
    }
}