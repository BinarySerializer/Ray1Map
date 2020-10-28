namespace R1Engine
{
    public interface ILegacyEditorWrapper
    {
        ushort Type { get; set; }
        int DES { get; set; }
        int ETA { get; set; }
        byte Etat { get; set; }
        byte SubEtat { get; set; }
        int EtatLength { get; }
        int SubEtatLength { get; }
        byte OffsetBX { get; set; }
        byte OffsetBY { get; set; }
        byte OffsetHY { get; set; }
        byte FollowSprite { get; set; }
        uint HitPoints { get; set; }
        byte HitSprite { get; set; }
        bool FollowEnabled { get; set; }
    }
}