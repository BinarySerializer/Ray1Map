namespace R1Engine
{
    public class Unity_ObjAnimationCollisionPart
    {
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public CollisionType Type { get; set; }

        public enum CollisionType
        {
            AttackBox,
            VulnerabilityBox,

            HitTriggerBox,
            TriggerBox,

            Gendoor,

            // RRR
            SizeChange,
            ExitLevel
        }
    }
}