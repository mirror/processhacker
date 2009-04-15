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

#ifndef _COMMON_H
#define _COMMON_H

#include <windows.h>
#include <string.h>
#include <stdlib.h>

typedef enum _CM_TYPE
{
    CmVoid = 0,
    CmBool,
    CmByte,
    CmInt16,
    CmInt32,
    CmPVoid,
    CmBytes,
    CmString,
    CmType = 0x00ffffff,

    CmHex = 0x01000000,
    CmDisplayHint = 0xff000000
} CM_TYPE, *PCM_TYPE;

PBYTE CmMakeDictionary(
    PULONG BufferLength,
    ULONG Length,
    ...
    );

#endif