namespace Ray1Map
{
    public class BaseLegacyEditorWrapper
    {
        public virtual ushort Type { get; set; }
        public virtual int DES { get; set; }
        public virtual int ETA { get; set; }
        public virtual byte Etat { get; set; }
        public virtual byte SubEtat { get; set; }
        public virtual int EtatLength => 0;
        public virtual int SubEtatLength => 0;
        public virtual byte OffsetBX { get; set; }
        public virtual byte OffsetBY { get; set; }
        public virtual byte OffsetHY { get; set; }
        public virtual byte FollowSprite { get; set; }
        public virtual uint HitPoints { get; set; }
        public virtual byte HitSprite { get; set; }
        public virtual bool FollowEnabled { get; set; }
    }
}