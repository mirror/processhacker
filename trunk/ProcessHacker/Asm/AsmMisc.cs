/* Original file: asmserv.c
 * 
 * Free Disassembler and Assembler -- Command data and service routines
 *
 * Copyright (C) 2008 wj32
 * Copyright (C) 2001 Oleh Yuschuk
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Asm
{
    public unsafe partial class Asm
    {
        public string PrintFloat4(float f)
        {
            uint* fp = (uint*) &f;

            if (*fp == 0x7f800000)
                return "+INF 7F800000";
            else if (*fp == 0xff800000)
                return "-INF FF800000";
            else if ((*fp & 0xff800000) == 0x7f800000)
                return string.Format("+NAN {0:X8}", *fp);
            else if ((*fp & 0xff800000) == 0xff800000)
                return string.Format("-NAN {0:X8}", *fp);
            else if (f == 0.0)
                return "0.0";
            else
                return f.ToString();
        }

        public string PrintFloat8(double d)
        {
            uint lod, hid;

            lod = ((uint*)&d)[0];
            hid = ((uint*)&d)[1];

            if (lod == 0 && hid == 0x7F800000L)
                return "+INF 7F800000 00000000";
            else if (lod == 0 && hid == 0xFF800000L)
                return "-INF FF800000 00000000";
            else if ((hid & 0xfff00000) == 0x7ff00000)
                return string.Format("+NAN {0:X8} {0:X8}", hid, lod);
            else if ((hid & 0xfff00000) == 0xfff00000)
                return string.Format("-NAN {0:X8} {0:X8}", hid, lod);
            else if (d == 0.0)
                return "0.0";
            else
                return d.ToString();
        }

        public string PrintFloat10(decimal d)
        {
            throw new Exception();   
        }

        public string Print3DNow(void* f)
        {
            return PrintFloat4(*(float*)((char*)f + 4)) + ", " + PrintFloat4(*(float*)f);
        }

        public int IsFilling(uint addr, byte* data, uint size, uint align)
        {
            align--;

            if (addr < size &&
                (data[addr] == NOP || data[addr] == INT3) &&
                ((addr & align) != 0))
                return 1;

            if (addr + 1 < size &&
                ((data[addr] & 0xfe) == 0x86 || (data[addr] & 0xfc) == 0x88) &&
                ((data[addr + 1] & 0xc0) == 0xc0) &&
                ((((data[addr + 1] >> 3) ^ data[addr + 1]) & 0x07) == 0) &&
                ((addr & align) != 0x0f) &&
                ((addr & align) != 0x00))
                return 2;

            if (addr + 2 < size &&
                (data[addr] == 0x8d) &&
                ((data[addr + 1] & 0xc0) == 0x40) &&
                (data[addr + 2] == 0x00) &&
                ((data[addr + 1] & 0x07) != REG_ESP) &&
                ((((data[addr + 1] >> 3) ^ data[addr + 1]) & 0x07) == 0))
                return 3;

            if (addr + 3 < size &&
                (data[addr] == 0x8d) &&
                ((data[addr + 1] & 0xc0) == 0x40) &&
                (data[addr + 3] == 0x00) &&
                ((((data[addr + 1] >> 3) ^ data[addr + 2]) & 0x07) == 0))
                return 4;

            if (addr + 5 < size &&
                (data[addr] == 0x8d) &&
                (((data[addr + 1] & 0xc0) == 0x80) && (*(ulong*)(data + addr + 2) == 0)) &&
                ((data[addr + 1] & 0x07) != REG_ESP) &&
                ((((data[addr + 1] >> 3) ^ data[addr + 1]) & 0x07) == 0))
                return 6;

            return 0;
        }
    }
}
