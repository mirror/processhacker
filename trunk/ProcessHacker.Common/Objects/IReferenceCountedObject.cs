namespace ProcessHacker.Common.Objects
{
    public interface IReferenceCountedObject
    {
        /// <summary>
        /// Decrements the reference count of the object.
        /// </summary>
        /// <returns>The old reference count.</returns>
        int Dereference();

        /// <summary>
        /// Decrements the reference count of the object.
        /// </summary>
        /// <param name="managed">Whether to dispose managed resources.</param>
        /// <returns>The old reference count.</returns>
        int Dereference(bool managed);

        /// <summary>
        /// Increments the reference count of the object.
        /// </summary>
        /// <returns>The old reference count.</returns>
        int Reference();
    }
}
