using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.Security.Authentication
{
    public interface IAuthenticationPackage
    {
        /// <summary>
        /// Gets the name of the authentication package. 
        /// This name must be recognizable by the LSA.
        /// </summary>
        string PackageName { get; }

        /// <summary>
        /// Gets a buffer containing authentication information. 
        /// The format of the information is specific to the 
        /// package.
        /// </summary>
        /// <returns>A memory region containing the information.</returns>
        MemoryRegion GetAuthData();

        /// <summary>
        /// Parses a buffer and returns profile-related information 
        /// specific to the package.
        /// </summary>
        /// <param name="buffer">A buffer containing profile information.</param>
        /// <returns>A package-specific object containing profile information.</returns>
        object GetProfileData(MemoryRegion buffer);

        /// <summary>
        /// Populates the appropriate class fields from authentication information.
        /// </summary>
        /// <returns>A memory region containing the information.</returns>
        void ReadAuthData(MemoryRegion buffer);
    }
}
