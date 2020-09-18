namespace R1Engine
{
    /// <summary>
    /// Common animation frame data
    /// </summary>
    public class Unity_ObjAnimationFrame
    {
        public Unity_ObjAnimationFrame(Unity_ObjAnimationPart[] spriteLayers, Unity_ObjAnimationCollisionPart[] collisionLayers = null)
        {
            SpriteLayers = spriteLayers;
            CollisionLayers = collisionLayers ?? new Unity_ObjAnimationCollisionPart[0];
        }

        /// <summary>
        /// The sprite layers
        /// </summary>
        public Unity_ObjAnimationPart[] SpriteLayers { get; }

        /// <summary>
        /// The collision layers
        /// </summary>
        public Unity_ObjAnimationCollisionPart[] CollisionLayers { get; }
    }
}