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
    ULONG value;

    va_start(ap, Length);

    for (i = 0; i < Length; i++)
    {
        type = va_arg(ap, CM_TYPE);
        length = va_arg(ap, ULONG);
        key = va_arg(ap, PWSTR);
        value = va_arg(ap, ULONG);

        switch (type & CmType)
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
            if (value)
            {
                if (!length)
                    length = wcslen((PWSTR)value) * sizeof(WCHAR);
            }
            else
            {
                length = 0;
            }

            break;
        case CmBytes:
            if (!value)
                length = 0;
        case CmVoid:
        default:
            break;
        }

        bufferLength += (wcslen(key) + 1) * sizeof(WCHAR);
        bufferLength += sizeof(CM_TYPE);
        bufferLength += sizeof(ULONG);
        bufferLength += length;
    }

    va_end(ap);

    buffer = (PBYTE)malloc(bufferLength);

    va_start(ap, Length);

    for (i = 0; i < Length; i++)
    {
        PULONG lengthw;
        PVOID dataw;

        type = va_arg(ap, CM_TYPE);
        length = va_arg(ap, ULONG);
        key = va_arg(ap, PWSTR);
        value = va_arg(ap, ULONG);

        wcscpy((PWSTR)(buffer + j), key);
        j += (wcslen(key) + 1) * sizeof(WCHAR);
        *(PCM_TYPE)(buffer + j) = type;
        j += sizeof(CM_TYPE);
        lengthw = (PULONG)(buffer + j);
        j += sizeof(ULONG);
        dataw = buffer + j;

        switch (type & CmType)
        {
        case CmBool:
            *(PBOOLEAN)dataw = (BOOLEAN)value;
            length = sizeof(BOOLEAN);
            break;
        case CmByte:
            *(PBYTE)dataw = (BYTE)value;
            length = sizeof(BYTE);
            break;
        case CmInt16:
            *(PUSHORT)dataw = (USHORT)value;
            length = sizeof(USHORT);
            break;
        case CmInt32:
            *(PULONG32)dataw = (ULONG32)value;
            length = sizeof(ULONG32);
            break;
        case CmPVoid:
            *(PVOID *)dataw = (PVOID)value;
            length = sizeof(PVOID);
            break;
        case CmBytes:
            if (value)
                memcpy(dataw, (PBYTE)value, length);
            else
                length = 0;
            break;
        case CmString:
            if (value)
            {
                if (!length)
                    memcpy(dataw, (PWSTR)value, length = wcslen((PWSTR)value) * sizeof(WCHAR));
                else
                    memcpy(dataw, (PWSTR)value, length);
            }
            else
            {
                length = 0;
            }

            break;
        case CmVoid:
        default:
            break;
        }

        *lengthw = length;
        j += length;
    }

    va_end(ap);

    *BufferLength = bufferLength;

    return buffer;
}
