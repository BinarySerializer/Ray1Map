namespace R1Engine.Jade {
	public class AI_Var {
		public int Index { get; set; }
		public AI_VarInfo Info { get; set; }
		public string Name { get; set; }
		public AI_VarEditorInfo EditorInfo { get; set; }
		public AI_VarValue Value { get; set; }

		public AI_Link Link { get; set; }
		public AI_VarType Type { get; set; }

		public void Init() {
			var links = Info.Context.GetStoredObject<AI_Links>("ai");
			Link = links.Links[(uint)Info.Type];
			Type = Link.VarType;
		}
	}
}
