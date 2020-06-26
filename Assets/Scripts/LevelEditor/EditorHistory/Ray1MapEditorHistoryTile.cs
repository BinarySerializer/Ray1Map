namespace R1Engine
{
    public class Ray1MapEditorHistoryTile
    {
        public Ray1MapEditorHistoryTile(Editor_MapTile item, int xPos, int yPos)
        {
            Item = item;
            XPos = xPos;
            YPos = yPos;
        }

        public Editor_MapTile Item { get; }
        public int XPos { get; }
        public int YPos { get; }
    }
}