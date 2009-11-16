Contents:

 1. Introduction
 2. Limitations
 3. Common issues

==== 1. Introduction ====

Process Hacker is a tool for viewing and manipulating processes. 
Its most basic functionality includes:

 * Viewing, terminating, suspending and resuming processes
 * Restarting processes, creating dump files, detaching from 
   any debuggers, viewing heaps, injecting DLLs, etc.
 * Viewing detailed process information, statistics, and 
   performance information
 * Viewing, terminating, suspending and resuming threads
 * Viewing detailed token information (including modifying 
   privileges)
 * Viewing and unloading modules
 * Viewing memory regions
 * Viewing environment variables
 * Viewing and closing handles
 * Viewing, controlling and editing services
 * Viewing and closing network connections

Process Hacker is brought to you by the Process Hacker team:
 * wj32 (Project Manager)
 * dmex (Developer)
 * XhmikosR (Installer Developer, Tester)
Inactive:
 * Dean (Developer)
 * Fliser (Developer)
 * Mikalai Chaly (Developer)
 * Uday Shanbhag (Developer)

See http://processhacker.sourceforge.net for more details.

==== 2. Limitations ====

Process Hacker runs on both 32-bit and 64-bit Windows, but 
certain functionality is only available on 32-bit systems, 
including:

 * Bypassing rootkits and security software when accessing 
   processes, threads, and other objects
 * Viewing kernel pool limits
 * Viewing hidden processes
 * Changing handle attributes
 * Viewing kernel-mode stack traces

==== 3. Common issues ====

P: When I try to run Process Hacker, I get the following message:

    Unable to find an entry point named 'GetPerformanceInfo' in DLL 'psapi.dll'.

S: The psapi.dll file is usually located in C:\Windows\system32 (or the appropriate 
   folder). You either do not have this file or your version is corrupt.
