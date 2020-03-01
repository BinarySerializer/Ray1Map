namespace R1Engine
{
    /// <summary>
    /// Defines common event information
    /// </summary>
    public class Common_EventInfo
    {
        /// <summary>
        /// The event type
        /// </summary>
        public int Type { get; set; }
        
        /// <summary>
        /// The Etat index
        /// </summary>
        public int Etat { get; set; }
        
        /// <summary>
        /// The SubEtat index. Sometimes this is a string.
        /// </summary>
        public string SubEtat { get; set; }
        
        /// <summary>
        /// The localized name from Rayman Designer
        /// </summary>
        public string DesignerName { get; set; }

        /// <summary>
        /// The custom name, if none was found in Rayman Designer
        /// </summary>
        public string CustomName { get; set; }
        
        /// <summary>
        /// The localized description from Rayman Designer
        /// </summary>
        public string DesignerDescription { get; set; }
        
        /// <summary>
        /// Indicates if the event is an always event or not
        /// </summary>
        public bool IsAlways { get; set; }
    }
}