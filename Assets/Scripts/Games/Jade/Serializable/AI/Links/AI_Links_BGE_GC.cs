using System.Linq;

namespace Ray1Map.Jade {
	public class AI_Links_BGE_GC : AI_Links_BGE_PC {
		protected override void InitFunctionDefs() {
			base.InitFunctionDefs();

			FunctionDefs = FunctionDefs.Concat(new AI_FunctionDef[] {
				new AI_FunctionDef(0x9E007752, "truc_se_deplace_tsd_init"),
				new AI_FunctionDef(0x9E007753, "truc_se_deplace_tsd_move"),
			}).ToArray();
		}
	}
}
