namespace R1Engine
{
    /// <summary>
    /// ROM data for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_ROM : R1Serializable
    {
        /// <summary>
        /// The map data for the current level
        /// </summary>
        public PS1_R1_MapBlock MapData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // TODO: Don't hard-code this!
            var mapPointer = this.Offset + 3175788;

            // Serialize map data
            s.DoAt(mapPointer, () => s.DoEncoded(new RNCEncoder(), () => MapData = s.SerializeObject<PS1_R1_MapBlock>(MapData, name: nameof(MapData))));
        }
    }
}