/*
 * Process Hacker - 
 *   PE imports reader
 * 
 * Copyright (C) 2008 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ProcessHacker.PE
{
    public class ImportLookupEntry
    {
        public bool UseOrdinal;
        public ushort Ordinal;
        public uint NameTableRVA;
        public ImportNameEntry NameEntry;
    }

    public class ImportNameEntry
    {
        public uint Hint;
        public string Name;
    }

    public class ImportDirectoryEntry
    {
        public ImportDirectoryEntry(BinaryReader br)
        {
            this.ImportLookupTableRVA = br.ReadUInt32();
            this.TimeDateStamp = br.ReadUInt32();
            this.ForwarderChain = br.ReadUInt32();
            this.NameRVA = br.ReadUInt32();
            this.ImportAddressTableRVA = br.ReadUInt32();  
        }

        public uint ImportLookupTableRVA;
        public uint TimeDateStamp;
        public uint ForwarderChain;
        public uint NameRVA;
        public uint ImportAddressTableRVA;
        public string Name;
    }

    public class ImportData
    {
        public ImportData(BinaryReader br, PEFile peFile)
        {
            while (true)
            {
                byte[] data = br.ReadBytes(20);

                if (Misc.IsEmpty(data))
                    break;

                br.BaseStream.Seek(-20, SeekOrigin.Current);

                this.ImportDirectoryTable.Add(new ImportDirectoryEntry(br));
            }

            int dllNumber = 0;

            while (dllNumber < this.ImportDirectoryTable.Count)
            {
                this.ImportLookupTable.Add(new List<ImportLookupEntry>());

                while (true)
                {
                    ImportLookupEntry entry = new ImportLookupEntry();
                    uint number = br.ReadUInt32();

                    if (number == 0)
                        break;

                    if ((number & 0x80000000) == 1)
                    {
                        entry.UseOrdinal = true;
                        entry.Ordinal = (ushort)(number & 0xffff);
                    }
                    else
                    {
                        entry.UseOrdinal = false;
                        entry.NameTableRVA = (uint)(number & 0x7fffffff);

                        try
                        {
                            peFile.RvaToVa(entry.NameTableRVA);
                        }
                        catch
                        {
                            entry.UseOrdinal = true;
                            entry.Ordinal = (ushort)(number & 0xffff);
                            continue;
                        }
                    }

                    this.ImportLookupTable[dllNumber].Add(entry);
                }

                dllNumber++;
            }

            if (peFile.GetNames)
            {
                foreach (ImportDirectoryEntry entry in this.ImportDirectoryTable)
                {
                    br.BaseStream.Seek(peFile.RvaToVa(entry.NameRVA), SeekOrigin.Begin);

                    entry.Name = Misc.ReadString(br.BaseStream);
                }

                for (int i = 0; i < this.ImportLookupTable.Count; i++)
                {
                    for (int j = 0; j < this.ImportLookupTable[i].Count; j++)
                    {
                        ImportLookupEntry entry = this.ImportLookupTable[i][j];

                        if (!entry.UseOrdinal)
                        {
                            br.BaseStream.Seek(peFile.RvaToVa(entry.NameTableRVA), SeekOrigin.Begin);

                            entry.NameEntry = new ImportNameEntry();
                            entry.NameEntry.Hint = br.ReadUInt16();
                            entry.NameEntry.Name = Misc.ReadString(br.BaseStream);
                        }
                    }
                }
            }
        }

        public List<ImportDirectoryEntry> ImportDirectoryTable = new List<ImportDirectoryEntry>();
        public List<List<ImportLookupEntry>> ImportLookupTable = new List<List<ImportLookupEntry>>();
    }
}
