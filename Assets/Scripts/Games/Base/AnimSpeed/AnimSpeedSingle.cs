namespace Ray1Map
{
    public abstract class AnimSpeedSingle : AnimSpeed
    {
        public float Speed { get; set; }
        public override bool HasSpeed() => Speed != 0;
    }
}