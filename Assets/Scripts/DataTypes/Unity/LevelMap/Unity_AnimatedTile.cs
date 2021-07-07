namespace R1Engine
{
    public class Unity_AnimatedTile
    {
        public float AnimationSpeed { get; set; } // In frames, for 60FPS
        public float[] AnimationSpeeds { get; set; }
        public int[] TileIndices { get; set; }
        public bool IgnoreFirstTile { get; set; }

        public class Instance {
            public Unity_AnimatedTile animatedTile;
            public int x;
            public int y;
            public int tileIndex;
            public float currentTimer;
            public int? combinedTileIndex;

            public Instance(Unity_AnimatedTile animatedTile, int tileIndex) {
                this.animatedTile = animatedTile;
                this.tileIndex = tileIndex;
            }
        }
    }
}