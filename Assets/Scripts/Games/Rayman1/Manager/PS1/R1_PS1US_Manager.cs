using BinarySerializer;
using BinarySerializer.Ray1;

namespace Ray1Map.Rayman1
{
    public class R1_PS1US_Manager : R1_PS1_Manager
    {
        public override string ExeFilePath => "SLUS-000.05";
        public override uint? ExeBaseAddress => 0x80125000 - 0x800;
        protected override PS1_ExecutableConfig GetExecutableConfig => PS1_ExecutableConfig.PS1_US;

        public override void AddContextPointers(Context context)
        {
            context.AddPreDefinedPointers(PS1_DefinedPointers.PS1_US);
        }
    }
}