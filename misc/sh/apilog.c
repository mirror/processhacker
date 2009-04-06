#include "apilog.h"

NT_HOOK AlNtOpenProcessHook;

NTSTATUS NTAPI AlNtOpenProcess(
    PHANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId
    )
{
    return ShNtCall(&AlNtOpenProcessHook, &ProcessHandle);
}

NTSTATUS NTAPI AlNewNtOpenProcess(
    PHANDLE ProcessHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    PCLIENT_ID ClientId
    )
{
    PBYTE dictionary;
    ULONG dictionaryLength;

    dictionary = CmMakeDictStringULong(&dictionaryLength, 4,
        L"ProcessHandle", ProcessHandle,
        L"DesiredAccess", DesiredAccess,
        L"ObjectAttributes", ObjectAttributes,
        L"ClientId", ClientId,
        L"UniqueProcess", ClientId->UniqueProcess
        );
    AlLogCall(L"NtOpenProcess", &AlNtOpenProcessHook, dictionary, dictionaryLength);
    free(dictionary);

    return AlNtOpenProcess(ProcessHandle, DesiredAccess, ObjectAttributes, ClientId);
}

VOID AlLogCall(
    PWSTR Name,
    PNT_HOOK NtHook,
    PBYTE Dictionary,
    ULONG DictionaryLength
    )
{
    ULONG nameLength = wcslen(Name);
    ULONG bufferLength = sizeof(ULONG) + (nameLength + 1) * sizeof(WCHAR) + sizeof(NT_HOOK) + DictionaryLength;
    PBYTE buffer = (PBYTE)malloc(bufferLength);

    *(PULONG)buffer = GetCurrentProcessId();
    wcscpy((PWSTR)(buffer + sizeof(ULONG)), Name);
    memcpy(buffer + sizeof(ULONG) + (nameLength + 1) * sizeof(WCHAR), NtHook, sizeof(NT_HOOK));
    memcpy(buffer + sizeof(ULONG) + (nameLength + 1) * sizeof(WCHAR) + sizeof(NT_HOOK), Dictionary, DictionaryLength);

    CallNamedPipe(AL_PIPE_NAME, buffer, bufferLength, NULL, 0, NULL, INFINITE);

    free(buffer);
}

VOID AlPatch()
{
    ShNtPatchCall("NtOpenProcess", AlNewNtOpenProcess, &AlNtOpenProcessHook);
}

VOID AlUnpatch()
{
    ShNtUnpatchCall(&AlNtOpenProcessHook);
}

VOID AlInit()
{
    AlPatch();
}

VOID AlDeinit()
{
    AlUnpatch();
}
