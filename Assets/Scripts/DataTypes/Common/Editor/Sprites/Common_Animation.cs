namespace R1Engine
{
    /// <summary>
    /// Common sprite animation data
    /// </summary>
    public class Common_Animation
    {
        /// <summary>
        /// The default frame x position for the sprite
        /// </summary>
        public int DefaultFrameXPosition { get; set; }

        /// <summary>
        /// The default frame y position for the sprite
        /// </summary>
        public int DefaultFrameYPosition { get; set; }

        /// <summary>
        /// The default frame width for the sprite
        /// </summary>
        public int DefaultFrameWidth { get; set; }

        /// <summary>
        /// The default frame height for the sprite
        /// </summary>
        public int DefaultFrameHeight { get; set; }

        /// <summary>
        /// The frames in the animation
        /// </summary>
        public Common_AnimationPart[,] Frames { get; set; }
    }
}