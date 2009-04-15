/*
 * Sh Hooking Library - 
 *   common functions
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

#include "common.h"

ULONG CmGetTypeLength(CM_TYPE Type, PVOID Data, ULONG Length)
{
    ULONG length = 0;

    switch (Type & CmType)
    {
    case CmBool:
        length = sizeof(BOOLEAN);
        break;
    case CmByte:
        length = sizeof(BYTE);
        break;
    case CmInt16:
        length = sizeof(USHORT);
        break;
    case CmInt32:
        length = sizeof(ULONG32);
        break;
    case CmPVoid:
        length = sizeof(PVOID);
        break;
    case CmString:
        if (Data)
        {
            if (!Length)
                length = wcslen((PWSTR)Data) * sizeof(WCHAR);
            else
                length = Length;
        }
        else
        {
            length = 0;
        }

        break;
    case CmBytes:
        if (!Data)
            length = 0;
        else
            length = Length;
    case CmVoid:
    default:
        break;
    }

    return length;
}

PBYTE CmMakeDictionary(
    PULONG BufferLength,
    ULONG Length,
    ...
    )
{
    va_list ap;
    ULONG i;    
    ULONG j = 0;
    PBYTE buffer;
    ULONG bufferLength = 0;
    CM_TYPE type;
    ULONG length;
    PWSTR key;
    PVOID value;

    va_start(ap, Length);

    for (i = 0; i < Length; i++)
    {
        type = va_arg(ap, CM_TYPE);
        length = va_arg(ap, ULONG);
        key = va_arg(ap, PWSTR);
        value = va_arg(ap, PVOID);

        bufferLength += (wcslen(key) + 1) * sizeof(WCHAR);
        bufferLength += sizeof(CM_TYPE);
        bufferLength += sizeof(ULONG);
        bufferLength += CmGetTypeLength(type, value, length);
    }

    va_end(ap);

    buffer = (PBYTE)malloc(bufferLength);

    va_start(ap, Length);

    for (i = 0; i < Length; i++)
    {
        PVOID data;

        type = va_arg(ap, CM_TYPE);
        length = va_arg(ap, ULONG);
        key = va_arg(ap, PWSTR);
        value = va_arg(ap, PVOID);

        wcscpy((PWSTR)(buffer + j), key);
        j += (wcslen(key) + 1) * sizeof(WCHAR);
        *(PCM_TYPE)(buffer + j) = type;
        j += sizeof(CM_TYPE);
        *(PULONG)(buffer + j) = length = CmGetTypeLength(type, value, length);
        j += sizeof(ULONG);
        data = buffer + j;

        switch (type & CmType)
        {
        case CmBool:
            *(PBOOLEAN)data = (BOOLEAN)value;
            break;
        case CmByte:
            *(PBYTE)data = (BYTE)value;
            break;
        case CmInt16:
            *(PUSHORT)data = (USHORT)value;
            break;
        case CmInt32:
            *(PULONG32)data = (ULONG32)value;
            break;
        case CmPVoid:
            *(PVOID *)data = (PVOID)value;
            break;
        case CmBytes:
            if (value)
                memcpy(data, (PBYTE)value, length);
            break;
        case CmString:
            if (value)
                memcpy(data, (PWSTR)value, length);
            break;
        case CmVoid:
        default:
            break;
        }

        j += length;
    }

    va_end(ap);

    *BufferLength = bufferLength;

    return buffer;
}
