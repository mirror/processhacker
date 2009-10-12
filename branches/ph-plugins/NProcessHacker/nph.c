/*
 * Process Hacker Library - 
 *   common code
 * 
 * Copyright (C) 2009 wj32
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

#include "nph.h"
#include "kph.h"
#include "obj.h"
#include "verify.h"

PVOID PHAPI PhAlloc(SIZE_T Size)
{
    PVOID memory;
    
    if (!(memory = malloc(Size)))
        RaiseException(EXCEPTION_NO_MEMORY, 0, 0, NULL);

    return memory;
}

PVOID PHAPI PhRealloc(PVOID Memory, SIZE_T Size)
{
    PVOID memory;

    if (!(memory = realloc(Memory, Size)))
        RaiseException(EXCEPTION_NO_MEMORY, 0, 0, NULL);

    return memory;
}

VOID PHAPI PhFree(PVOID Memory)
{
    free(Memory);
}

PVOID PHAPI PhGetProcAddress(PWSTR LibraryName, PSTR ProcName)
{
    return GetProcAddress(GetModuleHandle(LibraryName), ProcName);
}

VOID PHAPI PhVoid()
{
    return;
}

BOOL WINAPI DllMain(
    HINSTANCE hinstDLL,
    DWORD fdwReason,
    LPVOID lpvReserved
    )
{
    switch (fdwReason)
    {
    case DLL_PROCESS_ATTACH:
        if (!NT_SUCCESS(PhVerifyInit()))
            return FALSE;
        if (!NT_SUCCESS(PhObjInit()))
            return FALSE;
        if (!NT_SUCCESS(KphInit()))
            return FALSE;

        break;
    default:
        break;
    }

    return TRUE;
}
