using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Map2D_LayersBlock : GBAVV_BaseBlock
    {
        public GBAVV_Map2D_Data MapData { get; set; } // Set before serializing

        public GBAVV_TileLayerData[] TileLayerDatas { get; set; }
        public GBAVV_TileLayerData CollisionLayerData { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            var basePointer = s.CurrentPointer;

            if (TileLayerDatas == null)
                TileLayerDatas = new GBAVV_TileLayerData[MapData.MapLayers.Length];

            for (int i = 0; i < TileLayerDatas.Length; i++)
            {
                if (MapData.MapLayers[i] != null)
                    TileLayerDatas[i] = s.DoAt(basePointer + MapData.MapLayers[i].DataBlockOffset, () => s.SerializeObject<GBAVV_TileLayerData>(TileLayerDatas[i], x => x.MapLayer = MapData.MapLayers[i], name: $"{nameof(TileLayerDatas)}[{i}]"));
            }

            if (MapData.CollisionDataPointer != null)
                CollisionLayerData = s.DoAt(basePointer + MapData.CollisionLayer.DataBlockOffset, () => s.SerializeObject<GBAVV_TileLayerData>(CollisionLayerData, x => x.MapLayer = MapData.CollisionLayer, name: nameof(CollisionLayerData)));

            s.Goto(basePointer + BlockLength);
        }

        public class GBAVV_TileLayerData : BinarySerializable
        {
            public GBAVV_Map2D_MapLayer MapLayer { get; set; } // Set before serializing

            public ushort[] Offsets { get; set; }

            public GBAVV_TileMapTileCommands[] TileMapTileCommands { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Offsets = s.SerializeArray<ushort>(Offsets, MapLayer.TileMap.Max() + 1, name: nameof(Offsets));

                if (TileMapTileCommands == null)
                    TileMapTileCommands = new GBAVV_TileMapTileCommands[Offsets.Length];

                for (int i = 0; i < TileMapTileCommands.Length; i++)
                    TileMapTileCommands[i] = s.DoAt(Offset + Offsets[i] * 4, () => s.SerializeObject<GBAVV_TileMapTileCommands>(TileMapTileCommands[i], name: $"{nameof(TileMapTileCommands)}[{i}]"));
            }

            public class GBAVV_TileMapTileCommands : BinarySerializable
            {
                public GBAVV_TileCommand[] TileCommands { get; set; }

                public override void SerializeImpl(SerializerObject s)
                {
                    if (TileCommands == null)
                    {
                        var cmds = new List<GBAVV_TileCommand>();
                        var count = 0;
                        var index = 0;

                        while (count < 128)
                        {
                            var c = s.SerializeObject<GBAVV_TileCommand>(default, name: $"{nameof(TileCommands)}[{index++}]");
                            count += c.TilesCount;
                            cmds.Add(c);
                        }

                        TileCommands = cmds.ToArray();
                    }
                    else
                    {
                        s.SerializeObjectArray<GBAVV_TileCommand>(TileCommands, TileCommands.Length, name: nameof(TileCommands));
                    }
                }

                public class GBAVV_TileCommand : BinarySerializable
                {
                    public byte TilesCount { get; set; }
                    public byte CommandType { get; set; }
                    public ushort[] TileIndices { get; set; }
                    public sbyte[] TileChanges { get; set; }

                    public override void SerializeImpl(SerializerObject s)
                    {
                        TilesCount = s.Serialize<byte>(TilesCount, name: nameof(TilesCount));
                        CommandType = s.Serialize<byte>(CommandType, name: nameof(CommandType));

                        if ((CommandType & 0x80) == 0 && (CommandType & 0x40) == 0)
                            TileIndices = s.SerializeArray<ushort>(TileIndices, TilesCount, name: nameof(TileIndices));
                        else
                            TileIndices = s.SerializeArray<ushort>(TileIndices, 1, name: nameof(TileIndices));

                        if ((CommandType & 0x40) != 0)
                        {
                            TileChanges = s.SerializeArray<sbyte>(TileChanges, TilesCount - 1, name: nameof(TileChanges));
                            s.Align(2);
                        }
                    }
                }
            }
        }
    }
}