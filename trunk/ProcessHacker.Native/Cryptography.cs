using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

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
        public static readonly ProcessHacker.Native.Api.Guid GenericCertVerify =
            new ProcessHacker.Native.Api.Guid()
            {
                Data1 = 0x189a3842,
                Data2 = 0x3041,
                Data3 = 0x11d1,
                Data4 = new byte[] { 0x85, 0xe1, 0x00, 0xc0, 0x4f, 0xc2, 0x95, 0xee }
            };

        public static readonly ProcessHacker.Native.Api.Guid GenericVerifyV2 =
            new ProcessHacker.Native.Api.Guid()
            {
                Data1 = 0xaac56b,
                Data2 = 0xcd44,
                Data3 = 0x11d0,
                Data4 = new byte[] { 0x8c, 0xc2, 0x00, 0xc0, 0x4f, 0xc2, 0x95, 0xee }
            };

        public static readonly ProcessHacker.Native.Api.Guid GenericChainVerify = 
            new ProcessHacker.Native.Api.Guid()
            {
                Data1 = 0xfc451c16,
                Data2 = 0xac75,
                Data3 = 0x11d1,
                Data4 = new byte[] { 0xb4, 0xb8, 0x00, 0xc0, 0x4f, 0xb6, 0x6e, 0xa0 }
            };

        public static readonly ProcessHacker.Native.Api.Guid DriverVerify = 
            new ProcessHacker.Native.Api.Guid()
            {
                Data1 = 0xf750e6c3,
                Data2 = 0x38ee,
                Data3 = 0x11d1,
                Data4 = new byte[] { 0x85, 0xe5, 0x00, 0xc0, 0x4f, 0xc2, 0x95, 0xee }
            };

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
            VerifyResult result = VerifyResult.NoSignature;

            using (MemoryAlloc strMem = new MemoryAlloc(filePath.Length * 2 + 2))
            {
                WintrustFileInfo fileInfo = new WintrustFileInfo();

                strMem.WriteUnicodeString(0, filePath);
                strMem.WriteByte(filePath.Length * 2, 0);
                strMem.WriteByte(filePath.Length * 2 + 1, 0);

                fileInfo.Size = Marshal.SizeOf(fileInfo);
                fileInfo.FilePath = strMem;

                WintrustData trustData = new WintrustData();

                trustData.Size = 12 * 4;
                trustData.UIChoice = 2; // WTD_UI_NONE
                trustData.UnionChoice = 1; // WTD_CHOICE_FILE
                trustData.RevocationChecks = WtRevocationChecks.None;
                trustData.ProvFlags = WtProvFlags.Safer;

                if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
                    trustData.ProvFlags |= WtProvFlags.CacheOnlyUrlRetrieval;

                using (MemoryAlloc mem = new MemoryAlloc(fileInfo.Size))
                {
                    Marshal.StructureToPtr(fileInfo, mem, false);
                    trustData.UnionData = mem;

                    var action = GenericVerifyV2;
                    uint winTrustResult = Win32.WinVerifyTrust(0, ref action, ref trustData);

                    result = StatusToVerifyResult(winTrustResult);
                }
            }

            if (result == VerifyResult.NoSignature)
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

                int catAdmin;
                var action = DriverVerify;

                if (!Win32.CryptCATAdminAcquireContext(out catAdmin, ref action, 0))
                    return VerifyResult.NoSignature;

                int catInfo = Win32.CryptCATAdminEnumCatalogFromHash(catAdmin, hash, hashLength, 0, 0);

                if (catInfo == 0)
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

                WintrustCatalogInfo wci = new WintrustCatalogInfo();

                wci.Size = Marshal.SizeOf(wci);
                wci.CatalogFilePath = ci.CatalogFile;
                wci.MemberFilePath = filePath;
                wci.MemberTag = memberTag.ToString();

                WintrustData trustData = new WintrustData();

                trustData.Size = 12 * 4;
                trustData.UIChoice = 1;
                trustData.UnionChoice = 2;
                trustData.RevocationChecks = WtRevocationChecks.None;

                if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
                    trustData.ProvFlags = WtProvFlags.CacheOnlyUrlRetrieval;

                using (MemoryAlloc mem = new MemoryAlloc(wci.Size))
                {
                    Marshal.StructureToPtr(wci, mem, false);

                    try
                    {
                        trustData.UnionData = mem;

                        var action2 = DriverVerify;
                        uint winTrustResult = Win32.WinVerifyTrust(0, ref action2, ref trustData);

                        result = StatusToVerifyResult(winTrustResult);
                    }
                    finally
                    {
                        Win32.CryptCATAdminReleaseCatalogContext(catAdmin, catInfo, 0);
                        Win32.CryptCATAdminReleaseContext(catAdmin, 0);
                        Marshal.DestroyStructure(mem, typeof(WintrustCatalogInfo));
                    }
                }
            }

            return result;
        }
    }
}
