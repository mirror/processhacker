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

        public static readonly System.Guid DRIVER_ACTION_VERIFY = new System.Guid("{F750E6C3-38EE-11d1-85E5-00C04FC295EE}");
        public static readonly System.Guid HTTPSPROV_ACTION = new System.Guid("{573E31F8-AABA-11d0-8CCB-00C04FC295EE}");
        public static readonly System.Guid OFFICESIGN_ACTION_VERIFY = new System.Guid("{5555C2CD-17FB-11d1-85C4-00C04FC295EE}");
        public static readonly System.Guid WINTRUST_ACTION_GENERIC_CERT_VERIFY = new System.Guid("{189A3842-3041-11d1-85E1-00C04FC295EE}");
        public static readonly System.Guid WINTRUST_ACTION_GENERIC_CHAIN_VERIFY = new System.Guid("{fc451c16-ac75-11d1-b4b8-00c04fb66ea0}");
        public static readonly System.Guid WINTRUST_ACTION_GENERIC_VERIFY_V2 = new System.Guid("{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}");
        public static readonly System.Guid WINTRUST_ACTION_TRUSTPROVIDER_TEST = new System.Guid("{573E31F8-DDBA-11d0-8CCB-00C04FC295EE}");

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

            result = Win32.WinVerifyTrust(new IntPtr(-1)
                                          , WINTRUST_ACTION_GENERIC_VERIFY_V2
                                          , trustData);

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

                if (!Win32.CryptCATAdminAcquireContext(out catAdmin, DRIVER_ACTION_VERIFY, 0))
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
                    result = Win32.WinVerifyTrust(new IntPtr(-1), DRIVER_ACTION_VERIFY, trustData);
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
