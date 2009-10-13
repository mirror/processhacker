/*
 * Process Hacker - 
 *   type factory
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

using System;
using System.Text;
using ProcessHacker.Common; 
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.AccessControl;

namespace ProcessHacker.Native
{
    public static class NativeTypeFactory
    {
        private class FlagName
        {
            public string Name { get; set; }
            public long Value { get; set; }
            public bool Enabled { get; set; }
        }

        public enum ObjectType
        {
            Adapter,
            AlpcPort,
            Callback,
            DebugObject,
            Desktop,
            Device,
            Directory,
            Driver,
            EtwRegistration,
            Event,
            EventPair,
            File,
            FilterCommunicationPort,
            FilterConnectionPort,
            IoCompletion,
            Job,
            Key,
            KeyedEvent,
            Mutant,
            Process,
            Profile,
            Section,
            Semaphore,
            Service,
            SymbolicLink,
            Thread,
            Timer,
            TmEn,
            TmRm,
            TmTm,
            TmTx,
            Token,
            TpWorkerFactory,
            Type,
            WindowStation,
            WmiGuid
        }

        #region Base Functions

        public static AccessEntry[] GetAccessEntries(ObjectType type)
        {
            AccessEntry[] entries;

            switch (type)
            {
                case ObjectType.AlpcPort:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", PortAccess.All, true, true),
                        new AccessEntry("Connect", PortAccess.Connect, true, true)
                    };
                    break;
                case ObjectType.DebugObject:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", DebugObjectAccess.All, true, true),
                        new AccessEntry("Read events", DebugObjectAccess.ReadEvent, true, true),
                        new AccessEntry("Assign processes", DebugObjectAccess.ProcessAssign, true, true),
                        new AccessEntry("Query information", DebugObjectAccess.QueryInformation, true, true),
                        new AccessEntry("Set information", DebugObjectAccess.SetInformation, true, true)
                    };
                    break;
                case ObjectType.Desktop:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", DesktopAccess.All, true, true),
                        new AccessEntry("Read", DesktopAccess.GenericRead, true, false),
                        new AccessEntry("Write", DesktopAccess.GenericWrite, true, false),
                        new AccessEntry("Execute", DesktopAccess.GenericExecute, true, false),
                        new AccessEntry("Enumerate", DesktopAccess.Enumerate, false, true),
                        new AccessEntry("Read objects", DesktopAccess.ReadObjects, false, true),
                        new AccessEntry("Playback journals", DesktopAccess.JournalPlayback, false, true),
                        new AccessEntry("Write objects", DesktopAccess.WriteObjects, false, true),
                        new AccessEntry("Create windows", DesktopAccess.CreateWindow, false, true),
                        new AccessEntry("Create menus", DesktopAccess.CreateMenu, false, true),
                        new AccessEntry("Create window hooks", DesktopAccess.HookControl, false, true),
                        new AccessEntry("Record journals", DesktopAccess.JournalRecord, false, true),
                        new AccessEntry("Switch desktop", DesktopAccess.SwitchDesktop, false, true)
                    };
                    break;
                case ObjectType.Directory:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", DirectoryAccess.All, true, true),
                        new AccessEntry("Query", DirectoryAccess.Query, true, true),
                        new AccessEntry("Traverse", DirectoryAccess.Traverse, true, true),
                        new AccessEntry("Create objects", DirectoryAccess.CreateObject, true, true),
                        new AccessEntry("Create subdirectories", DirectoryAccess.CreateSubdirectory, true, true)
                    };
                    break;
                case ObjectType.Event:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", EventAccess.All, true, true),
                        new AccessEntry("Query", EventAccess.QueryState, true, true),
                        new AccessEntry("Modify", EventAccess.ModifyState, true, true)
                    };
                    break;
                case ObjectType.EventPair:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", EventPairAccess.All, true, true)
                    };
                    break;
                case ObjectType.File:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", FileAccess.All, true, true),
                        new AccessEntry("Read & execute", FileAccess.GenericRead | FileAccess.GenericExecute, true, false),
                        new AccessEntry("Read", FileAccess.GenericRead, true, false),
                        new AccessEntry("Write", FileAccess.GenericWrite, true, false),
                        new AccessEntry("Traverse folder / execute file", FileAccess.Execute, false, true),
                        new AccessEntry("List folder / read data", FileAccess.ReadData, false, true),
                        new AccessEntry("Read attributes", FileAccess.ReadAttributes, false, true),
                        new AccessEntry("Read extended attributes", FileAccess.ReadEa, false, true),
                        new AccessEntry("Create files / write data", FileAccess.WriteData, false, true),
                        new AccessEntry("Create folders / append data", FileAccess.AppendData, false, true),
                        new AccessEntry("Write attributes", FileAccess.WriteAttributes, false, true),
                        new AccessEntry("Write extended attributes", FileAccess.WriteEa, false, true),
                        new AccessEntry("Delete subfolders and files", FileAccess.DeleteChild, false, true)
                    };
                    break;
                case ObjectType.IoCompletion:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", IoCompletionAccess.All, true, true),
                        new AccessEntry("Query", IoCompletionAccess.QueryState, true, true),
                        new AccessEntry("Modify", IoCompletionAccess.ModifyState, true, true)
                    };
                    break;
                case ObjectType.Job:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", JobObjectAccess.All, true, true),
                        new AccessEntry("Query", JobObjectAccess.Query, true, true),
                        new AccessEntry("Assign processes", JobObjectAccess.AssignProcess, true, true),
                        new AccessEntry("Set attributes", JobObjectAccess.SetAttributes, true, true),
                        new AccessEntry("Set security attributes", JobObjectAccess.SetSecurityAttributes, true, true),
                        new AccessEntry("Terminate", JobObjectAccess.Terminate, true, true)
                    };
                    break;
                case ObjectType.Key:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", KeyAccess.All, true, true),
                        new AccessEntry("Read", KeyAccess.GenericRead, true, false), 
                        new AccessEntry("Write", KeyAccess.GenericWrite, true, false), 
                        new AccessEntry("Execute", KeyAccess.GenericExecute, true, false),
                        new AccessEntry("Enumerate subkeys", KeyAccess.EnumerateSubKeys, false, true),
                        new AccessEntry("Query values", KeyAccess.QueryValue, false, true),
                        new AccessEntry("Notify", KeyAccess.Notify, false, true),
                        new AccessEntry("Set values", KeyAccess.SetValue, false, true),
                        new AccessEntry("Create subkeys", KeyAccess.CreateSubKey, false, true),
                        new AccessEntry("Create links", KeyAccess.CreateLink, false, true)
                    };
                    break;
                case ObjectType.KeyedEvent:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", KeyedEventAccess.All, true, true),
                        new AccessEntry("Wait", KeyedEventAccess.Wait, true, true),
                        new AccessEntry("Wake", KeyedEventAccess.Wake, true, true)
                    };
                    break;
                case ObjectType.Mutant:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", MutantAccess.All, true, true),
                        new AccessEntry("Query", MutantAccess.QueryState, true, true)
                    };
                    break;
                case ObjectType.Process:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control",
                            OSVersion.HasQueryLimitedInformation ?
                            (ProcessAccess.All | ProcessAccess.QueryLimitedInformation) :
                            ProcessAccess.All, true, true),
                        OSVersion.HasQueryLimitedInformation ?
                            new AccessEntry("Query limited information", ProcessAccess.QueryLimitedInformation, true, true) :
                            new AccessEntry(null, 0, false, false),
                        new AccessEntry("Query information",
                            OSVersion.HasQueryLimitedInformation ?
                            (ProcessAccess.QueryInformation | ProcessAccess.QueryLimitedInformation) :
                            ProcessAccess.QueryInformation, true, true),
                        new AccessEntry("Set information", ProcessAccess.SetInformation, true, true),
                        new AccessEntry("Set quotas", ProcessAccess.SetQuota, true, true),
                        new AccessEntry("Set session ID", ProcessAccess.SetSessionId, true, true),
                        new AccessEntry("Create threads", ProcessAccess.CreateThread, true, true),
                        new AccessEntry("Create processes", ProcessAccess.CreateProcess, true, true),
                        new AccessEntry("Modify memory", ProcessAccess.VmOperation, true, true),
                        new AccessEntry("Read memory", ProcessAccess.VmRead, true, true),
                        new AccessEntry("Write memory", ProcessAccess.VmWrite, true, true),
                        new AccessEntry("Duplicate handles", ProcessAccess.DupHandle, true, true),
                        new AccessEntry("Suspend / resume / set port", ProcessAccess.SuspendResume, true, true),
                        new AccessEntry("Terminate", ProcessAccess.Terminate, true, true),
                    };
                    break;
                case ObjectType.Profile:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", ProfileAccess.All, true, true),
                        new AccessEntry("Control", ProfileAccess.Control, true, true)
                    };
                    break;
                case ObjectType.Section:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", SectionAccess.All, true, true),
                        new AccessEntry("Query", SectionAccess.Query, true, true),
                        new AccessEntry("Map for read", SectionAccess.MapRead, true, true),
                        new AccessEntry("Map for write", SectionAccess.MapWrite, true, true),
                        new AccessEntry("Map for execute", SectionAccess.MapExecute, true, true),
                        new AccessEntry("Map for execute (explicit)", SectionAccess.MapExecuteExplicit, true, true),
                        new AccessEntry("Extend size", SectionAccess.ExtendSize, true, true)
                    };
                    break;
                case ObjectType.Semaphore:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", SemaphoreAccess.All, true, true),
                        new AccessEntry("Query", SemaphoreAccess.QueryState, true, true),
                        new AccessEntry("Modify", SemaphoreAccess.ModifyState, true, true)
                    };
                    break;
                case ObjectType.Service:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", ServiceAccess.All, true, true),
                        new AccessEntry("Query status", ServiceAccess.QueryStatus, true, true),
                        new AccessEntry("Query configuration", ServiceAccess.QueryConfig, true, true),
                        new AccessEntry("Modify configuration", ServiceAccess.ChangeConfig, true, true),
                        new AccessEntry("Enumerate dependents", ServiceAccess.EnumerateDependents, true, true),
                        new AccessEntry("Start", ServiceAccess.Start, true, true),
                        new AccessEntry("Stop", ServiceAccess.Stop, true, true),
                        new AccessEntry("Pause / continue", ServiceAccess.PauseContinue, true, true),
                        new AccessEntry("Interrogate", ServiceAccess.Interrogate, true, true),
                        new AccessEntry("User-defined control", ServiceAccess.UserDefinedControl, true, true)
                    };
                    break;
                case ObjectType.SymbolicLink:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", SymbolicLinkAccess.All, true, true),
                        new AccessEntry("Query", SymbolicLinkAccess.Query, true, true)
                    };
                    break;
                case ObjectType.Thread:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control",
                            OSVersion.HasQueryLimitedInformation ?
                            (ThreadAccess.All | ThreadAccess.QueryLimitedInformation | ThreadAccess.SetLimitedInformation) :
                            ThreadAccess.All, true, true),
                        OSVersion.HasQueryLimitedInformation ?
                            new AccessEntry("Query limited information", ThreadAccess.QueryLimitedInformation, true, true) :
                            new AccessEntry(null, 0, false, false),
                        new AccessEntry("Query information", ThreadAccess.QueryInformation, true, true),
                        OSVersion.HasQueryLimitedInformation ?
                            new AccessEntry("Set limited information", ThreadAccess.SetLimitedInformation, true, true) :
                            new AccessEntry(null, 0, false, false),
                        new AccessEntry("Set information", ThreadAccess.SetInformation, true, true),
                        new AccessEntry("Get context", ThreadAccess.GetContext, true, true),
                        new AccessEntry("Set context", ThreadAccess.SetContext, true, true),
                        new AccessEntry("Set token", ThreadAccess.SetThreadToken, true, true),
                        new AccessEntry("Alert", ThreadAccess.Alert, true, true),
                        new AccessEntry("Impersonate", ThreadAccess.Impersonate, true, true),
                        new AccessEntry("Direct impersonate", ThreadAccess.DirectImpersonation, true, true),
                        new AccessEntry("Suspend / resume", ThreadAccess.SuspendResume, true, true),
                        new AccessEntry("Terminate", ThreadAccess.Terminate, true, true),
                    };
                    break;
                case ObjectType.Timer:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", TimerAccess.All, true, true),
                        new AccessEntry("Query", TimerAccess.QueryState, true, true),
                        new AccessEntry("Modify", TimerAccess.ModifyState, true, true)
                    };
                    break;
                case ObjectType.TmEn:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", EnlistmentAccess.All, true, true),
                        new AccessEntry("Read", EnlistmentAccess.GenericRead, true, false),
                        new AccessEntry("Write", EnlistmentAccess.GenericWrite, true, false),
                        new AccessEntry("Execute", EnlistmentAccess.GenericExecute, true, false),
                        new AccessEntry("Query information", EnlistmentAccess.QueryInformation, false, true),
                        new AccessEntry("Set information", EnlistmentAccess.SetInformation, false, true),
                        new AccessEntry("Recover", EnlistmentAccess.Recover, false, true),
                        new AccessEntry("Subordinate rights", EnlistmentAccess.SubordinateRights, false, true),
                        new AccessEntry("Superior rights", EnlistmentAccess.SuperiorRights, false, true)
                    };
                    break;
                case ObjectType.TmRm:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", ResourceManagerAccess.All, true, true),
                        new AccessEntry("Read", ResourceManagerAccess.GenericRead, true, false),
                        new AccessEntry("Write", ResourceManagerAccess.GenericWrite, true, false),
                        new AccessEntry("Execute", ResourceManagerAccess.GenericExecute, true, false),
                        new AccessEntry("Query information", ResourceManagerAccess.QueryInformation, false, true),
                        new AccessEntry("Set information", ResourceManagerAccess.SetInformation, false, true),
                        new AccessEntry("Get notifications", ResourceManagerAccess.GetNotification, false, true),
                        new AccessEntry("Enlist", ResourceManagerAccess.Enlist, false, true),
                        new AccessEntry("Recover", ResourceManagerAccess.Recover, false, true),
                        new AccessEntry("Register protocols", ResourceManagerAccess.RegisterProtocol, false, true),
                        new AccessEntry("Complete propagation", ResourceManagerAccess.CompletePropagation, false, true)
                    };
                    break;
                case ObjectType.TmTm:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", TmAccess.All, true, true),
                        new AccessEntry("Read", TmAccess.GenericRead, true, false),
                        new AccessEntry("Write", TmAccess.GenericWrite, true, false),
                        new AccessEntry("Execute", TmAccess.GenericExecute, true, false),
                        new AccessEntry("Query information", TmAccess.QueryInformation, true, false),
                        new AccessEntry("Set information", TmAccess.SetInformation, true, false),
                        new AccessEntry("Recover", TmAccess.Recover, true, false),
                        new AccessEntry("Rename", TmAccess.Rename, true, false),
                        new AccessEntry("Create resource manager", TmAccess.CreateRm, true, false),
                        new AccessEntry("Bind transactions", TmAccess.BindTransaction, true, false)
                    };
                    break;
                case ObjectType.TmTx:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", TransactionAccess.All, true, true),
                        new AccessEntry("Read", TransactionAccess.GenericRead, true, false),
                        new AccessEntry("Write", TransactionAccess.GenericWrite, true, false),
                        new AccessEntry("Execute", TransactionAccess.GenericExecute, true, false),
                        new AccessEntry("Query information", TransactionAccess.QueryInformation, false, true),
                        new AccessEntry("Set information", TransactionAccess.SetInformation, false, true),
                        new AccessEntry("Enlist", TransactionAccess.Enlist, false, true),
                        new AccessEntry("Commit", TransactionAccess.Commit, false, true),
                        new AccessEntry("Rollback", TransactionAccess.Rollback, false, true),
                        new AccessEntry("Propagate", TransactionAccess.Propagate, false, true),
                    };
                    break;
                case ObjectType.Token:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", TokenAccess.All, true, true),
                        new AccessEntry("Read", TokenAccess.GenericRead, true, false),
                        new AccessEntry("Write", TokenAccess.GenericWrite, true, false),
                        new AccessEntry("Execute", TokenAccess.GenericExecute, true, false),
                        new AccessEntry("Adjust privileges", TokenAccess.AdjustPrivileges, false, true),
                        new AccessEntry("Adjust groups", TokenAccess.AdjustGroups, false, true),
                        new AccessEntry("Adjust defaults", TokenAccess.AdjustDefault, false, true),
                        new AccessEntry("Adjust session ID", TokenAccess.AdjustSessionId, false, true),
                        new AccessEntry("Assign as primary token", TokenAccess.AssignPrimary, false, true),
                        new AccessEntry("Duplicate", TokenAccess.Duplicate, false, true),
                        new AccessEntry("Impersonate", TokenAccess.Impersonate, false, true),
                        new AccessEntry("Query", TokenAccess.Query, false, true),
                        new AccessEntry("Query source", TokenAccess.QuerySource, false, true)
                    };
                    break;
                case ObjectType.Type:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", TypeObjectAccess.All, true, true),
                        new AccessEntry("Create", TypeObjectAccess.Create, true, true)
                    };
                    break;
                case ObjectType.WindowStation:
                    entries = new AccessEntry[]
                    {
                        new AccessEntry("Full control", WindowStationAccess.All, true, true),
                        new AccessEntry("Read", WindowStationAccess.GenericRead, true, false),
                        new AccessEntry("Write", WindowStationAccess.GenericWrite, true, false),
                        new AccessEntry("Execute", WindowStationAccess.GenericExecute, true, false),
                        new AccessEntry("Enumerate", WindowStationAccess.Enumerate, false, true),
                        new AccessEntry("Enumerate desktops", WindowStationAccess.EnumDesktops, false, true),
                        new AccessEntry("Read attributes", WindowStationAccess.ReadAttributes, false, true),
                        new AccessEntry("Read screen", WindowStationAccess.ReadScreen, false, true),
                        new AccessEntry("Access clipboard", WindowStationAccess.AccessClipboard, false, true),
                        new AccessEntry("Access global atoms", WindowStationAccess.AccessGlobalAtoms, false, true),
                        new AccessEntry("Create desktop", WindowStationAccess.CreateDesktop, false, true),
                        new AccessEntry("Write attributes", WindowStationAccess.WriteAttributes, false, true),
                        new AccessEntry("Exit windows", WindowStationAccess.ExitWindows, false, true)
                    };
                    break;
                default:
                    entries = null;
                    break;
            }

            // Add the standard rights.
            return Utils.Concat<AccessEntry>(entries, new AccessEntry[]
            {
                new AccessEntry("Synchronize", StandardRights.Synchronize, false, true),
                new AccessEntry("Delete", StandardRights.Delete, false, true),
                new AccessEntry("Read permissions", StandardRights.ReadControl, false, true),
                new AccessEntry("Change permissions", StandardRights.WriteDac, false, true),
                new AccessEntry("Take ownership", StandardRights.WriteOwner, false, true)
            });
        }

        public static Type GetAccessType(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.AlpcPort:
                    return typeof(PortAccess);
                case ObjectType.DebugObject:
                    return typeof(DebugObjectAccess);
                case ObjectType.Desktop:
                    return typeof(DesktopAccess);
                case ObjectType.Directory:
                    return typeof(DirectoryAccess);
                case ObjectType.Event:
                    return typeof(EventAccess);
                case ObjectType.EventPair:
                    return typeof(EventPairAccess);
                case ObjectType.File:
                    return typeof(FileAccess);
                case ObjectType.FilterCommunicationPort:
                case ObjectType.FilterConnectionPort:
                    return typeof(FltPortAccess);
                case ObjectType.IoCompletion:
                    return typeof(IoCompletionAccess);
                case ObjectType.Job:
                    return typeof(JobObjectAccess);
                case ObjectType.Key:
                    return typeof(KeyAccess);
                case ObjectType.KeyedEvent:
                    return typeof(KeyedEventAccess);
                case ObjectType.Mutant:
                    return typeof(MutantAccess);
                case ObjectType.Process:
                    return typeof(ProcessAccess);
                case ObjectType.Profile:
                    return typeof(ProfileAccess);
                case ObjectType.Section:
                    return typeof(SectionAccess);
                case ObjectType.Semaphore:
                    return typeof(SemaphoreAccess);
                case ObjectType.Service:
                    return typeof(ServiceAccess);
                case ObjectType.SymbolicLink:
                    return typeof(SymbolicLinkAccess);
                case ObjectType.Thread:
                    return typeof(ThreadAccess);
                case ObjectType.Timer:
                    return typeof(TimerAccess);
                case ObjectType.TmEn:
                    return typeof(EnlistmentAccess);
                case ObjectType.TmRm:
                    return typeof(ResourceManagerAccess);
                case ObjectType.TmTm:
                    return typeof(TmAccess);
                case ObjectType.TmTx:
                    return typeof(TransactionAccess);
                case ObjectType.Token:
                    return typeof(TokenAccess);
                case ObjectType.Type:
                    return typeof(TypeObjectAccess);
                case ObjectType.WindowStation:
                    return typeof(WindowStationAccess);
                default:
                    throw new NotSupportedException();
            }
        }

        public static ObjectType GetObjectType(string typeName)
        {
            foreach (string value in Enum.GetNames(typeof(ObjectType)))
            {
                if (string.Equals(value, typeName, StringComparison.InvariantCultureIgnoreCase))
                    return (ObjectType)Enum.Parse(typeof(ObjectType), value);
            }

            if (string.Equals(typeName, "ALPC Port", StringComparison.InvariantCultureIgnoreCase))
                return ObjectType.AlpcPort;
            if (string.Equals(typeName, "Port", StringComparison.InvariantCultureIgnoreCase))
                return ObjectType.AlpcPort;
            if (string.Equals(typeName, "WaitablePort", StringComparison.InvariantCultureIgnoreCase))
                return ObjectType.AlpcPort;

            throw new NotSupportedException();
        }

        public static SeObjectType GetSeObjectType(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Desktop:
                case ObjectType.WindowStation:
                    return SeObjectType.WindowObject;
                case ObjectType.Service:
                    return SeObjectType.Service;
                default:
                    return SeObjectType.KernelObject;
            }
        }

        #endregion

        public static string GetAccessString(Type accessType, object access)
        {
            StringBuilder accessSb = new StringBuilder();
            long accessLong = Convert.ToInt64(access);
            var accessTypeNames = Utils.SortFlagNames(accessType).
                ConvertAll<FlagName>((kvp) => new FlagName() { Name = kvp.Key, Value = kvp.Value, Enabled = true });
            var srNames = Utils.SortFlagNames(typeof(StandardRights)).
                ConvertAll<FlagName>((kvp) => new FlagName() { Name = kvp.Key, Value = kvp.Value, Enabled = true });

            // Get the strings for the matching bits in the given enum type.
            foreach (var fn in accessTypeNames)
            {
                if (
                    fn.Enabled && 
                    (accessLong & fn.Value) == fn.Value
                    )
                {
                    accessSb.Append(fn.Name + ", ");
                    // Disable equal or more specific flag names in the lists.
                    accessTypeNames.ForEach((fn2) =>
                    {
                        if ((fn.Value | fn2.Value) == fn.Value)
                            fn2.Enabled = false;
                    });
                    srNames.ForEach((fn2) =>
                    {
                        if ((fn.Value | fn2.Value) == fn.Value)
                            fn2.Enabled = false;
                    });
                }
            }

            // Get the strings for the matching bits in standard rights.
            foreach (var fn in srNames)
            {
                if (
                    fn.Enabled &&
                    (accessLong & fn.Value) == fn.Value
                    )
                {
                    accessSb.Append(fn.Name + ", ");
                    // Disable equal or more specific flag names in the lists.
                    srNames.ForEach((fn2) =>
                    {
                        if ((fn.Value | fn2.Value) == fn.Value)
                            fn2.Enabled = false;
                    });
                }
            }

            string accessString = accessSb.ToString();

            // Removing trailing ", ".
            if (accessString.EndsWith(", "))
                return accessString.Remove(accessString.Length - 2, 2);
            else
                return accessString;
        }

        public static Type GetAccessType(string typeName)
        {
            return GetAccessType(GetObjectType(typeName));
        }
    }
}
