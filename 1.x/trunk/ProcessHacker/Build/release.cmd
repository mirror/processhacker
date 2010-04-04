@ECHO OFF
REM Original script by wj32.
REM Modifications and additions by XhmikosR and Yzfwl.
SETLOCAL
SET outd=%~p1
PUSHD %outd%

REM Copy various files to the "Release" folder
FOR %%a IN (
	"CHANGELOG.txt" "LICENSE.txt" "README.txt" ^
	"KProcessHacker\i386\kprocesshacker.sys" ^
	"NProcessHacker\Release\NProcessHacker.dll"
	) DO COPY "..\..\..\%%a" >NUL

REM Copy the 64-bit NPH to the "Release" folder
COPY "..\..\..\NProcessHacker\x64\Release\NProcessHacker.dll"^
 "NProcessHacker64.dll" >NUL

REM Clear older files present in "Release" folder
DEL/f/a "ProcessHacker.exe.config" "processhacker-*-setup.exe"^
 "Assistant.dll" "Assistant.exe" "processhacker-*.zip" >NUL 2>&1

REM Check if ILMerge is present in the default installation location or in PATH
SET ILMergePath="%PROGRAMFILES%\Microsoft\ILMerge\ILMerge.exe"
IF NOT EXIST %ILMergePath% (FOR %%a IN (ILMerge.exe) DO IF %%~$PATH:a' NEQ ' (
	SET ILMergePath="%%~$PATH:a") ELSE (SET "N_=T"
		ECHO:ILMerge IS NOT INSTALLED!!!&&(GOTO :PDBFILES)))

SET RequiredDLLs="Aga.Controls.dll" "ProcessHacker.Common.dll"^
 "ProcessHacker.Native.dll"

REM Create a temporary directory for the merged file(s)
MD "tmp" >NUL 2>&1

REM Merge DLLs with "ProcessHacker.exe" using ILMerge
%ILMergePath% /t:winexe /out:"tmp\ProcessHacker.exe" "ProcessHacker.exe"^
 %RequiredDLLs% && ECHO:DLLs merged successfully with ProcessHacker.exe!
IF %ERRORLEVEL% NEQ 0 GOTO :SubError

REM Delete the existing EXEs and PDBs
DEL/f/a "ProcessHacker.exe" "*.pdb" >NUL 2>&1

REM Copy the merged file(s) back into this directory
MOVE "tmp\*" .\ >NUL 2>&1

DEL/f/a %RequiredDLLs% "ProcessHacker.Common.xml"^
 "ProcessHacker.Native.xml" >NUL 2>&1

REM Delete the temporary directory
RD /Q "tmp" >NUL 2>&1

REM Detect if we are running on 64bit WIN and use Wow6432Node, set the path
REM of Inno Setup accordingly and compile installer
IF "%PROGRAMFILES(x86)%zzz"=="zzz" (SET "U_=HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
) ELSE (
SET "U_=HKLM\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
)

SET "I_=Inno Setup"
SET "A_=%I_% 5"
SET "M_=Inno Setup IS NOT INSTALLED!!!"
FOR /f "delims=" %%a IN (
	'REG QUERY "%U_%\%A_%_is1" /v "%I_%: App Path"2^>Nul^|FIND "REG_"') DO (
	SET "InnoSetupPath=%%a"&Call :Sub %%InnoSetupPath:*Z=%%)

IF DEFINED InnoSetupPath ("%InnoSetupPath%\iscc.exe" /Q /O"..\..\bin\Release"^
 "..\..\Build\Installer\Process_Hacker_installer.iss"&&(
	ECHO:Compiled installer!)) ELSE (ECHO:%M_%)

REM ZIP the files
IF NOT DEFINED N_ (START "" /B /WAIT "..\..\Build\7za\7za.exe" a -tzip -mx=9^
 "processhacker-bin.zip" "base.txt" "CHANGELOG.txt" "Help.htm"^
 "kprocesshacker.sys" "LICENSE.txt" "NProcessHacker.dll"^
 "NProcessHacker64.dll" "ProcessHacker.exe" "README.txt" "structs.txt") >NUL
IF %ERRORLEVEL% NEQ 0 (GOTO :SubError) ELSE (ECHO:ZIP package created successfully!)


:PDBFILES
REM Copy some PDBs over
FOR %%a IN (
	"KProcessHacker\i386\kprocesshacker.pdb" ^
	"NProcessHacker\Release\NProcessHacker.pdb"
	) DO COPY "..\..\..\%%a" >NUL

REM Make a PDB zip
"..\..\Build\7za\7za.exe" a -tzip -mx=9 "processhacker-pdb.zip" "*.pdb" >NUL
IF %ERRORLEVEL% NEQ 0 (GOTO :SubError) ELSE (ECHO:PDB ZIP package created successfully!)


:END
ENDLOCAL
GOTO :EOF

:Sub
SET InnoSetupPath=%*
GOTO :EOF

:SubError
ECHO:Error detected!!!
GOTO :END