using System.Text;
using BinarySerializer;

namespace Ray1Map
{
    /// <summary>
    /// Base file for GBA MusyX
    /// </summary>
    public class MusyX_File : BinarySerializable
    {
        public Pointer<MusyX_InstrumentTable> InstrumentTable { get; set; }
        public Pointer<MusyX_SFXGroup> SFXGroup1 { get; set; } // in Rayman Advance this points to a list. in RHR, this points to a pointer (or a list of pointers with only 1 entry), which points to 1 8 byte struct
        public Pointer<MusyX_SFXGroup> SFXGroup2 { get; set; }
        public Pointer<MusyX_SFXGroup> SFXGroup3 { get; set; }
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
            SFXGroup1 = s.SerializePointer<MusyX_SFXGroup>(SFXGroup1, anchor: Offset, resolve: false, name: nameof(SFXGroup1)); // Don't resolve for now, this isn't parsed correctly
            SFXGroup2 = s.SerializePointer<MusyX_SFXGroup>(SFXGroup2, anchor: Offset, resolve: true, name: nameof(SFXGroup2));
            SFXGroup3 = s.SerializePointer<MusyX_SFXGroup>(SFXGroup3, anchor: Offset, resolve: true, name: nameof(SFXGroup3));
            UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
            UInt_14 = s.Serialize<uint>(UInt_14, name: nameof(UInt_14));
            SongTable = s.SerializePointer<MusyX_SongTable>(SongTable, anchor: Offset, name: nameof(SongTable));
            SampleTable = s.SerializePointer<MusyX_SampleTable>(SampleTable, anchor: Offset, name: nameof(SampleTable));


            // Read instrument table
            InstrumentTable.Resolve(s, onPreSerialize: st => {
                st.BaseOffset = Offset;
                st.EndOffset = SFXGroup1.PointerValue;
            });

            // Read song table
            SongTable.Resolve(s, onPreSerialize: st => {
                st.BaseOffset = Offset;
                st.EndOffset = SampleTable.PointerValue;
            });

            // Read sample table
            SampleTable.Resolve(s, onPreSerialize: st => {
                st.BaseOffset = Offset;
                //st.EndOffset = SampleTable.pointer;
            });
        }
    }
}