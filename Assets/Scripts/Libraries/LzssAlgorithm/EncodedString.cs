namespace LzssAlgorithm
{
    public struct EncodedString
    {
        // Offset to start of longest match
        public int Offset { get; set; }

        // Length of longest match
        public int Length { get; set; }
    }
}
