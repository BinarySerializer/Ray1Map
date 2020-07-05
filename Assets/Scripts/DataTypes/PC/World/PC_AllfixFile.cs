namespace R1Engine
{
    public class PC_AllfixFile : PC_BaseWorldFile
    {
        public uint RaymanExeCheckSum3 { get; set; }

        public uint[] DESDataIndices { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize PC Header
            base.SerializeImpl(s);

            // Serialize the ETA
            Eta = s.SerializeArraySize<PC_ETA, byte>(Eta, name: nameof(Eta));
            Eta = s.SerializeObjectArray<PC_ETA>(Eta, Eta.Length, name: nameof(Eta));

            // Serialize the DES
            DesItemCount = s.Serialize<ushort>(DesItemCount, name: nameof(DesItemCount));

            // We need to read one less DES as DES 0 is not in this file
            DesItems = s.SerializeObjectArray<PC_DES>(DesItems, DesItemCount - 1, onPreSerialize: data => data.FileType = PC_DES.Type.AllFix, name: nameof(DesItems));

            RaymanExeCheckSum3 = s.Serialize(RaymanExeCheckSum3, name: nameof(RaymanExeCheckSum3));

            // NOTE: The length is always hard-coded in the games, so it's not dependent on the DES count value
            DESDataIndices = s.SerializeArray<uint>(DESDataIndices, DesItemCount - 1, name: nameof(DESDataIndices));
        }
    }
}