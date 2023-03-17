namespace Ray1Map.Jade {
	public class AI_Link {
		public uint Key { get; set; }
		public uint Size { get; set; }
		public string Name { get; set; }

		public AI_Link(uint key, uint size, string name, AI_VarType? type = null) {
			Key = key;
			Size = size;
			Name = name;
			OverrideType = type;
		}

		public AI_VarType? OverrideType { get; set; }

		public AI_VarType VarType {
			get {
				if(OverrideType.HasValue) return OverrideType.Value;
				if(Size == 0) return AI_VarType.None;
				return AI_VarType.Unknown;
			}
		}
	}
}
