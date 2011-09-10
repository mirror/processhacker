/*
 * LC -
 *   string functions
 *
 * Copyright (C) 2011 wj32
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

#include <lc.h>

PLC_OBJECT_TYPE LcStringType;

BOOLEAN PhStringInitialization(
    VOID
    )
{
    if (!NT_SUCCESS(LcCreateObjectType(
        &LcStringType,
        L"String",
        0,
        NULL
        )))
        return FALSE;
}

PLC_STRING LcCreateString(
    __in PWSTR Buffer
    )
{
    return LcCreateStringEx(String, Buffer, wcslen(Buffer) * sizeof(WCHAR));
}

PLC_STRING LcCreateStringEx(
    __in_opt PWSTR Buffer,
    __in SIZE_T Length
    )
{
    String->Length = Length;
    String->Buffer = Buffer;
    String->Flags = 0;
    String->Data = NULL;
}
