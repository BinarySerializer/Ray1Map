namespace R1Engine
{
    /// <summary>
    /// A base GBA block
    /// </summary>
    public abstract class GBA_BaseBlock : R1Serializable
    {
        /// <summary>
        /// The size of the block in bytes, not counting this value
        /// </summary>
        public uint BlockSize { get; set; }

        /// <summary>
        /// The block offset table
        /// </summary>
        public GBA_OffsetTable OffsetTable { get; set; }

		protected override void OnPreSerialize(SerializerObject s) {
			base.OnPreSerialize(s);
            s.DoAt(Offset - 4, () => {
                // Serialize the size
                BlockSize = s.Serialize<uint>(BlockSize, name: nameof(BlockSize));
            });
            s.DoAt(Offset + BlockSize, () => {
                // Align
                s.Align();
                // Serialize the offset table
                OffsetTable = s.SerializeObject<GBA_OffsetTable>(OffsetTable, name: nameof(OffsetTable));
            });
		}

		protected override void OnPostSerialize(SerializerObject s) {
			base.OnPostSerialize(s);

            if (Offset + BlockSize != s.CurrentPointer) {
                UnityEngine.Debug.LogWarning($"{GetType()} @ {Offset}: Serialized size: {(s.CurrentPointer  - Offset)} != BlockSize: {BlockSize}");
            }

            // Serialize data from the offset table
            SerializeOffsetData(s);
        }
        
        public virtual void SerializeOffsetData(SerializerObject s) { }
    }
}