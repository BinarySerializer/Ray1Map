namespace R1Engine
{
    public class R1_PS1US_Manager : R1_PS1_Manager
    {
        public override string ExeFilePath => "SLUS-000.05";
        public override uint? ExeBaseAddress => 0x80125000 - 0x800;

        public override uint? TypeZDCOffset => ExeBaseAddress + 0x9E294;
        public override uint? ZDCDataOffset => ExeBaseAddress + 0x9D294;
        public override uint? EventFlagsOffset => ExeBaseAddress + 0x9CA94;

        public override FileTableInfo[] FileTableInfos => new FileTableInfo[]
        {
            new FileTableInfo(0x801c4b38,3,"img_file"),
            new FileTableInfo(0x801c4ba4,2,"ldr_file"),
            new FileTableInfo(0x801c4bec,6,"vdo_file"),
            new FileTableInfo(0x801c4cc4,0x35,"trk_file"),
            new FileTableInfo(0x801c5438,5,"pre_file"),
            new FileTableInfo(0x801c54ec,6,"crd_file"),
            new FileTableInfo(0x801c55c4,6,"gam_file"),
            new FileTableInfo(0x801c569c,6,"vig_wld_file"),
            new FileTableInfo(0x801c5774,6,"wld_file"),
            new FileTableInfo(0x801c584c,0x7e,"map_file[0]"),
            new FileTableInfo(0x801c6a04,0x1f,"fnd_file"),
            new FileTableInfo(0x801c6e60,7,"vab_file"),
            new FileTableInfo(0x801c6f5c,7,"big_file"),
            new FileTableInfo(0x801c7058,7,"vab4sep_file"),
            new FileTableInfo(0x801c7154,2,"filefxs"),
            new FileTableInfo(0x801c719c,1,"ini_file"),
        };
    }
}