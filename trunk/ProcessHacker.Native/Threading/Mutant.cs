using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Threading
{
    /// <summary>
    /// Represents a mutant which can be used to synchronize access to a shared resource.
    /// </summary>
    public class Mutant : NativeObject<MutantHandle>
    {
        /// <summary>
        /// Creates a mutant.
        /// </summary>
        public Mutant()
            : this(null)
        { }

        /// <summary>
        /// Creates a mutant.
        /// </summary>
        /// <param name="owned">
        /// Whether the mutant should become owned by the current 
        /// thread when it is created.
        /// </param>
        public Mutant(bool owned)
            : this(null, owned)
        { }

        /// <summary>
        /// Creates or opens a mutant.
        /// </summary>
        /// <param name="name">
        /// The name of the new mutant, or the name of an existing mutant to open.
        /// </param>
        public Mutant(string name)
            : this(name, false)
        { }

        /// <summary>
        /// Creates a mutant.
        /// </summary>
        /// <param name="name">
        /// The name of the new mutant.
        /// </param>
        /// <param name="owned">
        /// Whether the mutant should become owned by the current 
        /// thread when it is created.
        /// </param>
        public Mutant(string name, bool owned)
        {
            this.Handle = MutantHandle.Create(
                MutantAccess.All,
                name,
                ObjectFlags.OpenIf,
                null,
                owned
                );
        }

        /// <summary>
        /// Gets whether the mutant is currently owned.
        /// </summary>
        public bool Owned
        {
            get { return this.Handle.GetBasicInformation().CurrentCount > 0; }
        }

        /// <summary>
        /// Releases the mutant, allowing other waiting threads to own it.
        /// </summary>
        public void Release()
        {
            this.Handle.Release();
        }
    }
}
