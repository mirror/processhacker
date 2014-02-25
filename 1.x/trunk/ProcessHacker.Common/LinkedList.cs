using System;

namespace ProcessHacker.Common
{
    public class LinkedListEntry<T>
    {
        public LinkedListEntry<T> Flink;
        public LinkedListEntry<T> Blink;
        public T Value;
    }

    public static class LinkedList
    {
        public static void InitializeListHead<T>(LinkedListEntry<T> listHead)
        {
            listHead.Flink = listHead.Blink = listHead;
        }

        public static bool RemoveEntryList<T>(LinkedListEntry<T> entry)
        {
            LinkedListEntry<T> blink;
            LinkedListEntry<T> flink;

            flink = entry.Flink;
            blink = entry.Blink;
            blink.Flink = flink;
            flink.Blink = blink;

            return flink == blink;
        }

        public static LinkedListEntry<T> RemoveHeadList<T>(LinkedListEntry<T> listHead)
        {
            LinkedListEntry<T> flink;
            LinkedListEntry<T> entry;

            entry = listHead.Flink;
            flink = entry.Flink;
            listHead.Flink = flink;
            flink.Blink = listHead;

            return entry;
        }

        public static LinkedListEntry<T> RemoveTailList<T>(LinkedListEntry<T> listHead)
        {
            LinkedListEntry<T> blink;
            LinkedListEntry<T> entry;

            entry = listHead.Blink;
            blink = entry.Blink;
            listHead.Blink = blink;
            blink.Flink = listHead;

            return entry;
        }

        public static void InsertTailList<T>(LinkedListEntry<T> listHead, LinkedListEntry<T> entry)
        {
            LinkedListEntry<T> blink;

            blink = listHead.Blink;
            entry.Flink = listHead;
            entry.Blink = blink;
            blink.Flink = entry;
            listHead.Blink = entry;
        }

        public static void InsertHeadList<T>(LinkedListEntry<T> listHead, LinkedListEntry<T> entry)
        {
            LinkedListEntry<T> flink;

            flink = listHead.Flink;
            entry.Flink = flink;
            entry.Blink = listHead;
            flink.Blink = entry;
            listHead.Flink = entry;
        }
    }
}
