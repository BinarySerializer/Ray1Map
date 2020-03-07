namespace R1Engine
{
    /// <summary>
    /// Common sprite data
    /// </summary>
    public class Common_Sprite
    {
        /// <summary>
        /// The sprite animations
        /// </summary>
        public Common_Animation[] Animations { get; set; }

        /// <summary>
        /// The default animation index for the sprite
        /// </summary>
        public int DefaultAnimation { get; set; }
    }
}