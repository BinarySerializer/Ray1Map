namespace R1Engine
{
    /// <summary>
    /// Extension methods for event types
    /// </summary>
    public static class EventTypeExtensions
    {
        /// <summary>
        /// Indicates if the HitPoints value is the current frame
        /// </summary>
        /// <param name="et">The event type</param>
        /// <returns></returns>
        public static bool IsHPFrame(this EventType et) => et == EventType.TYPE_PUNAISE4 || 
                                                           et == EventType.TYPE_FALLING_CRAYON ||
                                                           et == EventType.EDU_ArtworkObject;

        /// <summary>
        /// Indicates if the HitPoints value is the sub-palette to use
        /// </summary>
        /// <param name="et">The event type</param>
        /// <returns></returns>
        public static bool IsMultiColored(this EventType et) => et == EventType.TYPE_EDU_LETTRE || 
                                                                et ==EventType.MS_compteur || 
                                                                et ==EventType.MS_wiz_comptage || 
                                                                et ==EventType.MS_pap;

        /// <summary>
        /// Indicates if the event frame should be retained from the editor
        /// </summary>
        /// <param name="et">The event type</param>
        /// <returns></returns>
        public static bool UsesEditorFrame(this EventType et) => et == EventType.TYPE_EDU_LETTRE || 
                                                                 et ==EventType.TYPE_EDU_CHIFFRE;
    }
}