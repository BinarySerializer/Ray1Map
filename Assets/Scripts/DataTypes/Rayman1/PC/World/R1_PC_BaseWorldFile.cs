namespace R1Engine
{
    /// <summary>
    /// Base world data for PC
    /// </summary>
    public abstract class R1_PC_BaseWorldFile : R1_PCBaseFile
    {
        /// <summary>
        /// The amount of DES items
        /// </summary>
        public ushort DesItemCount { get; set; }

        /// <summary>
        /// The DES items
        /// </summary>
        public R1_PC_DES[] DesItems { get; set; }

        /// <summary>
        /// The ETA items
        /// </summary>
        public R1_PC_ETA[] Eta { get; set; }
    }
}