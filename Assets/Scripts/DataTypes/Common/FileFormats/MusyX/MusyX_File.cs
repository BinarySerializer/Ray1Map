using System.Text;

namespace R1Engine
{
    /// <summary>
    /// Base file for GBA MusyX
    /// </summary>
    public class MusyX_File : R1Serializable
    {
        public Pointer<MusyX_InstrumentTable> InstrumentTable { get; set; }
        public Pointer<MusyX_UnknownTable> Unknown2List1 { get; set; }
        public Pointer<MusyX_UnknownTable> Unknown2List2 { get; set; }
        public Pointer<MusyX_UnknownTable> Unknown2List3 { get; set; }
        public uint UInt_10 { get; set; }
        public uint UInt_14 { get; set; }
        public Pointer<MusyX_SongTable> SongTable { get; set; }
        public Pointer<MusyX_SampleTable> SampleTable { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            InstrumentTable = s.SerializePointer<MusyX_InstrumentTable>(InstrumentTable, anchor: Offset, name: nameof(InstrumentTable));
            Unknown2List1 = s.SerializePointer<MusyX_UnknownTable>(Unknown2List1, anchor: Offset, resolve: true, name: nameof(Unknown2List1));
            Unknown2List2 = s.SerializePointer<MusyX_UnknownTable>(Unknown2List2, anchor: Offset, resolve: true, name: nameof(Unknown2List2));
            Unknown2List3 = s.SerializePointer<MusyX_UnknownTable>(Unknown2List3, anchor: Offset, resolve: true, name: nameof(Unknown2List3));
            UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
            UInt_14 = s.Serialize<uint>(UInt_14, name: nameof(UInt_14));
            SongTable = s.SerializePointer<MusyX_SongTable>(SongTable, anchor: Offset, name: nameof(SongTable));
            SampleTable = s.SerializePointer<MusyX_SampleTable>(SampleTable, anchor: Offset, name: nameof(SampleTable));


            // Read instrument table
            InstrumentTable.Resolve(s, onPreSerialize: st => {
                st.BaseOffset = Offset;
                st.EndOffset = Unknown2List1.pointer;
            });

            // Read song table
            SongTable.Resolve(s, onPreSerialize: st => {
                st.BaseOffset = Offset;
                st.EndOffset = SampleTable.pointer;
            });

            // Read sample table
            SampleTable.Resolve(s, onPreSerialize: st => {
                st.BaseOffset = Offset;
                //st.EndOffset = SampleTable.pointer;
            });
        }
    }
}