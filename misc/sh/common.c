#include "common.h"

PBYTE CmMakeDictStringULong(
    PULONG BufferLength,
    ULONG Length,
    ...
    )
{
    va_list ap;
    ULONG i;    
    ULONG j = 0;
    PWSTR key;
    ULONG value;
    PBYTE buffer;
    ULONG bufferLength = 0;

    va_start(ap, Length);

    for (i = 0; i < Length; i++)
    {
        key = va_arg(ap, PWSTR);
        value = va_arg(ap, ULONG);
        bufferLength += (wcslen(key) + 1) * sizeof(WCHAR);
        bufferLength += sizeof(ULONG);
    }

    va_end(ap);

    buffer = (PBYTE)malloc(bufferLength);

    va_start(ap, Length);

    for (i = 0; i < Length; i++)
    {
        key = va_arg(ap, PWSTR);
        value = va_arg(ap, ULONG);

        wcscpy((PWSTR)(buffer + j), key);
        j += (wcslen(key) + 1) * sizeof(WCHAR);
        memcpy(buffer + j, &value, sizeof(ULONG));
        j += sizeof(ULONG);
    }

    va_end(ap);

    *BufferLength = bufferLength;

    return buffer;
}
