using System.Linq;

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

        public uint MainDataBlockLength { get; set; }

        public Pointer MainDataBlockPointer { get; set; }

        public Common_ImageDescriptor[][] ImageDescriptors { get; set; }

        public PS1_EDU_AnimationDescriptor[][] AnimationDescriptors { get; set; }

        /// <summary>
        /// The event states for every ETA
        /// </summary>
        public Common_EventState[][][] ETA { get; set; }

        public byte ETAStateCountTableCount { get; set; }

        public byte[] ETAStateCountTable { get; set; }

        public byte ETASubStateCountTableCount { get; set; }

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
            BG1 = s.Serialize<ushort>(BG1, name: nameof(BG1));
            BG2 = s.Serialize<ushort>(BG2, name: nameof(BG2));
            Plan0NumPcxCount = s.Serialize<byte>(Plan0NumPcxCount, name: nameof(Plan0NumPcxCount));

            if (Plan0NumPcx == null)
                Plan0NumPcx = new byte[Plan0NumPcxCount][];

            s.BeginXOR(0x19);
            for (int i = 0; i < Plan0NumPcx.Length; i++)
                Plan0NumPcx[i] = s.SerializeArray<byte>(Plan0NumPcx[i], 8, name: $"{nameof(Plan0NumPcx)}[{i}]");
            s.EndXOR();

            // Serialize counts
            DESCount = s.Serialize<ushort>(DESCount, name: nameof(DESCount));
            ETACount = s.Serialize<byte>(ETACount, name: nameof(ETACount));
            
            // Serialize DES data
            DESBlockLength = s.Serialize<uint>(DESBlockLength, name: nameof(DESBlockLength));
            DESData = s.SerializeObjectArray<PS1_EDU_DESData>(DESData, DESCount, name: nameof(DESData));

            Unk7 = s.SerializeArray<byte>(Unk7, 0x1A, name: nameof(Unk7));

            // Serialize main data block length
            MainDataBlockLength = s.Serialize<uint>(MainDataBlockLength, name: nameof(MainDataBlockLength));

            // We parse the main data block later...
            MainDataBlockPointer = s.CurrentPointer;
            s.Goto(MainDataBlockPointer + MainDataBlockLength);

            // Serialize ETA tables
            ETAStateCountTableCount = s.Serialize<byte>(ETAStateCountTableCount, name: nameof(ETAStateCountTableCount));
            ETAStateCountTable = s.SerializeArray<byte>(ETAStateCountTable, ETAStateCountTableCount, name: nameof(ETAStateCountTable));
            ETASubStateCountTableCount = s.Serialize<byte>(ETASubStateCountTableCount, name: nameof(ETASubStateCountTableCount));
            ETASubStateCountTable = s.SerializeArray<byte>(ETASubStateCountTable, ETASubStateCountTableCount, name: nameof(ETASubStateCountTable));

            AnimationDescriptorLayersBlockSizeTableCount = s.Serialize<uint>(AnimationDescriptorLayersBlockSizeTableCount, name: nameof(AnimationDescriptorLayersBlockSizeTableCount));
            AnimationDescriptorLayersBlockSizeTable = s.SerializeArray<ushort>(AnimationDescriptorLayersBlockSizeTable, AnimationDescriptorLayersBlockSizeTableCount, name: nameof(AnimationDescriptorLayersBlockSizeTable));

            AnimationLayers = s.SerializeObjectArray<Common_AnimationLayer>(AnimationLayers, 0xfe, name: nameof(AnimationLayers));

            // Serialize the main data block
            s.DoAt(MainDataBlockPointer, () =>
            {
                if (ImageDescriptors == null)
                    ImageDescriptors = new Common_ImageDescriptor[DESCount][];

                if (AnimationDescriptors == null)
                    AnimationDescriptors = new PS1_EDU_AnimationDescriptor[DESCount][];

                int curAnimDesc = 0;

                for (int i = 0; i < ImageDescriptors.Length; i++)
                {
                    ImageDescriptors[i] = s.SerializeObjectArray<Common_ImageDescriptor>(ImageDescriptors[i], DESData[i].ImageDescriptorsCount, name: $"{nameof(ImageDescriptors)}[{i}]");

                    AnimationDescriptors[i] = s.SerializeObjectArray<PS1_EDU_AnimationDescriptor>(AnimationDescriptors[i], DESData[i].AnimationDescriptorsCount, name: $"{nameof(AnimationDescriptors)}[{i}]");

                    // TODO: Save these in the AnimationDescriptor
                    for (int j = 0; j < AnimationDescriptors[i].Length; j++) {
                        var descriptor = AnimationDescriptors[i][j];
                        if (descriptor.FrameCount > 0) {
                            // Each of these indices is an index in the AnimationLayers array.
                            // 0 and 1 are special indices apparently, so the layer each byte links to is AnimationLayers[byte-2], otherwise the layer is null?
                            // TODO: Figure out what exactly 0 and 1 do
                            byte[] LayersIndices = s.SerializeArray<byte>(null, AnimationDescriptorLayersBlockSizeTable[curAnimDesc], name: "AnimationLayersIndices");
                            
                            if (AnimationDescriptorLayersBlockSizeTable[curAnimDesc] % 4 != 0) {
                                // Padding seems to contain garbage data in this case instead of 0xCD?
                                int padding = 4 - AnimationDescriptorLayersBlockSizeTable[curAnimDesc] % 4;
                                s.SerializeArray<byte>(Enumerable.Repeat((byte)0xCD, padding).ToArray(), padding, name: "Padding");
                            }
                            if (descriptor.AnimFramesPointer != 0xFFFFFFFF) {
                                Common_AnimationFrame[] frames = s.SerializeObjectArray<Common_AnimationFrame>(null, descriptor.FrameCount, name: "AnimationFrames");
                            }
                            curAnimDesc++;
                        }
                    }
                }

                // ETA begins at 0x552C6 for the Jungle world file
                // And we reached it :)
                if (ETA == null)
                    ETA = new Common_EventState[ETACount][][];

                var stateIndex = 0;

                // Serialize every ETA
                for (int i = 0; i < ETA.Length; i++)
                {
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
            });
        }

        #endregion
    }
}