using System;

namespace R1Engine
{
    public class GBA_GameCubeMap : R1Serializable
    {
        // Header

        public uint HeaderBlockTotalLength { get; set; }

        public GBA_Scene Scene { get; set; }

        // Data

        public uint DataBlockTotalLength { get; set; }

        public GBA_PlayField PlayField { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Header

            HeaderBlockTotalLength = s.Serialize<uint>(HeaderBlockTotalLength, name: nameof(HeaderBlockTotalLength));

            Scene = SerializeGCNBlock(s, Scene, (obj, length) => ((HeaderBlockTotalLength - length) / 4) - 1);
            Scene.SerializeOffsetData(s);

            // Data

            DataBlockTotalLength = s.Serialize<uint>(DataBlockTotalLength, name: nameof(DataBlockTotalLength));

            PlayField = SerializeGCNBlock<GBA_PlayField>(s, PlayField, (obj, length) => obj.TileKitOffsetIndex + 1);

            var baseOffset = Offset + HeaderBlockTotalLength + 4;

            PlayField.Clusters = new GBA_Cluster[PlayField.ClusterCount];
            PlayField.Layers = new GBA_TileLayer[PlayField.LayerCount];

            for (int i = 0; i < PlayField.ClusterCount; i++)
                PlayField.Clusters[i] = s.DoAt(baseOffset + PlayField.OffsetTable.Offsets[PlayField.ClusterTable[i]], () => SerializeGCNBlock<GBA_Cluster>(s, PlayField.Clusters[i], (obj, length) => 0));

            // Serialize layers
            for (int i = 0; i < PlayField.LayerCount; i++)
            {
                PlayField.Layers[i] = s.DoAt(baseOffset + PlayField.OffsetTable.Offsets[PlayField.LayerTable[i]], () => SerializeGCNBlock<GBA_TileLayer>(s, PlayField.Layers[i], (obj, length) => 0));

                PlayField.Layers[i].Cluster = PlayField.Clusters[PlayField.Layers[i].ClusterIndex];
            }

            // Serialize tilemap
            PlayField.BGTileTable = s.DoAt(baseOffset + PlayField.OffsetTable.Offsets[PlayField.BGTileTableOffsetIndex], () => SerializeGCNBlock<GBA_BGTileTable>(s, PlayField.BGTileTable, (obj, length) => 0));

            // Serialize tilemap (this is the only offset which leads to the ROM data block)
            PlayField.TileKit = s.DoAt(PlayField.OffsetTable.GetPointer(PlayField.TileKitOffsetIndex), () => s.SerializeObject<GBA_TileKit>(PlayField.TileKit, name: nameof(PlayField.TileKit)));
        }

        protected T SerializeGCNBlock<T>(SerializerObject s, T obj, Func<T, uint, long> getOffsetCount)
            where T : GBA_BaseBlock, new()
        {
            var blockLength = s.Serialize<uint>(obj?.BlockSize + 4 ?? default, name: "BlockLength");

            if (obj == null)
                obj = new T()
                {
                    BlockSize = blockLength - 4,
                };

            obj.Init(s.CurrentPointer);

            obj.SerializeBlock(s);

            if (obj.OffsetTable == null)
                obj.OffsetTable = new GBA_OffsetTable();

            obj.OffsetTable.Init(s.CurrentPointer);

            obj.OffsetTable.Offsets = s.SerializeArray<int>(obj.OffsetTable.Offsets, getOffsetCount(obj, blockLength), name: nameof(obj.OffsetTable.Offsets));
            obj.OffsetTable.OffsetsCount = obj.OffsetTable.Offsets.Length;
            obj.OffsetTable.UsedOffsets = new bool[obj.OffsetTable.OffsetsCount];

            return obj;
        }
    }
}