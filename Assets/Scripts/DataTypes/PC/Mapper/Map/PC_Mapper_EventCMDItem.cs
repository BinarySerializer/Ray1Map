namespace R1Engine
{
    /// <summary>
    /// Event CMD item data for the Mapper
    /// </summary>
    public class PC_Mapper_EventCMDItem
    {
        public string Name { get; set; }

        public string[] IfCommand { get; set; }

        public string ETAFile { get; set; }

        public int[] EventCommands { get; set; }

        public int XPosition { get; set; }

        public int YPosition { get; set; }

        public int Unk1 { get; set; }
        
        public int LinkID { get; set; }

        public uint Etat { get; set; }

        public string SubEtat { get; set; }

        public uint Offset_BX { get; set; }

        public uint Offset_BY { get; set; }

        public uint Offset_HY { get; set; }

        public uint Follow_enabled { get; set; }

        public uint Follow_sprite { get; set; }

        public uint Hitpoints { get; set; }

        public string Obj_type { get; set; }

        public uint Hit_sprite { get; set; }

        public int DesignerGroup { get; set; }
    }
}