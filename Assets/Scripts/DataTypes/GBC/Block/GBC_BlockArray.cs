using BinarySerializer;

namespace R1Engine
{
    public class GBC_BlockArray<T> : GBC_BaseBlock
        where T : GBC_BaseBlock, new()
    {
        public T[] Blocks { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            // Serialize blocks
            if (Blocks == null)
                Blocks = new T[DependencyTable.DependenciesCount];

            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = s.DoAt(DependencyTable.GetPointer(i), () => s.SerializeObject<T>(Blocks[i], name: $"{nameof(Blocks)}[{i}]"));
        }
    }
}