/*
 * http://www.codeproject.com/KB/cs/EnumComparer.aspx
 * 
 * by Omer Mor
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;

namespace ProcessHacker.Common
{
    public sealed class EnumComparer<TEnum> : IEqualityComparer<TEnum>
        where TEnum : struct, IComparable, IConvertible, IFormattable
    {
        public static readonly EnumComparer<TEnum> Instance;

        private static readonly Func<TEnum, TEnum, bool> _equals;
        private static readonly Func<TEnum, int> _getHashCode;

        static EnumComparer()
        {
            _getHashCode = generateGetHashCode();
            _equals = generateEquals();
            Instance = new EnumComparer<TEnum>();
        }

        private EnumComparer()
        {
            AssertTypeIsEnum();
            AssertUnderlyingTypeIsSupported();
        }

        public bool Equals(TEnum x, TEnum y)
        {
            return _equals(x, y);
        }

        public int GetHashCode(TEnum obj)
        {
            return _getHashCode(obj);
        }

        private static void AssertTypeIsEnum()
        {
            if (typeof(TEnum).IsEnum)
                return;

            throw new NotSupportedException();
        }

        private static void AssertUnderlyingTypeIsSupported()
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
            ICollection<Type> supportedTypes =
                new[]
                {
                    typeof (byte), typeof (sbyte), typeof (short), typeof (ushort),
                    typeof (int), typeof (uint), typeof (long), typeof (ulong)
                };

            if (supportedTypes.Contains(underlyingType))
                return;

            throw new NotSupportedException();
        }

        /// <summary>
        /// Generates a comparison method similar to this:
        /// <code>
        /// bool Equals(TEnum x, TEnum y)
        /// {
        ///     return x == y;
        /// }
        /// </code>
        /// </summary>
        /// <returns>The generated method.</returns>
        private static Func<TEnum, TEnum, bool> generateEquals()
        {
            var method = new DynamicMethod(typeof(TEnum).Name + "_Equals",
                typeof(bool),
                new[] { typeof(TEnum), typeof(TEnum) },
                typeof(TEnum), true);
            var generator = method.GetILGenerator();
            // Writing body
            generator.Emit(OpCodes.Ldarg_0);    // load x to stack
            generator.Emit(OpCodes.Ldarg_1);    // load y to stack
            generator.Emit(OpCodes.Ceq);        // x == y
            generator.Emit(OpCodes.Ret);        // return result
            return (Func<TEnum, TEnum, bool>)method.CreateDelegate
                (typeof(Func<TEnum, TEnum, bool>));
        }

        /// <summary>
        /// Generates a GetHashCode method similar to this:
        /// <code>
        /// int GetHashCode(TEnum obj)
        /// {
        ///     return ((int)obj).GetHashCode();
        /// }
        /// </code>
        /// </summary>
        /// <returns>The generated method.</returns>
        private static Func<TEnum, int> generateGetHashCode()
        {
            var method = new DynamicMethod(typeof(TEnum).Name + "_GetHashCode",
                typeof(int),
                new[] { typeof(TEnum) },
                typeof(TEnum), true);
            var generator = method.GetILGenerator();
            var underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
            var getHashCodeMethod = underlyingType.GetMethod("GetHashCode");
            var castValue = generator.DeclareLocal(underlyingType);
            // Writing body
            generator.Emit(OpCodes.Ldarg_0);                    // load obj to stack
            generator.Emit(OpCodes.Stloc_0);                    // castValue = obj
            generator.Emit(OpCodes.Ldloca_S, castValue);        // load *castValue to stack
            generator.Emit(OpCodes.Call, getHashCodeMethod);    // castValue.GetHashCode()
            generator.Emit(OpCodes.Ret);                        // return result
            return (Func<TEnum, int>)method.CreateDelegate(typeof(Func<TEnum, int>));
        }
    }
}
