namespace R1Engine.Jade {
	public abstract class Jade_File : R1Serializable {
		public LOA_Loader Loader { get; set; }
		public Jade_Key Key { get; set; }
		public uint FileSize { get; set; }
	}
}
