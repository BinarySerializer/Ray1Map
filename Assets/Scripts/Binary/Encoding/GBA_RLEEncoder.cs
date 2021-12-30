using DSDecmp.Formats.Nitro;
using System;
using System.IO;
using BinarySerializer;

namespace Ray1Map
{
    public class GBA_RLEEncoder : IStreamEncoder 
    {
        public string Name => "GBA_RLE";

        public void DecodeStream(Stream input, Stream output)
        {
            RLE rle = new RLE();
            rle.Decompress(input, input.Length, output);
        }

        public void EncodeStream(Stream input, Stream output)
        {
            throw new NotImplementedException();
        }
    }
}