using System;
using System.Collections.Generic;
using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_LevFile : ISerializableFile
    {
        // TODO: Add remaining properties

        public ushort Width { get; set; }

        public ushort Height { get; set; }

        public Type[] Tiles { get; set; }

        public Event[] Events { get; set; }

        // TODO: Remove?
        public PxlVec RaymanPos { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            // TODO: Redo everything below here to use stream rather than buffer and make sure to read entire file so we can write it back

            int event_off_pos = 0x1C;
            int event_off_type = 0x63;

            var XXX = stream.ReadBytes((int)stream.Length);

            int off_events = BitConverter.ToInt32(XXX, 0x8);
            int off_types = BitConverter.ToInt32(XXX, 0xC);
            int off_sprites = BitConverter.ToInt32(XXX, 0x10);

            Width = BitConverter.ToUInt16(XXX, off_types);
            Height = BitConverter.ToUInt16(XXX, off_types + 2);

            int i = off_types + 4;
            Tiles = new Type[Width * Height];

            for (int n = 0; n < Width * Height; n++, i += 2)
            {
                Tiles[n] = new Type();
                int g = XXX[i] + ((XXX[i + 1] & 3) << 8);
                Tiles[n].gX = g & 15;
                Tiles[n].gY = g >> 4;
                Tiles[n].col = (TileCollisionType)(XXX[i + 1] >> 2);
            }


            // Events
            var evs = new List<Event>();
            for (int e = off_events + 16; XXX[e] > 0; e += 0x70)
            {
                var ev = new Event
                {
                    pos = new PxlVec(
                        BitConverter.ToUInt16(XXX, e + event_off_pos),
                        BitConverter.ToUInt16(XXX, e + event_off_pos + 2)),
                    type = (EventType) XXX[e + event_off_type]
                };
                evs.Add(ev);

                off_types -= 113;
                off_sprites -= 113;
            }
            Events = evs.ToArray();

            // hack get rayman pos
            for (int b = 0; b + 4 < XXX.Length; b++)
                if (XXX[b] == 0x07 && XXX[b + 1] == 0x63 && XXX[b + 2] == 0 && XXX[b + 3] == 0)
                {
                    RaymanPos = new PxlVec(
                        BitConverter.ToUInt16(XXX, b - 0x46),
                        BitConverter.ToUInt16(XXX, b - 0x44));
                    break;
                }
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}