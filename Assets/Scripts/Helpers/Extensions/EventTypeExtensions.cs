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
        public static bool IsHPFrame(this R1_EventType et) => et == R1_EventType.TYPE_PUNAISE4 || 
                                                              et == R1_EventType.TYPE_FALLING_CRAYON ||
                                                              et == R1_EventType.EDU_ArtworkObject;

        /// <summary>
        /// Indicates if the HitPoints value is the sub-palette to use
        /// </summary>
        /// <param name="et">The event type</param>
        /// <returns></returns>
        public static bool IsMultiColored(this R1_EventType et) => et == R1_EventType.TYPE_EDU_LETTRE || 
                                                                   et == R1_EventType.TYPE_EDU_CHIFFRE ||
                                                                   et == R1_EventType.MS_compteur || 
                                                                   et == R1_EventType.MS_wiz_comptage || 
                                                                   et == R1_EventType.MS_pap;

        /// <summary>
        /// Indicates if the event frame should be retained from the editor
        /// </summary>
        /// <param name="et">The event type</param>
        /// <returns></returns>
        public static bool UsesEditorFrame(this R1_EventType et) => et == R1_EventType.TYPE_EDU_LETTRE || 
                                                                    et == R1_EventType.TYPE_EDU_CHIFFRE;

        /// <summary>
        /// Indicates if the event frame should be randomized
        /// </summary>
        /// <param name="et">The event type</param>
        /// <returns></returns>
        public static bool UsesRandomFrame(this R1_EventType et) => et == R1_EventType.TYPE_CRAYON_BAS || 
                                                                    et == R1_EventType.TYPE_CRAYON_HAUT ||
                                                                    et == R1_EventType.TYPE_HERSE_HAUT ||
                                                                    et == R1_EventType.TYPE_HERSE_BAS;

        /// <summary>
        /// Indicates if the event frame is from the link chain
        /// </summary>
        /// <param name="et">The event type</param>
        /// <returns></returns>
        public static bool UsesFrameFromLinkChain(this R1_EventType et) => et == R1_EventType.TYPE_HERSE_BAS_NEXT ||
                                                                           et == R1_EventType.TYPE_HERSE_HAUT_NEXT;
    }
}