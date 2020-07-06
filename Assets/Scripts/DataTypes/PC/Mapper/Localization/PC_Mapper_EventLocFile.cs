namespace R1Engine
{
    /// <summary>
    /// Event localization data for Rayman Mapper (PC)
    /// </summary>
    public class PC_Mapper_EventLocFile : PC_BaseFile
    {
        /// <summary>
        /// The amount of localization items
        /// </summary>
        public uint LocCount { get; set; }

        /// <summary>
        /// Unknown header values
        /// </summary>
        public ushort[][] UnkHeader { get; set; }

        /// <summary>
        /// The localization items
        /// </summary>
        public PC_Mapper_EventLocItem[] LocItems { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // Serialize header
            base.SerializeImpl(s);

            // Serialize the count
            LocCount = s.Serialize<uint>(LocCount, name: nameof(LocCount));

            // Serialize unknown header
            if (UnkHeader == null)
                UnkHeader = new ushort[LocCount][];

            for (int i = 0; i < UnkHeader.Length; i++)
                UnkHeader[i] = s.SerializeArray<ushort>(UnkHeader[i], 3, name: $"{nameof(UnkHeader)}[{i}]");

            // Serialize the localization items
            LocItems = s.SerializeObjectArray<PC_Mapper_EventLocItem>(LocItems, LocCount, name: nameof(LocItems));
        }
    }
}