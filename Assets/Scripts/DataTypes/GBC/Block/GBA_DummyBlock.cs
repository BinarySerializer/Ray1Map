using System.Linq;

namespace R1Engine
{
    public class GBC_DummyBlock : GBC_BaseBlock
    {
        public byte[] Data { get; set; }
        public GBC_DummyBlock[] SubBlocks { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);

            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1)
                Data = s.SerializeArray<byte>(Data, GBC_DataLength, name: nameof(Data));

            // Serialize sub-blocks
            if (SubBlocks == null)
                SubBlocks = new GBC_DummyBlock[OffsetTable.OffsetsCount];

            // Get all root pointers
            var rootBlock = ((GBC_BaseManager)s.GameSettings.GetGameManager).GetSceneList(s.Context);
            var rootTable = rootBlock.OffsetTable;
            var rootPointers = Enumerable.Range(0, rootTable.Offsets.Length).Select(x => rootTable.GetPointer(x)).ToArray();

            for (int i = 0; i < OffsetTable.Offsets.Length; i++)
            {
                var p = OffsetTable.GetPointer(i);

                if (rootPointers.Contains(p) && Offset != rootBlock.Offset)
                    continue;

                SubBlocks[i] = s.DoAt(p, () => s.SerializeObject<GBC_DummyBlock>(SubBlocks[i], name: $"{nameof(SubBlocks)}[{i}]"));
            }
        }
    }
}