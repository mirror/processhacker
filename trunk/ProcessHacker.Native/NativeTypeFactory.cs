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
            switch (type)
            {
                case ObjectType.AlpcPort:
                    return new AccessEntry[]
                    {
                        new AccessEntry("Full control", PortAccess.All, true, true),
                        new AccessEntry("Connect", PortAccess.Connect, true, true)
                    };
                default:
                    throw new NotSupportedException();
            }
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
                    return typeof(ObjectTypeAccess);
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
