using System;
using System.Collections.Generic;
using System.IO;

namespace R1Engine {
    public static partial class ReplaceThisClass {

        public static int event_off_pos = 0x1C;
        public static int event_off_type = 0x63;

        public static Level PS1_LoadLevel(World world, int lvlIndex) {
            var lvl = new Level();
            var mapFile = new FileStream(Environment.CurrentDirectory +
                $"/{Settings.gameDirs[GameMode.RaymanPS1]}/{world}/{world}{lvlIndex.ToString("00")}.XXX", FileMode.Open);
            var XXX = new byte[mapFile.Length];
            mapFile.Read(XXX, 0, XXX.Length);
            mapFile.Close();

            int off_events = BitConverter.ToInt32(XXX, 0x8);
            int off_types = BitConverter.ToInt32(XXX, 0xC);
            int off_sprites = BitConverter.ToInt32(XXX, 0x10);

            lvl.width = BitConverter.ToUInt16(XXX, off_types);
            lvl.height = BitConverter.ToUInt16(XXX, off_types + 2);

            if (lvl.width == 0 || lvl.height == 0)
                return lvl;

            int i = off_types + 4;
            lvl.types = new Type[lvl.width * lvl.height];

            for (int n = 0; n < lvl.width * lvl.height; n++, i += 2) {
                lvl.types[n] = new Type();
                int g = XXX[i] + ((XXX[i + 1] & 3) << 8);
                lvl.types[n].gX = g & 15;
                lvl.types[n].gY = g >> 4;
                lvl.types[n].col = (TypeCollision)(XXX[i + 1] >> 2);
            }


            // Events
            var evs = new List<Event>();
            for (int e = off_events + 16; XXX[e] > 0; e += 0x70) {
                var ev = new Event() {
                    pos = new PxlVec(
                        BitConverter.ToUInt16(XXX, e + event_off_pos),
                        BitConverter.ToUInt16(XXX, e + event_off_pos + 2))
                };
                ev.behaviour = (EventBehaviours)XXX[e + event_off_type];
                evs.Add(ev);

                off_types -= 113;
                off_sprites -= 113;
            }
            lvl.events = evs.ToArray();



            // hack get rayman pos
            for (int b = 0; b + 4 < XXX.Length; b++)
                if (XXX[b] == 0x07 && XXX[b + 1] == 0x63 && XXX[b + 2] == 0 && XXX[b + 3] == 0) {
                    lvl.raymanPos = new PxlVec(
                        BitConverter.ToUInt16(XXX, b - 0x46),
                        BitConverter.ToUInt16(XXX, b - 0x44));
                    break;
                }


            return lvl;
        }
    }
}