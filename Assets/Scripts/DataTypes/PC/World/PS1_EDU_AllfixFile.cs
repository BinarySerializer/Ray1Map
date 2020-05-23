namespace R1Engine
{
    /// <summary>
    /// Allfix data for EDU on PS1
    /// </summary>
    public class PS1_EDU_AllfixFile : R1Serializable
    {
        #region Public Properties

        public byte ETACount { get; set; }
        
        public ushort DESCount { get; set; }

        public PS1_EDU_DESData[] DESData { get; set; }

        public uint ETABlockLength { get; set; }

        public Pointer ETABlockPointer { get; set; }

        public byte[] UnkETAData { get; set; }

        /// <summary>
        /// The event states for every ETA
        /// </summary>
        public Common_EventState[][][] ETA { get; set; }

        // Index table for something?
        public uint[] UnkDwordArray { get; set; }

        public byte[] ETAStateCountTable { get; set; }

        public byte ETASubStateCountTableCount { get; set; }

        // The count of every state array
        public byte[] ETASubStateCountTable { get; set; }

        public uint UnkBlock5Count { get; set; }

        public ushort[] UnkBlock5 { get; set; }

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
            ETACount = s.Serialize<byte>(ETACount, name: nameof(ETACount));
            DESCount = s.Serialize<ushort>(DESCount, name: nameof(DESCount));

            // Serialize the DES data
            DESData = s.SerializeObjectArray<PS1_EDU_DESData>(DESData, DESCount, name: nameof(DESData));

            // Serialize ETA block length
            ETABlockLength = s.Serialize<uint>(ETABlockLength, name: nameof(ETABlockLength));

            // We parse the ETA later...
            ETABlockPointer = s.CurrentPointer;
            s.Goto(ETABlockPointer + ETABlockLength);

            UnkDwordArray = s.SerializeArray<uint>(UnkDwordArray, 8, name: nameof(UnkDwordArray));

            // Serialize ETA tables
            ETAStateCountTable = s.SerializeArray<byte>(ETAStateCountTable, ETACount, name: nameof(ETAStateCountTable));
            ETASubStateCountTableCount = s.Serialize<byte>(ETASubStateCountTableCount, name: nameof(ETASubStateCountTableCount));
            ETASubStateCountTable = s.SerializeArray<byte>(ETASubStateCountTable, ETASubStateCountTableCount, name: nameof(ETASubStateCountTable));

            UnkBlock5Count = s.Serialize<uint>(UnkBlock5Count, name: nameof(UnkBlock5Count));
            UnkBlock5 = s.SerializeArray(UnkBlock5, UnkBlock5Count, name: nameof(UnkBlock5));

            if (UnkBlock6 == null)
                UnkBlock6 = new byte[4][];

            for (int i = 0; i < UnkBlock6.Length; i++)
                UnkBlock6[i] = s.SerializeArray<byte>(UnkBlock6[i], 0xFE, name: $"{nameof(UnkBlock6)}[{i}]");

            // Serialize ETA
            s.DoAt(ETABlockPointer, () =>
            {
                UnkETAData = s.SerializeArray<byte>(UnkETAData, 4 * 8, name: nameof(UnkETAData));

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