using BinarySerializer;
using System.Linq;

namespace Ray1Map.Jade {
	public class AI_Instance : Jade_File {
		public override bool HasHeaderBFFile => true;
		public override string Export_Extension => "oin";

		public Jade_Reference<AI_Model> Model { get; set; }
		public Jade_Reference<AI_Vars> Vars { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Model = s.SerializeObject<Jade_Reference<AI_Model>>(Model, name: nameof(Model))?.Resolve();
			Vars = s.SerializeObject<Jade_Reference<AI_Vars>>(Vars, name: nameof(Vars))?
				.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Flag3);
		}

		public void CheckVariables() {
			var modelVars = Model?.Value?.Vars;
			var instanceVars = Vars?.Value;
			if (instanceVars != null && modelVars != null) {
				foreach (var var in instanceVars?.Vars) {
					var modelVar = modelVars.Vars.FirstOrDefault(v => v.Name == var.Name);
					if (modelVar == null) {
						Context.SystemLogger?.LogWarning($"{Key}: Variable {var.Name}: not found in model!");
					} else {
						if (var.Type != modelVar.Type) {
							Context.SystemLogger?.LogWarning($"{Key}: Variable {var.Name}: type mismatch! ( Instance: {var.Type}, Model: {modelVar.Type} )");
						}
						if (var.Info.ArrayDimensionsCount != modelVar.Info.ArrayDimensionsCount) {
							Context.SystemLogger?.LogWarning($"{Key}: Variable {var.Name}: Array dimensions count mismatch! ( Instance: {var.Info.ArrayDimensionsCount}, Model: {modelVar.Info.ArrayDimensionsCount} )");
						}
						if (var.Info.ArrayLength != modelVar.Info.ArrayLength) {
							Context.SystemLogger?.LogWarning($"{Key}: Variable {var.Name}: Array length mismatch! ( Instance: {var.Info.ArrayLength}, Model: {modelVar.Info.ArrayLength} )");
						}
					}
				}
			}
		}
	}
}
