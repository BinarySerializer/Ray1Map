using System;

namespace CSharp_PVRTC_EncDec
{
    public static class CommonColors
    {
        public static byte[] GetAllMax(int elements)
        {
            if (elements == 3)
            {
                return new byte[3] { 255, 255, 255 };
            }
            else if (elements == 4)
            {
                return new byte[4] { 255, 255, 255, 255 };
            }

            throw new ArgumentException("Only 3 and 4 are supported as elements");
        }

        public static byte[] GetAllMin(int elements)
        {
            if (elements == 3)
            {
                return new byte[3] { 0, 0, 0 };
            }
            else if (elements == 4)
            {
                return new byte[4] { 0, 0, 0, 0 };
            }

            throw new ArgumentException("Only 3 and 4 are supported as elements");
        }
    }
}