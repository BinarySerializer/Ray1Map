using BinarySerializer;

namespace R1Engine
{
    public class R1_PC_AllfixFile : R1_PC_BaseWorldFile
    {
        public uint RaymanExeCheckSum3 { get; set; }

        public uint DESIndex_Ray { get; set; }
        public uint DESIndex_Alpha { get; set; }
        public uint DESIndex_Alpha2 { get; set; }
        public uint DESIndex_Alpha3 { get; set; } // Japan/China only
        public uint DESIndex_RayLittle { get; set; }
        public uint DESIndex_MapObj { get; set; }
        public uint DESIndex_ClockObj { get; set; }
        public uint DESIndex_DivObj { get; set; }
        public uint DESIndex_PARCHEM { get; set; } // EDU/Kit only

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize PC Header
            base.SerializeImpl(s);

            // Serialize the ETA
            Eta = s.SerializeArraySize<R1_PC_ETA, byte>(Eta, name: nameof(Eta));
            Eta = s.SerializeObjectArray<R1_PC_ETA>(Eta, Eta.Length, name: nameof(Eta));

            // Serialize the DES
            DesItemCount = s.Serialize<ushort>(DesItemCount, name: nameof(DesItemCount));

            // We need to read one less DES as DES 0 is not in this file
            DesItems = s.SerializeObjectArray<R1_PC_DES>(DesItems, DesItemCount - 1, onPreSerialize: data => data.FileType = R1_PC_DES.Type.AllFix, name: nameof(DesItems));

            RaymanExeCheckSum3 = s.Serialize(RaymanExeCheckSum3, name: nameof(RaymanExeCheckSum3));

            DESIndex_Ray = s.Serialize<uint>(DESIndex_Ray, name: nameof(DESIndex_Ray));
            DESIndex_Alpha = s.Serialize<uint>(DESIndex_Alpha, name: nameof(DESIndex_Alpha));
            DESIndex_Alpha2 = s.Serialize<uint>(DESIndex_Alpha2, name: nameof(DESIndex_Alpha2));

            if (s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanPC_1_21_JP)
                DESIndex_Alpha3 = s.Serialize<uint>(DESIndex_Alpha3, name: nameof(DESIndex_Alpha3));

            DESIndex_RayLittle = s.Serialize<uint>(DESIndex_RayLittle, name: nameof(DESIndex_RayLittle));
            DESIndex_MapObj = s.Serialize<uint>(DESIndex_MapObj, name: nameof(DESIndex_MapObj));
            DESIndex_ClockObj = s.Serialize<uint>(DESIndex_ClockObj, name: nameof(DESIndex_ClockObj));
            DESIndex_DivObj = s.Serialize<uint>(DESIndex_DivObj, name: nameof(DESIndex_DivObj));

            if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Kit || s.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Edu || s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_Edu)
                DESIndex_PARCHEM = s.Serialize<uint>(DESIndex_PARCHEM, name: nameof(DESIndex_PARCHEM));
        }
    }
}