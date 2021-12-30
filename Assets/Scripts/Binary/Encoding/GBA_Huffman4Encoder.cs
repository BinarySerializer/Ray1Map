using DSDecmp.Formats.Nitro;
using System;
using System.IO;
using BinarySerializer;

namespace Ray1Map
{
    /// <summary>
    /// Compresses/decompresses data using Huffman4
    /// </summary>
    public class GBA_Huffman4Encoder : IStreamEncoder 
    {
        public string Name => "GBA_Huffman4";

        public void DecodeStream(Stream input, Stream output) 
        {
            Huffman4 huff = new Huffman4();
            huff.Decompress(input, input.Length, output);
        }

        public void EncodeStream(Stream input, Stream output) 
        {
            throw new NotImplementedException();
        }
    }
}