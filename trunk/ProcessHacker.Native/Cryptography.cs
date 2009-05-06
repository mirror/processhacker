using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.WinTrust;
using System;

namespace ProcessHacker.Native
{
    public enum VerifyResult : int
    {
        Unknown = 0,
        NoSignature,
        Trusted,
        TrustedInstaller,
        Expired,
        Revoked,
        Distrust,
        SecuritySettings
    }

    public static class Cryptography
    {
        // GUID of the action to perform
        public static readonly System.Guid DriverActionVerify = new System.Guid("{f750e6c3-38ee-11d1-85e5-00c04fc295ee}");
        public static readonly System.Guid HttpsProvAction = new System.Guid("{573e31f8-aaba-11d0-8ccb-00c04fc295ee}");
        public static readonly System.Guid OfficeSignActionVerify = new System.Guid("{5555c2cd-17fb-11d1-85c4-00c04fc295ee}");
        public static readonly System.Guid WintrustActionGenericCertVerify = new System.Guid("{189a3842-3041-11d1-85e1-00c04fc295ee}");
        public static readonly System.Guid WintrustActionGenericChainVerify = new System.Guid("{fc451c16-ac75-11d1-b4b8-00c04fb66ea0}");
        public static readonly System.Guid WintrustActionGenericVerifyV2 = new System.Guid("{00aac56b-cd44-11d0-8cc2-00c04fc295ee}");
        public static readonly System.Guid WintrustActionTrustProviderTest = new System.Guid("{573e31f8-ddba-11d0-8ccb-00c04fc295ee}");

        public static VerifyResult StatusToVerifyResult(uint status)
        {
            if (status == 0)
                return VerifyResult.Trusted;
            else if (status == 0x800b0100)
                return VerifyResult.NoSignature;
            else if (status == 0x800b0101)
                return VerifyResult.Expired;
            else if (status == 0x800b010c)
                return VerifyResult.Revoked;
            else if (status == 0x800b0111)
                return VerifyResult.Distrust;
            else if (status == 0x80092026)
                return VerifyResult.SecuritySettings;
            else
                return VerifyResult.SecuritySettings;
        }

        public static VerifyResult VerifyFile(string filePath)
        {
            WinVerifyTrustResult result = WinVerifyTrustResult.ActionUnknown;

            WinTrustData trustData = new WinTrustData();
            WinTrustFileInfo fileInfo = new WinTrustFileInfo(filePath);
            trustData.StructSize = 12 * 4;
            trustData.UIChoice = WinTrustDataUIChoice.None;
            trustData.UnionChoice = WinTrustDataChoice.File;
            trustData.ProvFlags = WinTrustDataProvFlags.SaferFlag;
            trustData.RevocationChecks = WinTrustDataRevocationChecks.None;
            trustData.UnionData = fileInfo;

            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
                trustData.ProvFlags |= WinTrustDataProvFlags.CacheOnlyUrlRetrieval;

            result = Win32.WinVerifyTrust(
                new IntPtr(-1),
                WintrustActionGenericVerifyV2,
                trustData
                );

            if (StatusToVerifyResult((uint)result) == VerifyResult.NoSignature)
            {
                FileHandle sourceFile = new FileHandle(filePath, FileAccess.GenericRead, FileShareMode.Read,
                    FileCreationDisposition.OpenExisting);
                byte[] hash = new byte[256];
                int hashLength = 256;
                if (!Win32.CryptCATAdminCalcHashFromFileHandle(sourceFile, ref hashLength, hash, 0))
                {
                    hash = new byte[hashLength];

                    if (!Win32.CryptCATAdminCalcHashFromFileHandle(sourceFile, ref hashLength, hash, 0))
                        return VerifyResult.NoSignature;
                }

                StringBuilder memberTag = new StringBuilder(hashLength * 2);

                for (int i = 0; i < hashLength; i++)
                    memberTag.Append(hash[i].ToString("X2"));

                IntPtr catAdmin;

                if (!Win32.CryptCATAdminAcquireContext(out catAdmin, DriverActionVerify, 0))
                    return VerifyResult.NoSignature;

                IntPtr catInfo = Win32.CryptCATAdminEnumCatalogFromHash(catAdmin, hash, hashLength, 0, IntPtr.Zero);

                if (catInfo == IntPtr.Zero)
                {
                    Win32.CryptCATAdminReleaseContext(catAdmin, 0);
                    return VerifyResult.NoSignature;
                }

                CatalogInfo ci = new CatalogInfo();

                if (!Win32.CryptCATCatalogInfoFromContext(catInfo, ref ci, 0))
                {
                    Win32.CryptCATAdminReleaseCatalogContext(catAdmin, catInfo, 0);
                    Win32.CryptCATAdminReleaseContext(catAdmin, 0);
                    return VerifyResult.NoSignature;
                }

                WinTrustCatalogInfo wci = new WinTrustCatalogInfo();

                wci.Size = Marshal.SizeOf(wci);
                wci.CatalogFilePath = ci.CatalogFile;
                wci.MemberFilePath = filePath;
                wci.MemberTag = memberTag.ToString();

                trustData = new WinTrustData();

                trustData.StructSize = 12 * 4;
                trustData.UIChoice = WinTrustDataUIChoice.None;
                trustData.UnionChoice = WinTrustDataChoice.Catalog;
                trustData.RevocationChecks = WinTrustDataRevocationChecks.None;

                if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
                    trustData.ProvFlags |= WinTrustDataProvFlags.CacheOnlyUrlRetrieval;

                trustData.UnionData = wci;

                try
                {
                    result = Win32.WinVerifyTrust(new IntPtr(-1), DriverActionVerify, trustData);
                }
                finally
                {
                    Win32.CryptCATAdminReleaseCatalogContext(catAdmin, catInfo, 0);
                    Win32.CryptCATAdminReleaseContext(catAdmin, 0);
                }
            }

            return StatusToVerifyResult((uint)result);
        }
    }
}
