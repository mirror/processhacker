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
            private int _nDump;
            private int _nResult;
            private bool _addComment;

            private byte* _cmd;
            private byte* _pFixup;
            private uint _size;
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

                if (!Ideal)
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

                    if (!memonly)
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


        }
    }
}
