namespace R1Engine
{
    public class R1_PS1USDemo_Manager : R1_PS1_Manager
    {
        public override string ExeFilePath => "SLUS-900.01";
        public override uint? ExeBaseAddress => 0x80125000 - 0x800;

        public override uint? TypeZDCOffset => ExeBaseAddress + 0x9BDF8;
        public override uint? ZDCDataOffset => ExeBaseAddress + 0x9ADF8;
        public override uint? EventFlagsOffset => ExeBaseAddress + 0x9A5F8;

        public override FileTableInfo[] FileTableInfos => new FileTableInfo[]
        {
            new FileTableInfo(0x801c269c,3,"img_file"),
            new FileTableInfo(0x801c2708,2,"ldr_file"),
            new FileTableInfo(0x801c2750,6,"vdo_file"),
            new FileTableInfo(0x801c2828,0x35,"trk_file"),
            new FileTableInfo(0x801c2f9c,5,"pre_file"),
            new FileTableInfo(0x801c3050,6,"crd_file"),
            new FileTableInfo(0x801c3128,6,"gam_file"),
            new FileTableInfo(0x801c3200,6,"vig_wld_file"),
            new FileTableInfo(0x801c32d8,6,"wld_file"),
            new FileTableInfo(0x801c33b0,0x7e,"map_file[0]"),
            new FileTableInfo(0x801c4568,0x1f,"fnd_file"),
            new FileTableInfo(0x801c49c4,7,"vab_file"),
            new FileTableInfo(0x801c4ac0,7,"big_file"),
            new FileTableInfo(0x801c4bbc,7,"vab4sep_file"),
            new FileTableInfo(0x801c4cb8,2,"filefxs"),
            new FileTableInfo(0x801c4d00,1,"ini_file"),
        };
    }
}