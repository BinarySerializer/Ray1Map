using System.Collections.Generic;
using System.Text;

namespace R1Engine
{
    public class SpyroEncoding : Encoding
    {
        public override int GetByteCount(char[] chars, int index, int count) => count;

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) => throw new System.NotImplementedException();

        public override int GetCharCount(byte[] bytes, int index, int count) => count;

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = byteIndex; i < byteCount; i++)
            {
                chars[charIndex] = CharTable.TryGetItem(bytes[i]);
                charIndex++;
            }

            return byteCount;
        }

        public Dictionary<byte, char> CharTable { get; } = new Dictionary<byte, char>
        {
            [0] = ' ',
            [1] = '!',

            [4] = '\'',

            [8] = '+', // ?
            [9] = ',',
            [10] = '-',
            [11] = '.',
            
            [13] = '0',
            [14] = '1',
            [15] = '2',
            [16] = '3',
            [17] = '4',
            [18] = '5',
            [19] = '6',
            [20] = '7',
            [21] = '8',
            [22] = '9',

            [23] = ':',
            [24] = '?',

            [25] = 'A',
            [26] = 'B',
            [27] = 'C',
            [28] = 'D',
            [29] = 'E',
            [30] = 'F',
            [31] = 'G',
            [32] = 'H',
            [33] = 'I',
            [34] = 'J',
            [35] = 'K',
            [36] = 'L',
            [37] = 'M',
            [38] = 'N',
            [39] = 'O',
            [40] = 'P',
            [41] = 'Q',
            [42] = 'R',
            [43] = 'S',
            [44] = 'T',
            [45] = 'U',
            [46] = 'V',
            [47] = 'W',
            [48] = 'X',
            [49] = 'Y',
            [50] = 'Z',

            [51] = 'a',
            [52] = 'b',
            [53] = 'c',
            [54] = 'd',
            [55] = 'e',
            [56] = 'f',
            [57] = 'g',
            [58] = 'h',
            [59] = 'i',
            [60] = 'j',
            [61] = 'k',
            [62] = 'l',
            [63] = 'm',
            [64] = 'n',
            [65] = 'o',
            [66] = 'p',
            [67] = 'q',
            [68] = 'r',
            [69] = 's',
            [70] = 't',
            [71] = 'u',
            [72] = 'v',
            [73] = 'w',
            [74] = 'x',
            [75] = 'y',
            [76] = 'z',

            [77] = '™',
            [78] = 'Ç',
            [79] = 'Ñ',
        };

        public override int GetMaxByteCount(int charCount) => charCount;

        public override int GetMaxCharCount(int byteCount) => byteCount;
    }
}