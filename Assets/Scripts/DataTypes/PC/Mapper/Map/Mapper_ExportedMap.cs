using System;

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
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            // Read the header
            Header = deserializer.ReadArray<byte>(4);
            MapPropertiesPointer = deserializer.Read<uint>();
            MapPropertiesLength = deserializer.Read<uint>();
            MapDataPointer = deserializer.Read<uint>();
            MapDataLength = deserializer.Read<uint>();
            EventMevDataPointer = deserializer.Read<uint>();
            EventMevDataLength = deserializer.Read<uint>();
            EventSevDataPointer = deserializer.Read<uint>();
            EventSevDataLength = deserializer.Read<uint>();

            // Read the data blocks
            MapProperties = deserializer.ReadArray<byte>(MapPropertiesLength);
            MapData = deserializer.Read<Mapper_Map>();
            EventMevData = deserializer.ReadArray<byte>(EventMevDataLength);
            EventSevData = deserializer.ReadArray<byte>(EventSevDataLength);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}