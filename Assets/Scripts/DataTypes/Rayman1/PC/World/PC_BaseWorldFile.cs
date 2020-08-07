namespace R1Engine
{
    /// <summary>
    /// Base world data for PC
    /// </summary>
    public abstract class PC_BaseWorldFile : PC_BaseFile
    {
        /// <summary>
        /// The amount of DES items
        /// </summary>
        public ushort DesItemCount { get; set; }

        /// <summary>
        /// The DES items
        /// </summary>
        public PC_DES[] DesItems { get; set; }

        /// <summary>
        /// The ETA items
        /// </summary>
        public PC_ETA[] Eta { get; set; }
    }
}