using System;

namespace Ray1Map
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class Unity_TextureFormatInfoAttribute : Attribute
    {
        public Unity_TextureFormatInfoAttribute(int bpp, bool indexed)
        {
            BPP = bpp;
            Indexed = indexed;
        }

        public int BPP { get; }
        public bool Indexed { get; }
    }
}