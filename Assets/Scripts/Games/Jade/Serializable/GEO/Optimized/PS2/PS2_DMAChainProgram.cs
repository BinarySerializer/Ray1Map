using BinarySerializer;

namespace Ray1Map.Jade {
    public class PS2_DMAChainProgram : BinarySerializable {
		public int DMAChainProgramID { get; set; } // Used as address (ADDR = ID << 24)
		public uint DataSize { get; set; }
		//public byte[] Bytes { get; set; }
		public PS2_DMACommand[] Commands { get; set; }
		public override void SerializeImpl(SerializerObject s) {
			DMAChainProgramID = s.Serialize<int>(DMAChainProgramID, name: nameof(DMAChainProgramID));
			DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
			//Bytes = s.SerializeArray<byte>(Bytes, DataSize, name: nameof(Bytes));

			Commands = s.SerializeObjectArrayUntil<PS2_DMACommand>(Commands, gc => s.CurrentAbsoluteOffset >= Offset.AbsoluteOffset + 8 + DataSize, name: nameof(Commands));
			if (s.CurrentAbsoluteOffset > Offset.AbsoluteOffset + 8 + DataSize) {
				s.SystemLogger?.LogWarning($"{Offset}: Read too many DMA commands");
			}
			s.Goto(Offset + 8 + DataSize);
		}
	}
}