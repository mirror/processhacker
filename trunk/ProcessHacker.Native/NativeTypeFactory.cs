using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native
{
    public static class NativeTypeFactory
    {
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
                case ObjectType.Job:
                    return typeof(JobObjectAccess);
                case ObjectType.KeyedEvent:
                    return typeof(KeyedEventAccess);
                case ObjectType.Mutant:
                    return typeof(MutantAccess);
                case ObjectType.Type:
                    return typeof(ObjectTypeAccess);
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
                case ObjectType.Token:
                    return typeof(TokenAccess);
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

            throw new NotSupportedException();
        }

        #endregion  

        public static Type GetAccessType(string typeName)
        {
            return GetAccessType(GetObjectType(typeName));
        }
    }
}
