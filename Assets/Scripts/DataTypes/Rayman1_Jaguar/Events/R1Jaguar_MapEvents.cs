namespace R1Engine
{
    /// <summary>
    /// Event map data for Rayman 1 (Jaguar), as well as the event.mev files in Rayman Designer
    /// </summary>
    public class R1Jaguar_MapEvents : R1Serializable
    {
        public bool HasEvents { get; set; } // Always 1?

        // Event map dimensions, always the map size divided by 4
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        // Mapped to a 2D plane based on width and height
        public ushort[] EventIndexMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            HasEvents = s.Serialize<bool>(HasEvents, name: nameof(HasEvents));
            s.SerializeArray<byte>(new byte[3], 3, name: "Padding");

            // Serialize event map dimensions
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            EventIndexMap = s.SerializeArray<ushort>(EventIndexMap, Width * Height, name: nameof(EventIndexMap));
        }
    }
}