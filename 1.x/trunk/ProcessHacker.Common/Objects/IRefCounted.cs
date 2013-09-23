using System;

namespace ProcessHacker.Common.Objects
{
    public interface IRefCounted : IDisposable
    {
        /// <summary>
        /// Decrements the reference count of the object.
        /// </summary>
        /// <returns>The new reference count.</returns>
        long Dereference();

        /// <summary>
        /// Decrements the reference count of the object.
        /// </summary>
        /// <param name="managed">Whether to dispose managed resources.</param>
        /// <returns>The new reference count.</returns>
        long Dereference(bool managed);

        /// <summary>
        /// Decreases the reference count of the object.
        /// </summary>
        /// <param name="count">The number of times to dereference the object.</param>
        /// <returns>The new reference count.</returns>
        long Dereference(long count);

        /// <summary>
        /// Decreases the reference count of the object.
        /// </summary>
        /// <param name="count">The number of times to dereference the object.</param>
        /// <param name="managed">Whether to dispose managed resources.</param>
        /// <returns>The new reference count.</returns>
        long Dereference(long count, bool managed);

        /// <summary>
        /// Ensures that the reference counting system has exclusive control 
        /// over the lifetime of the object.
        /// </summary>
        /// <param name="managed">Whether to dispose managed resources.</param>
        void Dispose(bool managed);

        /// <summary>
        /// Increments the reference count of the object.
        /// </summary>
        /// <returns>The new reference count.</returns>
        long Reference();

        /// <summary>
        /// Increases the reference count of the object.
        /// </summary>
        /// <param name="count">The number of times to reference the object.</param>
        /// <returns>The new reference count.</returns>
        long Reference(long count);
    }
}
