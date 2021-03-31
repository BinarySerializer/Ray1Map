using BinarySerializer;

namespace R1Engine
{
    public class R1_PC_BigRayFile : R1_PC_BaseWorldFile
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
            DesItems = s.SerializeObjectArray<R1_PC_DES>(DesItems, DesItemCount, onPreSerialize: data => data.FileType = R1_PC_DES.Type.BigRay, name: nameof(DesItems));

            // Serialize the ETA
            Eta = s.SerializeArraySize<R1_PC_ETA, byte>(Eta, name: nameof(Eta));
            Eta = s.SerializeObjectArray<R1_PC_ETA>(Eta, Eta.Length, name: nameof(Eta));
        }
    }
}