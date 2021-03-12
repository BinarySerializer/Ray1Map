namespace R1Engine
{
    public class Ray1MapEditorHistoryTile
    {
        public Ray1MapEditorHistoryTile(Unity_Tile item, int xPos, int yPos, int layerIndex)
        {
            Item = item;
            XPos = xPos;
            YPos = yPos;
            LayerIndex = layerIndex;
        }

        public Unity_Tile Item { get; }
        public int XPos { get; }
        public int YPos { get; }
        public int LayerIndex { get; }
    }
}