namespace R1Engine
{
    /// <summary>
    /// World data for EDU on PS1
    /// </summary>
    public class PS1_EDU_WorldFile : R1Serializable
    {
        #region Public Properties

        public ushort BG1 { get; set; }

        public ushort BG2 { get; set; }

        public byte Plan0NumPcxCount { get; set; }

        public byte[][] Plan0NumPcx { get; set; }

        public ushort DESCount { get; set; }
        
        public byte ETACount { get; set; }

        public uint DESBlockLength { get; set; }

        public PS1_EDU_DESData[] DESData { get; set; }

        public byte[] Unk7 { get; set; }

        public uint ETABlockLength { get; set; }

        public Pointer ETABlockPointer { get; set; }

        /// <summary>
        /// The event states for every ETA
        /// </summary>
        public Common_EventState[][][] ETA { get; set; }

        public byte ETAStateCountTableCount { get; set; }

        public byte[] ETAStateCountTable { get; set; }

        public byte ETASubStateCountTableCount { get; set; }

        public byte[] ETASubStateCountTable { get; set; }

        public uint Unk10Length { get; set; }

        public ushort[] Unk10 { get; set; }

        public byte[][] UnkBlock6 { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // Serialize header
            BG1 = s.Serialize<ushort>(BG1, name: nameof(BG1));
            BG2 = s.Serialize<ushort>(BG2, name: nameof(BG2));
            Plan0NumPcxCount = s.Serialize<byte>(Plan0NumPcxCount, name: nameof(Plan0NumPcxCount));

            if (Plan0NumPcx == null)
                Plan0NumPcx = new byte[Plan0NumPcxCount][];

            for (int i = 0; i < Plan0NumPcx.Length; i++)
                Plan0NumPcx[i] = s.SerializeArray<byte>(Plan0NumPcx[i], 8, name: $"{nameof(Plan0NumPcx)}[{i}]");

            // Serialize counts
            DESCount = s.Serialize<ushort>(DESCount, name: nameof(DESCount));
            ETACount = s.Serialize<byte>(ETACount, name: nameof(ETACount));
            
            // Serialize DES data
            DESBlockLength = s.Serialize<uint>(DESBlockLength, name: nameof(DESBlockLength));
            DESData = s.SerializeObjectArray<PS1_EDU_DESData>(DESData, DESCount, name: nameof(DESData));

            Unk7 = s.SerializeArray<byte>(Unk7, 0x1A, name: nameof(Unk7));

            // Serialize ETA block length
            ETABlockLength = s.Serialize<uint>(ETABlockLength, name: nameof(ETABlockLength));

            // We parse the ETA later...
            ETABlockPointer = s.CurrentPointer;
            s.Goto(ETABlockPointer + ETABlockLength);

            // Serialize ETA tables
            ETAStateCountTableCount = s.Serialize<byte>(ETAStateCountTableCount, name: nameof(ETAStateCountTableCount));
            ETAStateCountTable = s.SerializeArray<byte>(ETAStateCountTable, ETAStateCountTableCount, name: nameof(ETAStateCountTable));
            ETASubStateCountTableCount = s.Serialize<byte>(ETASubStateCountTableCount, name: nameof(ETASubStateCountTableCount));
            ETASubStateCountTable = s.SerializeArray<byte>(ETASubStateCountTable, ETASubStateCountTableCount, name: nameof(ETASubStateCountTable));

            Unk10Length = s.Serialize<uint>(Unk10Length, name: nameof(Unk10Length));
            Unk10 = s.SerializeArray<ushort>(Unk10, Unk10Length, name: nameof(Unk10));

            if (UnkBlock6 == null)
                UnkBlock6 = new byte[4][];

            for (int i = 0; i < UnkBlock6.Length; i++)
                UnkBlock6[i] = s.SerializeArray<byte>(UnkBlock6[i], 0xFE, name: $"{nameof(UnkBlock6)}[{i}]");

            // Serialize ETA
            s.DoAt(ETABlockPointer, () =>
            {
                // TODO: ETA begins at 0x552C6 for the Jungle world file - what's the data before it?

                return;

                if (ETA == null)
                    ETA = new Common_EventState[ETACount][][];

                var stateIndex = 0;

                // Serialize every ETA
                for (int i = 0; i < ETA.Length; i++)
                {
                    if (ETA[i] == null)
                        ETA[i] = new Common_EventState[ETAStateCountTable[i]][];

                    // Serialize every state
                    for (int j = 0; j < ETA[i].Length; j++)
                    {
                        // Serialize sub-states
                        ETA[i][j] = s.SerializeObjectArray<Common_EventState>(ETA[i][j], ETASubStateCountTable[stateIndex], name: $"{nameof(ETA)}[{i}][{j}]");

                        stateIndex++;
                    }
                }
            });
        }

        #endregion
    }
}