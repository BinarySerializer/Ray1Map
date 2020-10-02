using System;

namespace R1Engine
{
    public class GBA_GameCubeMap : R1Serializable
    {
        // Header

        public uint SceneBlockLength { get; set; }

        public GBA_Scene Scene { get; set; }

        // Data

        public uint PlayFieldBlockLength { get; set; }

        public GBA_PlayField PlayField { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Header
            SceneBlockLength = s.Serialize<uint>(SceneBlockLength, name: nameof(SceneBlockLength));
            Scene = s.SerializeObject<GBA_Scene>(Scene, onPreSerialize: sc => sc.IsGCNBlock = true, name: nameof(Scene));

            // Data
            s.Goto(Offset + SceneBlockLength);
            PlayFieldBlockLength = s.Serialize<uint>(PlayFieldBlockLength, name: nameof(PlayFieldBlockLength));
            PlayField = s.SerializeObject<GBA_PlayField>(PlayField, onPreSerialize: pf => pf.IsGCNBlock = true, name: nameof(PlayField));
        }
    }
}