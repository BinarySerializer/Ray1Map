namespace R1Engine
{
    public class PC_BigRayFile : PC_BaseWorldFile
    {
        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize PC Header
            base.SerializeImpl(s);

            // Hard-code to 1 item
            DesItemCount = 1;

            // Serialize the DES
            DesItems = s.SerializeObjectArray<PC_DES>(DesItems, DesItemCount, onPreSerialize: data => data.FileType = PC_DES.Type.BigRay, name: nameof(DesItems));

            // Serialize the ETA
            Eta = s.SerializeArraySize<PC_ETA, byte>(Eta, name: nameof(Eta));
            Eta = s.SerializeObjectArray<PC_ETA>(Eta, Eta.Length, name: nameof(Eta));
        }
    }
}