namespace LzssAlgorithm
{
    public static class Constants
    {
        public const int WindowSize = 4096;

        public const int NullIndex = WindowSize + 1;

        public const int MaxUncoded = 2;

        public const int MaxCoded = MaxUncoded + 16;

        public const int HashSize = 1024;

        public const byte InitValue = 0x00;
    }
}
