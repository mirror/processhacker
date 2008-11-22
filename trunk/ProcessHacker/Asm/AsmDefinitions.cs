/* Original file: disasm.h
 * 
 * Free Disassembler and Assembler -- Header file
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
    public partial class Asm
    {
        public const int NegLimit = -16384;      // Limit to display constans as signed
        public const int PseudoOp = 128;         // Base for pseudooperands
        public const int TextLen = 256;          // Maximum length of text string

        public const byte WW = 0x01;             // Bit W (size of operand)
        public const byte SS = 0x02;             // Bit S (sign extension of immediate) 
        public const byte WS = 0x03;             // Bits W and S
        public const byte W3 = 0x08;             // Bit W at position 3
        public const byte CC = 0x10;             // Conditional jump
        public const byte FF = 0x20;             // Forced 16-bit size
        public const byte LL = 0x40;             // Conditional loop
        public const byte PR = 0x80;             // Protected command
        public const byte WP = 0x81;             // I/O command with bit W

        public const byte NNN = 0;               // No operand
        public const byte REG = 1;               // Integer register in Reg field
        public const byte RCM = 2;               // Integer register in command byte
        public const byte RG4 = 3;               // Integer 4-byte register in Reg field
        public const byte RAC = 4;               // Accumulator (AL/AX/EAX, implicit)
        public const byte RAX = 5;               // AX (2-byte, implicit)
        public const byte RDX = 6;               // DX (16-bit implicit port address)
        public const byte RCL = 7;               // Implicit CL register (for shifts)
        public const byte RS0 = 8;               // Top of FPU stack (ST(0), implicit)
        public const byte RST = 9;               // FPU register (ST(i)) in command byte
        public const byte RMX = 10;              // MMX register MMx
        public const byte R3D = 11;              // 3DNow! register MMx
        public const byte MRG = 12;              // Memory/register in ModRM byte
        public const byte MR1 = 13;              // 1-byte memory/register in ModRM byte
        public const byte MR2 = 14;              // 2-byte memory/register in ModRM byte
        public const byte MR4 = 15;              // 4-byte memory/register in ModRM byte
        public const byte RR4 = 16;              // 4-byte memory/register (register only)
        public const byte MR8 = 17;              // 8-byte memory/MMX register in ModRM
        public const byte RR8 = 18;              // 8-byte MMX register only in ModRM
        public const byte MRD = 19;              // 8-byte memory/3DNow! register in ModRM
        public const byte RRD = 20;              // 8-byte memory/3DNow! (register only)
        public const byte MRJ = 21;              // Memory/reg in ModRM as JUMP target
        public const byte MMA = 22;              // Memory address in ModRM byte for LEA
        public const byte MML = 23;              // Memory in ModRM byte (for LES)
        public const byte MMS = 24;              // Memory in ModRM byte (as SEG:OFFS)
        public const byte MM6 = 25;              // Memory in ModRm (6-byte descriptor)
        public const byte MMB = 26;              // Two adjacent memory locations (BOUND)
        public const byte MD2 = 27;              // Memory in ModRM (16-bit integer)
        public const byte MB2 = 28;              // Memory in ModRM (16-bit binary)
        public const byte MD4 = 29;              // Memory in ModRM byte (32-bit integer)
        public const byte MD8 = 30;              // Memory in ModRM byte (64-bit integer)
        public const byte MDA = 31;              // Memory in ModRM byte (80-bit BCD)
        public const byte MF4 = 32;              // Memory in ModRM byte (32-bit float)
        public const byte MF8 = 33;              // Memory in ModRM byte (64-bit float)
        public const byte MFA = 34;              // Memory in ModRM byte (80-bit float)
        public const byte MFE = 35;              // Memory in ModRM byte (FPU environment)
        public const byte MFS = 36;              // Memory in ModRM byte (FPU state)
        public const byte MFX = 37;              // Memory in ModRM byte (ext. FPU state)
        public const byte MSO = 38;              // Source in string op's ([ESI])
        public const byte MDE = 39;              // Destination in string op's ([EDI])
        public const byte MXL = 40;              // XLAT operand ([EBX+AL])
        public const byte IMM = 41;              // Immediate data (8 or 16/32)
        public const byte IMU = 42;              // Immediate unsigned data (8 or 16/32)
        public const byte VXD = 43;              // VxD service
        public const byte IMX = 44;              // Immediate sign-extendable byte
        public const byte C01 = 45;              // Implicit constant 1 (for shifts)
        public const byte IMS = 46;              // Immediate byte (for shifts)
        public const byte IM1 = 47;              // Immediate byte
        public const byte IM2 = 48;              // Immediate word (ENTER/RET)
        public const byte IMA = 49;              // Immediate absolute near data address
        public const byte JOB = 50;              // Immediate byte offset (for jumps)
        public const byte JOW = 51;              // Immediate full offset (for jumps)
        public const byte JMF = 52;              // Immediate absolute far jump/call addr
        public const byte SGM = 53;              // Segment register in ModRM byte
        public const byte SCM = 54;              // Segment register in command byte
        public const byte CRX = 55;              // Control register CRx
        public const byte DRX = 56;              // Debug register DRx

        // Pseudooperands (implicit operands, never appear in assembler commands). Must
        // have index equal to or exceeding PSEUDOOP.
        public const byte PRN = PseudoOp + 0;    // Near return address
        public const byte PRF = PseudoOp + 1;    // Far return address
        public const byte PAC = PseudoOp + 2;    // Accumulator (AL/AX/EAX)
        public const byte PAH = PseudoOp + 3;    // AH (in LAHF/SAHF commands)
        public const byte PFL = PseudoOp + 4;    // Lower byte of flags (in LAHF/SAHF)
        public const byte PS0 = PseudoOp + 5;    // Top of FPU stack (ST(0))
        public const byte PS1 = PseudoOp + 6;    // ST(1)
        public const byte PCX = PseudoOp + 7;    // CX/ECX
        public const byte PDI = PseudoOp + 8;    // EDI (in MMX extentions)

        // Errors detected during command disassembling.
        public const byte DAE_NOERR = 0;         // No error
        public const byte DAE_BADCMD = 1;        // Unrecognized command
        public const byte DAE_CROSS = 2;         // Command crosses end of memory block
        public const byte DAE_BADSEG = 3;        // Undefined segment register
        public const byte DAE_MEMORY = 4;        // Register where only memory allowed
        public const byte DAE_REGISTER = 5;      // Memory where only register allowed
        public const byte DAE_INTERN = 6;        // Internal error

        public struct TAddrDec
        {
            public int DefSeg;
            public string Descr;
        }

        public struct TCmdData
        {
            public uint Mask;                           // Mask for first 4 bytes of the command
            public uint Code;                           // Compare masked bytes with this
            public byte Length;                         // Length of the main command code
            public byte Bits;                           // Special bits within the command
            public byte Arg1, Arg2, Arg3;               // Types of possible arguments
            public byte Type;                           // C_xxx + additional information
            public string Name;                         // Symbolic name for this command
        }

        public const int MaxCmdSize = 16;
        public const int MaxCallSize = 8;
        public const int NModels = 8;

        public const byte INT3 = 0xcc;
        public const byte NOP = 0x90;
        public const uint TRAPFLAG = 0x00000100;

        public const int REG_EAX = 0;
        public const int REG_ECX = 1;
        public const int REG_EDX = 2;
        public const int REG_EBX = 3;
        public const int REG_ESP = 4;
        public const int REG_EBP = 5;
        public const int REG_ESI = 6;
        public const int REG_EDI = 7;

        public const int SEG_UNDEF = -1;
        public const int SEG_ES = 0;
        public const int SEG_CS = 1;
        public const int SEG_SS = 2;
        public const int SEG_DS = 3;
        public const int SEG_FS = 4;
        public const int SEG_GS = 5;

        public const byte C_TYPEMASK = 0xF0;
        public const byte C_CMD = 0x00;
        public const byte C_PSH = 0x10;
        public const byte C_POP = 0x20;
        public const byte C_MMX = 0x30;
        public const byte C_FLT = 0x40;
        public const byte C_JMP = 0x50;
        public const byte C_JMC = 0x60;
        public const byte C_CAL = 0x70;
        public const byte C_RET = 0x80;
        public const byte C_FLG = 0x90;
        public const byte C_RTF = 0xA0;
        public const byte C_REP = 0xB0;
        public const byte C_PRI = 0xC0;
        public const byte C_DAT = 0xD0;
        public const byte C_NOW = 0xE0;
        public const byte C_BAD = 0xF0;
        public const byte C_RARE = 0x08;
        public const byte C_SIZEMASK = 0x07;
        public const byte C_EXPL = 0x01;

        public const byte C_DANGER95 = 0x01;
        public const byte C_DANGER = 0x03;
        public const byte C_DANGERLOCK = 0x07;

        public const byte DEC_TYPEMASK = 0x1F;
        public const byte DEC_UNKNOWN = 0x00;
        public const byte DEC_BYTE = 0x01;
        public const byte DEC_WORD = 0x02;
        public const byte DEC_NEXTDATA = 0x03;
        public const byte DEC_DWORD = 0x04;
        public const byte DEC_FLOAT4 = 0x05;
        public const byte DEC_FWORD = 0x06;
        public const byte DEC_FLOAT8 = 0x07;
        public const byte DEC_QWORD = 0x08;
        public const byte DEC_FLOAT10 = 0x09;
        public const byte DEC_TBYTE = 0x0A;
        public const byte DEC_STRING = 0x0B;
        public const byte DEC_UNICODE = 0x0C;
        public const byte DEC_3DNOW = 0x0D;
        public const byte DEC_BYTESW = 0x11;
        public const byte DEC_NEXTCODE = 0x13;
        public const byte DEC_COMMAND = 0x1D;
        public const byte DEC_JMPDEST = 0x1E;
        public const byte DEC_CALLDEST = 0x1F;
        public const byte DEC_PROCMASK = 0x60;
        public const byte DEC_PROC = 0x20;
        public const byte DEC_PBODY = 0x40;
        public const byte DEC_PEND = 0x60;
        public const byte DEC_CHECKED = 0x80;

        public const byte DECR_TYPEMASK = 0x3F;
        public const byte DECR_BYTE = 0x21;
        public const byte DECR_WORD = 0x22;
        public const byte DECR_DWORD = 0x24;
        public const byte DECR_QWORD = 0x28;
        public const byte DECR_FLOAT10 = 0x29;
        public const byte DECR_SEG = 0x2A;
        public const byte DECR_3DNOW = 0x2D;
        public const byte DECR_ISREG = 0x20;

        public const byte DISASM_SIZE = 0;
        public const byte DISASM_DATA = 1;
        public const byte DISASM_FILE = 3;
        public const byte DISASM_CODE = 4;

        public const int DAW_FARADDR = 0x0001;
        public const int DAW_SEGMENT = 0x0002;
        public const int DAW_PRIV = 0x0004;
        public const int DAW_IO = 0x0008;
        public const int DAW_SHIFT = 0x0010;
        public const int DAW_PREFIX = 0x0020;
        public const int DAW_LOCK = 0x0040;
        public const int DAW_STACK = 0x0080;
        public const int DAW_DANGER95 = 0x1000;
        public const int DAW_DANGEROUS = 0x3000;

        public struct Disasm
        {
            public uint IP;            // Instrucion pointer
            public string Dump;        // Hexadecimal dump of the command
            public StringBuilder Result;      // Disassembled command
            public string Comment;     // Brief comment
            public int CmdType;        // One of C_xxx
            public int MemType;        // Type of addressed variable in memory
            public int NPrefix;        // Number of prefixes
            public int Indexed;        // Address contains register(s)
            public uint JmpConst;      // Constant jump address
            public int JmpTable;      // Possible address of switch table
            public int AdrConst;      // Constant part of address
            public int ImmConst;      // Immediate constant
            public int ZeroConst;      // Whether contains zero constant
            public int FixupOffset;    // Possible offset of 32-bit fixups
            public int FixupSize;      // Possible total size of fixups or 0
            public int Error;          // Error while disassembling command
            public int Warnings;       // Combination of DAW_xxx      
        }

        public struct AsmModel
        {
            public byte[] Code;        // Binary code
            public byte[] Mask;        // Mask for binary code (0: bit ignored)
            public int Length;         // Length of code, bytes (0: empty)
            public int JmpSize;        // Offset size if relative jump
            public int JmpOffset;      // Offset relative to IP
            public int JmpPos;         // Position of jump offset in command
        }

        public static bool Ideal;                // Force IDEAL decoding mode
        public static bool Lowercase = true;            // Force lowercase display
        public static int TabArguments;         // Tab between mnemonic and arguments
        public static int ExtraSpace;           // Extra space between arguments
        public static int PutDefSeg;            // Display default segments in listing
        public static int ShowMemSize;          // Always show memory size
        public static int ShowNear;             // Show NEAR modifiers
        public static int ShortStringCmds;      // Use short form of string commands
        public static int SizeSens;             // How to decode size-sensitive mnemonics
        public static bool Symbolic;             // Show symbolic addresses in disasm
        public static int Farcalls;             // Accept far calls, returns & addresses
        public static int DecodeVxd;            // Decode VxD calls (Win95/98)
        public static int Privileged;           // Accept privileged commands
        public static int IOCommand;            // Accept I/O commands
        public static int BadShift;             // Accept shift out of range 1..31
        public static int ExtraPrefix;          // Accept superfluous prefixes
        public static int LockedBus;            // Accept LOCK prefixes
        public static int StackAlign;           // Accept unaligned stack operations
        public static int IsWindowsNT;          // When checking for dangers, assume NT
    }
}
