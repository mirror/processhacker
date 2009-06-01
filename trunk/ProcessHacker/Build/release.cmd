@ECHO OFF
:: Original script by wj32.
:: Modifications and additions by XhmikosR and Yzöwl.
SETLOCAL
SET outd=%~p1
PUSHD %outd%

:: Copy CHANGELOG.txt, LICENSE.txt, README.txt and kprocesshacker.sys to the
:: "Release" folder
FOR %%a IN (
	"CHANGELOG.txt" "LICENSE.txt" "README.txt" "KProcessHacker\i386\kprocesshacker.sys" 
    "NProcessHacker\Release\NProcessHacker.dll"
	) DO COPY "..\..\..\%%a" >NUL

:: Clear older files present in "Release" folder
DEL/f/a "ProcessHacker.exe.config" "processhacker-*-setup.exe"^
 "ProcessHacker_in.exe" "processhacker-bin.zip" >NUL 2>&1

:: Check if ILMerge is present in the default installation location or in PATH
SET ILMergePath="%PROGRAMFILES%\Microsoft\ILMerge\ILMerge.exe"
IF NOT EXIST %ILMergePath% (FOR %%a IN (ILMerge.exe) DO IF %%~$PATH:a' NEQ ' (
		SET ILMergePath="%%~$PATH:a") ELSE (SET "N_=T"
			ECHO:ILMerge IS NOT INSTALLED!!!&&(GOTO CLEANUP)))

:: Rename "ProcessHacker.exe" and "Assistant.exe" first
REN "ProcessHacker.exe" "ProcessHacker_in.exe"^
 && REN "Assistant.exe" "Assistant_in.exe"
ECHO.

:: Merge DLLs with "Assistant.exe"
SET RequiredDLLs="Aga.Controls.dll" "ProcessHacker.Common.dll"^
 "ProcessHacker.Native.dll"

%ILMergePath% /t:winexe /out:"Assistant.exe" "Assistant_in.exe"^
 %RequiredDLLs% && ECHO:DLLs merged successfully with Assistant.exe!

:: Merge "Aga.Controls.dll" with "ProcessHacker.exe" using ILMerge
%ILMergePath% /t:winexe /out:"ProcessHacker.exe" "ProcessHacker_in.exe"^
 %RequiredDLLs% && ECHO:DLLs merged successfully with ProcessHacker.exe!

DEL/f/a "ProcessHacker_in.exe" "Assistant_in.exe" %RequiredDLLs%^
 "ProcessHacker.Common.xml" "ProcessHacker.Native.xml" >NUL 2>&1

:: Set the path of Inno Setup and compile installer
SET "U_=HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
SET "I_=Inno Setup"
SET "A_=%I_% 5"
SET "M_=Inno Setup IS NOT INSTALLED!!!"
FOR /f "delims=" %%a IN (
	'REG QUERY "%U_%\%A_%_is1" /v "%I_%: App Path"2^>Nul^|FIND "REG_"') DO (
	SET "InnoSetupPath=%%a"&Call :Sub %%InnoSetupPath:*Z=%%)

IF DEFINED InnoSetupPath ("%InnoSetupPath%\iscc.exe" /Q /O"..\..\bin\Release"^
 "..\..\Build\Installer\Process_Hacker_installer.iss"&&(
	ECHO:Installer compiled successfully!)) ELSE (ECHO:%M_%)

:CLEANUP
:: Clean up the .pdb files in "Release" folder
DEL /f/a/q *.pdb >NUL 2>&1

:: ZIP the binaries
IF NOT DEFINED N_ (START "" /B /WAIT "..\..\Build\7za\7za.exe" a -tzip^
 "processhacker-bin.zip" "*" -x!*setup.exe -mx=9 >NUL&&(
	ECHO:ZIP created successfully!))

:END
GOTO :EOF

:Sub
SET InnoSetupPath=%*