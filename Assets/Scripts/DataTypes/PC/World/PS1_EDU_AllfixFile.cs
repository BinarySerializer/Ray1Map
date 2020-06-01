using System.Linq;

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

        public uint MainDataBlockLength { get; set; }

        public Pointer MainDataBlockPointer { get; set; }

        public byte[] MainData_UnkBytes { get; set; }

        /// <summary>
        /// The event states for every ETA
        /// </summary>
        public Common_EventState[][][] ETA { get; set; }

        // Index table for DES. not sure what for yet
        public uint[] DESDataIndices { get; set; }

        public byte[] ETAStateCountTable { get; set; }

        public byte ETASubStateCountTableCount { get; set; }

        // The count of every state array
        public byte[] ETASubStateCountTable { get; set; }

        public uint AnimationDescriptorLayersBlockSizeTableCount { get; set; }

        public ushort[] AnimationDescriptorLayersBlockSizeTable { get; set; }

        public Common_AnimationLayer[] AnimationLayers { get; set; }

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

            // Serialize main data block length
            MainDataBlockLength = s.Serialize<uint>(MainDataBlockLength, name: nameof(MainDataBlockLength));

            // We parse the main data block later...
            MainDataBlockPointer = s.CurrentPointer;
            s.Goto(MainDataBlockPointer + MainDataBlockLength);

            DESDataIndices = s.SerializeArray<uint>(DESDataIndices, 8, name: nameof(DESDataIndices));

            // Serialize ETA tables
            ETAStateCountTable = s.SerializeArray<byte>(ETAStateCountTable, ETACount, name: nameof(ETAStateCountTable));
            ETASubStateCountTableCount = s.Serialize<byte>(ETASubStateCountTableCount, name: nameof(ETASubStateCountTableCount));
            ETASubStateCountTable = s.SerializeArray<byte>(ETASubStateCountTable, ETASubStateCountTableCount, name: nameof(ETASubStateCountTable));

            AnimationDescriptorLayersBlockSizeTableCount = s.Serialize<uint>(AnimationDescriptorLayersBlockSizeTableCount, name: nameof(AnimationDescriptorLayersBlockSizeTableCount));
            AnimationDescriptorLayersBlockSizeTable = s.SerializeArray<ushort>(AnimationDescriptorLayersBlockSizeTable, AnimationDescriptorLayersBlockSizeTableCount, name: nameof(AnimationDescriptorLayersBlockSizeTable));

            AnimationLayers = s.SerializeObjectArray<Common_AnimationLayer>(AnimationLayers, 0xfe, name: nameof(AnimationLayers));


            // Serialize the main data block
            s.DoAt(MainDataBlockPointer, () => {
                if (ETA == null)
                    ETA = new Common_EventState[ETACount][][];

                var stateIndex = 0;

                // Serialize every ETA
                for (int i = 0; i < ETA.Length; i++) {
                    if (ETA[i] == null)
                        ETA[i] = new Common_EventState[ETAStateCountTable[i]][];

                    // EDU serializes the pointer structs, but the pointers are invalid. They can be anything as they're overwritten with valid memory pointers upon load
                    uint[] pointerStructs = Enumerable.Repeat((uint)1, ETA[i].Length).ToArray();
                    pointerStructs = s.SerializeArray<uint>(pointerStructs, pointerStructs.Length, name: $"ETAPointers[{i}]");


                    // Serialize every state
                    for (int j = 0; j < ETA[i].Length; j++)
                    {
                        // Serialize sub-states
                        ETA[i][j] = s.SerializeObjectArray<Common_EventState>(ETA[i][j], ETASubStateCountTable[stateIndex], name: $"{nameof(ETA)}[{i}][{j}]");

                        stateIndex++;
                    }
                }

                // TODO: Serialize image/animation descriptors - check world file where some of that data has been parsed!
                // Note: From a quick glance at the code it seems to be exactly the same as used for the World file, except this time after the ETA.
            });
        }

        #endregion
    }
}