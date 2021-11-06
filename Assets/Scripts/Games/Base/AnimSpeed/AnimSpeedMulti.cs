namespace Ray1Map
{
    public abstract class AnimSpeedMulti : AnimSpeed
    {
        public float[] Speeds { get; set; }
        public override bool HasSpeed() => true;
    }
}