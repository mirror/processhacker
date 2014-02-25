/*
 * Process Hacker - 
 *   cryptography functions
 *
 * Copyright (C) 2009 Flavio Erlich
 * Copyright (C) 2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Common;
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
        Expired,
        Revoked,
        Distrust,
        SecuritySettings
    }

    public static class Cryptography
    {
        public static readonly Guid DriverActionVerify = 
            new Guid("{f750e6c3-38ee-11d1-85e5-00c04fc295ee}");
        public static readonly Guid HttpsProvAction = 
            new Guid("{573e31f8-aaba-11d0-8ccb-00c04fc295ee}");
        public static readonly Guid OfficeSignActionVerify = 
            new Guid("{5555c2cd-17fb-11d1-85c4-00c04fc295ee}");
        public static readonly Guid WintrustActionGenericCertVerify = 
            new Guid("{189a3842-3041-11d1-85e1-00c04fc295ee}");
        public static readonly Guid WintrustActionGenericChainVerify = 
            new Guid("{fc451c16-ac75-11d1-b4b8-00c04fb66ea0}");
        public static readonly Guid WintrustActionGenericVerifyV2 = 
            new Guid("{00aac56b-cd44-11d0-8cc2-00c04fc295ee}");
        public static readonly System.Guid WintrustActionTrustProviderTest = 
            new Guid("{573e31f8-ddba-11d0-8ccb-00c04fc295ee}");

        private static string GetX500Value(string subject, string keyName)
        {
            Tokenizer t = new Tokenizer(subject);

            // Use the "tokenizer" to get the Common Name (CN).
            while (true)
            {
                t.EatWhitespace();

                string key = t.EatId();

                if (string.IsNullOrEmpty(key))
                    return null;

                t.EatWhitespace();
                string equals = t.EatSymbol();

                if (equals != "=")
                    return null;

                t.EatWhitespace();
                string value = t.EatQuotedString();

                if (string.IsNullOrEmpty(value))
                {
                    // The value probably isn't quoted.
                    value = t.EatUntil(',');
                }

                if (string.IsNullOrEmpty(value))
                    return null;

                if (key == keyName)
                    return value;

                string comma = t.EatSymbol();

                if (comma != ",")
                    return null;
            }
        }

        private static string GetSignerNameFromStateData(IntPtr stateData)
        {
            // Well, here's a shitload of indirection for you...

            // 1. State data -> Provider data

            IntPtr provData = Win32.WTHelperProvDataFromStateData(stateData);

            if (provData == IntPtr.Zero)
                return null;

            // 2. Provider data -> Provider signer

            IntPtr signerInfo = Win32.WTHelperGetProvSignerFromChain(provData, 0, false, 0);

            if (signerInfo == IntPtr.Zero)
                return null;

            CryptProviderSgnr sngr = (CryptProviderSgnr)Marshal.PtrToStructure(signerInfo, typeof(CryptProviderSgnr));

            if (sngr.CertChain == IntPtr.Zero)
                return null;
            if (sngr.CertChainCount == 0)
                return null;

            // 3. Provider signer -> Provider cert

            CryptProviderCert cert = (CryptProviderCert)Marshal.PtrToStructure(sngr.CertChain, typeof(CryptProviderCert));

            if (cert.Cert == IntPtr.Zero)
                return null;

            // 4. Provider cert -> Cert context

            CertContext context = (CertContext)Marshal.PtrToStructure(cert.Cert, typeof(CertContext));

            if (context.CertInfo != IntPtr.Zero)
            {
                // 5. Cert context -> Cert info

                CertInfo certInfo = (CertInfo)Marshal.PtrToStructure(context.CertInfo, typeof(CertInfo));

                unsafe
                {
                    using (var buffer = new MemoryAlloc(0x200))
                    {
                        int length;

                        // 6. Cert info subject -> Subject X.500 string

                        length = Win32.CertNameToStr(
                            1,
                            new IntPtr(&certInfo.Subject),
                            3,
                            buffer,
                            buffer.Size / 2
                            );

                        if (length > buffer.Size / 2)
                        {
                            buffer.ResizeNew(length * 2);

                            length = Win32.CertNameToStr(
                                1,
                                new IntPtr(&certInfo.Subject),
                                3,
                                buffer,
                                buffer.Size / 2
                                );
                        }

                        string name = buffer.ReadUnicodeString(0);
                        string value;

                        // 7. Subject X.500 string -> CN or OU value

                        value = GetX500Value(name, "CN");

                        if (value == null)
                            value = GetX500Value(name, "OU");

                        return value;
                    }
                }
            }

            return null;
        }

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

        public static VerifyResult VerifyFile(string fileName)
        {
            string signerName;

            return VerifyFile(fileName, out signerName);
        }

        public static VerifyResult VerifyFile(string fileName, out string signerName)
        {
            VerifyResult result = VerifyResult.NoSignature;

            using (MemoryAlloc strMem = new MemoryAlloc(fileName.Length * 2 + 2))
            {
                WintrustFileInfo fileInfo = new WintrustFileInfo();

                strMem.WriteUnicodeString(0, fileName);
                strMem.WriteInt16(fileName.Length * 2, 0);

                fileInfo.Size = Marshal.SizeOf(fileInfo);
                fileInfo.FilePath = strMem;

                WintrustData trustData = new WintrustData();

                trustData.Size = Marshal.SizeOf(typeof(WintrustData));
                trustData.UIChoice = 2; // WTD_UI_NONE
                trustData.UnionChoice = 1; // WTD_CHOICE_FILE
                trustData.RevocationChecks = WtdRevocationChecks.None;
                trustData.ProvFlags = WtdProvFlags.Safer;
                trustData.StateAction = WtdStateAction.Verify;

                if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
                    trustData.ProvFlags |= WtdProvFlags.CacheOnlyUrlRetrieval;

                using (MemoryAlloc mem = new MemoryAlloc(fileInfo.Size))
                {
                    mem.WriteStruct<WintrustFileInfo>(fileInfo);
                    trustData.UnionData = mem;

                    uint winTrustResult = Win32.WinVerifyTrust(IntPtr.Zero, WintrustActionGenericVerifyV2, ref trustData);

                    result = StatusToVerifyResult(winTrustResult);

                    try
                    {
                        if (result != VerifyResult.NoSignature)
                        {
                            signerName = GetSignerNameFromStateData(trustData.StateData);

                            return result;
                        }
                    }
                    finally
                    {
                        // Close the state data.
                        trustData.StateAction = WtdStateAction.Close;
                        Win32.WinVerifyTrust(IntPtr.Zero, WintrustActionGenericVerifyV2, ref trustData);
                    }
                }
            }

            signerName = null;

            using (FileHandle sourceFile = FileHandle.CreateWin32(fileName, FileAccess.GenericRead, FileShareMode.Read,
                FileCreationDispositionWin32.OpenExisting))
            {
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

                CatalogInfo ci;

                if (!Win32.CryptCATCatalogInfoFromContext(catInfo, out ci, 0))
                {
                    Win32.CryptCATAdminReleaseCatalogContext(catAdmin, catInfo, 0);
                    Win32.CryptCATAdminReleaseContext(catAdmin, 0);
                    return VerifyResult.NoSignature;
                }

                WintrustCatalogInfo wci = new WintrustCatalogInfo();

                wci.Size = Marshal.SizeOf(wci);
                wci.CatalogFilePath = ci.CatalogFile;
                wci.MemberFilePath = fileName;
                wci.MemberTag = memberTag.ToString();

                WintrustData trustData = new WintrustData();

                trustData.Size = Marshal.SizeOf(typeof(WintrustData));
                trustData.UIChoice = 1;
                trustData.UnionChoice = 2;
                trustData.RevocationChecks = WtdRevocationChecks.None;
                trustData.StateAction = WtdStateAction.Verify;

                if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
                    trustData.ProvFlags = WtdProvFlags.CacheOnlyUrlRetrieval;

                using (MemoryAlloc mem = new MemoryAlloc(wci.Size))
                {
                    mem.WriteStruct<WintrustCatalogInfo>(wci);

                    try
                    {
                        trustData.UnionData = mem;

                        uint winTrustResult = Win32.WinVerifyTrust(IntPtr.Zero, DriverActionVerify, ref trustData);

                        result = StatusToVerifyResult(winTrustResult);

                        if (result != VerifyResult.NoSignature)
                        {
                            signerName = GetSignerNameFromStateData(trustData.StateData);
                        }
                    }
                    finally
                    {
                        try
                        {
                            // Close the state data.
                            trustData.StateAction = WtdStateAction.Close;
                            Win32.WinVerifyTrust(IntPtr.Zero, DriverActionVerify, ref trustData);
                        }
                        finally
                        {
                            Win32.CryptCATAdminReleaseCatalogContext(catAdmin, catInfo, 0);
                            Win32.CryptCATAdminReleaseContext(catAdmin, 0);
                            mem.DestroyStruct<WintrustCatalogInfo>();
                        }
                    }
                }
            }

            return result;
        }
    }
}
