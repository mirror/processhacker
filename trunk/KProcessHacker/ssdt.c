/*
 * Process Hacker Driver - 
 *   SSDT code
 * 
 * 
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

#include "ssdt.h"

__declspec(dllimport) SSDT KeServiceDescriptorTable;
MDL *ssdtMdl;
PVOID *mappedSsdtCallTable;

NTSTATUS SsdtInit()
{
    ssdtMdl = MmCreateMdl(NULL, 
        KeServiceDescriptorTable.ServiceTableBase, KeServiceDescriptorTable.NumberOfServices * 4);
    
    if (ssdtMdl == NULL)
        return STATUS_UNSUCCESSFUL;
    
    MmBuildMdlForNonPagedPool(ssdtMdl);
    ssdtMdl->MdlFlags |= MDL_MAPPED_TO_SYSTEM_VA;
    mappedSsdtCallTable = MmMapLockedPages(ssdtMdl, KernelMode);
    
    return STATUS_SUCCESS;
}

void SsdtDeinit()
{
    if (ssdtMdl != NULL)
    {
        MmUnmapLockedPages(mappedSsdtCallTable, ssdtMdl);
        IoFreeMdl(ssdtMdl);
    }
}

ULONG SsdtGetCount()
{
    return KeServiceDescriptorTable.NumberOfServices;
}

PVOID SsdtGetEntryByCall(PVOID zwFunction)
{
    return SYSCALL_NT(zwFunction);
}

PVOID SsdtGetEntryByIndex(int index)
{
    return KeServiceDescriptorTable.ServiceTableBase[index];
}

PVOID *SsdtGetServiceTable()
{
    return KeServiceDescriptorTable.ServiceTableBase;
}

PVOID SsdtModifyEntryByCall(PVOID zwFunction, PVOID ntFunction)
{
    PVOID oldValue = SsdtGetEntryByCall(zwFunction);
    
    InterlockedExchange(&mappedSsdtCallTable[SYSCALL_INDEX(zwFunction)], ntFunction);
    
    return oldValue;
}

PVOID SsdtModifyEntryByIndex(int index, PVOID ntFunction)
{
    PVOID oldValue = SsdtGetEntryByIndex(index);
    
    InterlockedExchange(&mappedSsdtCallTable[index], ntFunction);
    
    return oldValue;
}

void SsdtRestoreEntryByCall(PVOID zwFunction, PVOID oldNtFunction, PVOID newNtFunction)
{
    PVOID oldValue = SsdtGetEntryByCall(zwFunction);
    
    InterlockedExchange(&mappedSsdtCallTable[SYSCALL_INDEX(zwFunction)], oldNtFunction);
    
    return oldValue;
}

void SsdtRestoreEntryByIndex(int index, PVOID oldNtFunction, PVOID newNtFunction)
{
    PVOID oldValue = SsdtGetEntryByIndex(index);
    
    InterlockedExchange(&mappedSsdtCallTable[index], oldNtFunction);
    
    return oldValue;
}
