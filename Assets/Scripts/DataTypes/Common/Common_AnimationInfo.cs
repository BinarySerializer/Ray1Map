namespace R1Engine
{
    /// <summary>
    /// Common animation info
    /// </summary>
    public class Common_AnimationInfo
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="animationIndex">AnimationIndex</param>
        /// <param name="animationSpeed">AnimationSpeed</param>
        public Common_AnimationInfo(int animationIndex, int animationSpeed)
        {
            AnimationIndex = animationIndex;
            AnimationSpeed = animationSpeed;
        }

        /// <summary>
        /// The animation index
        /// </summary>
        public int AnimationIndex { get; }
        
        /// <summary>
        /// The animation speed
        /// </summary>
        public int AnimationSpeed { get; }
    }
}