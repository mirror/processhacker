using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Common
{
    /// <summary>
    /// Stores a string with a maximum length of 255 characters.
    /// </summary>
    public unsafe struct String255 : IEquatable<String255>, IEquatable<string>
    {
        private const string MsgStrMustBeLessThanMax = "The string must be less than 256 characters in length.";
        private const string MsgNoMoreSpace = "There is not enough storage space.";
        public const int MaximumLength = 255;

        public fixed char Buffer[256];
        public byte Length;

        public String255(string str)
        {
            if (str.Length > MaximumLength)
                throw new ArgumentException(MsgStrMustBeLessThanMax);

            fixed (char* buffer = this.Buffer)
            {
                for (int i = 0; i < str.Length; i++)
                    buffer[i] = str[i];

                buffer[str.Length] = '\0';
            }

            this.Length = (byte)str.Length;
        }

        public String255(String255 str, int startIndex, int length)
        {
            if (startIndex >= str.Length)
                throw new ArgumentException("The start index is too large.");
            if (length > str.Length - startIndex)
                throw new ArgumentException("The length is too large.");
            if (length > MaximumLength)
                throw new ArgumentException(MsgStrMustBeLessThanMax);

            fixed (char* buffer = this.Buffer)
            {
                LibC.WMemCpy(buffer, &str.Buffer[startIndex], length);
                buffer[length] = '\0';
            }

            this.Length = (byte)length;
        }

        public String255(char* str, int length)
        {
            if (length > MaximumLength)
                throw new ArgumentException(MsgStrMustBeLessThanMax);

            fixed (char* buffer = this.Buffer)
            {
                LibC.WMemCpy(buffer, str, length);
                buffer[length] = '\0';
            }

            this.Length = (byte)length;
        }

        public char this[int index]
        {
            get
            {
                fixed (char* buffer = this.Buffer)
                    return buffer[index];
            }
            set
            {
                fixed (char* buffer = this.Buffer)
                    buffer[index] = value;
            }
        }

        public void Append(char c)
        {
            if ((this.Length + 1) > MaximumLength)
                throw new InvalidOperationException(MsgNoMoreSpace);

            fixed (char* buffer = this.Buffer)
            {
                buffer[this.Length++] = c;
                buffer[this.Length] = '\0';
            }
        }

        public void Append(String255 str)
        {
            if ((this.Length + str.Length) > MaximumLength)
                throw new InvalidOperationException(MsgNoMoreSpace);

            fixed (char* buffer = this.Buffer)
            {
                LibC.WMemCpyUnaligned(&buffer[this.Length], str.Buffer, str.Length);
                this.Length += str.Length;
                buffer[this.Length] = '\0';
            }
        }

        public void Append(string str)
        {
            if ((this.Length + str.Length) > MaximumLength)
                throw new InvalidOperationException(MsgNoMoreSpace);

            fixed (char* buffer = this.Buffer)
            {
                for (int i = 0; i < str.Length; i++)
                    buffer[this.Length++] = str[i];

                buffer[this.Length] = '\0';
            }
        }

        public int CompareTo(String255 str)
        {
            fixed (char* buffer = this.Buffer)
            {
                int result;

                result = LibC.WMemCmp(buffer, str.Buffer, this.Length < str.Length ? this.Length : str.Length);

                if (result == 0)
                    return this.Length - str.Length;
                else
                    return result;
            }
        }

        public bool EndsWith(String255 str)
        {
            if (str.Length > this.Length)
                return false;

            fixed (char* buffer = this.Buffer)
                return LibC.WMemCmp(&buffer[this.Length - str.Length], str.Buffer, str.Length) == 0;
        }

        public override bool Equals(object other)
        {
            if (other is String255)
                return this.Equals((String255)other);
            else if (other is string)
                return this.Equals((string)other);
            else
                return false;
        }

        public bool Equals(String255 other)
        {
            if (this.Length != other.Length)
                return false;

            fixed (char* buffer = this.Buffer)
            {
                for (int i = 0; i < other.Length; i++)
                {
                    if (buffer[i] != other.Buffer[i])
                        return false;
                }
            }

            return true;
        }

        public bool Equals(string other)
        {
            if (this.Length != other.Length)
                return false;

            fixed (char* buffer = this.Buffer)
            {
                for (int i = 0; i < other.Length; i++)
                {
                    if (buffer[i] != other[i])
                        return false;
                }
            }

            return true;
        }

        public int IndexOf(char c)
        {
            char* ptr;

            fixed (char* buffer = this.Buffer)
            {
                ptr = LibC.WMemChr(buffer, c, this.Length);

                if (ptr != null)
                    return (int)(ptr - buffer);
                else
                    return -1;
            }
        }

        public override int GetHashCode()
        {
            int hashCode = 0x15051505;

            fixed (char* buffer = this.Buffer)
            {
                for (int i = 0; i < this.Length; i += 4)
                {
                    hashCode += hashCode ^ (hashCode << ((i % 4) * 8));
                }
            }

            return hashCode;
        }

        public bool StartsWith(String255 str)
        {
            if (str.Length > this.Length)
                return false;

            fixed (char* buffer = this.Buffer)
                return LibC.WMemCmp(buffer, str.Buffer, str.Length) == 0;
        }

        public String255 Substring(int startIndex)
        {
            return this.Substring(startIndex, this.Length - startIndex);
        }

        public String255 Substring(int startIndex, int length)
        {
            return new String255(this, startIndex, length);
        }

        public override string ToString()
        {
            fixed (char* buffer = this.Buffer)
            {
                return new string(buffer, 0, this.Length);
            }
        }
    }
}
