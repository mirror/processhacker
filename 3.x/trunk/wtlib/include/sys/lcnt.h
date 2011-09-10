#ifndef _LCNT_H
#define _LCNT_H

// This header file provides access to NT APIs.

// Definitions are annotated to indicate their source.
// If a definition is not annotated, it has been retrieved
// from an official Microsoft source (NT headers, DDK headers, winnt.h).

// "winbase" indicates that a definition has been reconstructed from
// a Win32-ized NT definition in winbase.h.
// "rev" indicates that a definition has been reverse-engineered.
// "dbg" indicates that a definition has been obtained from a debug
// message or assertion in a checked build of the kernel or file.

// Reliability:
// 1. No annotation.
// 2. dbg.
// 3. symbols, private. Types may be incorrect.
// 4. winbase. Names and types may be incorrect.
// 5. rev.

// Mode
#define LCNT_MODE_KERNEL 0
#define LCNT_MODE_USER 1

// Version
#define LCNT_WIN2K 50
#define LCNT_WINXP 51
#define LCNT_WS03 52
#define LCNT_VISTA 60
#define LCNT_WIN7 61

#ifndef LCNT_MODE
#define LCNT_MODE LCNT_MODE_USER
#endif

#ifndef LCNT_VERSION
#define LCNT_VERSION LCNT_WINXP
#endif

// Options

//#define LCNT_NO_INLINE_INIT_STRING

#ifdef __cplusplus
extern "C" {
#endif

#if (LCNT_MODE != LCNT_MODE_KERNEL)
#include <ntbasic.h>
#include <ntstatus.h>
#include <ntnls.h>
#include <ntkeapi.h>
#endif

#include <ntexapi.h>

#if (LCNT_MODE != LCNT_MODE_KERNEL)
#include <ntmmapi.h>
#endif

#include <ntobapi.h>
#include <ntpsapi.h>

#if (LCNT_MODE != LCNT_MODE_KERNEL)
#include <ntcm.h>
#include <ntdbg.h>
#include <ntioapi.h>
#include <ntlpcapi.h>
#include <ntpfapi.h>
#include <ntpnpapi.h>
#include <ntpoapi.h>
#include <ntregapi.h>
#endif

#include <ntrtl.h>

#if (LCNT_MODE != LCNT_MODE_KERNEL)

#include <ntseapi.h>
#include <nttmapi.h>
#include <nttp.h>
#include <ntxcapi.h>

#include <ntwow64.h>

#include <ntlsa.h>
#include <ntsam.h>

#include <ntmisc.h>

#include <ntzwapi.h>

#endif

#ifdef __cplusplus
}
#endif

#endif
