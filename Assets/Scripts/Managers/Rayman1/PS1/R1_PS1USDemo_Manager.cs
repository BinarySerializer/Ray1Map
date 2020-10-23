namespace R1Engine
{
    public class R1_PS1USDemo_Manager : R1_PS1_Manager
    {
        public override string ExeFilePath => "SLUS-900.01";
        public override uint? ExeBaseAddress => 0x80125000 - 0x800;

        public override uint? TypeZDCOffset => ExeBaseAddress + 0x9BDF8;
        public override uint? ZDCDataOffset => ExeBaseAddress + 0x9ADF8;
        public override uint? EventFlagsOffset => ExeBaseAddress + 0x9A5F8;
        public override uint? LevelBackgroundIndexTableOffset => ExeBaseAddress + 0x9D708;
        public override uint? WorldInfoOffset => ExeBaseAddress + 0x9C6C0;

        public override FileTableInfo[] FileTableInfos => new FileTableInfo[]
        {
            new FileTableInfo(0x801c269c,3,R1_PS1_FileType.img_file),
            new FileTableInfo(0x801c2708,2,R1_PS1_FileType.ldr_file),
            new FileTableInfo(0x801c2750,6,R1_PS1_FileType.vdo_file),
            new FileTableInfo(0x801c2828,0x35,R1_PS1_FileType.trk_file),
            new FileTableInfo(0x801c2f9c,5,R1_PS1_FileType.pre_file),
            new FileTableInfo(0x801c3050,6,R1_PS1_FileType.crd_file),
            new FileTableInfo(0x801c3128,6,R1_PS1_FileType.gam_file),
            new FileTableInfo(0x801c3200,6,R1_PS1_FileType.vig_wld_file),
            new FileTableInfo(0x801c32d8,6,R1_PS1_FileType.wld_file),
            new FileTableInfo(0x801c33b0,0x7e,R1_PS1_FileType.map_file),
            new FileTableInfo(0x801c4568,0x1f,R1_PS1_FileType.fnd_file ),
            new FileTableInfo(0x801c49c4,7,R1_PS1_FileType.vab_file),
            new FileTableInfo(0x801c4ac0,7,R1_PS1_FileType.big_file),
            new FileTableInfo(0x801c4bbc,7,R1_PS1_FileType.vab4sep_file),
            new FileTableInfo(0x801c4cb8,2,R1_PS1_FileType.filefxs),
            new FileTableInfo(0x801c4d00,1,R1_PS1_FileType.ini_file),
        };
    }
}