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

#include "verify.h"

_CryptCATAdminCalcHashFromFileHandle CryptCATAdminCalcHashFromFileHandle;
_CryptCATAdminAcquireContext CryptCATAdminAcquireContext;
_CryptCATAdminEnumCatalogFromHash CryptCATAdminEnumCatalogFromHash;
_CryptCATCatalogInfoFromContext CryptCATCatalogInfoFromContext;
_CryptCATAdminReleaseCatalogContext CryptCATAdminReleaseCatalogContext;
_CryptCATAdminReleaseContext CryptCATAdminReleaseContext;

NTSTATUS PhvInit()
{
    LoadLibrary(L"wintrust.dll");

    CryptCATAdminCalcHashFromFileHandle = 
        PhGetProcAddress(L"wintrust.dll", "CryptCATAdminCalcHashFromFileHandle");
    CryptCATAdminAcquireContext = 
        PhGetProcAddress(L"wintrust.dll", "CryptCATAdminAcquireContext");
    CryptCATAdminEnumCatalogFromHash =
        PhGetProcAddress(L"wintrust.dll", "CryptCATAdminEnumCatalogFromHash");
    CryptCATCatalogInfoFromContext =
        PhGetProcAddress(L"wintrust.dll", "CryptCATCatalogInfoFromContext");
    CryptCATAdminReleaseCatalogContext =
        PhGetProcAddress(L"wintrust.dll", "CryptCATAdminReleaseCatalogContext");
    CryptCATAdminReleaseContext =
        PhGetProcAddress(L"wintrust.dll", "CryptCATAdminReleaseContext");

    return STATUS_SUCCESS;
}

VERIFY_RESULT PhvStatusToVerifyResult(LONG Status)
{
    switch (Status)
    {
    case 0:
        return VrTrusted;
    case TRUST_E_NOSIGNATURE:
        return VrNoSignature;
    case CERT_E_EXPIRED:
        return VrExpired;
    case CERT_E_REVOKED:
        return VrRevoked;
    case TRUST_E_EXPLICIT_DISTRUST:
        return VrDistrust;
    case CRYPT_E_SECURITY_SETTINGS:
        return VrSecuritySettings;
    default:
        return VrSecuritySettings;
    }
}

VERIFY_RESULT PhvVerifyFileBasic(PWSTR FileName)
{
    WINTRUST_DATA trustData = { 0 };
    WINTRUST_FILE_INFO fileInfo = { 0 };
    GUID actionGenericVerifyV2 = WINTRUST_ACTION_GENERIC_VERIFY_V2;

    fileInfo.cbStruct = sizeof(fileInfo);
    fileInfo.pcwszFilePath = FileName;

    trustData.cbStruct = sizeof(trustData);
    trustData.dwUIChoice = WTD_UI_NONE;
    trustData.dwProvFlags = WTD_SAFER_FLAG;
    trustData.dwUnionChoice = WTD_CHOICE_FILE;
    trustData.pFile = &fileInfo;

    return PhvStatusToVerifyResult(WinVerifyTrust(NULL, &actionGenericVerifyV2, &trustData));
}

VERIFY_RESULT PhvVerifyFileCat(PWSTR FileName)
{
    LONG status = TRUST_E_NOSIGNATURE;
    WINTRUST_DATA trustData = { 0 };
    WINTRUST_CATALOG_INFO catalogInfo = { 0 };
    GUID driverActionVerify = DRIVER_ACTION_VERIFY;
    HANDLE fileHandle;
    PBYTE fileHash = NULL;
    ULONG fileHashLength;
    PWSTR fileHashTag = NULL;
    HANDLE catAdminHandle = NULL;
    HANDLE catInfoHandle = NULL;
    ULONG i;

    fileHandle = CreateFile(
        FileName,
        GENERIC_READ,
        FILE_SHARE_READ,
        NULL,
        OPEN_EXISTING,
        FILE_ATTRIBUTE_NORMAL,
        NULL
        );

    if (fileHandle == INVALID_HANDLE_VALUE)
        return VrNoSignature;

    fileHashLength = 256;
    fileHash = (PBYTE)PhAlloc(fileHashLength);

    if (!CryptCATAdminCalcHashFromFileHandle(fileHandle, &fileHashLength, fileHash, 0))
    {
        fileHash = (PBYTE)PhRealloc(fileHash, fileHashLength);

        if (!CryptCATAdminCalcHashFromFileHandle(fileHandle, &fileHashLength, fileHash, 0))
        {
            CloseHandle(fileHandle);
            PhFree(fileHash);
            return VrNoSignature;
        }
    }

    if (!CryptCATAdminAcquireContext(&catAdminHandle, &driverActionVerify, 0))
    {
        CloseHandle(fileHandle);
        PhFree(fileHash);
        return VrNoSignature;
    }

    fileHashTag = (PWSTR)PhAlloc((fileHashLength * 2 + 1) * sizeof(WCHAR));

    for (i = 0; i < fileHashLength; i++)
        wsprintfW(&fileHashTag[i * 2], L"%02X", fileHash[i]);

    catInfoHandle = CryptCATAdminEnumCatalogFromHash(
        catAdminHandle,
        fileHash,
        fileHashLength,
        0,
        NULL
        );

    PhFree(fileHash);

    if (catInfoHandle)
    {
        CATALOG_INFO ci = { 0 };

        if (CryptCATCatalogInfoFromContext(catInfoHandle, &ci, 0))
        {
            catalogInfo.cbStruct = sizeof(catalogInfo);
            catalogInfo.pcwszCatalogFilePath = ci.wszCatalogFile;
            catalogInfo.pcwszMemberFilePath = FileName;
            catalogInfo.pcwszMemberTag = fileHashTag;

            trustData.cbStruct = sizeof(trustData);
            trustData.dwUIChoice = WTD_UI_NONE;
            trustData.fdwRevocationChecks = WTD_STATEACTION_VERIFY;
            trustData.dwUnionChoice = WTD_CHOICE_CATALOG;
            trustData.pCatalog = &catalogInfo;

            status = WinVerifyTrust(NULL, &driverActionVerify, &trustData);
        }

        CryptCATAdminReleaseCatalogContext(catAdminHandle, catInfoHandle, 0);
    }

    PhFree(fileHashTag);
    CryptCATAdminReleaseContext(catAdminHandle, 0);
    CloseHandle(fileHandle);

    return PhvStatusToVerifyResult(status);
}

VERIFY_RESULT PhvVerifyFile(PWSTR FileName)
{
    VERIFY_RESULT result = VrNoSignature;
    
    result = PhvVerifyFileBasic(FileName);

    if (result == VrNoSignature)
    {
        result = PhvVerifyFileCat(FileName);
    }

    return result;
}
