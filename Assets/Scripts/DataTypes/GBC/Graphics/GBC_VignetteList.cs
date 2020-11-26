namespace R1Engine
{
    public class GBC_VignetteList : GBC_BaseBlock 
    {
        public GBC_Vignette[] Vignettes { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            // Serialize vignettes
            if (Vignettes == null)
                Vignettes = new GBC_Vignette[OffsetTable.OffsetsCount];

            for (int i = 0; i < Vignettes.Length; i++)
                Vignettes[i] = s.DoAt(OffsetTable.GetPointer(i), () => s.SerializeObject<GBC_Vignette>(Vignettes[i], name: $"{nameof(Vignettes)}[{i}]"));
        }
    }
}