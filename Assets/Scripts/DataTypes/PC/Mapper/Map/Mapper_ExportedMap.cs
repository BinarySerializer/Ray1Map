namespace R1Engine
{
    /// <summary>
    /// Exported map data for the Mapper
    /// </summary>
    public class Mapper_ExportedMap : IBinarySerializable
    {
        #region Header

        /// <summary>
        /// The map header, 4 bytes long
        /// </summary>
        public byte[] Header { get; set; }

        /// <summary>
        /// The pointer to <see cref="MapProperties"/>
        /// </summary>
        public uint MapPropertiesPointer { get; set; }

        /// <summary>
        /// The length of <see cref="MapProperties"/>
        /// </summary>
        public uint MapPropertiesLength { get; set; }

        /// <summary>
        /// The pointer to <see cref="MapData"/>
        /// </summary>
        public uint MapDataPointer { get; set; }

        /// <summary>
        /// The length of <see cref="MapData"/>
        /// </summary>
        public uint MapDataLength { get; set; }

        /// <summary>
        /// The pointer to <see cref="EventMevData"/>
        /// </summary>
        public uint EventMevDataPointer { get; set; }

        /// <summary>
        /// The length of <see cref="EventMevData"/>
        /// </summary>
        public uint EventMevDataLength { get; set; }

        /// <summary>
        /// The pointer to <see cref="EventSevData"/>
        /// </summary>
        public uint EventSevDataPointer { get; set; }

        /// <summary>
        /// The length of <see cref="EventSevData"/>
        /// </summary>
        public uint EventSevDataLength { get; set; }

        #endregion

        #region Content

        /// <summary>
        /// The map properties
        /// </summary>
        public byte[] MapProperties { get; set; }

        /// <summary>
        /// The map data
        /// </summary>
        public Mapper_Map MapData { get; set; }

        public byte[] EventMevData { get; set; }

        public byte[] EventSevData { get; set; }

        #endregion

        #region Serializer Methods

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            // Serialize the header
            serializer.SerializeArray<byte>(nameof(Header), 4);
            serializer.Serialize(nameof(MapPropertiesPointer));
            serializer.Serialize(nameof(MapPropertiesLength));
            serializer.Serialize(nameof(MapDataPointer));
            serializer.Serialize(nameof(MapDataLength));
            serializer.Serialize(nameof(EventMevDataPointer));
            serializer.Serialize(nameof(EventMevDataLength));
            serializer.Serialize(nameof(EventSevDataPointer));
            serializer.Serialize(nameof(EventSevDataLength));

            // Read the data blocks
            serializer.SerializeArray<byte>(nameof(MapProperties), MapPropertiesLength);
            serializer.Serialize(nameof(MapData));
            serializer.SerializeArray<byte>(nameof(EventMevData), EventMevDataLength);
            serializer.SerializeArray<byte>(nameof(EventSevData), EventSevDataLength);
        }

        #endregion
    }
}