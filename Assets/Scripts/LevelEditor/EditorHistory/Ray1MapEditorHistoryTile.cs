namespace R1Engine
{
    public class Ray1MapEditorHistoryTile
    {
        public Ray1MapEditorHistoryTile(Unity_Tile item, int xPos, int yPos)
        {
            Item = item;
            XPos = xPos;
            YPos = yPos;
        }

        public Unity_Tile Item { get; }
        public int XPos { get; }
        public int YPos { get; }
    }
}