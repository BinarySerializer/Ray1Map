namespace R1Engine
{
    public class R1_PS1US_Manager : R1_PS1_Manager
    {
        public override string ExeFilePath => "SLUS-000.05";
        public override uint? ExeBaseAddress => 0x80125000 - 0x800;

        public override uint? TypeZDCOffset => ExeBaseAddress + 0x9E294;
        public override uint? ZDCDataOffset => ExeBaseAddress + 0x9D294;
        public override uint? EventFlagsOffset => ExeBaseAddress + 0x9CA94;
        public override uint? LevelBackgroundIndexTableOffset => 0x801c43a4;
        public override uint? WorldInfoOffset => ExeBaseAddress + 0x9EB5C;

        public override FileTableInfo[] FileTableInfos => new FileTableInfo[]
        {
            new FileTableInfo(0x801c4b38,3,R1_PS1_FileType.img_file),
            new FileTableInfo(0x801c4ba4,2,R1_PS1_FileType.ldr_file),
            new FileTableInfo(0x801c4bec,6,R1_PS1_FileType.vdo_file),
            new FileTableInfo(0x801c4cc4,0x35,R1_PS1_FileType.trk_file),
            new FileTableInfo(0x801c5438,5,R1_PS1_FileType.pre_file),
            new FileTableInfo(0x801c54ec,6,R1_PS1_FileType.crd_file),
            new FileTableInfo(0x801c55c4,6,R1_PS1_FileType.gam_file),
            new FileTableInfo(0x801c569c,6,R1_PS1_FileType.vig_wld_file),
            new FileTableInfo(0x801c5774,6,R1_PS1_FileType.wld_file),
            new FileTableInfo(0x801c584c,0x7e,R1_PS1_FileType.map_file),
            new FileTableInfo(0x801c6a04,0x1f,R1_PS1_FileType.fnd_file),
            new FileTableInfo(0x801c6e60,7,R1_PS1_FileType.vab_file),
            new FileTableInfo(0x801c6f5c,7,R1_PS1_FileType.big_file),
            new FileTableInfo(0x801c7058,7,R1_PS1_FileType.vab4sep_file),
            new FileTableInfo(0x801c7154,2,R1_PS1_FileType.filefxs),
            new FileTableInfo(0x801c719c,1,R1_PS1_FileType.ini_file),
        };
    }
}