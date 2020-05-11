namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_Level : R1Serializable
    {
        // Some world-specific info - appears to be tiles?
        public Pointer WorldPointer { get; set; }

        // Leads to a data structure where byte 5 and 7 are the map width and height - maybe after that is the compressed map?
        public Pointer Pointer_04 { get; set; }

        public Pointer Pointer_08 { get; set; }
        public Pointer Pointer_0B { get; set; }
        public Pointer Pointer_10 { get; set; }

        public byte[] Unk_10 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            WorldPointer = s.SerializePointer(WorldPointer, name: nameof(WorldPointer));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            Pointer_0B = s.SerializePointer(Pointer_0B, name: nameof(Pointer_0B));
            Pointer_10 = s.SerializePointer(Pointer_10, name: nameof(Pointer_10));
            Unk_10 = s.SerializeArray<byte>(Unk_10, 8, name: nameof(Unk_10));
        }
    }

    /*
     
       v202A598 = *(_DWORD *)(28 * mapIndex + 0x85485B4);
       Pointer_04 = *(_DWORD *)(28 * mapIndex + 0x85485B8);
       if ( *(_BYTE *)(28 * mapIndex + 0x85485CC) & 1 )
       {
           sub_48428();
           Pointer_04 = 33563184;
       }
       v20305E8 = Pointer_04 + 4;
       v8 = *(_DWORD *)(28 * mapIndex + 0x85485BC);
       if ( *(_BYTE *)(28 * mapIndex + 0x85485CC) & 2 )
       {
           sub_48428();
           v8 = 33725136;
       }
       sub_38C(Pointer_04, v8, *(_DWORD *)(28 * mapIndex + 0x85485C0), v5, v17);
       v20305E0 = v202A5A0;
       v20305E2 = v202A578;
       v9 = v202A578 * v202A5A0;
       v20305E4 = v202A578 * v202A5A0;
       if ( v18 )
       {
           sub_44AA4(v9);
           v10 = sub_FF7C(v202BF10 | 1);
           v11 = sub_2E0F8(v10);
           v12 = sub_2D3C4(v11);
           v9 = sub_10334(v12);
       }
       sub_3F8(v9);
       v13 = *(_DWORD *)(4 * v17 + 0x8153980);
       v20011D8 = *(_BYTE *)(*(_BYTE *)(28 * mapIndex + 0x85485CB) + v13);
       if ( !*(_DWORD *)(36 * v20011D8 + 0x86D4D60) )
           v20011D8 = *(_BYTE *)(*(_BYTE *)(28 * mapIndex + 0x85485CA) + v13);
       if ( v20011D8 == 25 )
           v20011D8 = 26;
       sub_4841C(*(_DWORD *)(28 * mapIndex + 0x85485C4), 83886080, 80);
     
     
     */
}