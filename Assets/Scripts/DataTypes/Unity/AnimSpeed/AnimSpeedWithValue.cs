namespace R1Engine
{
    public abstract class AnimSpeedWithValue : AnimSpeed
    {
        public float Speed { get; set; }
        public override bool HasSpeed() => Speed != 0;
    }
}