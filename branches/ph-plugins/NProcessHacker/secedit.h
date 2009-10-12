/*
 * Process Hacker Library
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

#ifndef _SECEDIT_H
#define _SECEDIT_H

#include "nph.h"

typedef HRESULT (__stdcall *_QueryInterface)(
    PVOID This,
    REFIID Id,
    PVOID *Object
    );

typedef ULONG (__stdcall *_AddRef)(
    PVOID This
    );

typedef ULONG (__stdcall *_Release)(
    PVOID This
    );

typedef struct _ISECURITY_INFORMATION
{
    _QueryInterface QueryInterface;
    _AddRef AddRef;
    _Release Release;
} ISECURITY_INFORMATION, *PISECURITY_INFORMATION;

#endif
