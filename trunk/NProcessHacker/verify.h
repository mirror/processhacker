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

#ifndef _VERIFY_H
#define _VERIFY_H

#include "nph.h"
#include <wintrust.h>
#include <softpub.h>

typedef enum _VERIFY_RESULT
{
    VrUnknown = 0,
    VrNoSignature,
    VrTrusted,
    VrTrustedInstaller,
    VrExpired,
    VrRevoked,
    VrDistrust,
    VrSecuritySettings
} VERIFY_RESULT, *PVERIFY_RESULT;

typedef struct _CATALOG_INFO
{
    DWORD cbStruct;
    WCHAR wszCatalogFile[MAX_PATH];
} CATALOG_INFO, *PCATALOG_INFO;

typedef BOOL (WINAPI *_CryptCATAdminCalcHashFromFileHandle)(
    HANDLE hFile,
    DWORD *pcbHash,
    BYTE *pbHash,
    DWORD dwFlags
    );

typedef BOOL (WINAPI *_CryptCATAdminAcquireContext)(
    HANDLE *phCatAdmin,
    GUID *pgSubsystem,
    DWORD dwFlags
    );

typedef HANDLE (WINAPI *_CryptCATAdminEnumCatalogFromHash)(
    HANDLE hCatAdmin,
    BYTE *pbHash,
    DWORD cbHash,
    DWORD dwFlags,
    HANDLE *phPrevCatInfo
    );

typedef BOOL (WINAPI *_CryptCATCatalogInfoFromContext)(
    HANDLE hCatInfo,
    CATALOG_INFO *psCatInfo,
    DWORD dwFlags
    );

typedef BOOL (WINAPI *_CryptCATAdminReleaseCatalogContext)(
    HANDLE hCatAdmin,
    HANDLE hCatInfo,
    DWORD dwFlags
    );

typedef BOOL (WINAPI *_CryptCATAdminReleaseContext)(
    HANDLE hCatAdmin,
    DWORD dwFlags
    );

NTSTATUS PhvInit();
VERIFY_RESULT PhvStatusToVerifyResult(LONG Status);
VERIFY_RESULT PhvVerifyFileBasic(PWSTR FileName);
VERIFY_RESULT PhvVerifyFileCat(PWSTR FileName);
NPHAPI VERIFY_RESULT PhvVerifyFile(PWSTR FileName);

#endif