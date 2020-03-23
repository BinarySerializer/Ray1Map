namespace R1Engine
{
    /// <summary>
    /// Exported map data for the Mapper
    /// </summary>
    public class Mapper_ExportedMap : R1Serializable
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
        public override void SerializeImpl(SerializerObject s) {
            // Serialize the header
            Header = s.SerializeArray<byte>(Header, 4, name: "Header");
            MapPropertiesPointer = s.Serialize<uint>(MapPropertiesPointer, name: "MapPropertiesPointer");
            MapPropertiesLength = s.Serialize<uint>(MapPropertiesLength, name: "MapPropertiesLength");
            MapDataPointer = s.Serialize<uint>(MapDataPointer, name: "MapDataPointer");
            MapDataLength = s.Serialize<uint>(MapDataLength, name: "MapDataLength");
            EventMevDataPointer = s.Serialize<uint>(EventMevDataPointer, name: "EventMevDataPointer");
            EventMevDataLength = s.Serialize<uint>(EventMevDataLength, name: "EventMevDataLength");
            EventSevDataPointer = s.Serialize<uint>(EventSevDataPointer, name: "EventSevDataPointer");
            EventSevDataLength = s.Serialize<uint>(EventSevDataLength, name: "EventSevDataLength");

            // Read the data blocks
            MapProperties = s.SerializeArray<byte>(MapProperties, MapPropertiesLength, name: "MapProperties");
            MapData = s.Serialize<Mapper_Map>(MapData, name: "MapData");
            EventMevData = s.SerializeArray<byte>(EventMevData, EventMevDataLength, name: "EventMevData");
            EventSevData = s.SerializeArray<byte>(EventSevData, EventSevDataLength, name: "EventSevData");
        }

        #endregion
    }
}