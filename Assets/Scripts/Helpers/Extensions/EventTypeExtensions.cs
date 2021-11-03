using BinarySerializer.Ray1;

namespace Ray1Map
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
        public static bool IsHPFrame(this ObjType et) => et == ObjType.TYPE_PUNAISE4 || 
                                                              et == ObjType.TYPE_FALLING_CRAYON ||
                                                              et == ObjType.EDU_ArtworkObject;

        /// <summary>
        /// Indicates if the HitPoints value is the sub-palette to use
        /// </summary>
        /// <param name="et">The event type</param>
        /// <returns></returns>
        public static bool IsMultiColored(this ObjType et) => et == ObjType.TYPE_EDU_LETTRE || 
                                                                   et == ObjType.TYPE_EDU_CHIFFRE ||
                                                                   et == ObjType.MS_compteur || 
                                                                   et == ObjType.MS_wiz_comptage || 
                                                                   et == ObjType.MS_pap;

        /// <summary>
        /// Indicates if the event frame should be retained from the editor
        /// </summary>
        /// <param name="et">The event type</param>
        /// <returns></returns>
        public static bool UsesEditorFrame(this ObjType et) => et == ObjType.TYPE_EDU_LETTRE || 
                                                                    et == ObjType.TYPE_EDU_CHIFFRE;

        /// <summary>
        /// Indicates if the linked event frames should be randomized in order
        /// </summary>
        /// <param name="et">The event type</param>
        /// <returns></returns>
        public static bool UsesRandomFrameLinks(this ObjType et) => et == ObjType.TYPE_HERSE_HAUT ||
                                                                         et == ObjType.TYPE_HERSE_BAS;

        /// <summary>
        /// Indicates if the event frame should be randomized
        /// </summary>
        /// <param name="et">The event type</param>
        /// <returns></returns>
        public static bool UsesRandomFrame(this ObjType et) => et == ObjType.TYPE_CRAYON_BAS || 
                                                                    et == ObjType.TYPE_CRAYON_HAUT ||
                                                                    et == ObjType.TYPE_HERSE_HAUT ||
                                                                    et == ObjType.TYPE_HERSE_BAS;

        /// <summary>
        /// Indicates if the event frame is from the link chain
        /// </summary>
        /// <param name="et">The event type</param>
        /// <returns></returns>
        public static bool UsesFrameFromLinkChain(this ObjType et) => et == ObjType.TYPE_HERSE_BAS_NEXT ||
                                                                           et == ObjType.TYPE_HERSE_HAUT_NEXT;
    }
}