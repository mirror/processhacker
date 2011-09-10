#ifndef _LC_LC_H
#define _LC_LC_H

#pragma once

#ifndef LC_NO_DEFAULT_LIB
#pragma comment(lib, "ntdll.lib")
#endif

// nonstandard extension used : nameless struct/union
#pragma warning(disable: 4201)
// nonstandard extension used : bit field types other than int
#pragma warning(disable: 4214)
// 'function': attributes not present on previous declaration
#pragma warning(disable: 4985)
// 'function': was declared deprecated
#pragma warning(disable: 4996)

#include <ntwin.h>
#include <lcnt.h>
#include <intrin.h>
#include <wchar.h>
#include <assert.h>
#include <stdio.h>

#ifdef __cplusplus
extern "C" {
#endif

// Memory

#define PTR_ADD(Pointer, Offset) ((PVOID)((ULONG_PTR)(Pointer) + (ULONG_PTR)(Offset)))
#define PTR_SUB(Pointer, Offset) ((PVOID)((ULONG_PTR)(Pointer) - (ULONG_PTR)(Offset)))
#define PTR_ALIGN(Pointer, Align) ((PVOID)(((ULONG_PTR)(Pointer) + (Align) - 1) & ~((Align) - 1)))
#define PTR_REBASE(Pointer, OldBase, NewBase) \
    ((PVOID)((ULONG_PTR)(Pointer) - (ULONG_PTR)(OldBase) + (ULONG_PTR)(NewBase)))

#define PAGE_SIZE 0x1000

#define LC_LARGE_BUFFER_SIZE (16 * 1024 * 1024)

// Exceptions

#define LcRaiseStatus(Status) RtlRaiseStatus(Status)

#define SIMPLE_EXCEPTION_FILTER(Condition) \
    ((Condition) ? EXCEPTION_EXECUTE_HANDLER : EXCEPTION_CONTINUE_SEARCH)

// Compiler

#ifdef DEBUG
#define ASSUME_ASSERT(Expression) assert(Expression)
#define ASSUME_NO_DEFAULT assert(FALSE)
#else
#define ASSUME_ASSERT(Expression) __assume(Expression)
#define ASSUME_NO_DEFAULT __assume(FALSE)
#endif

// Time

#define LC_TICKS_PER_NS ((LONG64)1 * 10)
#define LC_TICKS_PER_MS (LC_TICKS_PER_NS * 1000)
#define LC_TICKS_PER_SEC (LC_TICKS_PER_MS * 1000)
#define LC_TICKS_PER_MIN (LC_TICKS_PER_SEC * 60)
#define LC_TICKS_PER_HOUR (LC_TICKS_PER_MIN * 60)
#define LC_TICKS_PER_DAY (LC_TICKS_PER_HOUR * 24)

#define LC_TICKS_PARTIAL_MS(Ticks) (((ULONG64)(Ticks) / LC_TICKS_PER_MS) % 1000)
#define LC_TICKS_PARTIAL_SEC(Ticks) (((ULONG64)(Ticks) / LC_TICKS_PER_SEC) % 60)
#define LC_TICKS_PARTIAL_MIN(Ticks) (((ULONG64)(Ticks) / LC_TICKS_PER_MIN) % 60)
#define LC_TICKS_PARTIAL_HOURS(Ticks) (((ULONG64)(Ticks) / LC_TICKS_PER_HOUR) % 24)
#define LC_TICKS_PARTIAL_DAYS(Ticks) ((ULONG64)(Ticks) / LC_TICKS_PER_DAY)

#define LC_TIMEOUT_MS LC_TICKS_PER_MS
#define LC_TIMEOUT_SEC LC_TICKS_PER_SEC

// Annotations

/**
 * Indicates that a function assumes the relevant
 * locks have been acquired.
 */
#define __assumeLocked

/**
 * Indicates that a function assumes the specified
 * number of references are available for the object.
 *
 * \remarks Usually functions reference objects if they
 * store them for later usage; this annotation specifies
 * that the caller must supply these extra references
 * itself. In effect these references are "transferred"
 * to the function and must not be used. E.g. if you
 * create an object and immediately call a function
 * with __assumeRefs(1), you may no longer use the object
 * since that one reference you held is no longer yours.
 */
#define __assumeRefs(count)

/**
 * Indicates that a function may raise a software
 * exception.
 *
 * \remarks Do not use this annotation for
 * temporary usages of exceptions, e.g. unimplemented
 * functions.
 */
#define __mayRaise

/**
 * Indicates that a function requires the specified
 * value to be aligned at the specified number of bytes.
 */
#define __needsAlign(align)

// Casts

// Zero extension and sign extension macros
#define C_1uTo2(x) ((unsigned short)(unsigned char)(x))
#define C_1sTo2(x) ((unsigned short)(signed char)(x))
#define C_1uTo4(x) ((unsigned int)(unsigned char)(x))
#define C_1sTo4(x) ((unsigned int)(signed char)(x))
#define C_2uTo4(x) ((unsigned int)(unsigned short)(x))
#define C_2sTo4(x) ((unsigned int)(signed short)(x))
#define C_4uTo8(x) ((unsigned __int64)(unsigned int)(x))
#define C_4sTo8(x) ((unsigned __int64)(signed int)(x))

// Sorting

typedef enum _LC_SORT_ORDER
{
    NoSortOrder = 0,
    AscendingSortOrder,
    DescendingSortOrder
} LC_SORT_ORDER, *PLC_SORT_ORDER;

FORCEINLINE LONG LcModifySort(
    __in LONG Result,
    __in LC_SORT_ORDER Order
    )
{
    if (Order == AscendingSortOrder)
        return Result;
    else if (Order == DescendingSortOrder)
        return -Result;
    else
        return Result;
}

#define LC_BUILTIN_COMPARE(value1, value2) \
    if (value1 > value2) \
        return 1; \
    else if (value1 < value2) \
        return -1; \
    \
    return 0

FORCEINLINE int charcmp(
    __in signed char value1,
    __in signed char value2
    )
{
    return C_1sTo4(value1 - value2);
}

FORCEINLINE int ucharcmp(
    __in unsigned char value1,
    __in unsigned char value2
    )
{
    LC_BUILTIN_COMPARE(value1, value2);
}

FORCEINLINE int shortcmp(
    __in signed short value1,
    __in signed short value2
    )
{
    return C_2sTo4(value1 - value2);
}

FORCEINLINE int ushortcmp(
    __in unsigned short value1,
    __in unsigned short value2
    )
{
    LC_BUILTIN_COMPARE(value1, value2);
}

FORCEINLINE int intcmp(
    __in int value1,
    __in int value2
    )
{
    return value1 - value2;
}

FORCEINLINE int uintcmp(
    __in unsigned int value1,
    __in unsigned int value2
    )
{
    LC_BUILTIN_COMPARE(value1, value2);
}

FORCEINLINE int int64cmp(
    __in __int64 value1,
    __in __int64 value2
    )
{
    LC_BUILTIN_COMPARE(value1, value2);
}

FORCEINLINE int uint64cmp(
    __in unsigned __int64 value1,
    __in unsigned __int64 value2
    )
{
    LC_BUILTIN_COMPARE(value1, value2);
}

FORCEINLINE int intptrcmp(
    __in LONG_PTR value1,
    __in LONG_PTR value2
    )
{
    LC_BUILTIN_COMPARE(value1, value2);
}

FORCEINLINE int uintptrcmp(
    __in ULONG_PTR value1,
    __in ULONG_PTR value2
    )
{
    LC_BUILTIN_COMPARE(value1, value2);
}

FORCEINLINE int singlecmp(
    __in float value1,
    __in float value2
    )
{
    LC_BUILTIN_COMPARE(value1, value2);
}

FORCEINLINE int doublecmp(
    __in double value1,
    __in double value2
    )
{
    LC_BUILTIN_COMPARE(value1, value2);
}

FORCEINLINE int wcsicmp_withnull(
    __in_opt PWSTR Value1,
    __in_opt PWSTR Value2
    )
{
    if (Value1 && Value2)
        return wcsicmp(Value1, Value2);
    else if (!Value1)
        return !Value2 ? 0 : -1;
    else
        return 1;
}

// Synchronization

#ifdef _M_IX86

FORCEINLINE void *_InterlockedCompareExchangePointer(
    void *volatile *Destination,
    void *Exchange,
    void *Comparand
    )
{
    return (PVOID)_InterlockedCompareExchange(
        (PLONG_PTR)Destination,
        (LONG_PTR)Exchange,
        (LONG_PTR)Comparand
        );
}

FORCEINLINE void *_InterlockedExchangePointer(
    void *volatile *Destination,
    void *Exchange
    )
{
    return (PVOID)_InterlockedExchange(
        (PLONG_PTR)Destination,
        (LONG_PTR)Exchange
        );
}

#endif

FORCEINLINE LONG_PTR _InterlockedExchangeAddPointer(
    __inout LONG_PTR volatile *Addend,
    __in LONG_PTR Value
    )
{
#ifdef _M_IX86
    return (LONG_PTR)_InterlockedExchangeAdd((PLONG)Addend, (LONG)Value);
#else
    return (LONG_PTR)_InterlockedExchangeAdd64((PLONG64)Addend, (LONG64)Value);
#endif
}

FORCEINLINE LONG_PTR _InterlockedIncrementPointer(
    __inout LONG_PTR volatile *Addend
    )
{
#ifdef _M_IX86
    return (LONG_PTR)_InterlockedIncrement((PLONG)Addend);
#else
    return (LONG_PTR)_InterlockedIncrement64((PLONG64)Addend);
#endif
}

FORCEINLINE LONG_PTR _InterlockedDecrementPointer(
    __inout LONG_PTR volatile *Addend
    )
{
#ifdef _M_IX86
    return (LONG_PTR)_InterlockedDecrement((PLONG)Addend);
#else
    return (LONG_PTR)_InterlockedDecrement64((PLONG64)Addend);
#endif
}

FORCEINLINE BOOLEAN _InterlockedBitTestAndResetPointer(
    __inout LONG_PTR volatile *Base,
    __in LONG_PTR Bit
    )
{
#ifdef _M_IX86
    return _interlockedbittestandreset((PLONG)Base, (LONG)Bit);
#else
    return _interlockedbittestandreset64((PLONG64)Base, (LONG64)Bit);
#endif
}

FORCEINLINE BOOLEAN _InterlockedBitTestAndSetPointer(
    __inout LONG_PTR volatile *Base,
    __in LONG_PTR Bit
    )
{
#ifdef _M_IX86
    return _interlockedbittestandset((PLONG)Base, (LONG)Bit);
#else
    return _interlockedbittestandset64((PLONG64)Base, (LONG64)Bit);
#endif
}

FORCEINLINE BOOLEAN _InterlockedIncrementNoZero(
    __inout LONG volatile *Addend
    )
{
    LONG value;
    LONG newValue;

    value = *Addend;

    while (TRUE)
    {
        if (value == 0)
            return FALSE;

        if ((newValue = _InterlockedCompareExchange(
            Addend,
            value + 1,
            value
            )) == value)
        {
            return TRUE;
        }

        value = newValue;
    }
}

// Strings

#define LC_INT32_STR_LEN 12
#define LC_INT32_STR_LEN_1 (PH_INT32_STR_LEN + 1)

#define LC_INT64_STR_LEN 50
#define LC_INT64_STR_LEN_1 (PH_INT64_STR_LEN + 1)

#define LC_PTR_STR_LEN 24
#define LC_PTR_STR_LEN_1 (PH_PTR_STR_LEN + 1)

#define STR_EQUAL(Str1, Str2) (strcmp(Str1, Str2) == 0)
#define WSTR_EQUAL(Str1, Str2) (wcscmp(Str1, Str2) == 0)

#define STR_IEQUAL(Str1, Str2) (stricmp(Str1, Str2) == 0)
#define WSTR_IEQUAL(Str1, Str2) (wcsicmp(Str1, Str2) == 0)

FORCEINLINE VOID LcPrintInt32(
    __out_ecount(LC_INT32_STR_LEN_1) PWSTR Destination,
    __in LONG Int32
    )
{
    _ltow(Int32, Destination, 10);
}

FORCEINLINE VOID LcPrintUInt32(
    __out_ecount(LC_INT32_STR_LEN_1) PWSTR Destination,
    __in ULONG UInt32
    )
{
    _ultow(UInt32, Destination, 10);
}

FORCEINLINE VOID LcPrintInt64(
    __out_ecount(LC_INT64_STR_LEN_1) PWSTR Destination,
    __in LONG64 Int64
    )
{
    _i64tow(Int64, Destination, 10);
}

FORCEINLINE VOID LcPrintUInt64(
    __out_ecount(LC_INT64_STR_LEN_1) PWSTR Destination,
    __in ULONG64 UInt64
    )
{
    _ui64tow(UInt64, Destination, 10);
}

FORCEINLINE VOID LcPrintPointer(
    __out_ecount(LC_PTR_STR_LEN_1) PWSTR Destination,
    __in PVOID Pointer
    )
{
    Destination[0] = '0';
    Destination[1] = 'x';
#ifdef _M_IX86
    _ultow((ULONG)Pointer, &Destination[2], 16);
#else
    _ui64tow((ULONG64)Pointer, &Destination[2], 16);
#endif
}

// Misc.

FORCEINLINE NTSTATUS LcGetLastWin32ErrorAsNtStatus()
{
    ULONG win32Result;

    // This is needed because NTSTATUS_FROM_WIN32 uses the argument multiple times.
    win32Result = GetLastError();

    return NTSTATUS_FROM_WIN32(win32Result);
}

FORCEINLINE ULONG LcCountBits(
    __in ULONG Value
    )
{
    ULONG count = 0;

    while (Value)
    {
        count++;
        Value &= Value - 1;
    }

    return count;
}

FORCEINLINE ULONG LcRoundNumber(
    __in ULONG Value,
    __in ULONG Multiplier
    )
{
    ULONG newValue;

    newValue = Value - (Value % Multiplier);

    // This new value has the multiplier truncated.
    // E.g. 1099 / 100 * 100 = 1000.
    // If the difference is less than half the multiplier,
    // use the new value.
    // E.g.
    // 1099 -> 1000 (100). 1099 - 1000 >= 50, so use
    // the new value plus the multiplier.
    // 1010 -> 1000 (100). 1010 - 1000 < 50, so use
    // the new value.

    if (Value - newValue < Multiplier / 2)
        return newValue;
    else
        return newValue + Multiplier;
}

FORCEINLINE PVOID LcGetProcAddress(
    __in PWSTR LibraryName,
    __in PSTR ProcName
    )
{
    HMODULE module;

    module = GetModuleHandle(LibraryName);

    if (module)
        return GetProcAddress(module, ProcName);
    else
        return NULL;
}

FORCEINLINE VOID LcProbeAddress(
    __in PVOID UserAddress,
    __in SIZE_T UserLength,
    __in PVOID BufferAddress,
    __in SIZE_T BufferLength,
    __in ULONG Alignment
    )
{
    if (UserLength != 0)
    {
        if (((ULONG_PTR)UserAddress & (Alignment - 1)) != 0)
            LcRaiseStatus(STATUS_DATATYPE_MISALIGNMENT);

        if (
            ((ULONG_PTR)UserAddress + UserLength < (ULONG_PTR)UserAddress) ||
            ((ULONG_PTR)UserAddress < (ULONG_PTR)BufferAddress) ||
            ((ULONG_PTR)UserAddress + UserLength > (ULONG_PTR)BufferAddress + BufferLength)
            )
            LcRaiseStatus(STATUS_ACCESS_VIOLATION);
    }
}

FORCEINLINE PLARGE_INTEGER LcTimeoutFromMilliseconds(
    __out PLARGE_INTEGER Timeout,
    __in ULONG Milliseconds
    )
{
    if (Milliseconds == INFINITE)
        return NULL;

    Timeout->QuadPart = -(LONGLONG)UInt32x32To64(Milliseconds, LC_TIMEOUT_MS);

    return Timeout;
}

// queuedlock

#define LC_QUEUED_LOCK_OWNED ((ULONG_PTR)0x1)
#define LC_QUEUED_LOCK_OWNED_SHIFT 0
#define LC_QUEUED_LOCK_WAITERS ((ULONG_PTR)0x2)

// Valid only if Waiters = 0
#define LC_QUEUED_LOCK_SHARED_INC ((ULONG_PTR)0x4)
#define LC_QUEUED_LOCK_SHARED_SHIFT 2

// Valid only if Waiters = 1
#define LC_QUEUED_LOCK_TRAVERSING ((ULONG_PTR)0x4)
#define LC_QUEUED_LOCK_MULTIPLE_SHARED ((ULONG_PTR)0x8)

#define LC_QUEUED_LOCK_FLAGS ((ULONG_PTR)0xf)

#define LcGetQueuedLockSharedOwners(Value) \
    ((ULONG_PTR)(Value) >> LC_QUEUED_LOCK_SHARED_SHIFT)
#define LcGetQueuedLockWaitBlock(Value) \
    ((PLC_QUEUED_WAIT_BLOCK)((ULONG_PTR)(Value) & ~LC_QUEUED_LOCK_FLAGS))

typedef struct _LC_QUEUED_LOCK
{
    ULONG_PTR Value;
} LC_QUEUED_LOCK, *PLC_QUEUED_LOCK;

#define LC_QUEUED_LOCK_INIT { 0 }

#define LC_QUEUED_WAITER_EXCLUSIVE 0x1
#define LC_QUEUED_WAITER_SPINNING 0x2
#define LC_QUEUED_WAITER_SPINNING_SHIFT 1

typedef struct DECLSPEC_ALIGN(16) _LC_QUEUED_WAIT_BLOCK
{
    /** A pointer to the next wait block, i.e. the
     * wait block pushed onto the list before this
     * one.
     */
    struct _LC_QUEUED_WAIT_BLOCK *Next;
    /** A pointer to the previous wait block, i.e. the
     * wait block pushed onto the list after this
     * one.
     */
    struct _LC_QUEUED_WAIT_BLOCK *Previous;
    /** A pointer to the last wait block, i.e. the
     * first waiter pushed onto the list.
     */
    struct _LC_QUEUED_WAIT_BLOCK *Last;

    ULONG SharedOwners;
    ULONG Flags;
} LC_QUEUED_WAIT_BLOCK, *PLC_QUEUED_WAIT_BLOCK;

BOOLEAN
NTAPI
LcQueuedLockInitialization(
    VOID
    );

FORCEINLINE VOID LcInitializeQueuedLock(
    __out PLC_QUEUED_LOCK QueuedLock
    )
{
    QueuedLock->Value = 0;
}

VOID
FASTCALL
LcfAcquireQueuedLockExclusive(
    __inout PLC_QUEUED_LOCK QueuedLock
    );

VOID
FASTCALL
LcfAcquireQueuedLockShared(
    __inout PLC_QUEUED_LOCK QueuedLock
    );

VOID
FASTCALL
LcfReleaseQueuedLockExclusive(
    __inout PLC_QUEUED_LOCK QueuedLock
    );

VOID
FASTCALL
LcfReleaseQueuedLockShared(
    __inout PLC_QUEUED_LOCK QueuedLock
    );

VOID
FASTCALL
LcfTryWakeQueuedLock(
    __inout PLC_QUEUED_LOCK QueuedLock
    );

VOID
FASTCALL
LcfWakeForReleaseQueuedLock(
    __inout PLC_QUEUED_LOCK QueuedLock,
    __in ULONG_PTR Value
    );

#define PhPulseCondition PhfPulseCondition
VOID
FASTCALL
LcfPulseCondition(
    __inout PLC_QUEUED_LOCK Condition
    );

#define PhPulseAllCondition PhfPulseAllCondition
VOID
FASTCALL
LcfPulseAllCondition(
    __inout PLC_QUEUED_LOCK Condition
    );

#define PhWaitForCondition PhfWaitForCondition
VOID
FASTCALL
LcfWaitForCondition(
    __inout PLC_QUEUED_LOCK Condition,
    __inout PLC_QUEUED_LOCK Lock,
    __in_opt PLARGE_INTEGER Timeout
    );

#define LC_CONDITION_WAIT_QUEUED_LOCK 0x1
#define LC_CONDITION_WAIT_CRITICAL_SECTION 0x2
#define LC_CONDITION_WAIT_FAST_LOCK 0x4
#define LC_CONDITION_WAIT_LOCK_TYPE_MASK 0xfff

#define LC_CONDITION_WAIT_SHARED 0x1000
#define LC_CONDITION_WAIT_SPIN 0x2000

#define LcWaitForConditionEx LcfWaitForConditionEx
VOID
FASTCALL
LcfWaitForConditionEx(
    __inout PLC_QUEUED_LOCK Condition,
    __inout PVOID Lock,
    __in ULONG Flags,
    __in_opt PLARGE_INTEGER Timeout
    );

#define LcQueueWakeEvent LcfQueueWakeEvent
VOID
FASTCALL
LcfQueueWakeEvent(
    __inout PLC_QUEUED_LOCK WakeEvent,
    __out PLC_QUEUED_WAIT_BLOCK WaitBlock
    );

VOID
FASTCALL
LcfSetWakeEvent(
    __inout PLC_QUEUED_LOCK WakeEvent,
    __inout_opt PLC_QUEUED_WAIT_BLOCK WaitBlock
    );

#define LcWaitForWakeEvent LcfWaitForWakeEvent
NTSTATUS
FASTCALL
LcfWaitForWakeEvent(
    __inout PLC_QUEUED_LOCK WakeEvent,
    __inout PLC_QUEUED_WAIT_BLOCK WaitBlock,
    __in BOOLEAN Spin,
    __in_opt PLARGE_INTEGER Timeout
    );

// Inline functions

FORCEINLINE VOID LcAcquireQueuedLockExclusive(
    __inout PLC_QUEUED_LOCK QueuedLock
    )
{
    if (_InterlockedBitTestAndSetPointer((PLONG_PTR)&QueuedLock->Value, LC_QUEUED_LOCK_OWNED_SHIFT))
    {
        // Owned bit was already set. Slow path.
        LcfAcquireQueuedLockExclusive(QueuedLock);
    }
}

FORCEINLINE VOID LcAcquireQueuedLockShared(
    __inout PLC_QUEUED_LOCK QueuedLock
    )
{
    if ((ULONG_PTR)_InterlockedCompareExchangePointer(
        (PPVOID)&QueuedLock->Value,
        (PVOID)(LC_QUEUED_LOCK_OWNED | LC_QUEUED_LOCK_SHARED_INC),
        (PVOID)0
        ) != 0)
    {
        LcfAcquireQueuedLockShared(QueuedLock);
    }
}

FORCEINLINE BOOLEAN LcTryAcquireQueuedLockExclusive(
    __inout PLC_QUEUED_LOCK QueuedLock
    )
{
    if (!_InterlockedBitTestAndSetPointer((PLONG_PTR)&QueuedLock->Value, LC_QUEUED_LOCK_OWNED_SHIFT))
    {
        return TRUE;
    }
    else
    {
        return FALSE;
    }
}

FORCEINLINE VOID LcReleaseQueuedLockExclusive(
    __inout PLC_QUEUED_LOCK QueuedLock
    )
{
    ULONG_PTR value;

    value = (ULONG_PTR)_InterlockedExchangeAddPointer((PLONG_PTR)&QueuedLock->Value, -(LONG_PTR)LC_QUEUED_LOCK_OWNED);

    if ((value & (LC_QUEUED_LOCK_WAITERS | LC_QUEUED_LOCK_TRAVERSING)) == LC_QUEUED_LOCK_WAITERS)
    {
        LcfWakeForReleaseQueuedLock(QueuedLock, value - LC_QUEUED_LOCK_OWNED);
    }
}

FORCEINLINE VOID LcReleaseQueuedLockShared(
    __inout PLC_QUEUED_LOCK QueuedLock
    )
{
    ULONG_PTR value;

    value = LC_QUEUED_LOCK_OWNED | LC_QUEUED_LOCK_SHARED_INC;

    if ((ULONG_PTR)_InterlockedCompareExchangePointer(
        (PPVOID)&QueuedLock->Value,
        (PVOID)0,
        (PVOID)value
        ) != value)
    {
        LcfReleaseQueuedLockShared(QueuedLock);
    }
}

FORCEINLINE VOID LcAcquireReleaseQueuedLockExclusive(
    __inout PLC_QUEUED_LOCK QueuedLock
    )
{
    BOOLEAN owned;

    MemoryBarrier();
    owned = !!(QueuedLock->Value & LC_QUEUED_LOCK_OWNED);
    MemoryBarrier();

    if (owned)
    {
        LcAcquireQueuedLockExclusive(QueuedLock);
        LcReleaseQueuedLockExclusive(QueuedLock);
    }
}

FORCEINLINE BOOLEAN LcTryAcquireReleaseQueuedLockExclusive(
    __inout PLC_QUEUED_LOCK QueuedLock
    )
{
    BOOLEAN owned;

    // Need two memory barriers because we don't want the
    // compiler re-ordering the following check in either
    // direction.
    MemoryBarrier();
    owned = !(QueuedLock->Value & LC_QUEUED_LOCK_OWNED);
    MemoryBarrier();

    return owned;
}

FORCEINLINE VOID LcSetWakeEvent(
    __inout PLC_QUEUED_LOCK WakeEvent,
    __inout_opt PLC_QUEUED_WAIT_BLOCK WaitBlock
    )
{
    // The wake event is similar to a synchronization event
    // in that it does not have thread-safe pulsing; we can
    // simply skip the function call if there's nothing to
    // wake. However, if we're cancelling a wait
    // (WaitBlock != NULL) we need to make the call.

    if (WakeEvent->Value || WaitBlock)
        LcfSetWakeEvent(WakeEvent, WaitBlock);
}

// ref

// Configuration

#define LC_REF_SMALL_OBJECT_SIZE 48
#define LC_REF_SMALL_OBJECT_COUNT 512

//#define LC_REF_STRICT_CHECKS
#define LC_REF_ALLOCATE_NEVER_NULL

// Object flags
#define LC_OBJECT_RAISE_ON_FAIL 0x00000001
#define LC_OBJECT_VALID_FLAGS 0x00000001

// Object type flags
#define LC_OBJECT_TYPE_USE_FREE_LIST 0x00000001
#define LC_OBJECT_TYPE_VALID_FLAGS 0x00000001

typedef struct _LC_OBJECT_TYPE *PLC_OBJECT_TYPE;

/**
 * The delete procedure for an object type, called when
 * an object of the type is being freed.
 *
 * \param Object A pointer to the object being freed.
 * \param Flags Reserved.
 */
typedef VOID (NTAPI *PLC_TYPE_DELETE_PROCEDURE)(
    __in PVOID Object,
    __in ULONG Flags
    );

#ifdef DEBUG
typedef VOID (NTAPI *PLC_CREATE_OBJECT_HOOK)(
    __in PVOID Object,
    __in SIZE_T Size,
    __in ULONG Flags,
    __in PLC_OBJECT_TYPE ObjectType
    );
#endif

extern PLC_OBJECT_TYPE LcObjectTypeObject;
extern PLC_OBJECT_TYPE LcAllocType;

#ifdef DEBUG
extern LIST_ENTRY LcDbgObjectListHead;
extern LC_QUEUED_LOCK LcDbgObjectListLock;
extern PLC_CREATE_OBJECT_HOOK LcDbgCreateObjectHook;
#endif

typedef struct _LC_OBJECT_TYPE_PARAMETERS
{
    SIZE_T FreeListSize;
    ULONG FreeListCount;

    UCHAR Reserved1;
    UCHAR Reserved2;
    UCHAR Reserved3;
    UCHAR Reserved4;
    ULONG Reserved5[4];
} LC_OBJECT_TYPE_PARAMETERS, *PLC_OBJECT_TYPE_PARAMETERS;

typedef struct _LC_OBJECT_TYPE_INFORMATION
{
    PWSTR Name;
    ULONG NumberOfObjects;
} LC_OBJECT_TYPE_INFORMATION, *PLC_OBJECT_TYPE_INFORMATION;

NTSTATUS
NTAPI
LcRefInitialization(
    VOID
    );

__mayRaise
NTSTATUS
NTAPI
LcCreateObject(
    __out PVOID *Object,
    __in SIZE_T ObjectSize,
    __in ULONG Flags,
    __in PLC_OBJECT_TYPE ObjectType
    );

VOID
NTAPI
LcReferenceObject(
    __in PVOID Object
    );

__mayRaise
LONG
NTAPI
LcReferenceObjectEx(
    __in PVOID Object,
    __in LONG RefCount
    );

BOOLEAN
NTAPI
LcReferenceObjectSafe(
    __in PVOID Object
    );

VOID
NTAPI
LcDereferenceObject(
    __in PVOID Object
    );

BOOLEAN
NTAPI
LcDereferenceObjectDeferDelete(
    __in PVOID Object
    );

__mayRaise
LONG
NTAPI
LcDereferenceObjectEx(
    __in PVOID Object,
    __in LONG RefCount,
    __in BOOLEAN DeferDelete
    );

PPH_OBJECT_TYPE
NTAPI
LcGetObjectType(
    __in PVOID Object
    );

NTSTATUS
NTAPI
LcCreateObjectType(
    __out PLC_OBJECT_TYPE *ObjectType,
    __in PWSTR Name,
    __in ULONG Flags,
    __in_opt PLC_TYPE_DELETE_PROCEDURE DeleteProcedure
    );

NTSTATUS
NTAPI
LcCreateObjectTypeEx(
    __out PLC_OBJECT_TYPE *ObjectType,
    __in PWSTR Name,
    __in ULONG Flags,
    __in_opt PLC_TYPE_DELETE_PROCEDURE DeleteProcedure,
    __in_opt PLC_OBJECT_TYPE_PARAMETERS Parameters
    );

VOID
NTAPI
LcGetObjectTypeInformation(
    __in PLC_OBJECT_TYPE ObjectType,
    __out PLC_OBJECT_TYPE_INFORMATION Information
    );

FORCEINLINE VOID LcSwapReference(
    __inout PVOID *ObjectReference,
    __in_opt PVOID NewObject
    )
{
    PVOID oldObject;

    oldObject = *ObjectReference;
    *ObjectReference = NewObject;

    if (NewObject) LcReferenceObject(NewObject);
    if (oldObject) LcDereferenceObject(oldObject);
}

FORCEINLINE VOID LcSwapReference2(
    __inout PVOID *ObjectReference,
    __in_opt __assumeRefs(1) PVOID NewObject
    )
{
    PVOID oldObject;

    oldObject = *ObjectReference;
    *ObjectReference = NewObject;

    if (oldObject) LcDereferenceObject(oldObject);
}

NTSTATUS
NTAPI
LcCreateAlloc(
    __out PVOID *Alloc,
    __in SIZE_T Size
    );

/** The size of the static array in an auto-release pool. */
#define PH_AUTO_POOL_STATIC_SIZE 64
/** The maximum size of the dynamic array for it to be
 * kept after the auto-release pool is drained. */
#define PH_AUTO_POOL_DYNAMIC_BIG_SIZE 256

/**
 * An auto-dereference pool can be used for
 * semi-automatic reference counting. Batches of
 * objects are dereferenced at a certain time.
 *
 * This object is not thread-safe and cannot
 * be used across thread boundaries. Always
 * store them as local variables.
 */
typedef struct _PH_AUTO_POOL
{
    ULONG StaticCount;
    PVOID StaticObjects[PH_AUTO_POOL_STATIC_SIZE];

    ULONG DynamicCount;
    ULONG DynamicAllocated;
    PVOID *DynamicObjects;

    struct _PH_AUTO_POOL *NextPool;
} PH_AUTO_POOL, *PPH_AUTO_POOL;

PHLIBAPI
VOID
NTAPI
PhInitializeAutoPool(
    __out PPH_AUTO_POOL AutoPool
    );

__mayRaise
PHLIBAPI
VOID
NTAPI
PhDeleteAutoPool(
    __inout PPH_AUTO_POOL AutoPool
    );

__mayRaise
PHLIBAPI
VOID
NTAPI
PhaDereferenceObject(
    __in PVOID Object
    );

PHLIBAPI
VOID
NTAPI
PhDrainAutoPool(
    __in PPH_AUTO_POOL AutoPool
    );

/**
 * Calls PhaDereferenceObject() and returns the given object.
 *
 * \param Object A pointer to an object. The value can be
 * null; in that case no action is performed.
 *
 * \return The value of \a Object.
 */
FORCEINLINE PVOID PHA_DEREFERENCE(
    __in PVOID Object
    )
{
    if (Object)
        PhaDereferenceObject(Object);

    return Object;
}

// string

typedef struct _LC_STRING
{
    SIZE_T Length;
    PWCHAR Buffer;
    ULONG Flags;
    union
    {
        PVOID Pointer;
        WCHAR Data[1];
    };
} LC_STRING, *PLC_STRING;

#define LC_STRING_INIT(Buffer) { wcslen(Buffer) * sizeof(WCHAR), (Buffer), 0, NULL }

VOID LcInitializeString(
    __out PLC_STRING String,
    __in_opt PWSTR Buffer
    );

VOID LcInitializeStringEx(
    __out PLC_STRING String,
    __in_opt PWSTR Buffer,
    __in SIZE_T Length
    );

#ifdef __cplusplus
}
#endif

#endif
