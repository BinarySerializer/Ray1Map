﻿using BinarySerializer;

namespace R1Engine
{
    public class TGA_Header : BinarySerializable
    {
        public bool ForceNoColorMap { get; set; } // Set before serializing

        public byte IdentificationFieldLength { get; set; }
        public bool HasColorMap { get; set; }
        public TGA_ImageType ImageType { get; set; }

        // Color Map
        public ushort ColorMapOrigin { get; set; }
        public ushort ColorMapLength { get; set; }
        public byte ColorMapEntrySize { get; set; } // Number of bits

        // Image
        public ushort OriginX { get; set; }
        public ushort OriginY { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public byte BitsPerPixel { get; set; }
        public byte AttributeBitsCount { get; set; } // Determines how many bits are used for alpha
        public byte Reserved { get; set; }
        public TGA_Origin OriginPoint { get; set; }
        public TGA_Interleaving InterleavingFlag { get; set; }

        public byte[] IdentificationField { get; set; }
        public byte[] ColorMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            IdentificationFieldLength = s.Serialize<byte>(IdentificationFieldLength, name: nameof(IdentificationFieldLength));
            HasColorMap = s.Serialize<bool>(HasColorMap, name: nameof(HasColorMap));
            ImageType = s.Serialize<TGA_ImageType>(ImageType, name: nameof(ImageType));

            ColorMapOrigin = s.Serialize<ushort>(ColorMapOrigin, name: nameof(ColorMapOrigin));
            ColorMapLength = s.Serialize<ushort>(ColorMapLength, name: nameof(ColorMapLength));
            ColorMapEntrySize = s.Serialize<byte>(ColorMapEntrySize, name: nameof(ColorMapEntrySize));

            OriginX = s.Serialize<ushort>(OriginX, name: nameof(OriginX));
            OriginY = s.Serialize<ushort>(OriginY, name: nameof(OriginY));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            BitsPerPixel = s.Serialize<byte>(BitsPerPixel, name: nameof(BitsPerPixel));
            s.SerializeBitValues<byte>(bitFunc =>
            {
                AttributeBitsCount = (byte)bitFunc(AttributeBitsCount, 4, name: nameof(AttributeBitsCount));
                Reserved = (byte)bitFunc(Reserved, 1, name: nameof(Reserved));
                OriginPoint = (TGA_Origin)bitFunc((byte)OriginPoint, 1, name: nameof(OriginPoint));
                InterleavingFlag = (TGA_Interleaving)bitFunc((byte)InterleavingFlag, 2, name: nameof(InterleavingFlag));
            });

            IdentificationField = s.SerializeArray<byte>(IdentificationField, IdentificationFieldLength, name: nameof(IdentificationField));
            ColorMap = s.SerializeArray<byte>(ColorMap, !ForceNoColorMap ? ColorMapLength : 0, name: nameof(ColorMap));
        }

        public enum TGA_ImageType : byte
        {
            ColorMapped = 1,
            UnmappedRGB = 2,
            ColorMapped_RLE = 9,
            UnmappedRGB_RLE = 10
        }
        public enum TGA_Origin : byte
        {
            BottomLeft = 0,
            BottomRight = 1
        }
        public enum TGA_Interleaving : byte
        {
            None = 0,
            TwoWay = 1,
            FourWay = 2,
            Reserved = 3
        }
    }
}