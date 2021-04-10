namespace R1Engine
{
    public abstract class Jade_BGE_Manager : Jade_BaseManager 
    {
        // Version properties
        public override string[] BFFiles => new string[] {
            "SALLY.BF"
        };

        public override string[] FixWorlds => new string[] {
            "_main_fix"
        };

        public override string JadeSpePath => "jade.spe";
    }
}