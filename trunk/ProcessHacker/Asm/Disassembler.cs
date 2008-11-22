/* Original file: disasm.c
 * 
 * Free Disassembler and Assembler -- Disassembler
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

namespace ProcessHacker
{
    public unsafe partial class Asm
    {
        public class Disassembler
        {
            private int _dataSize;
            private int _addrSize;
            private int _segPrefix;
            private bool _hasRM;
            private bool _hasSIB;
            private int _dispSize;
            private int _immSize;
            private int _softError;
            private bool _addComment;

            private byte* _cmd;
            private byte* _pFixup;
            private int _size;
            private Disasm _da;
            private int _mode;

            private void DecodeRG(int index, int dataSize, int type)
            {
                int sizeIndex;
                string name = "";

                if (_mode < DISASM_DATA)
                    return;

                index &= 0x07;

                if (dataSize == 1)
                    sizeIndex = 0;
                else if (dataSize == 2)
                    sizeIndex = 1;
                else if (dataSize == 4)
                    sizeIndex = 2;
                else
                {
                    _da.Error = DAE_INTERN;
                    return;
                }

                if (_mode >= DISASM_FILE)
                {
                    name = RegName[sizeIndex][index];

                    if (Lowercase)
                        name = name.ToLower();
                    if (type < PseudoOp)
                        _da.Result.Append(name);
                }
            }

            private void DecodeST(int index, int pseudoOp)
            {
                if (_mode < DISASM_FILE)
                    return;

                index &= 0x07;

                if (pseudoOp == 0)
                {
                    _da.Result.Append(string.Format("{0}({1})", Lowercase ? "st" : "ST", index));
                }
            }

            private void DecodeMX(int index)
            {
                if (_mode < DISASM_FILE)
                    return;

                index &= 0x07;

                _da.Result.Append(string.Format("{0}{1}", Lowercase ? "mm" : "MM", index));
            }

            private void DecodeNR(int index)
            {
                if (_mode < DISASM_FILE)
                    return;

                index &= 0x07;

                _da.Result.Append(string.Format("{0}{1}", Lowercase ? "mm" : "MM", index));
            }

            private string DecodeAddress(int addr, out string comment)
            {
                comment = "";

                return "0x" + addr.ToString("x8");
            }

            private void MemAdr(int defSeg, string descr, int offset, int dsize)
            {
                int seg, i;
                string pr = "";
                string s = "";

                if (_mode < DISASM_FILE)
                    return;

                if (_segPrefix != SEG_UNDEF)
                    seg = _segPrefix;
                else
                    seg = defSeg;

                if (Ideal)
                    pr += "[";

                if (ShowMemSize != 0 ||
                    (_da.CmdType & C_TYPEMASK) == C_MMX ||
                    (_da.CmdType & C_TYPEMASK) == C_NOW ||
                    (_da.CmdType & C_EXPL) != 0)
                {
                    if (dsize < SizeName.Length)
                        pr += string.Format("{0} {1}", SizeName[dsize], (!Ideal ? "PTR " : ""));
                    else
                        pr += string.Format("({0}-BYTE) {1}", dsize, (!Ideal ? "PTR " : ""));
                }

                if ((PutDefSeg != 0 || seg != defSeg) && seg != SEG_UNDEF)
                    pr += SegName[seg] + ":";

                if (Ideal)
                    pr += "[";

                pr += descr;

                if (Lowercase)
                    pr = pr.ToLower();

                if (offset == 0)
                {
                    if (descr == "")
                        pr += "0";
                }
                else
                {
                    if (Symbolic && _mode >= DISASM_CODE)
                    {
                        string t;

                        s = DecodeAddress(offset, out t);
                        i = 1;
                    }
                    else
                        i = 0;

                    if (i > 0)
                    {
                        if (descr != "")
                            pr += "+";

                        pr += s;
                    }
                    else if (offset < 0 && offset > -16384 && descr != "")
                        pr += string.Format("-{0:X}", -offset);
                    else
                    {
                        if (descr != "")
                            pr += "+";

                        pr += string.Format("{0:X}", offset);
                    }
                }

                pr += "]";

                _da.Result.Append(pr);
            }

            private void DecodeMR(int type)
            {
                bool inmemory, memonly;
                int j;
                int seg = 0;
                int c, sib;
                int dsize, regsize, addr;
                string s = "";

                if (_size < 2)
                {
                    _da.Error = DAE_CROSS;
                    return;
                }

                _hasRM = true;
                dsize = regsize = _dataSize;
                memonly = false;

                c = _cmd[1] & 0xc7;

                if (_mode >= DISASM_DATA)
                {
                    if ((c & 0xc0) == 0xc0)
                        inmemory = false;
                    else
                        inmemory = true;

                    #region Switch

                    switch (type)
                    {
                        case MRG:
                            if (inmemory)
                            {
                                if (_dataSize == 1)
                                    _da.MemType = DEC_BYTE;
                                else if (_dataSize == 2)
                                    _da.MemType = DEC_WORD;
                                else
                                    _da.MemType = DEC_DWORD;
                            }

                            break;

                        case MRJ:
                            if (_dataSize != 2 && inmemory)
                                _da.MemType = DEC_DWORD;
                            if (_mode >= DISASM_FILE && ShowNear != 0)
                                _da.Result.Append(Lowercase ? "near " : "NEAR ");

                            break;

                        case MR1:
                            dsize = regsize = 1;

                            if (inmemory)
                                _da.MemType = DEC_BYTE;

                            break;

                        case MR2:
                            dsize = regsize = 2;

                            if (inmemory)
                                _da.MemType = DEC_WORD;

                            break;

                        case MR4:
                        case RR4:
                            dsize = regsize = 4;

                            if (inmemory)
                                _da.MemType = DEC_DWORD;

                            break;

                        case MR8:
                        case RR8:
                            dsize = 8;

                            if (inmemory)
                                _da.MemType = DEC_QWORD;

                            break;

                        case MRD:
                        case RRD:
                            dsize = 8;

                            if (inmemory)
                                _da.MemType = DEC_3DNOW;

                            break;

                        case MMA:
                            memonly = true;

                            break;

                        case MML:
                            dsize = _dataSize + 2;
                            memonly = true;

                            if (_dataSize == 4 && inmemory)
                                _da.MemType = DEC_FWORD;

                            _da.Warnings |= DAW_SEGMENT;

                            break;

                        case MMS:
                            dsize = _dataSize + 2;
                            memonly = true;

                            if (_dataSize == 4 && inmemory)
                                _da.MemType = DEC_FWORD;
                            if (_mode >= DISASM_FILE)
                                _da.Result.Append(Lowercase ? "far " : "FAR ");

                            break;

                        case MM6:
                            dsize = 6;
                            memonly = true;

                            if (inmemory)
                                _da.MemType = DEC_FWORD;

                            break;

                        case MMB:
                            dsize = (Ideal ? _dataSize : _dataSize * 2);
                            memonly = true;

                            break;

                        case MD2:
                        case MB2:
                            dsize = 2;
                            memonly = true;

                            if (inmemory)
                                _da.MemType = DEC_WORD;

                            break;

                        case MD4:
                            dsize = 4;
                            memonly = true;

                            if (inmemory)
                                _da.MemType = DEC_DWORD;

                            break;

                        case MD8:
                            dsize = 8;
                            memonly = true;

                            if (inmemory)
                                _da.MemType = DEC_QWORD;

                            break;

                        case MDA:
                            dsize = 10;
                            memonly = true;

                            if (inmemory)
                                _da.MemType = DEC_TBYTE;

                            break;

                        case MF4:
                            dsize = 4;
                            memonly = true;

                            if (inmemory)
                                _da.MemType = DEC_FLOAT4;

                            break;

                        case MF8:
                            dsize = 8;
                            memonly = true;

                            if (inmemory)
                                _da.MemType = DEC_FLOAT8;

                            break;

                        case MFA:
                            dsize = 10;
                            memonly = true;

                            if (inmemory)
                                _da.MemType = DEC_FLOAT10;

                            break;

                        case MFE:
                            dsize = 28;
                            memonly = true;

                            break;

                        case MFS:
                            dsize = 108;
                            memonly = true;

                            break;

                        case MFX:
                            dsize = 512;
                            memonly = true;

                            break;

                        default:
                            _da.Error = DAE_INTERN;

                            break;
                    }

                    #endregion
                }

                addr = 0;

                if ((c & 0xc0) == 0xc0)
                {
                    if (type == MR8 || type == RR8)
                        DecodeMX(c);
                    else if (type == MRD || type == RRD)
                        DecodeNR(c);
                    else
                        DecodeRG(c, regsize, type);

                    if (memonly)
                        _softError = DAE_MEMORY;

                    return;
                }

                if (_addrSize == 2)
                {
                    if (c == 0x06)
                    {
                        _dispSize = 2;

                        if (_size < 4)
                            _da.Error = DAE_CROSS;
                        else if (_mode >= DISASM_DATA)
                        {
                            _da.AdrConst = addr = *(ushort*)(_cmd + 2);

                            if (addr == 0)
                                _da.ZeroConst = 1;

                            seg = SEG_DS;
                            MemAdr(seg, "", addr, dsize);
                        }
                    }
                    else
                    {
                        _da.Indexed = 1;

                        if ((c & 0xc0) == 0x40)
                        {
                            if (_size < 3)
                                _da.Error = DAE_CROSS;
                            else
                                addr = (sbyte)_cmd[2] & 0xffff;

                            _dispSize = 1;
                        }
                        else if ((c & 0xc0) == 0x80)
                        {
                            if (_size < 4)
                                _da.Error = DAE_CROSS;
                            else
                                addr = *(ushort*)(_cmd + 2);

                            _dispSize = 2;
                        }

                        if (_mode >= DISASM_DATA && _da.Error == DAE_NOERR)
                        {
                            _da.AdrConst = addr;

                            if (addr == 0)
                                _da.ZeroConst = 1;

                            seg = Addr16[c & 0x07].DefSeg;
                            MemAdr(seg, Addr16[c & 0x07].Descr, addr, dsize);
                        }
                    }
                }
                else if (c == 0x05)
                {
                    _dispSize = 4;

                    if (_size < 6)
                        _da.Error = DAE_CROSS;
                    else if (_mode >= DISASM_DATA)
                    {
                        _da.AdrConst = addr = *(int*)(_cmd + 2);

                        if (_pFixup == null)
                            _pFixup = _cmd + 2;

                        _da.FixupSize += 4;

                        if (addr == 0)
                            _da.ZeroConst = 1;

                        seg = SEG_DS;
                        MemAdr(seg, "", addr, dsize);
                    }
                }
                else if ((c & 0x07) == 0x04)
                {
                    sib = _cmd[2];
                    _hasSIB = true;

                    if (c == 0x04 && (sib & 0x07) == 0x05)
                    {
                        _dispSize = 4;

                        if (_size < 7)
                            _da.Error = DAE_CROSS;
                        else
                        {
                            _da.AdrConst = addr = *(int*)(_cmd + 3);

                            if (_pFixup == null)
                                _pFixup = _cmd + 3;

                            _da.FixupSize += 4;

                            if (addr == 0)
                                _da.ZeroConst = 1;

                            if ((sib & 0x38) != 0x20)
                            {
                                _da.Indexed = 1;

                                if (type == MRJ)
                                    _da.JmpTable = addr;
                            }
                            seg = SEG_DS;
                        }
                    }
                    else
                    {
                        if ((c & 0xc0) == 0x40)
                        {
                            _dispSize = 1;

                            if (_size < 4)
                                _da.Error = DAE_CROSS;
                            else
                            {
                                _da.AdrConst = addr = (sbyte)_cmd[3];

                                if (addr == 0)
                                    _da.ZeroConst = 1;
                            }
                        }
                        else if ((c & 0xc0) == 0x80)
                        {
                            _dispSize = 4;

                            if (_size < 7)
                                _da.Error = DAE_CROSS;
                            else
                            {
                                _da.AdrConst = addr = *(int*)(_cmd + 3);

                                if (_pFixup == null)
                                    _pFixup = _cmd + 3;

                                _da.FixupSize += 4;

                                if (addr == 0)
                                    _da.ZeroConst = 1;

                                if (type == MRJ)
                                    _da.JmpTable = addr;
                            }
                        }

                        _da.Indexed = 1;
                        j = sib & 0x07;

                        if (_mode >= DISASM_FILE)
                        {
                            s = RegName[2][j];
                            seg = Addr32[j].DefSeg;
                        }
                    }

                    if ((sib & 0x38) != 0x20)
                    {
                        if ((sib & 0xc0) == 0x40)
                            _da.Indexed = 2;
                        else if ((sib & 0xc0) == 0x80)
                            _da.Indexed = 4;
                        else if ((sib & 0xc0) == 0xc0)
                            _da.Indexed = 8;
                        else
                            _da.Indexed = 1;
                    }

                    if (_mode >= DISASM_FILE && _da.Error == DAE_NOERR)
                    {
                        if ((sib & 0x38) != 0x20)
                        {
                            if (s != "")
                                s += "+";

                            s += Addr32[(sib >> 3) & 0x07].Descr;

                            if ((sib & 0xc0) == 0x40)
                            {
                                _da.JmpTable = 0;
                                s += "*2";
                            }
                            else if ((sib & 0xc0) == 0x80)
                            {
                                s += "*4";
                            }
                            else if ((sib & 0xc0) == 0xc0)
                            {
                                _da.JmpTable = 0;
                                s += "*8";
                            }
                        }

                        MemAdr(seg, s, addr, dsize);
                    }
                }
                else
                {
                    if ((c & 0xc0) == 0x40)
                    {
                        _dispSize = 1;

                        if (_size < 3)
                            _da.Error = DAE_CROSS;
                        else
                        {
                            _da.AdrConst = addr = (sbyte)_cmd[2];

                            if (addr == 0)
                                _da.ZeroConst = 1;
                        }
                    }
                    else if ((c & 0xc0) == 0x80)
                    {
                        _dispSize = 4;

                        if (_size < 6)
                            _da.Error = DAE_CROSS;
                        else
                        {
                            _da.AdrConst = addr = *(int*)(_cmd + 2);

                            if (_pFixup == null)
                                _pFixup = _cmd + 2;

                            _da.FixupSize += 4;

                            if (addr == 0)
                                _da.ZeroConst = 1;

                            if (type == MRJ)
                                _da.JmpTable = addr;
                        }
                    }

                    _da.Indexed = 1;

                    if (_mode == DISASM_FILE && _da.Error == DAE_NOERR)
                    {
                        seg = Addr32[c & 0x07].DefSeg;
                        MemAdr(seg, Addr32[c & 0x07].Descr, addr, dsize);
                    }
                }
            }

            private void DecodeSO()
            {
                if (_mode < DISASM_FILE)
                    return;

                if (_dataSize == 1)
                    _da.MemType = DEC_BYTE;
                else if (_dataSize == 2)
                    _da.MemType = DEC_WORD;
                else if (_dataSize == 4)
                    _da.MemType = DEC_DWORD;

                _da.Indexed = 1;
                MemAdr(SEG_DS, RegName[_addrSize == 2 ? 1 : 2][REG_ESI], 0, _dataSize);
            }

            private void DecodeDE()
            {
                int seg;

                if (_mode < DISASM_FILE)
                    return;

                if (_dataSize == 1)
                    _da.MemType = DEC_BYTE;
                else if (_dataSize == 2)
                    _da.MemType = DEC_WORD;
                else if (_dataSize == 4)
                    _da.MemType = DEC_DWORD;

                _da.Indexed = 1;
                seg = _segPrefix;
                _segPrefix = SEG_ES;
                MemAdr(SEG_DS, RegName[_addrSize == 2 ? 1 : 2][REG_EDI], 0, _dataSize);
                _segPrefix = seg;
            }

            private void DecodeXL()
            {
                if (_mode < DISASM_FILE)
                    return;

                _da.MemType = DEC_BYTE;
                _da.Indexed = 1;
                MemAdr(SEG_DS, (_addrSize == 2 ? "BX+AL" : "EBX+AL"), 0, 1);
            }

            private void DecodeIM(int constSize, int sxt, int type)
            {
                int i;
                int data;
                int l;
                string name = "";
                string comment = "";

                _immSize += constSize;

                if (_mode < DISASM_DATA)
                    return;

                l = 1 + (_hasRM ? 1 : 0) + (_hasSIB ? 1 : 0) + _dispSize + (_immSize - constSize);
                data = 0;

                if (_size < l + constSize)
                    _da.Error = DAE_CROSS;
                else if (constSize == 1)
                {
                    if (sxt == 0)
                        data = _cmd[l];
                    else
                        data = (sbyte)_cmd[l];

                    if (type == IMS && ((data & 0xe0) != 0 || data == 0))
                    {
                        _da.Warnings |= DAW_SHIFT;
                        _da.CmdType |= C_RARE;
                    }
                }
                else if (constSize == 2)
                {
                    if (sxt == 0)
                        data = *(ushort*)(_cmd + l);
                    else
                        data = *(short*)(_cmd + l);
                }
                else
                {
                    data = *(int*)(_cmd + l);

                    if (_pFixup == null)
                        _pFixup = _cmd + l;

                    _da.FixupSize += 4;
                }

                if (sxt == 2)
                    data &= 0x0000ffff;
                if (data == 0 && _da.Error == 0)
                    _da.ZeroConst = 1;

                if (_da.ImmConst == 0)
                    _da.ImmConst = data;
                if (_mode >= DISASM_FILE && _da.Error == DAE_NOERR)
                {
                    if (_mode >= DISASM_CODE && type != IMU)
                    {
                        name = DecodeAddress(data, out comment);
                        i = 1;
                    }
                    else
                    {
                        i = 0;
                        comment = "";
                    }

                    if (i != 0 && Symbolic)
                        _da.Result.Append(name);
                    else if (type == IMU || type == IMS || type == IM2 || data >= 0 || data < NegLimit)
                        _da.Result.Append(string.Format("{0:X}", data));
                    else
                        _da.Result.Append(string.Format("-{0:X}", -data));

                    if (_addComment && comment != "")
                        _da.Comment += comment;
                }
            }

            private void DecodeVX()
            {
                int l, data;
                _immSize += 4;

                if (_mode < DISASM_DATA)
                    return;

                l = 1 + (_hasRM ? 1 : 0) + (_hasSIB ? 1 : 0) + _dispSize + (_immSize - 4);

                if (_size < l + 4)
                {
                    _da.Error = DAE_CROSS;
                    return;
                }

                data = *(int*)(_cmd + l);

                if (data == 0 && _da.Error == 0)
                    _da.ZeroConst = 1;

                if (_da.ImmConst == 0)
                    _da.ImmConst = data;

                if (_mode >= DISASM_FILE && _da.Error == DAE_NOERR)
                {
                    if ((data & 0x00008000) != 0 && _da.Result.ToString().StartsWith("VxDCall"))
                        _da.Result.Append(Lowercase ? "vxdjmp" : "VxDJump");

                    _da.Result.Append(string.Format("{0:X}", data));
                }
            }

            private void DecodeC1()
            {
                if (_mode < DISASM_DATA)
                    return;

                _da.ImmConst = 1;

                if (_mode >= DISASM_FILE)
                    _da.Result.Append("1");
            }

            private void DecodeIA()
            {
                int addr;

                if (_size < 1 + _addrSize)
                {
                    _da.Error = DAE_CROSS;
                    return;
                }

                _dispSize = _addrSize;

                if (_mode < DISASM_DATA)
                    return;

                if (_dataSize == 1)
                    _da.MemType = DEC_BYTE;
                else if (_dataSize == 2)
                    _da.MemType = DEC_WORD;
                else if (_dataSize == 4)
                    _da.MemType = DEC_DWORD;

                if (_addrSize == 2)
                    addr = *(ushort*)(_cmd + 1);
                else
                {
                    addr = *(int*)(_cmd + 1);

                    if (_pFixup == null)
                        _pFixup = _cmd + 1;

                    _da.FixupSize += 4;
                }

                _da.AdrConst = addr;

                if (addr == 0)
                    _da.ZeroConst = 1;

                if (_mode >= DISASM_FILE)
                    MemAdr(SEG_DS, "", addr, _dataSize);
            }

            private void DecodeRJ(int offsize, int nextIp)
            {
                int i;
                int addr;
                string s = "";

                if (_size < offsize + 1)
                {
                    _da.Error = DAE_CROSS;
                    return;
                }

                _dispSize = offsize;

                if (_mode < DISASM_DATA)
                    return;

                if (offsize == 1)
                    addr = (sbyte)_cmd[1] + nextIp;
                else if (offsize == 2)
                    addr = *(short*)(_cmd + 1) + nextIp;
                else
                    addr = *(int*)(_cmd + 1) + nextIp;

                if (_dataSize == 2)
                    addr &= 0xffff;

                _da.JmpConst = addr;

                if (_mode >= DISASM_FILE)
                {
                    if (offsize == 1)
                        _da.Result.Append(Lowercase ? "short " : "SHORT ");

                    if (_mode >= DISASM_CODE)
                    {
                        s = DecodeAddress(addr, out _da.Comment);
                        i = s.Length;
                    }
                    else
                    {
                        i = 0;
                    }

                    if (!Symbolic || i == 0)
                        _da.Result.Append(string.Format("{0:x8}", addr));
                    else
                        _da.Result.Append(s);

                    if (!Symbolic && i != 0 && _da.Comment != "")
                        _da.Comment = s;
                }
            }

            private void DecodeJF()
            {
                int addr, seg;

                if (_size < (1 + _addrSize + 2))
                {
                    _da.Error = DAE_CROSS;
                    return;
                }

                _dispSize = _addrSize;
                _immSize = 2;

                if (_mode < DISASM_DATA)
                    return;

                if (_addrSize == 2)
                {
                    addr = *(ushort*)(_cmd + 1);
                    seg = *(ushort*)(_cmd + 3);
                }
                else
                {
                    addr = *(int*)(_cmd + 1);
                    seg = *(ushort*)(_cmd + 5);
                }

                _da.JmpConst = addr;
                _da.ImmConst = seg;

                if (addr == 0 || seg == 0)
                    _da.ZeroConst = 1;

                if (_mode >= DISASM_FILE)
                {
                    _da.Result.Append(string.Format("{0} {1:x4}:{1:x8}",
                        Lowercase ? "far" : "FAR", seg, addr));
                }
            }

            private void DecodeSG(int index)
            {
                if (_mode < DISASM_DATA)
                    return;

                index &= 0x07;

                if (index >= 6)
                    _softError = DAE_BADSEG;

                if (_mode >= DISASM_FILE)
                {
                    _da.Result.Append(Lowercase ? SegName[index].ToLower() : SegName[index]);
                }
            }

            private void DecodeCR(int index)
            {
                _hasRM = true;

                if (_mode >= DISASM_FILE)
                {
                    index = (index >> 3) & 0x07;
                    _da.Result.Append(Lowercase ? CrName[index].ToLower() : CrName[index]);
                }
            }

            private void DecodeDR(int index)
            {
                _hasRM = true;

                if (_mode >= DISASM_FILE)
                {
                    index = (index >> 3) & 0x07;
                    _da.Result.Append(Lowercase ? DrName[index].ToLower() : DrName[index]);
                }
            }

            private int Get3DNowSuffix()
            {
                int c, sib;
                int offset;

                if (_size < 3)
                    return -1;

                offset = 3;
                c = _cmd[3] & 0xc7;

                if ((c & 0xc0) == 0xc0)
                { }
                else if (_addrSize == 2)
                {
                    if (c == 0x06)
                        offset += 2;
                    else if ((c & 0xc0) == 0x40)
                        offset++;
                    else if ((c & 0xc0) == 0x80)
                        offset += 2;
                }
                else if (c == 0x05)
                    offset += 4;
                else if ((c & 0x07) == 0x04)
                {
                    if (_size < 4)
                        return -1;

                    sib = _cmd[3];
                    offset++;

                    if (c == 0x04 && (sib & 0x07) == 0x05)
                        offset += 4;
                    else if ((c & 0xc0) == 0x40)
                        offset += 1;
                    else if ((c & 0xc0) == 0x80)
                        offset += 4;
                }
                else if ((c & 0xc0) == 0x40)
                    offset += 1;
                else if ((c & 0xc0) == 0x80)
                    offset += 4;

                if (offset >= _size)
                    return -1;

                return _cmd[offset];
            }

            private int CheckCondition(int code, int flags)
            {
                int cond, temp;

                switch (code & 0x0e)
                {
                    case 0:
                        cond = flags & 0x0800;
                        break;
                    case 2:
                        cond = flags & 0x0001;
                        break;
                    case 4:
                        cond = flags & 0x0040;
                        break;
                    case 6:
                        cond = flags & 0x0041;
                        break;
                    case 8:
                        cond = flags & 0x0080;
                        break;
                    case 10:
                        cond = flags & 0x0004;
                        break;
                    case 12:
                        temp = flags & 0x0880;
                        cond = (temp == 0x0800 || temp == 0x0080) ? 1 : 0;
                        break;
                    case 14:
                        temp = flags & 0x0880;
                        cond = (temp == 0x0800 || temp == 0x0080 || (flags & 0x0040) != 0) ? 1 : 0;
                        break;

                    default:
                        return -1;
                }

                if ((code & 0x01) == 0)
                    return (cond != 0) ? 1 : 0;
                else
                    return (cond == 0) ? 1 : 0;
            }

            public int Disassemble(byte[] src, int srcsize, int srcIp, int disasmmode)
            {
                fixed (byte* pSrc = src)
                {
                    return Disassemble(pSrc, srcsize, srcIp, disasmmode);
                }
            }

            public int Disassemble(byte* src, int srcsize, int srcIp, int disasmmode)
            {
                bool repeated, is3dnow;
                int searchi = 0;
                int i, j, isprefix, operand, mnemosize, arg;
                int u, code;
                int lockprefix;
                int repprefix;
                int cxsize;
                string name, pname;
                TCmdData pd = new TCmdData();

                #region Initialization

                _dataSize = _addrSize = 4;
                _segPrefix = SEG_UNDEF;
                _hasRM = _hasSIB = false;
                _dispSize = _immSize = 0;
                lockprefix = 0;
                repprefix = 0;
                _cmd = src;
                _size = srcsize;
                _pFixup = null;
                _softError = 0;
                is3dnow = false;
                _da.Result = new StringBuilder();
                _da.Dump = new StringBuilder();
                _da.IP = srcIp;
                _da.Comment = "";
                _da.CmdType = C_BAD;
                _da.NPrefix = 0;
                _da.MemType = DEC_UNKNOWN;
                _da.Indexed = 0;
                _da.JmpConst = 0;
                _da.JmpTable = 0;
                _da.AdrConst = 0;
                _da.ImmConst = 0;
                _da.ZeroConst = 0;
                _da.FixupOffset = 0;
                _da.FixupSize = 0;
                _da.Warnings = 0;
                _da.Error = DAE_NOERR;
                _mode = disasmmode;

                u = 0;
                repeated = false;

                #endregion

                while (_size > 0)
                {
                    isprefix = 1;

                    #region Switch

                    switch (*_cmd)
                    {
                        case 0x26:
                            if (_segPrefix == SEG_UNDEF)
                                _segPrefix = SEG_ES;
                            else
                                repeated = true;
                            break;

                        case 0x2e:
                            if (_segPrefix == SEG_UNDEF)
                                _segPrefix = SEG_CS;
                            else
                                repeated = true;
                            break;

                        case 0x36:
                            if (_segPrefix == SEG_UNDEF)
                                _segPrefix = SEG_SS;
                            else
                                repeated = true;
                            break;

                        case 0x3e:
                            if (_segPrefix == SEG_UNDEF)
                                _segPrefix = SEG_DS;
                            else
                                repeated = true;
                            break;

                        case 0x64:
                            if (_segPrefix == SEG_UNDEF)
                                _segPrefix = SEG_FS;
                            else
                                repeated = true;
                            break;

                        case 0x65:
                            if (_segPrefix == SEG_UNDEF)
                                _segPrefix = SEG_GS;
                            else
                                repeated = true;
                            break;

                        case 0x66:
                            if (_dataSize == 4)
                                _dataSize = 2;
                            else
                                repeated = true;
                            break;

                        case 0x67:
                            if (_addrSize == 4)
                                _addrSize = 2;
                            else
                                repeated = true;
                            break;

                        case 0xf0:
                            if (lockprefix == 0)
                                lockprefix = 0xf0;
                            else
                                repeated = true;
                            break;

                        case 0xf2:
                            if (repprefix == 0)
                                repprefix = 0xf2;
                            else
                                repeated = true;
                            break;

                        case 0xf3:
                            if (repprefix == 0)
                                repprefix = 0xf3;
                            else
                                repprefix = 1;
                            break;

                        default:
                            isprefix = 0;
                            break;
                    }

                    #endregion

                    if (isprefix == 0 || repeated)
                        break;
                    if (_mode >= DISASM_FILE)
                        _da.Dump.Append(string.Format("{0:x2}:", *_cmd));

                    _da.NPrefix++;
                    _cmd++;
                    srcIp++;
                    _size--;
                    u++;
                }

                if (repeated)
                {
                    if (_mode >= DISASM_FILE)
                    {
                        _da.Dump = new StringBuilder(_da.Dump.ToString().Substring(0, 2));
                        _da.NPrefix = 1;

                        #region Switch

                        switch (_cmd[-u])
                        {
                            case 0x26:
                                pname = SegName[SEG_ES];
                                break;
                            case 0x2e:
                                pname = SegName[SEG_CS];
                                break;
                            case 0x36:
                                pname = SegName[SEG_SS];
                                break;
                            case 0x3e:
                                pname = SegName[SEG_DS];
                                break;
                            case 0x64:
                                pname = SegName[SEG_FS];
                                break;
                            case 0x65:
                                pname = SegName[SEG_GS];
                                break;
                            case 0x66:
                                pname = "DATASIZE";
                                break;
                            case 0x67:
                                pname = "ADDRSIZE";
                                break;
                            case 0xf0:
                                pname = "LOCK";
                                break;
                            case 0xf2:
                                pname = "REPNE";
                                break;
                            case 0xf3:
                                pname = "REPE";
                                break;
                            default:
                                pname = "?";
                                break;
                        }

                        #endregion

                        _da.Result.Append((Lowercase ? "prefix" : "PREFIX") + " " + (Lowercase ? pname.ToLower() : pname));

                        if (!ExtraPrefix)
                            _da.Comment = "Superfluous prefix";
                    }

                    _da.Warnings |= DAW_PREFIX;

                    if (lockprefix != 0)
                        _da.Warnings |= DAW_LOCK;

                    _da.CmdType = C_RARE;

                    return 1;
                }

                if (lockprefix != 0)
                {
                    if (_mode >= DISASM_FILE)
                        _da.Result.Append("LOCK ");

                    _da.Warnings |= DAW_LOCK;
                }

                code = 0;

                if (_size > 0) *(((byte*)&code) + 0) = _cmd[0];
                if (_size > 1) *(((byte*)&code) + 1) = _cmd[1];
                if (_size > 2) *(((byte*)&code) + 2) = _cmd[2];

                if (repprefix != 0)
                    code = (code << 8) | repprefix;

                if (DecodeVxd && (code & 0xffff) == 0x20cd)
                    pd = VxdCmd[0];
                else
                {
                    for (searchi = 0; searchi < CmdData.Length; searchi++)
                    {
                        TCmdData pdi = CmdData[searchi];

                        if (((code ^ pdi.Code) & pdi.Mask) != 0)
                            continue;

                        if (_mode >= DISASM_FILE && ShortStringCmds &&
                            (pdi.Arg1 == MSO || pdi.Arg1 == MDE || pdi.Arg2 == MSO || pdi.Arg2 == MDE))
                            continue;

                        pd = pdi;
                        break;
                    }
                }

                if ((pd.Type & C_TYPEMASK) == C_NOW)
                {
                    is3dnow = true;
                    j = Get3DNowSuffix();

                    if (j < 0)
                        _da.Error = DAE_CROSS;
                    else
                    {
                        for (; searchi < CmdData.Length; searchi++)
                        {
                            if (((code ^ pd.Code) & pd.Mask) != 0)
                                continue;

                            if (((byte*)&pd.Code)[2] == j)
                                break;
                        }
                    }
                }

                if (pd.Mask == 0)
                {
                    _da.CmdType = C_BAD;

                    if (_size < 2)
                        _da.Error = DAE_CROSS;
                    else
                        _da.Error = DAE_BADCMD;
                }
                else
                {
                    _da.CmdType = pd.Type;
                    cxsize = pd.Type;

                    if (_segPrefix == SEG_FS || _segPrefix == SEG_GS || lockprefix != 0)
                        _da.CmdType |= C_RARE;

                    if (pd.Bits == PR)
                        _da.Warnings |= DAW_PRIV;
                    else if (pd.Bits == WP)
                        _da.Warnings |= DAW_IO;

                    if (_cmd[0] == 0x44 || _cmd[0] == 0x4C ||
                        (_size >= 3 && (_cmd[0] == 0x81 || _cmd[0] == 0x83) &&
                        (_cmd[1] == 0xC4 || _cmd[1] == 0xEC) && (_cmd[2] & 0x03) != 0))
                    {
                        _da.Warnings |= DAW_STACK;
                        _da.CmdType |= C_RARE;
                    }

                    if (_cmd[0] == 0x8e)
                        _da.Warnings |= DAW_SEGMENT;

                    if (pd.Length == 2)
                    {
                        if (_size == 0)
                            _da.Error = DAE_CROSS;
                        else
                        {
                            if (_mode >= DISASM_FILE)
                                _da.Dump.Append(string.Format("{0:x2}", *_cmd));

                            _cmd++;
                            srcIp++;
                            _size--;
                        }
                    }

                    if (_size == 0)
                        _da.Error = DAE_CROSS;

                    if ((pd.Bits & WW) != 0 && (*_cmd & WW) == 0)
                        _dataSize = 1;
                    else if ((pd.Bits & W3) != 0 && (*_cmd & W3) == 0)
                        _dataSize = 1;
                    else if ((pd.Bits & FF) != 0)
                        _dataSize = 2;

                    if (_mode >= DISASM_FILE)
                    {
                        if (pd.Name[0] == '&')
                            mnemosize = _dataSize;
                        else if (pd.Name[0] == '$')
                            mnemosize = _addrSize;
                        else
                            mnemosize = 0;

                        if (mnemosize != 0)
                        {
                            name = "";

                            for (i = 0, j = 1; j < pd.Name.Length; j++)
                            {
                                if (pd.Name[j] == ':')
                                {
                                    if (mnemosize == 4)
                                        i = 0;
                                    else
                                        break;
                                }
                                else if (pd.Name[j] == '*')
                                {
                                    if (mnemosize == 4 && SizeSens != 2)
                                        name += 'D';
                                    else if (mnemosize != 4 && SizeSens != 0)
                                        name += 'W';
                                }
                                else
                                    name += pd.Name[j];
                            }
                        }
                        else
                        {
                            name = pd.Name.Split(',')[0];
                        }

                        if (repprefix != 0 && TabArguments)
                        {
                            for (i = 0; i < name.Length && name[i] != ' '; i++)
                                _da.Result.Append(name[i]);

                            if (name[i] == ' ')
                            {
                                _da.Result.Append(' ');
                                i++;
                            }

                            while (_da.Result.Length < 8)
                                _da.Result.Append(' ');

                            for (; i < name.Length; i++)
                                _da.Result.Append(name[i]);
                        }
                        else
                            _da.Result.Append(name);

                        if (Lowercase)
                            _da.Result = new StringBuilder(_da.Result.ToString().ToLower());
                    }

                    for (operand = 0; operand < 3; operand++)
                    {
                        if (_da.Error != 0)
                            break;

                        if (operand == 0 && pd.Arg2 != NNN && pd.Arg2 < PseudoOp)
                            _addComment = false;
                        else
                            _addComment = true;

                        if (operand == 0)
                            arg = pd.Arg1;
                        else if (operand == 1)
                            arg = pd.Arg2;
                        else
                            arg = pd.Arg3;

                        if (arg == NNN)
                            break;

                        if ((_mode >= DISASM_FILE) && arg < PseudoOp)
                        {
                            if (operand == 0)
                            {
                                _da.Result.Append(' ');

                                if (TabArguments)
                                {
                                    while (_da.Result.Length < 8)
                                        _da.Result.Append(' ');
                                }
                            }
                            else
                            {
                                _da.Result.Append(',');

                                if (ExtraSpace)
                                    _da.Result.Append(' ');
                            }
                        }

                        #region Switch

                        switch (arg)
                        {
                            case REG:
                                if (_size < 2)
                                    _da.Error = DAE_CROSS;
                                else
                                    DecodeRG(_cmd[1] >> 3, _dataSize, REG);

                                _hasRM = true;
                                break;

                            case RCM:
                                DecodeRG(_cmd[0], _dataSize, RCM);
                                break;

                            case RG4:
                                if (_size < 2)
                                    _da.Error = DAE_CROSS;
                                else
                                    DecodeRG(_cmd[1] >> 3, 4, RG4);

                                _hasRM = true;
                                break;

                            case RAC:
                                DecodeRG(REG_EAX, _dataSize, RAC);
                                break;

                            case RAX:
                                DecodeRG(REG_EAX, 2, RAX);
                                break;

                            case RDX:
                                DecodeRG(REG_EDX, 2, RDX);
                                break;

                            case RCL:
                                DecodeRG(REG_ECX, 1, RCL);
                                break;

                            case RS0:
                                DecodeST(0, 0);
                                break;

                            case RST:
                                DecodeST(_cmd[0], 0);
                                break;

                            case RMX:
                                if (_size < 2)
                                    _da.Error = DAE_CROSS;
                                else
                                    DecodeMX(_cmd[1] >> 3);

                                _hasRM = true;
                                break;

                            case R3D:
                                if (_size < 2)
                                    _da.Error = DAE_CROSS;
                                else
                                    DecodeNR(_cmd[1] >> 3);

                                _hasRM = true;
                                break;

                            case MRG:
                            case MRJ:
                            case MR1:
                            case MR2:
                            case MR4:
                            case MR8:
                            case MRD:
                            case MMA:
                            case MML:
                            case MM6:
                            case MMB:
                            case MD2:
                            case MB2:
                            case MD4:
                            case MD8:
                            case MDA:
                            case MF4:
                            case MF8:
                            case MFA:
                            case MFE:
                            case MFS:
                            case MFX:
                                DecodeMR(arg);
                                break;

                            case MMS:
                                DecodeMR(arg);
                                _da.Warnings |= DAW_FARADDR;
                                break;

                            case RR4:
                            case RR8:
                            case RRD:
                                if ((_cmd[1] & 0xc0) != 0xc0)
                                    _softError = DAE_REGISTER;

                                DecodeMR(arg);
                                break;

                            case MSO:
                                DecodeSO();
                                break;

                            case MDE:
                                DecodeDE();
                                break;

                            case MXL:
                                DecodeXL();
                                break;

                            case IMM:
                            case IMU:
                                if ((pd.Bits & SS) != 0 && (*_cmd & 0x02) != 0)
                                    DecodeIM(1, _dataSize, arg);
                                else
                                    DecodeIM(_dataSize, 0, arg);

                                break;

                            case VXD:
                                DecodeVX();
                                break;

                            case IMX:
                                DecodeIM(1, _dataSize, arg);
                                break;

                            case C01:
                                DecodeC1();
                                break;

                            case IMS:
                            case IM1:
                                DecodeIM(1, 0, arg);
                                break;

                            case IM2:
                                DecodeIM(2, 0, arg);

                                if ((_da.ImmConst & 0x03) != 0)
                                    _da.Warnings |= DAW_STACK;

                                break;

                            case IMA:
                                DecodeIA();
                                break;

                            case JOB:
                                DecodeRJ(1, srcIp + 2);
                                break;

                            case JOW:
                                DecodeRJ(_dataSize, srcIp + _dataSize + 1);
                                break;

                            case JMF:
                                DecodeJF();
                                _da.Warnings |= DAW_FARADDR;
                                break;

                            case SGM:
                                if (_size < 2)
                                    _da.Error = DAE_CROSS;
                                else
                                    DecodeSG(_cmd[1] >> 3);

                                _hasRM = true;
                                break;

                            case SCM:
                                DecodeSG(_cmd[0] >> 3);

                                if ((_da.CmdType & C_TYPEMASK) == C_POP)
                                    _da.Warnings |= DAW_SEGMENT;

                                break;

                            case CRX:
                                if ((_cmd[1] & 0xc0) != 0xc0)
                                    _da.Error |= DAE_REGISTER;

                                DecodeCR(_cmd[1]);
                                break;

                            case DRX:
                                if ((_cmd[1] & 0xc0) != 0xc0)
                                    _da.Error |= DAE_REGISTER;

                                DecodeDR(_cmd[1]);
                                break;

                            case PRN:
                                break;

                            case PRF:
                                _da.Warnings |= DAW_FARADDR;
                                break;

                            case PAC:
                                DecodeRG(REG_EAX, _dataSize, PAC);
                                break;

                            case PAH:
                            case PFL:
                                break;

                            case PS0:
                                DecodeST(0, 1);
                                break;

                            case PS1:
                                DecodeST(1, 1);
                                break;

                            case PCX:
                                DecodeRG(REG_ECX, cxsize, PCX);
                                break;

                            case PDI:
                                DecodeRG(REG_EDI, 4, PDI);
                                break;

                            default:
                                _da.Error = DAE_INTERN;
                                break;
                        }

                        #endregion
                    }

                    if (_pFixup != null && _da.FixupSize > 0)
                        _da.FixupOffset = (int)(_pFixup - src);

                    if (_da.MemType == DEC_UNKNOWN &&
                        (_segPrefix != SEG_UNDEF || (_addrSize != 4 && pd.Name[0] != '$')))
                    {
                        _da.Warnings |= DAW_PREFIX;
                        _da.CmdType |= C_RARE;
                    }

                    if (_addrSize != 4)
                        _da.CmdType |= C_RARE;
                }

                if (is3dnow)
                {
                    if (_immSize != 0)
                        _da.Error = DAE_BADCMD;
                    else
                        _immSize = 1;
                }

                if (_da.Error != 0)
                {
                    if (_mode >= DISASM_FILE)
                        _da.Result = new StringBuilder("???");

                    if (_da.Error == DAE_BADCMD &&
                        (*_cmd == 0x0f || *_cmd == 0xff)
                        && _size > 0)
                    {
                        if (_mode >= DISASM_FILE)
                            _da.Dump.Append(string.Format("{0:x2}", *_cmd));

                        _cmd++;
                        _size--;
                    }

                    if (_size > 0)
                    {
                        if (_mode >= DISASM_FILE)
                            _da.Dump.Append(string.Format("{0:x2}", *_cmd));

                        _cmd++;
                        _size--;
                    }
                }
                else
                {
                    if (_mode >= DISASM_FILE)
                    {
                        _da.Dump.Append(string.Format("{0:x2}", *_cmd++));

                        if (_hasRM)
                            _da.Dump.Append(string.Format("{0:x2}", *_cmd++));
                        if (_hasSIB)
                            _da.Dump.Append(string.Format("{0:x2}", *_cmd++));

                        if (_dispSize != 0)
                        {
                            _da.Dump.Append(' ');

                            for (i = 0; i < _dispSize; i++)
                            {
                                _da.Dump.Append(string.Format("{0:x2}", *_cmd++));
                            }
                        }

                        if (_immSize != 0)
                        {
                            _da.Dump.Append(' ');

                            for (i = 0; i < _immSize; i++)
                            {
                                _da.Dump.Append(string.Format("{0:x2}", *_cmd++));
                            }
                        }
                    }
                    else
                        _cmd += 1 + (_hasRM ? 1 : 0) + (_hasSIB ? 1 : 0) + _dispSize + _immSize;

                    _size -= 1 + (_hasRM ? 1 : 0) + (_hasSIB ? 1 : 0) + _dispSize + _immSize;
                }

                if (_da.Error == 0 && _softError != 0)
                    _da.Error = _softError;

                if (_mode >= DISASM_FILE)
                {
                    if (_da.Error != DAE_NOERR)
                    {
                        switch (_da.Error)
                        {
                            case DAE_CROSS:
                                _da.Comment = "Command crosses end of memory block";
                                break;

                            case DAE_BADCMD:
                                _da.Comment = "Unknown command";
                                break;

                            case DAE_BADSEG:
                                _da.Comment = "Undefined segment register";
                                break;

                            case DAE_MEMORY:
                                _da.Comment = "Illegal use of register";
                                break;

                            case DAE_REGISTER:
                                _da.Comment = "Memory address not allowed";
                                break;

                            case DAE_INTERN:
                                _da.Comment = "Internal disassembler error";
                                break;

                            default:
                                _da.Comment = "Unknown error";
                                break;
                        }
                    }
                    else if ((_da.Warnings & DAW_PRIV) != 0 && Privileged == 0)
                        _da.Comment = "Privileged command";
                    else if ((_da.Warnings & DAW_IO) != 0 && IOCommand == 0)
                        _da.Comment = "I/O command";
                    else if ((_da.Warnings & DAW_FARADDR) != 0 && Farcalls == 0)
                    {
                        if ((_da.CmdType & C_TYPEMASK) == C_JMP)
                            _da.Comment = "Far jump";
                        else if ((_da.CmdType & C_TYPEMASK) == C_CAL)
                            _da.Comment = "Far call";
                        else if ((_da.CmdType & C_TYPEMASK) == C_RET)
                            _da.Comment = "Far return";
                    }
                    else if ((_da.Warnings & DAW_SEGMENT) != 0 && Farcalls == 0)
                        _da.Comment = "Modification of segment register";
                    else if ((_da.Warnings & DAW_SHIFT) != 0 && BadShift == 0)
                        _da.Comment = "Shift constant out of range 1..31";
                    else if ((_da.Warnings & DAW_LOCK) != 0 && LockedBus == 0)
                        _da.Comment = "LOCK prefix";
                    else if ((_da.Warnings & DAW_STACK) != 0 && StackAlign == 0)
                        _da.Comment = "Unaligned stack operation";
                }

                return srcsize - _size;
            }

            public Disasm Result
            {
                get { return _da; }
            }
        }
    }
}
