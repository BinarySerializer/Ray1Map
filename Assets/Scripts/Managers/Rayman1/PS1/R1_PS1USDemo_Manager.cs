using BinarySerializer;
using BinarySerializer.Ray1;

namespace R1Engine
{
    public class R1_PS1USDemo_Manager : R1_PS1_Manager
    {
        public override string ExeFilePath => "SLUS-900.01";
        public override uint? ExeBaseAddress => 0x80125000 - 0x800;
        protected override PS1_ExecutableConfig GetExecutableConfig => PS1_ExecutableConfig.PS1_USDemo;

        public override void AddContextPointers(Context context)
        {
            context.AddPreDefinedPointers(PS1_DefinedPointers.PS1_USDemo);
        }
    }
}