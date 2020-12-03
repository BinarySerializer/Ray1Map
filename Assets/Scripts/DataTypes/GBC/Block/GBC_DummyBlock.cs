using System.Linq;

namespace R1Engine
{
    public class GBC_DummyBlock : GBC_BaseBlock
    {
        public byte[] Data { get; set; }
        public GBC_DummyBlock[] SubBlocks { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, BlockSize, name: nameof(Data));

            // Serialize sub-blocks
            if (SubBlocks == null)
                SubBlocks = new GBC_DummyBlock[DependencyTable.DependenciesCount];

            // Get all root pointers
            var rootBlock = ((GBC_BaseManager)s.GameSettings.GetGameManager).GetLevelList(s.Context);
            var rootTable = rootBlock.DependencyTable;
            var rootPointers = Enumerable.Range(0, rootTable.Dependencies.Length).Select(x => rootTable.GetPointer(x)).ToArray();

            for (int i = 0; i < DependencyTable.Dependencies.Length; i++)
            {
                var p = DependencyTable.GetPointer(i);

                if (rootPointers.Contains(p) && Offset != rootBlock.Offset)
                    continue;

                SubBlocks[i] = s.DoAt(p, () => s.SerializeObject<GBC_DummyBlock>(SubBlocks[i], name: $"{nameof(SubBlocks)}[{i}]"));
            }
        }
    }
}